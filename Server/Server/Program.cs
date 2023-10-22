using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Utilities.Net;
using Server.Data;
using Server.DB;
using Server.Game;
using Server.Game.Object;
using ServerCore;
using Shared;

namespace Server
{
	// 스레드 현황
	// 1. receive packet (N개)
	// 2. GameLogic (1개)
	// 3. Send(1)
	// 4. DB (1개)
	class Program
	{
		static Listener _listener = new Listener();
		static void GameLogicTask()
		{
			DateTime time1 = DateTime.Now;
			DateTime time2 = DateTime.Now;

			while (true)
			{
				time2 = DateTime.Now;
				float deltaTime = (time2.Ticks - time1.Ticks) / 10000000f;
				time1 = time2;

				GameLogic.Instance.Update(deltaTime);
				//Thread.Sleep(0);
			}
		}
		static void DbTask()
		{
			while (true)
			{
				DbThread.Instance.Flush(); // DB 스레드
				Thread.Sleep(0);
			}
		}
		static void NetworkTask()
		{
			while (true)
			{
				var clientSession = SessionManager.Instance.GetSessions();
				foreach (var session in clientSession)
				{
					session.FlushSend();
				}
				Thread.Sleep(0);
			}
		}
		static void StartServerInfoTask()
		{
			var t = new System.Timers.Timer();
			t.AutoReset = true;
			t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) =>
			{
				UpdateServerInfo request = new UpdateServerInfo()
				{
					serverName = ServerName,
					busyScore = SessionManager.Instance.GetBusyScore()
				};
				request.serverName = ServerName;
				SharedDB.Request(request);
			});
			t.Interval = 10 * 1000;
			t.Start();
		}

		public static string ServerName { get; } = "서버_1";
		public static string Ip { get; set; }
		public static int Port { get; } = 7777;

		static void Main(string[] args)
		{

			ConfigManager.LoadConfig();
			DataManager.LoadData();

			GameLogic.Instance.Push(() =>
			{
				GameLogic.Instance.Add();
			});

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
            System.Net.IPAddress ipAddr = ipHost.AddressList[1];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, Port);

			Ip = ipAddr.ToString();

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			StartServerInfoTask();

			// DBTask
			Thread dbTask = new Thread(DbTask);
			dbTask.Name = "DB";
			dbTask.Start();

			// NetworkTask
			Thread networkTask = new Thread(NetworkTask);
			networkTask.Name = "Network Send";
			networkTask.Start();

			// GameLogicTask
			Thread.CurrentThread.Name = "GameLogic";
			GameLogicTask();
		}
	}
}

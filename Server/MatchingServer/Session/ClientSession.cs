using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;
using Server.Game.Object;
using Server.Data;

namespace Server
{
    public partial class ClientSession : PacketSession
	{
		public Player MyPlayer { get; set; }
		public int SessionId { get; set; }
		List<ArraySegment<byte>> _reserveQueue = new List<ArraySegment<byte>>();

		// 패킷 모아보내기
		int _reservedSendBytes = 0;
		long _lastSendTick = 0;

		object _lock = new object();
		public PlayerServerState ServerState { get; private set; } = PlayerServerState.ServerStateLogin;

		#region Network
		public void Send(IMessage packet)
		{
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

			// 예약만 하고 보내지는 않는다.
			lock(_lock)
			{
				_reserveQueue.Add(sendBuffer);
				_reservedSendBytes += sendBuffer.Length;
			}
			//Send(new ArraySegment<byte>(sendBuffer));
		}
		public void FlushSend()
		{
			List<ArraySegment<byte>> sendList = null;
			lock (_lock)
			{
				// 패킷 보아보내기 (대신 반응 늦어짐)
				// 0.01초가 지났거나 너무 패킷이 많이 모였을 때
				long delta = (System.Environment.TickCount64 - _lastSendTick);
				if (delta < 10 && _reservedSendBytes < 10000)
					return;
                

                // 패킷 모아보내기
                _reservedSendBytes = 0;
				_lastSendTick = System.Environment.TickCount64;
				//if (_reserveQueue.Count == 0)
				//	return;
				//Console.WriteLine(_reserveQueue.Count);


				sendList = _reserveQueue;
				_reserveQueue = new List<ArraySegment<byte>>();
				
			}

			Send(sendList);
		}

		public override void OnConnected(EndPoint endPoint)
		{

			S_MatchingConnected connectedPacket = new S_MatchingConnected();
			Send(connectedPacket);

		}
		public void HandleEnterGame(C_MatchingLogin packet)
		{
			MyPlayer = PlayerManager.Instance.Add();
			MyPlayer.ObjectInfo.Name = packet.UserName;
			MyPlayer.Session = this;

			GameLogic.Instance.Push(() =>
			{
				GameRoom room = GameLogic.Instance.Find(1);
				room.Push(room.EnterGame, MyPlayer);
			});
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			GameLogic.Instance.Push(() =>
			{
				if (MyPlayer == null)
					return;
				GameRoom room = GameLogic.Instance.Find(1);
				room.Push(room.LeaveGame, MyPlayer.ObjectInfo.ObjectId);
			});



			SessionManager.Instance.Remove(this);
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
		#endregion


	}
}

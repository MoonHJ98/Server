using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;
using System.Linq.Expressions;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Threading;
using System.Data;

public class NetworkManager
{
	public ServerSession _session = null;
	public ServerSession _matchingSession = null;
	public ServerSession _dungeonSession = null;


	public string AccountName { get; set; }
	public int Token { get; set; }


	public IPAddress IPAddress { get; set; }

	public void Send(IMessage packet)
	{
		if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
			_session.Send(packet);
		else if(Managers.Scene.CurrentScene.SceneType == Define.Scene.Dungeon)
			_dungeonSession.Send(packet);

	}
	public void SendMatching(IMessage packet)
	{
		_matchingSession.Send(packet);
	}

	public string Server = "192.168.0.15";
	public void ConnectToGame(ServerInfo info)
	{

		IPAddress ipAddr = IPAddress.Parse(info.IpAdress);
		IPAddress = ipAddr;
		IPEndPoint endPoint = new IPEndPoint(ipAddr, info.Port);

		Connector connector = new Connector();

		//Interlocked.Exchange(ref _session._disconnected, 0);
		_session = new ServerSession();
		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}
	public void ConnectToGame()
	{

		IPAddress ipAddr = IPAddress.Parse(Server);
		IPAddress = ipAddr;
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Connector connector = new Connector();

		//Interlocked.Exchange(ref _session._disconnected, 0);
		_session = new ServerSession();

		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}
	public void ConnectToDungeon()
	{

		IPAddress ipAddr = IPAddress.Parse(Server);
		IPAddress = ipAddr;
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 9999);

		Connector connector = new Connector();

		//Interlocked.Exchange(ref _dungeonSession._disconnected, 0);
		_dungeonSession = new ServerSession();
		connector.Connect(endPoint,
			() => { return _dungeonSession; },
			1);
	}
	public void DisConnectServer()
	{
		_session.Disconnect();
		_session = null;


	}
	public void DisConnectDungeon()
	{
		_dungeonSession.Disconnect();
		_dungeonSession = null;
	}
	public void DisConnectMatching()
	{
		_matchingSession.Disconnect();
		_matchingSession = null;
	}
	public void ConnectToMatching()
	{

		IPAddress ipAddr = IPAddress.Parse(Server);
		IPAddress = ipAddr;
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 8888);
		
		Connector connector = new Connector();

		_matchingSession = new ServerSession();
		connector.Connect(endPoint,
			() => { return _matchingSession; },
			1);
	}

	public void Update()
	{
		List<PacketMessage> list = PacketQueue.Instance.PopAll();


		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}	
	}
}

using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;
using System.Linq.Expressions;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class NetworkManager
{
	ServerSession _session = new ServerSession();
	ServerSession _matchingSession = new ServerSession();

	List<ServerSession> sessionList = new List<ServerSession>();


	public string AccountName { get; set; }
	public int Token { get; set; }


	public IPAddress IPAddress { get; set; }

	public void Send(IMessage packet)
	{
		_session.Send(packet);
	}

	public string Server = "192.168.0.15";
	public void ConnectToGame(ServerInfo info)
	{

		IPAddress ipAddr = IPAddress.Parse(info.IpAdress);
		IPAddress = ipAddr;
		IPEndPoint endPoint = new IPEndPoint(ipAddr, info.Port);

		Connector connector = new Connector();

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

		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}
	public void DisConnecetMatching()
	{
		_matchingSession.Disconnect();
	}
	public void ConnectToMatching()
	{

		IPAddress ipAddr = IPAddress.Parse(Server);
		IPAddress = ipAddr;
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 8888);
		
		Connector connector = new Connector();
		
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

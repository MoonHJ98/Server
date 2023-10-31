using ServerCore;
using System;
using System.Net;
using System.Threading;


class Program
{
	static int DummyClientSount { get; } = 50;

	static void Main(string[] args)
	{
		Thread.Sleep(30000);

		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddr = ipHost.AddressList[1];
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Connector connector = new Connector();

		connector.Connect(endPoint,
			() => { return SessionManager.Instance.Generate(); },
			DummyClientSount);

		while (true)
		{
			Thread.Sleep(10000);
		}
	}
}


﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
	public class Listener
	{
		Socket _listenSocket;
		Func<Session> _sessionFactory;

		public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
		{
			_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_sessionFactory += sessionFactory;

			// 문지기 교육
			_listenSocket.Bind(endPoint);

			// 영업 시작
			// backlog : 최대 대기수
			_listenSocket.Listen(backlog);

			for (int i = 0; i < register; i++)
			{
				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
				RegisterAccept(args);
			}
		}

		void RegisterAccept(SocketAsyncEventArgs args)
		{
			args.AcceptSocket = null;

			try
			{
				bool pending = _listenSocket.AcceptAsync(args);
				if (pending == false)
					OnAcceptCompleted(null, args);

			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);
            }

		}

		void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
		{
			try
			{
				if (args.SocketError == SocketError.Success)
				{
					Session session = _sessionFactory.Invoke();
					session.Start(args.AcceptSocket);
					session.OnConnected(args.AcceptSocket.RemoteEndPoint);
				}
				else
					Console.WriteLine(args.SocketError.ToString());

			}
			catch (Exception ex)
			{
                Console.WriteLine(ex);
            }

			// 성공 실패와 상관 없이 args를 이용해 다시 한번 예약해야해서 try안에 들어가면 안됨
			RegisterAccept(args);
		}
	}
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChattingServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChattingController : ControllerBase
	{
		[HttpPost]
		[Route("chat")]
		public void Chatting([FromBody] ChattingReq req)
		{
			// 존서버로 보내기
			using (TcpClient client = new TcpClient("192.168.0.15", 5555))
			{
				using (NetworkStream stream = client.GetStream())
				{
					string data = req.PlayerName + " : " + req.Chatting;
					byte[] messageBytes = Encoding.UTF8.GetBytes(data);
					stream.Write(messageBytes, 0, messageBytes.Length);
				}
			}

			// 던전서버로 보내기
			using (TcpClient client = new TcpClient("192.168.0.15", 6666))
			{
				using (NetworkStream stream = client.GetStream())
				{
					string data = req.PlayerName + " : " + req.Chatting;
					byte[] messageBytes = Encoding.UTF8.GetBytes(data);
					stream.Write(messageBytes, 0, messageBytes.Length);
				}
			}
		}

	}
}

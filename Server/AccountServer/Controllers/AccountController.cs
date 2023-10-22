using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		[HttpPost]
		[Route("create")]
		public CreateAccountPacketRes CreateAccount([FromBody] CreateAccountPacketReq req)
		{
			// [FromBody] CreateAccountPacketReq : 클라에서 json으로 보내는 패킷을 자동으로 파싱해줌

			CreateAccountPacketRes res = new CreateAccountPacketRes();

			CreateAccount request = new CreateAccount() { accountName = req.AccountName, password = req.Password };
			var result = AccountDB.Request(request) as CreateAccountResult;

			if (result.result == CreateAccountResult.DBResult.Success)
				res.CreateOk = true;
			else
				res.CreateOk = false;

			return res;
		}

		[HttpPost]
		[Route("login")]
		public LoginAccountPacketRes LoginAccount([FromBody] LoginAccountPacketReq req)
		{
			LoginAccountPacketRes res = new LoginAccountPacketRes();

			GetAccount request = new GetAccount() { accountName = req.AccountName, password = req.Password };
			var result = AccountDB.Request(request) as GetAccountResult;

			if (result.result == GetAccountResult.DBResult.Success)
			{
				res.LoginOk = true;

				// 토큰 발급
				DateTime expired = DateTime.UtcNow;
				expired.AddSeconds(600);

				UpdateToken updateTokenRequest = new UpdateToken()
				{
					accountName = result.accountName,
					token = new Random().Next(Int32.MinValue, Int32.MaxValue),
					expiredTime = expired
				};
				var tokenResult = SharedDB.Request(updateTokenRequest) as UpdateTokenResult;
				if(tokenResult.result == UpdateTokenResult.Result.Success)
				{
					res.AccountName = updateTokenRequest.accountName;
					res.Token = updateTokenRequest.token;
					res.ServerInfos = new List<ServerInfo>();
				}

				// 서버 정보
				GetServerInfo getServerInfo = new GetServerInfo();
				var serverInfoResult = SharedDB.Request(getServerInfo) as GetServerInfoResult;

				foreach(var info in serverInfoResult.infos)
				{
					var serverInfo = new ServerInfo()
					{
						Name = info.serverName,
						IpAdress = info.IpAdress,
						Port = info.Port,
						BusyScore = info.BusyScore
					};
					res.ServerInfos.Add(serverInfo);
				}
			}
			else
				res.LoginOk = false;

			return res;
		}

	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
	public static class DbProvider
	{
		public static Result UpdateToken(UpdateToken _request)
		{
			return Repository.sp_update_token(_request);
		}
		public static Result GetServerInfo(GetServerInfo _request)
		{
			return Repository.sp_get_server_info(_request);
		}
		public static Result UpdateServerInfo(UpdateServerInfo _request)
		{
			return Repository.sp_update_server_busy_score(_request);
		}
		//public static Result CreateAccount(CreateAccount _request)
		//{
		//	return Repository.sp_create_account(_request);
		//}
		//public static Result GetAccount(GetAccount _request)
		//{
		//	return Repository.sp_get_account_name(_request);
		//}
		//public static Result GetPlayerByAccount(GetPlayerByAccount _request)
		//{
		//	return Repository.sp_get_player_by_account(_request);
		//}
		//public static Result CreatePlayer(CreatePlayer _request)
		//{
		//	return Repository.sp_create_player(_request);
		//}
	}
}

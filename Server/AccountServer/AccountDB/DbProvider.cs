using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccountServer
{
	public static class DbProvider
	{
		public static Result CreateAccount(CreateAccount _request)
		{
			return Repository.sp_create_account(_request);
		}
		public static Result GetAccount(GetAccount _request)
		{
			return Repository.sp_get_account_name(_request);
		}
	}
}

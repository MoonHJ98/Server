using AccountServer;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccountServer
{
	public static class AccountDB
	{
		public static MySqlConnection GetConnection()
		{
			return new MySqlConnection("Server=localhost;Port=3306;Database=AccountDB;Uid=root;Pwd=gudwns1!");
		}

		public static Result Request(Request _request)
		{

			switch (_request)
			{
				case CreateAccount request:
					return DbProvider.CreateAccount(request);
				case GetAccount request:
					return DbProvider.GetAccount(request);
			}

			return null;
		}
	}
}

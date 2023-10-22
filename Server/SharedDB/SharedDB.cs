
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
	public static class SharedDB
	{
		public static MySqlConnection GetConnection()
		{
			return new MySqlConnection("Server=localhost;Port=3306;Database=SharedDB;Uid=root;Pwd=gudwns1!");
		}

		public static Result Request(Request _request)
		{

			switch (_request)
			{
				case UpdateToken request:
					return DbProvider.UpdateToken(request);
				case GetServerInfo request:
					return DbProvider.GetServerInfo(request);
				case UpdateServerInfo request:
					return DbProvider.UpdateServerInfo(request);
			}

			return null;
		}
	}
}

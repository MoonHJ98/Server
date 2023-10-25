using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.DB
{
	public static class GameDB
	{
		public static MySqlConnection GetConnection()
		{
			return new MySqlConnection("Server=localhost;Port=3306;Database=GameDB;Uid=root;Pwd=gudwns1!");
		}

		public static async Task<Result> Request(Request _request)
		{

			switch (_request)
			{
				case GetPlayerByAccount request:
					return await DbProvider.GetPlayerByAccount(request);
				case CreatePlayer request:
					return await DbProvider.CreatePlayer(request);
				case SetStat request:
					return await DbProvider.SetStat(request);
				case GetItems request:
					return await DbProvider.GetItems(request);
				case AddItem request:
					return await DbProvider.AddItem(request);
				case ChangeItemEquipState request:
					return await DbProvider.ChangeItemEquipState(request);

			}

			return null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using static Shared.GetServerInfoResult;

namespace Shared
{
	public class Repository
	{
		public static Result sp_update_token(UpdateToken _request)
		{
			string procedureName = "sp_update_token";

			using (MySqlConnection connection = SharedDB.GetConnection())
			{
				UpdateTokenResult result = new UpdateTokenResult();
				connection.Open();

				// MySQL 저장 프로시저 호출
				using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// 필요한 매개변수 추가 (있는 경우)
					cmd.Parameters.AddWithValue("@i_accountName", _request.accountName);
					cmd.Parameters.AddWithValue("@i_tokenId", _request.token);
					cmd.Parameters.AddWithValue("@i_expiredTime", _request.expiredTime);


					// out 파라미터 정의
					cmd.Parameters.Add("o_error", MySqlDbType.Int32);

					cmd.ExecuteNonQuery();

					int o_error = Convert.ToInt32(cmd.Parameters["o_error"].Value);
					if (o_error == -1)
					{
						result.result = UpdateTokenResult.Result.Fail;
					}
					else
					{
						result.result = UpdateTokenResult.Result.Success;
					}
				}
				return result;
			}
		}
		public static Result sp_get_server_info(GetServerInfo _request)
		{
			string procedureName = "sp_get_server_info";

			using (MySqlConnection connection = SharedDB.GetConnection())
			{
				GetServerInfoResult result = new GetServerInfoResult();
				connection.Open();

				// MySQL 저장 프로시저 호출
				using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					try
					{
						cmd.CommandType = CommandType.StoredProcedure;
						using (DbDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								var info = new Info();
								info.serverName = reader.GetString("serverName");
								info.IpAdress = reader.GetString("ipAddress");
								info.Port = reader.GetInt32("port");
								info.BusyScore = reader.GetInt32("busyScore");
								result.infos.Add(info);

							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
					return result;
				}

			}
		}
		public static Result sp_update_server_busy_score(UpdateServerInfo _request)
		{
			string procedureName = "sp_update_server_busy_score";

			using (MySqlConnection connection = SharedDB.GetConnection())
			{
				UpdateServerInfoResult result = new UpdateServerInfoResult();
				connection.Open();

				// MySQL 저장 프로시저 호출
				using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// 필요한 매개변수 추가 (있는 경우)
					cmd.Parameters.AddWithValue("@i_serverName", _request.serverName);
					cmd.Parameters.AddWithValue("@i_busyScore", _request.busyScore);



					// out 파라미터 정의
					cmd.Parameters.Add("o_error", MySqlDbType.Int32);

					cmd.ExecuteNonQuery();

					int o_error = Convert.ToInt32(cmd.Parameters["o_error"].Value);
					if (o_error == -1)
					{
						result.result = UpdateServerInfoResult.Result.Fail;
					}
					else
					{
						result.result = UpdateServerInfoResult.Result.Success;
					}
				}
				return result;
			}
		}
	}

}


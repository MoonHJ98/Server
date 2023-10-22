using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AccountServer
{

	public class Repository
	{
		public static Result sp_create_account(CreateAccount _request)
		{
			string procedureName = "sp_create_account";

			using (MySqlConnection connection = AccountDB.GetConnection())
			{
				CreateAccountResult result = new CreateAccountResult();
				connection.Open();

				// MySQL 저장 프로시저 호출
				using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// 필요한 매개변수 추가 (있는 경우)
					cmd.Parameters.AddWithValue("@i_accountName", _request.accountName);
					cmd.Parameters.AddWithValue("@i_password", _request.password);

					// out 파라미터 정의
					cmd.Parameters.Add("o_error", MySqlDbType.Int32);

					using (DbDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							// 결과에서 데이터 비동기적으로 읽기
							result.accountName = reader.GetString("accountName");
						}
					}
					int o_error = Convert.ToInt32(cmd.Parameters["o_error"].Value);
					if (o_error == -1)
					{
						result.result = CreateAccountResult.DBResult.Fail;
					}
					else
					{
						result.result = CreateAccountResult.DBResult.Success;
					}
				}
				return result;
			}
		}
		public static Result sp_get_account_name(GetAccount _request)
		{
			string procedureName = "sp_get_account_name";

			using (MySqlConnection connection = AccountDB.GetConnection())
			{
				GetAccountResult result = new GetAccountResult();
				connection.Open();

				// MySQL 저장 프로시저 호출
				using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					try
					{
						cmd.CommandType = CommandType.StoredProcedure;

						// 필요한 매개변수 추가 (있는 경우)
						cmd.Parameters.AddWithValue("@i_accountName", _request.accountName);
						cmd.Parameters.AddWithValue("@i_password", _request.password);

						// out 파라미터 정의
						cmd.Parameters.Add("o_error", MySqlDbType.Int32);

						using (DbDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								result.accountName = reader.GetString("accountName");
							}

						}
						int o_error = Convert.ToInt32(cmd.Parameters["o_error"].Value);
						if (o_error == -1)
						{
							result.result = GetAccountResult.DBResult.Fail;
						}
						else
						{
							result.result = GetAccountResult.DBResult.Success;
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
	}

}


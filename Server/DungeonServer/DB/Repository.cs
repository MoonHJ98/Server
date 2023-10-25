using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using static Server.DB.Result;
using Google.Protobuf.Protocol;

namespace Server.DB
{

	public class Repository
	{
		public static async Task<Result> sp_get_player_by_account(GetPlayerByAccount _request)
		{
			string procedureName = "sp_get_player_by_account";

			await using (MySqlConnection connection = GameDB.GetConnection())
			{
				GetPlayerByAccountResult result = new GetPlayerByAccountResult();
				await connection.OpenAsync();

				// MySQL 저장 프로시저 호출
				await using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// 필요한 매개변수 추가 (있는 경우)
					cmd.Parameters.AddWithValue("@i_accountName", _request.accountName);
					// out 파라미터 정의
					cmd.Parameters.Add("o_error", MySqlDbType.Int32);

					await using (DbDataReader reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							// 결과에서 데이터 비동기적으로 읽기
							LobbyPlayerInfo playerInfo = new LobbyPlayerInfo()
							{
								Name = reader.GetString("playerName"),
								StatInfo = new StatInfo()
								{
									Level = reader.GetInt32("level"),
									Hp = reader.GetInt32("hp"),
									MaxHp = reader.GetInt32("maxHp"),
									Damage = reader.GetInt32("damage"),
									Defense = reader.GetInt32("defense"),
									Exp = reader.GetInt32("exp"),
									TotalExp = reader.GetInt32("totalExp"),
								}
							};
							result.playerList.Add(playerInfo);
						};


						int o_error = Convert.ToInt32(cmd.Parameters["o_error"].Value);

						if (o_error == -1)
						{
							result.result = GetPlayerByAccountResult.DBResult.Fail;
						}
						else
						{
							result.result = GetPlayerByAccountResult.DBResult.Success;
						}
					}
					return result;
				}

			}
		}
		public static async Task<Result> sp_create_player(CreatePlayer _request)
		{
			string procedureName = "sp_create_player";

			using (MySqlConnection connection = GameDB.GetConnection())
			{
				await connection.OpenAsync();
				CreatePlayerResult result = new CreatePlayerResult();

				using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					try
					{
						cmd.CommandType = CommandType.StoredProcedure;
						// 파라미터 설정 (필요에 따라 추가)
						cmd.Parameters.AddWithValue("@i_playerName", _request.playerName);
						cmd.Parameters.AddWithValue("@i_accountName", _request.accountName);
						cmd.Parameters.AddWithValue("@i_level", _request.level);
						cmd.Parameters.AddWithValue("@i_hp", _request.hp);
						cmd.Parameters.AddWithValue("@i_maxHp", _request.maxHp);
						cmd.Parameters.AddWithValue("@i_damage", _request.damage);
						cmd.Parameters.AddWithValue("@i_defense", _request.defense);
						cmd.Parameters.AddWithValue("@i_exp", _request.exp);
						cmd.Parameters.AddWithValue("@i_totalExp", _request.totalExp);

						// out 파라미터 정의
						cmd.Parameters.Add("o_error", MySqlDbType.Int32);

						// 비동기로 저장 프로시저 실행
						await cmd.ExecuteNonQueryAsync();
						int o_error = Convert.ToInt32(cmd.Parameters["o_error"].Value);
						if (o_error == 0)
						{
							result.result = CreatePlayerResult.DBResult.Success;
							result.playerInfo = new LobbyPlayerInfo()
							{
								Name = _request.playerName,
								StatInfo = new StatInfo()
								{
									Level = _request.level,
									Hp = _request.hp,
									MaxHp = _request.maxHp,
									Damage = _request.damage,
									Defense = _request.defense,
									Exp = _request.exp,
									TotalExp = _request.totalExp,
								}
							};
						}
						if (o_error == -1)
						{
							result.result = CreatePlayerResult.DBResult.Already_Exists;
						}

					}
					catch (Exception ex)
					{
						await Console.Out.WriteLineAsync($"sp_get_accounts error : {ex}");
					}
					return result;
				}
			}
		}
		public static async Task<Result> sp_set_stat(SetStat _request)
		{
			string procedureName = "sp_set_stat";

			using (MySqlConnection connection = GameDB.GetConnection())
			{
				await connection.OpenAsync();
				SetStatResult result = new SetStatResult();

				using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					try
					{
						cmd.CommandType = CommandType.StoredProcedure;
						// 파라미터 설정 (필요에 따라 추가)
						cmd.Parameters.AddWithValue("@i_playerName", _request.playerName);
						cmd.Parameters.AddWithValue("@i_level", _request.statInfo.Level);
						cmd.Parameters.AddWithValue("@i_hp", _request.statInfo.Hp);
						cmd.Parameters.AddWithValue("@i_maxHp", _request.statInfo.MaxHp);
						cmd.Parameters.AddWithValue("@i_damage", _request.statInfo.Damage);
						cmd.Parameters.AddWithValue("@i_defense", _request.statInfo.Defense);
						cmd.Parameters.AddWithValue("@i_exp", _request.statInfo.Exp);
						cmd.Parameters.AddWithValue("@i_totalExp", _request.statInfo.TotalExp);

						// out 파라미터 정의
						cmd.Parameters.Add("o_error", MySqlDbType.Int32);

						// 비동기로 저장 프로시저 실행
						await cmd.ExecuteNonQueryAsync();
						int o_error = Convert.ToInt32(cmd.Parameters["o_error"].Value);
						if (o_error == 0)
						{
							result.result = SetStatResult.DBResult.Success;

						}
						if (o_error == -1)
						{
							result.result = SetStatResult.DBResult.Fail;
						}

					}
					catch (Exception ex)
					{
						await Console.Out.WriteLineAsync($"sp_get_accounts error : {ex}");
					}
					return result;
				}
			}
		}
		public static async Task<Result> sp_get_items(GetItems _request)
		{
			string procedureName = "sp_get_items";

			await using (MySqlConnection connection = GameDB.GetConnection())
			{
				GetItemsResult result = new GetItemsResult();
				await connection.OpenAsync();

				// MySQL 저장 프로시저 호출
				await using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// 필요한 매개변수 추가 (있는 경우)
					cmd.Parameters.AddWithValue("@i_playerName", _request.playerName);
					// out 파라미터 정의
					cmd.Parameters.Add("o_error", MySqlDbType.Int32);

					await using (DbDataReader reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							// 결과에서 데이터 비동기적으로 읽기
							ItemInfo itemInfo = new ItemInfo()
							{
								ItemId = reader.GetInt32("itemId"),
								TemplateId = reader.GetInt32("itemTemplateId"),
								Count = reader.GetInt32("count"),
								Slot = reader.GetInt32("slot"),
								Equipped = reader.GetBoolean("equipped")
							};
							result.items.Add(itemInfo);
						};


						int o_error = Convert.ToInt32(cmd.Parameters["@o_error"].Value);

						if (o_error == -1)
						{
							result.result = GetItemsResult.DBResult.Fail;
						}
						else
						{
							result.result = GetItemsResult.DBResult.Success;
						}
					}
					return result;
				}

			}
		}
		public static async Task<Result> sp_add_item(AddItem _request)
		{
			string procedureName = "sp_add_item";

			using (MySqlConnection connection = GameDB.GetConnection())
			{
				await connection.OpenAsync();
				AddItemResult result = new AddItemResult();

				using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					try
					{
						cmd.CommandType = CommandType.StoredProcedure;
						// 파라미터 설정 (필요에 따라 추가)
						cmd.Parameters.AddWithValue("@i_itemTemplateId", _request.itemTemplateId);
						cmd.Parameters.AddWithValue("@i_count", _request.count);
						cmd.Parameters.AddWithValue("@i_slot", _request.slot);
						cmd.Parameters.AddWithValue("@i_owner", _request.owner);

						// out 파라미터 정의
						//cmd.Parameters.Add("@o_id", MySqlDbType.Int32);
						//cmd.Parameters.Add("@o_error", MySqlDbType.Int32);

						MySqlParameter outputParameter = new MySqlParameter("o_id", MySqlDbType.Int32);
						outputParameter.Direction = ParameterDirection.Output;
						cmd.Parameters.Add(outputParameter);
						MySqlParameter outputParameter2 = new MySqlParameter("o_error", MySqlDbType.Int32);
						outputParameter2.Direction = ParameterDirection.Output;
						cmd.Parameters.Add(outputParameter2);
						//cmd.Parameters.Add("@o_id", MySqlDbType.Int32).Direction = ParameterDirection.Output;
						//cmd.Parameters.Add("@o_error", MySqlDbType.Int32).Direction = ParameterDirection.Output;


						// 비동기로 저장 프로시저 실행
						await cmd.ExecuteNonQueryAsync();


						int o_id = Convert.ToInt32(cmd.Parameters["o_id"].Value);
						int o_error = Convert.ToInt32(cmd.Parameters["o_error"].Value);
						if (o_error == 0)
						{
							result.result = AddItemResult.DBResult.Success;
							result.itemId = o_id;
							result.itemTemplateId = _request.itemTemplateId;
							result.count = _request.count;
							result.slot = _request.slot;

						}
						if (o_error == -1)
						{
							result.result = AddItemResult.DBResult.Fail;
						}

					}
					catch (Exception ex)
					{
						await Console.Out.WriteLineAsync($"sp_get_accounts error : {ex}");
					}
					return result;
				}
			}
		}
		public static async Task<Result> sp_change_item_equip_state(ChangeItemEquipState _request)
		{
			string procedureName = "sp_change_item_equip_state";

			using (MySqlConnection connection = GameDB.GetConnection())
			{
				await connection.OpenAsync();
				ChangeItemEquipStateResult result = new ChangeItemEquipStateResult();

				using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
				{
					try
					{
						cmd.CommandType = CommandType.StoredProcedure;
						// 파라미터 설정 (필요에 따라 추가)
						cmd.Parameters.AddWithValue("@i_itemId", _request.itemId);
						cmd.Parameters.AddWithValue("@i_equipped", _request.equipped);

						// out 파라미터 정의
						cmd.Parameters.Add("o_error", MySqlDbType.Int32);

						// 비동기로 저장 프로시저 실행
						await cmd.ExecuteNonQueryAsync();
						int o_error = Convert.ToInt32(cmd.Parameters["o_error"].Value);

						result.itemId = _request.itemId;
						result.equipped = _request.equipped;
						if (o_error == 0)
						{
							result.result = ChangeItemEquipStateResult.DBResult.Success;

						}
						if (o_error == -1)
						{
							result.result = ChangeItemEquipStateResult.DBResult.Fail;
						}

					}
					catch (Exception ex)
					{
						await Console.Out.WriteLineAsync($"sp_get_accounts error : {ex}");
					}
					return result;
				}
			}
		}
	}

}


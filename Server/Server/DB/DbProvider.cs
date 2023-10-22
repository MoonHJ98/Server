using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.DB
{
	public static class DbProvider
	{
		public static async Task<Result> GetPlayerByAccount(GetPlayerByAccount _request)
		{
			return await Repository.sp_get_player_by_account(_request);
		}
		public static async Task<Result> CreatePlayer(CreatePlayer _request)
		{
			return await Repository.sp_create_player(_request);
		}
		public static async Task<Result> SetStat(SetStat _request)
		{
			return await Repository.sp_set_stat(_request);
		}
		public static async Task<Result> GetItems(GetItems _request)
		{
			return await Repository.sp_get_items(_request);
		}
		public static async Task<Result> AddItem(AddItem _request)
		{
			return await Repository.sp_add_item(_request);
		}
		public static async Task<Result> ChangeItemEquipState(ChangeItemEquipState _request)
		{
			return await Repository.sp_change_item_equip_state(_request);
		}
	}
}

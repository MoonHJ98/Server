using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
	public class Inventory
	{
		public Dictionary<int, Item> Items = new Dictionary<int, Item>();
		List<int> reservedSlots = new List<int>();

		public void Add(Item item)
		{
			Items.Add(item.ItemId, item);
		}
		public Item Get(int itemId)
		{
			Item item = null;
			Items.TryGetValue(itemId, out item);

			return item;
		}
		public Item Find(Func<Item, bool> condition)
		{
			foreach (var item in Items.Values)
			{
				if (condition(item))
					return item;
			}

			return null;
		}
		public int? GetEmptySlot()
		{
			for (int slot = 0; slot < 20; ++slot)
			{
				Item item = Items.Values.FirstOrDefault(i => i.Slot == slot);
				if (item == null)
				{
					// 예약된 슬롯 찾기
					bool find = false;
					foreach (var reservedSlot in reservedSlots)
					{
						if (reservedSlot == slot)
						{
							find = true;
							break;
						}
					}
					if (find == true)
						continue;

					
					reservedSlots.Add(slot);
					return slot;

				}


			}
			return null;
		}

		public void RemoveReservedSlot(int slot)
		{
			if (reservedSlots == null || reservedSlots.Count == 0)
				return;

			
			reservedSlots.Remove(slot);
		}
	}
}

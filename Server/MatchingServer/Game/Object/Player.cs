
namespace Server.Game.Object
{
	public partial class Player : GameObject
	{
		public ClientSession Session { get; set; }

		public Player()
		{
			ObjectType = GameObjectType.Player;
		}

	}
}

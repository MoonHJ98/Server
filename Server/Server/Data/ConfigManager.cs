using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// 설정 파일들 관리
namespace Server.Data
{
	[Serializable]
	public class ServerConfig
	{
		public string dataPath;
	}
	public class ConfigManager
	{
		public static ServerConfig Config { get; private set; }

		public static void LoadConfig()
		{
			// server.exe와 동일한 경로에서 config.json을 읽어서	
			string text = File.ReadAllText("config.json");
			// ServerConfig로 역직렬화
			Config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);
			
		}
	}
}

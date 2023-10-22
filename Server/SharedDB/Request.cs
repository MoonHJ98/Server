using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
	public class Request
	{

	}
	public class Result
	{

	}

	public class UpdateToken : Request
	{
		public string accountName;
		public int token;
		public DateTime expiredTime;
	}
    public class  UpdateTokenResult : Result
    {
		public enum Result { Success, Fail }
		public Result result;
    }
	public class GetServerInfo : Request
	{
	}
	public class GetServerInfoResult : Result
	{
		public enum Result { Success, Fail }
		public Result result;

		public class Info
		{
			public string serverName;
			public string IpAdress;
			public int Port;
			public int BusyScore;
		}
		public List<Info> infos = new List<Info>();
	}
	public class UpdateServerInfo : Request
	{
		public string serverName;
		public int busyScore;
	}
	public class UpdateServerInfoResult : Result
	{
		public enum Result { Success, Fail }
		public Result result;
	}
}

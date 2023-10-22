using System;
using System.Collections.Generic;
using System.Text;

namespace AccountServer
{
	public class Request
	{

	}
	public class Result
	{

	}
	public class CreateAccount : Request
	{
		public string accountName;
		public string password;
	}
	public class CreateAccountResult : Result
	{
		public string accountName;
		public enum DBResult { Success, Fail }
		public DBResult result;
	}
	public class GetAccount : Request
	{
		public string accountName;
		public string password;
	}
	public class GetAccountResult : Result 
	{
		public string accountName;
		public enum DBResult { Success, Fail }
		public DBResult result;
	}
	
}

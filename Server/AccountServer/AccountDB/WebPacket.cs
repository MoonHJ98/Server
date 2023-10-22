﻿using System.Collections.Generic;
using System.Globalization;

public class CreateAccountPacketReq
{
	public string AccountName { get; set; }
	public string Password { get; set;}
}
public class CreateAccountPacketRes
{
	public bool CreateOk { get; set;}
}
public class LoginAccountPacketReq
{
	public string AccountName { get; set; }
	public string Password { get; set; }
}
public class ServerInfo
{
	public string Name { get; set; }
	public string IpAdress { get; set; }
	public int Port { get; set; }
	public int BusyScore{ get; set; }
}
public class LoginAccountPacketRes
{
	public bool LoginOk { get; set; }
	public List<ServerInfo> ServerInfos { get; set; } = new List<ServerInfo>();
	public string AccountName { get; set; }
	public int Token { get; set; }
}


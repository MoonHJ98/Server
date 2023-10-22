using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class CreateAccountPacketReq
{
	public string AccountName;
	public string Password;
}
public class CreateAccountPacketRes
{
	public bool CreateOk;
}
public class LoginAccountPacketReq
{
	public string AccountName;
	public string Password;
}
public class ServerInfo
{
	public string Name;
	public string IpAdress;
	public int Port;
	public int BusyScore;
}
public class LoginAccountPacketRes
{
	public bool LoginOk;
	public List<ServerInfo> ServerInfos = new List<ServerInfo>();
	public string AccountName;
	public int Token;
}

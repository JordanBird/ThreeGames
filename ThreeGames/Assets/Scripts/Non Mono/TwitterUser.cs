using UnityEngine;
using System.Collections;

[System.Serializable]
public class TwitterUser
{
	public string id = "";
	public string username = "";
	public string displayName = "";

	public int followerCount = 0;
	public int friendsCount = 0;
	public int listedCount = 0;
	public int favouritesCount = 0;
	public int statusesCount = 0;

	public bool verified = false;

	public TwitterUser(string iID, string iUsername, string iDisplayName)
	{
		id = iID;
		username = iUsername;
		displayName = iDisplayName;
	}

	public TwitterUser(string iID, string iUsername, string iDisplayName, int followerCount, int friendsCount, int listedCount, int favouritesCount, int statusesCount)
	{
		id = iID;
		username = iUsername;
		displayName = iDisplayName;

		this.followerCount = followerCount;
		this.friendsCount = friendsCount;
		this.listedCount = listedCount;
		this.favouritesCount = favouritesCount;
		this.statusesCount = statusesCount;
	}

	public TwitterUser(string iID, string iUsername, string iDisplayName, int followerCount, int friendsCount, int listedCount, int favouritesCount, int statusesCount, bool verified)
	{
		id = iID;
		username = iUsername;
		displayName = iDisplayName;
		
		this.followerCount = followerCount;
		this.friendsCount = friendsCount;
		this.listedCount = listedCount;
		this.favouritesCount = favouritesCount;
		this.statusesCount = statusesCount;

		this.verified = verified;
	}
}

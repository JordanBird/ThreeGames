using UnityEngine;
using System.Collections;

public class PartyTweetData
{
	public Tweet tweet;
	public Party party;

	public PartyTweetData (Tweet tweet, Party party)
	{
		this.tweet = tweet;
		this.party = party;
	}
}

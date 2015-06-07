using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class Twitter
{
	public static string oauthconsumerkey = "REFIgn1zA7nlBy447Os5uCxAO";
	public static string oauthtoken = "20883292-iqPfJiFFQADYOA7EL1jyOBZBm8vErXy6LFKzyhGEp";
	public static string oauthconsumersecret = "WYvr2AqXfwAqtcTdVCsNrcAt9btsxz6sAiRThlQ2Wu02z0XrwL";
	public static string oauthtokensecret = "C0TrE24FG7Ck0vJMFezVzzktZQmltRwNy8lLH3g2Vu06A";

	static public void Verify_Credentials()
	{
		string oauthsignaturemethod = "HMAC-SHA1";
		string oauthversion = "1.0";
		string oauthnonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
		TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		string oauthtimestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
		SortedDictionary<string, string> sd = new SortedDictionary<string, string>();
		sd.Add("oauth_version", "1.0");
		sd.Add("oauth_consumer_key", oauthconsumerkey);
		sd.Add("oauth_nonce", oauthnonce);
		sd.Add("oauth_signature_method", "HMAC-SHA1");
		sd.Add("oauth_timestamp", oauthtimestamp);
		sd.Add("oauth_token", oauthtoken);
		//GS - Build the signature string
		string baseString = String.Empty;
		baseString += "GET" + "&";
		baseString += Uri.EscapeDataString(
			"https://api.twitter.com/1.1/account/verify_credentials.json") + "&";
		foreach (KeyValuePair<string, string> entry in sd)
		{
			baseString += Uri.EscapeDataString(entry.Key + "=" + entry.Value + "&");
		}
		
		//Remove the trailing ambersand char(last 3 chars - %26)
		baseString = baseString.Substring(0, baseString.Length - 3);
		
		//Build the signing key
		string signingKey = Uri.EscapeDataString(oauthconsumersecret) +
			"&" + Uri.EscapeDataString(oauthtokensecret);
		
		//Sign the request
		HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
		string oauthsignature = Convert.ToBase64String(
			hasher.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));
		
		//Tell Twitter we don't do the 100 continue thing
		ServicePointManager.Expect100Continue = false;
		
		//authorization header
		HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(
			@"https://api.twitter.com/1.1/account/verify_credentials.json");
		string authorizationHeaderParams = String.Empty;
		authorizationHeaderParams += "OAuth ";
		authorizationHeaderParams += "oauth_nonce=" + "\"" +
			Uri.EscapeDataString(oauthnonce) + "\",";
		authorizationHeaderParams += "oauth_signature_method=" + "\"" +
			Uri.EscapeDataString(oauthsignaturemethod) + "\",";
		authorizationHeaderParams += "oauth_timestamp=" + "\"" +
			Uri.EscapeDataString(oauthtimestamp) + "\",";
		authorizationHeaderParams += "oauth_consumer_key=" + "\"" +
			Uri.EscapeDataString(oauthconsumerkey) + "\",";
		authorizationHeaderParams += "oauth_token=" + "\"" +
			Uri.EscapeDataString(oauthtoken) + "\",";
		authorizationHeaderParams += "oauth_signature=" + "\"" +
			Uri.EscapeDataString(oauthsignature) + "\",";
		authorizationHeaderParams += "oauth_version=" + "\"" +
			Uri.EscapeDataString(oauthversion) + "\"";
		hwr.Headers.Add("Authorization", authorizationHeaderParams);
		hwr.Method = "GET";
		hwr.ContentType = "application/x-www-form-urlencoded";
		
		//Allow us a reasonable timeout in case Twitter's busy
		hwr.Timeout = 3 * 60 * 1000;
		try
		{
			//hwr.Proxy = new WebProxy("enter proxy details/address");
			HttpWebResponse rsp = hwr.GetResponse() as HttpWebResponse;
			Stream dataStream = rsp.GetResponseStream();
			//Open the stream using a StreamReader for easy access.
			StreamReader reader = new StreamReader(dataStream);
			//Read the content.
			string responseFromServer = reader.ReadToEnd();
			Console.WriteLine(responseFromServer);
		}
		catch (Exception ex)
		{
			
		}
	}


	public Tweet[] SearchNonAsync(string searchTerm)
	{
		//Escape searchTerm so Twitter accepts the query.
		searchTerm = System.Uri.EscapeDataString(searchTerm);
		
		string url = "https://api.twitter.com/1.1/search/tweets.json?result_type=recent&include_entities=false&count=100&lang=en&q=" + searchTerm;
		string oauthsignaturemethod = "HMAC-SHA1";
		string oauthversion = "1.0";
		string oauthnonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
		TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		string oauthtimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
		SortedDictionary<string, string> basestringParameters = new SortedDictionary<string, string>();
		basestringParameters.Add("q", searchTerm);
		basestringParameters.Add("result_type", "recent");
		basestringParameters.Add("include_entities", "false");
		basestringParameters.Add("count", "100");
		basestringParameters.Add("lang", "en");
		basestringParameters.Add("oauth_version", oauthversion);
		basestringParameters.Add("oauth_consumer_key", oauthconsumerkey);
		basestringParameters.Add("oauth_nonce", oauthnonce);
		basestringParameters.Add("oauth_signature_method", oauthsignaturemethod);
		basestringParameters.Add("oauth_timestamp", oauthtimestamp);
		basestringParameters.Add("oauth_token", oauthtoken);
		//Build the signature string
		string baseString = String.Empty;
		baseString += "GET" + "&";
		baseString += Uri.EscapeDataString(url.Split('?')[0]) + "&";
		foreach (KeyValuePair<string, string> entry in basestringParameters)
		{
			baseString += Uri.EscapeDataString(entry.Key + "=" + entry.Value + "&");
		}
		
		//Remove the trailing ambersand char last 3 chars - %26
		baseString = baseString.Substring(0, baseString.Length - 3);
		
		//Build the signing key
		string signingKey = Uri.EscapeDataString(oauthconsumersecret) +
			"&" + Uri.EscapeDataString(oauthtokensecret);
		
		//Sign the request
		HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
		string oauthsignature = Convert.ToBase64String(
			hasher.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));
		
		//Tell Twitter we don't do the 100 continue thing
		//http://stackoverflow.com/questions/4926676/mono-webrequest-fails-with-https
		//http://www.mono-project.com/docs/faq/security/
		ServicePointManager.ServerCertificateValidationCallback = Validator; //http://www.mono-project.com/archived/usingtrustedrootsrespectfully/
		ServicePointManager.Expect100Continue = false;
		
		//authorization header
		HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@url);
		string authorizationHeaderParams = String.Empty;
		authorizationHeaderParams += "OAuth ";
		authorizationHeaderParams += "oauth_nonce=" + "\"" +
			Uri.EscapeDataString(oauthnonce) + "\",";
		authorizationHeaderParams += "oauth_signature_method=" + "\"" +
			Uri.EscapeDataString(oauthsignaturemethod) + "\",";
		authorizationHeaderParams += "oauth_timestamp=" + "\"" +
			Uri.EscapeDataString(oauthtimestamp) + "\",";
		authorizationHeaderParams += "oauth_consumer_key=" + "\"" +
			Uri.EscapeDataString(oauthconsumerkey) + "\",";
		authorizationHeaderParams += "oauth_token=" + "\"" +
			Uri.EscapeDataString(oauthtoken) + "\",";
		authorizationHeaderParams += "oauth_signature=" + "\"" +
			Uri.EscapeDataString(oauthsignature) + "\",";
		authorizationHeaderParams += "oauth_version=" + "\"" +
			Uri.EscapeDataString(oauthversion) + "\"";
		webRequest.Headers.Add("Authorization", authorizationHeaderParams);
		
		webRequest.Method = "GET";
		webRequest.ContentType = "application/x-www-form-urlencoded";
		//Allow us a reasonable timeout in case Twitter's busy
		webRequest.Timeout = 3 * 60 * 1000;
		
		string responseFromServer = "";
		
		try
		{
			//Proxy settings
			//webRequest.Proxy = new WebProxy("enter proxy details/address");
			HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse;
			Stream dataStream = webResponse.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			StreamReader reader = new StreamReader(dataStream);
			// Read the content.
			responseFromServer = reader.ReadToEnd();
		}
		catch (Exception ex)
		{
			Debug.Log (ex.Message);
		}

		return ParseSearchResults(responseFromServer);
	}

	public static bool Validator (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}

	public static Tweet[] ParseSearchResults(string searchResults)
	{
		string[] rawTweets = searchResults.Split(new string[] { "\"metadata\"" }, StringSplitOptions.RemoveEmptyEntries);

		if (rawTweets.Length == 0)
			return new Tweet[0];

		Tweet[] tweets = new Tweet[rawTweets.Length - 1];

		for (int i = 1; i < rawTweets.Length; i++)
		{
			string baseRawTweet = rawTweets[i];

			string id = CleanJSONString (baseRawTweet.Split(new string[] { "\",\"id\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]);
			string text = CleanJSONString (baseRawTweet.Split(new string[] { "\",\"text\":\"" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new string[] { "\",\"source\":\"\\" }, StringSplitOptions.RemoveEmptyEntries)[0]);
			
			string userID = CleanJSONString (baseRawTweet.Split(new string[] { ",\"user\":{\"id\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]);
			string userName = CleanJSONString (baseRawTweet.Split(new string[] { "\",\"screen_name\":\"" }, StringSplitOptions.RemoveEmptyEntries)[1].Split('"')[0]);
			string name = CleanJSONString (baseRawTweet.Split(new string[] { "\",\"name\":\"" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new string[] { "\",\"screen_name\":\"" }, StringSplitOptions.RemoveEmptyEntries)[0]);

			int followersCount = int.Parse (CleanJSONString (baseRawTweet.Split(new string[] { "\"followers_count\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]));
			int friendsCount = int.Parse (CleanJSONString (baseRawTweet.Split(new string[] { "\"friends_count\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]));
			int listedCount = int.Parse (CleanJSONString (baseRawTweet.Split(new string[] { "\"listed_count\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]));
			int userFavourites = int.Parse (CleanJSONString (baseRawTweet.Split(new string[] { "\"favourites_count\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]));
			int statusCount = int.Parse (CleanJSONString (baseRawTweet.Split(new string[] { "\"statuses_count\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]));
			bool verified = bool.Parse (CleanJSONString (baseRawTweet.Split(new string[] { "\"verified\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]));

			DateTime createdAt = DateTime.ParseExact (CleanJSONString (baseRawTweet.Split(new string[] { "\"created_at\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0].Replace ("\"", "")), "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);

			int favourites = 0;

			if (baseRawTweet.Contains ("\"favorite_count\":"))
				favourites= int.Parse (CleanJSONString (baseRawTweet.Split(new string[] { "\"favorite_count\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]));

			int retweets = 0;

			if (baseRawTweet.Contains ("\"retweet_count\":"))
				retweets = int.Parse (CleanJSONString (baseRawTweet.Split(new string[] { "\"retweet_count\":" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(',')[0]));

			tweets[i - 1] = new Tweet(id, text, new TwitterUser(userID, userName, name, followersCount, friendsCount, listedCount, userFavourites, statusCount, verified), favourites, retweets);
			tweets[i - 1].createdAt = createdAt;
		}
		
		return tweets;
	}
	
	public static string CleanJSONString(string s)
	{
		//Clean HTML Elements
		//s = WebUtility.HtmlDecode(s);
		
		//Recode Unicode due to // error.
		//http://stackoverflow.com/questions/11137330/replacing-doubleslash-to-single-slash
		MatchEvaluator replacer = m => ((char)int.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)).ToString();
		s = Regex.Replace(s, @"\\u([a-fA-F0-9]{4})", replacer);
		
		//Clean Escape Characters
		s = s.Replace("\\n", "\n");
		s = s.Replace("\\/", "/");
		s = s.Replace("\\\"", "\"");
		
		return s;
	}

	public static string CleanDroppedCharacters(string s)
	{
		s = s.Replace ("'", "%27");

		return s;
	}
}

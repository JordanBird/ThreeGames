using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

public class TwitterManager : MonoBehaviour
{
	public static string oauthconsumerkey = "REFIgn1zA7nlBy447Os5uCxAO";
	public static string oauthtoken = "20883292-iqPfJiFFQADYOA7EL1jyOBZBm8vErXy6LFKzyhGEp";
	public static string oauthconsumersecret = "WYvr2AqXfwAqtcTdVCsNrcAt9btsxz6sAiRThlQ2Wu02z0XrwL";
	public static string oauthtokensecret = "C0TrE24FG7Ck0vJMFezVzzktZQmltRwNy8lLH3g2Vu06A";

	// Use this for initialization
	void Start ()
	{
        //GetBearerToken();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	public static bool Validator (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}

	public WWW GetTrendingHashtagsUserWWW()
	{
		ServicePointManager.ServerCertificateValidationCallback = Validator;
		
		string url = "https://api.twitter.com/1.1/trends/place.json?id=23424975"; //1= Everywhere, 23424975 = UK
		string oauthsignaturemethod = "HMAC-SHA1";
		string oauthversion = "1.0";
		string oauthnonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
		TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		string oauthtimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
		
		WWWForm request = new WWWForm ();
		
		SortedDictionary<string, string> basestringParameters = new SortedDictionary<string, string>();
		basestringParameters.Add("id", "23424975");
		basestringParameters.Add("oauth_version", oauthversion);
		basestringParameters.Add("oauth_consumer_key", oauthconsumerkey);
		basestringParameters.Add("oauth_nonce", oauthnonce);
		basestringParameters.Add("oauth_signature_method", oauthsignaturemethod);
		basestringParameters.Add("oauth_timestamp", oauthtimestamp);
		basestringParameters.Add("oauth_token", oauthtoken);
		
		foreach (KeyValuePair<string, string> p in basestringParameters)
		{
			request.AddField (p.Key, p.Value);
		}
		
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
		
		string signingKey = Uri.EscapeDataString(oauthconsumersecret) +
			"&" + Uri.EscapeDataString(oauthtokensecret);
		
		//Sign the request
		HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
		string oauthsignature = Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));
		
		//Header
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
		//Debug.Log (request.headers.Contains ("Authorization").ToString());
		request.headers.Add ("Authorization", authorizationHeaderParams);
		
		Dictionary<string, string> auth = request.headers;
		
		auth["Authorization"] = authorizationHeaderParams;
		auth["Content-Type"] = "application/x-www-formurlencoded";
		
		WWW theW = new WWW (url, null, auth); //http://forum.unity3d.com/threads/get-method-with-header-in-www-class-unity.88853/

		return theW;
	}

    public string GetBearerToken()
    {
        ServicePointManager.ServerCertificateValidationCallback = Validator;

        string encode = Convert.ToBase64String(new ASCIIEncoding().GetBytes((oauthconsumerkey + ":" + oauthconsumersecret)));

        string authorizationHeaderParams = "Basic " + encode;

        WebClient wc = new WebClient();

        wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded;charset=UTF-8");
        wc.Headers[HttpRequestHeader.Authorization] = authorizationHeaderParams;

        byte[] stuff = wc.UploadData(new Uri("https://api.twitter.com/oauth2/token"), new ASCIIEncoding().GetBytes("grant_type=client_credentials"));

        return "";
    }

    public WWW GetTrendingHashtagsAppWWW()
    {
        ServicePointManager.ServerCertificateValidationCallback = Validator;

		string url = "https://api.twitter.com/1.1/trends/place.json?id=23424975"; //1=WW, 23424975 = 23424975
        string oauthsignaturemethod = "HMAC-SHA1";
        string oauthversion = "1.0";
        string oauthnonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
        TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        string oauthtimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

        WWWForm request = new WWWForm();

        SortedDictionary<string, string> basestringParameters = new SortedDictionary<string, string>();
		basestringParameters.Add("id", "23424975");
        basestringParameters.Add("oauth_version", oauthversion);
        basestringParameters.Add("oauth_consumer_key", oauthconsumerkey);
        basestringParameters.Add("oauth_nonce", oauthnonce);
        basestringParameters.Add("oauth_signature_method", oauthsignaturemethod);
        basestringParameters.Add("oauth_timestamp", oauthtimestamp);
        basestringParameters.Add("oauth_token", oauthtoken);

        foreach (KeyValuePair<string, string> p in basestringParameters)
        {
            request.AddField(p.Key, p.Value);
        }

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

        string signingKey = Uri.EscapeDataString(oauthconsumersecret) +
            "&" + Uri.EscapeDataString(oauthtokensecret);

        //Sign the request
        HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
        string oauthsignature = Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));

        //Header
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
        //Debug.Log (request.headers.Contains ("Authorization").ToString());
        request.headers.Add("Authorization", authorizationHeaderParams);

        Dictionary<string, string> auth = request.headers;

        auth["Authorization"] = authorizationHeaderParams;
        auth["Content-Type"] = "application/x-www-formurlencoded";

        WWW theW = new WWW(url, null, auth);

        return theW;
    }

	private void SearchWC()
	{
		WebClient wc = new WebClient ();
		//Escape searchTerm so Twitter accepts the query.
		string searchTerm = System.Uri.EscapeDataString("GamerGate");
		
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
			wc.Headers[HttpRequestHeader.Authorization] = authorizationHeaderParams;
			wc.DownloadStringCompleted += TweetDataDownloaded;
			wc.DownloadStringAsync (new Uri(url));
		}
		catch (Exception ex)
		{
			Debug.Log (ex.Message);
		}
	}

	public void TweetDataDownloaded(object sender, DownloadStringCompletedEventArgs e)
	{
		Debug.Log ("Data Downloaded");
	}

	public WWW CreateSearchUserWWW(string searchTerm, string resultType)
	{
		searchTerm = Twitter.CleanJSONString (searchTerm);

		ServicePointManager.ServerCertificateValidationCallback = Validator;

		//Clean search term for Twitter search.
		searchTerm = System.Uri.EscapeDataString(searchTerm);
		searchTerm = Twitter.CleanDroppedCharacters (searchTerm);

		string url = "https://api.twitter.com/1.1/search/tweets.json?result_type=" + resultType +"&include_entities=false&count=100&lang=en&q=" + searchTerm;
		string oauthsignaturemethod = "HMAC-SHA1";
		string oauthversion = "1.0";
		string oauthnonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
		TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		string oauthtimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
		
		WWWForm request = new WWWForm ();

		SortedDictionary<string, string> basestringParameters = new SortedDictionary<string, string>();
		basestringParameters.Add("q", searchTerm);
		basestringParameters.Add("result_type", resultType);
		basestringParameters.Add("include_entities", "false");
		basestringParameters.Add("count", "100");
		basestringParameters.Add("lang", "en");
		basestringParameters.Add("oauth_version", oauthversion);
		basestringParameters.Add("oauth_consumer_key", oauthconsumerkey);
		basestringParameters.Add("oauth_nonce", oauthnonce);
		basestringParameters.Add("oauth_signature_method", oauthsignaturemethod);
		basestringParameters.Add("oauth_timestamp", oauthtimestamp);
		basestringParameters.Add("oauth_token", oauthtoken);

		foreach (KeyValuePair<string, string> p in basestringParameters)
		{
			request.AddField (p.Key, p.Value);
		}

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

		string signingKey = Uri.EscapeDataString(oauthconsumersecret) +
			"&" + Uri.EscapeDataString(oauthtokensecret);
		
		//Sign the request
		HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
		string oauthsignature = Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));

		//Header
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
		//Debug.Log (request.headers.Contains ("Authorization").ToString());
		request.headers.Add ("Authorization", authorizationHeaderParams);

		Dictionary<string, string> auth = request.headers;

		auth["Authorization"] = authorizationHeaderParams;
		auth["Content-Type"] = "application/x-www-formurlencoded";

		WWW theW = new WWW (url, null, auth); //http://forum.unity3d.com/threads/get-method-with-header-in-www-class-unity.88853/

		return theW;
	}

	//http://stackoverflow.com/questions/8951489/unity-get-post-wrapper
	private IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		
		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.responseHeaders);
		}
		else
		{
			Debug.Log("WWW Error: "+ www.error);
		}    
	}
}

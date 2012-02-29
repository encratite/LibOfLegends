using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

using FluorineFx.Net;

namespace LibOfLegends
{
	/// <summary>
	/// RESTful API for Auth login stuff
	/// </summary>
	public class AuthService
	{
		public AuthService(string loginQueueURL, Proxy proxy = null)
		{
			LoginQueueURL = loginQueueURL;
			Proxy = proxy;

			if (Proxy != null && Proxy.Type != ProxyType.HTTP)
				throw new NotImplementedException("Proxy not supported for login queue with type other than HTTP");
		}

		/// <summary>
		/// Authenticates with the REST queue service.
		/// Attempts may succeed and allow you straight to LOGIN, or succeed and enter you into a QUEUE.
		/// </summary>
		/// <param name="name">The username</param>
		/// <param name="password">The user's password</param>
		/// <returns></returns>
		public AuthResponse Authenticate(string name, string password)
		{
			// Authenticate with the queue service
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}authenticate", LoginQueueURL));
			request.Method = "POST";

			if (Proxy != null)
			{
				string[] proxyParts = Proxy.Server.Split(':');
				request.Proxy = new WebProxy(proxyParts[0], int.Parse(proxyParts[1]));
			}

			string postBody = string.Format("user={0},password={1}", System.Web.HttpUtility.UrlEncode(name), System.Web.HttpUtility.UrlEncode(password));

			// WRONG! We don't take into account encoding here.
			// But it'll probably work, so write the body to the request.
			request.ContentLength = postBody.Length;
			StreamWriter writer = new StreamWriter(request.GetRequestStream());
			writer.Write(postBody);
			writer.Close();

			// Read the JSON back from the response.
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			StreamReader streamReader = new StreamReader(response.GetResponseStream());
			string json = streamReader.ReadLine();
			streamReader.Close();

			// Deserialize the JSON into an AuthResponse
			JavaScriptSerializer s = new JavaScriptSerializer();
			AuthResponse ar = s.Deserialize<AuthResponse>(json);

			return ar;
		}

		#region Configuration variables

		string LoginQueueURL;
		Proxy Proxy;

		#endregion
	}
}

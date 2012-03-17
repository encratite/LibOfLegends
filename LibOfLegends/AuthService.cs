using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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

		public static bool TrustAllCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			//Console.WriteLine("LibOfLegends TrustAllCertificates:\n{0}", certificate);
			return true;
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
			//This is a hack to ignore certificate issues
			ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificates;

			// Authenticate with the queue service
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/authenticate", LoginQueueURL));
			request.Method = "POST";

			if (Proxy != null)
			{
				string[] proxyParts = Proxy.Server.Split(':');
				request.Proxy = new WebProxy(proxyParts[0], int.Parse(proxyParts[1]));
			}

			string innerPostBody = "user=" + System.Web.HttpUtility.UrlEncode(name) + ",password=" + System.Web.HttpUtility.UrlEncode(password);
			string postBody = "payload=" + System.Web.HttpUtility.UrlEncode(innerPostBody);

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
			JavaScriptSerializer serialiser = new JavaScriptSerializer();
			AuthResponse authResponse = serialiser.Deserialize<AuthResponse>(json);

			return authResponse;
		}

		#region Configuration variables

		string LoginQueueURL;
		Proxy Proxy;

		#endregion
	}
}

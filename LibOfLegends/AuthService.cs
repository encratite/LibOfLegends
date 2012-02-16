using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;




namespace LibOfLegends
{
    /// <summary>
    /// RESTful API for Auth login stuff
    /// </summary>
    public class AuthService
    {
        public static string API = "https://lq.eu.lol.riotgames.com/login-queue/rest/queue/";

        /// <summary>
        /// Authenticates with the REST queue service.
        /// Attempts may succeed and allow you straight to LOGIN, or succeed and enter you into a QUEUE.
        /// </summary>
        /// <param name="name">The username</param>
        /// <param name="password">The user's password</param>
        /// <returns></returns>
        public static AuthResponse Authenticate(string name, string password)
        {
            // Authenticate with the queue service
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}authenticate", API));
            req.Method = "POST";

            string postBody = string.Format("user={0},password={1}", name, password);

            // WRONG! We don't take into account encoding here.
            // But it'll probably work, so write the body to the request.
            req.ContentLength = postBody.Length;
            StreamWriter writer = new StreamWriter(req.GetRequestStream());
            writer.Write(postBody);
            writer.Close();

            // Read the JSON back from the response.
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            string json = sr.ReadLine();
            sr.Close();

            // Deserialize the JSON into an AuthResponse
            JavaScriptSerializer s = new JavaScriptSerializer();
            AuthResponse ar = s.Deserialize<AuthResponse>(json);

            return ar;
        }
    }
}

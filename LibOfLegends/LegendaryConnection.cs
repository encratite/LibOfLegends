using System;

using FluorineFx;
using FluorineFx.Net;
using FluorineFx.Messaging.Api.Service;
using FluorineFx.Messaging.Messages;
using FluorineFx.Messaging.Rtmp;
using FluorineFx.AMF3;
using FluorineFx.IO;
using FluorineFx.Configuration;
using System.Net;
using System.IO;

using FluorineFx.Messaging;

namespace LibOfLegends
{
    public class RPCService
    {
        public RPCService(RegionTag region)
        {
            _region = region;
            _remotingServer = string.Format("rtmps://prod.{0}.lol.riotgames.com:2099/", Regions.HostnameTags[_region]);
        }

        public void Connect(string user, string password, ConnectSubscriber connectSubscriber)
        {
            /// TODO: Make these safe strings and delete as soon as faesible.
            _user = user;
            _password = password;

            _connectSubscriber = connectSubscriber;

            // TODO: Run this in another thread and call back, this is a blocking operation.
            try
            {
                AuthService authService = new AuthService(_region);
                // Get an Auth token (Dumb, assumes no queueing, blocks)
                _authResponse = authService.Authenticate(_user, _password);
            }
            catch (WebException)
            {
                _connectSubscriber(false);
                return;
            }

            // Initialise our rtmps connection
            _netConnection = new NetConnection();
            // We should use AMF3 to behave as closely to the client as possible.
            _netConnection.ObjectEncoding = ObjectEncoding.AMF3;

            // Setup handlers for different network events.
            _netConnection.OnConnect += new ConnectHandler(netConnection_OnConnect);
            _netConnection.OnDisconnect += new DisconnectHandler(netConnection_OnDisconnect);
            _netConnection.NetStatus += new NetStatusHandler(netConnection_NetStatus);

            // Connect to the rtmps server
            _netConnection.Connect(_remotingServer);
        }


        #region Net connection state handlers
        void netConnection_OnDisconnect(object sender, EventArgs e)
        {
            /// TODO: Setup a delegate to call here
        }

        void netConnection_OnConnect(object sender, EventArgs e)
        {
            /// TODO: Check if there was a problem connecting

            // Now that we are connected call the remote login function
            com.riotgames.platform.login.AuthenticationCredentials ad = new com.riotgames.platform.login.AuthenticationCredentials();
            ad.authToken = _authResponse.Token;

            // Older versions
            //ad.clientVersion = "1.50.11_12_20_17_53";
            //ad.clientVersion = "1.52.12_02_07_16_03";
            ad.clientVersion = "1.54.12_02_14_13_07";
            ad.domain = "lolclient.lol.riotgames.com";
            ad.ipAddress = "10.0.0.1";
            ad.locale = "en_GB";
            ad.oldPassword = null;
            ad.partnerCredentials = null;
            ad.securityAnswer = null;
            ad.password = _password;
            ad.username = _user;

            // Add some default headers
            _netConnection.AddHeader(MessageBase.RequestTimeoutHeader, false, 60);
            _netConnection.AddHeader(MessageBase.FlexClientIdHeader, false, Guid.NewGuid().ToString());
            _netConnection.AddHeader(MessageBase.EndpointHeader, false, _endpoint);

            _netConnection.Call(_endpoint, "loginService", null, "login", new Responder<com.riotgames.platform.login.Session>(_OnLogin), ad);
        }

        void netConnection_NetStatus(object sender, NetStatusEventArgs e)
        {
            string level = e.Info["level"] as string;
            /// TODO: Setup a delegate to call here
        }
        #endregion


        #region LoL and Flex login
        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        private void _OnLogin(com.riotgames.platform.login.Session session)
        {
            /// TODO: Convert this function to receive an arbitrary object and check for errors.
            
            // if (error)
            //  _connectSubscriber(false);

            Console.WriteLine(session.token);

            // Client header should be set to the token we received from REST authentication
            _netConnection.AddHeader(MessageBase.FlexClientIdHeader, false, session.token);

            // Create the command message which will do flex authentication
            CommandMessage m = new CommandMessage();
            m.operation = CommandMessage.LoginOperation;
            m.body = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(_user + ":" + session.token));
            m.clientId = _netConnection.ClientId;
            m.correlationId = null;
            m.destination = "";
            m.messageId = Guid.NewGuid().ToString();

            // Perform flex authentication.
            //_netConnection.Call("auth", new Responder<AcknowledgeMessage>(_OnFlexLogin), m);
            _netConnection.Call("auth", new Responder<string>(_OnFlexLogin), m);
        }

        private void _OnFlexLogin(string am)
        {
            /// TODO: Convert this function to receive an arbitrary object and check for errors if necessary.

            _connectSubscriber(true);
        }
        #endregion

        #region RPCs

        private const string _summonerService = "summonerService";
        private const string _playerStatsService = "playerStatsService";

        public void GetSummonerByName(string name, Responder<object> responder)
        {
            _netConnection.Call(_endpoint, _summonerService, null, "getSummonerByName", responder, new object[] { name });
        }

        public void GetRecentGames(int accountID, Responder<object> responder)
        {
            _netConnection.Call(_endpoint, _playerStatsService, null, "getRecentGames", responder, new object[] { accountID });
        }

        #endregion

        #region Delegates
        public delegate void ConnectSubscriber(bool success);
        private ConnectSubscriber _connectSubscriber = null;
#endregion

        #region Client configuration
        private string _user;
        private string _password;
        #endregion

        #region Server constants
        
        private const string _endpoint = "my-rtmps";

        #endregion

        #region Configuration variables

        private string _remotingServer;
        private RegionTag _region;
        
        #endregion

        #region Runtime variables

        public static NetConnection NetConnection { get { return _netConnection; } set { _netConnection = value; } }
        private static NetConnection _netConnection;

        private static AuthResponse _authResponse;

        #endregion
    }
}

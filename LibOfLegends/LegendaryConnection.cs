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
using com.riotgames.platform.clientfacade.domain;
using com.riotgames.platform.login;

namespace LibOfLegends
{
	public class RegionData
	{
		public string LoginQueueURL;
		public string RPCURL;

		public RegionData(string loginQueueURL, string rpcURL)
		{
			LoginQueueURL = loginQueueURL;
			RPCURL = rpcURL;
		}
	}

	public class ConnectionData
	{
		public RegionData RegionData;

		public string User;
		public string Password;

		public ConnectionData(RegionData regionData, string user, string password)
		{
			RegionData = regionData;
			User = user;
			Password = password;
		}
	}

    public class RPCService
    {
        public RPCService(ConnectionData connectionData)
        {
			_connectionData = connectionData;
        }

        public void Connect(ConnectSubscriber connectSubscriber)
        {
            _connectSubscriber = connectSubscriber;

            // TODO: Run this in another thread and call back, this is a blocking operation.
            try
            {
                AuthService authService = new AuthService(_connectionData.RegionData.LoginQueueURL);
                // Get an Auth token (Dumb, assumes no queueing, blocks)
				_authResponse = authService.Authenticate(_connectionData.User, _connectionData.Password);
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
            _netConnection.Connect(_connectionData.RegionData.RPCURL);
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
            ad.password = _connectionData.Password;
			ad.username = _connectionData.User;

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
        private void _OnLogin(Session session)
        {
            /// TODO: Convert this function to receive an arbitrary object and check for errors.
            
            // if (error)
            //  _connectSubscriber(false);

            Console.WriteLine(session.token);

            // Store the session
            _session = session;

            // Client header should be set to the token we received from REST authentication
            _netConnection.AddHeader(MessageBase.FlexClientIdHeader, false, session.token);

            // Create the command message which will do flex authentication
            CommandMessage m = new CommandMessage();
            m.operation = CommandMessage.LoginOperation;
			m.body = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(_connectionData.User + ":" + session.token));
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
            if (am == "success")
                _connectSubscriber(true);
            else
                _connectSubscriber(false);
        }

        #endregion

        #region RPCs

        public void GetSummonerByName(string name, Responder<object> responder)
        {
            _netConnection.Call(_endpoint, _summonerService, null, "getSummonerByName", responder, new object[] { name });
        }

        public void GetRecentGames(int accountID, Responder<object> responder)
        {
            _netConnection.Call(_endpoint, _playerStatsService, null, "getRecentGames", responder, new object[] { accountID });
        }

        public void GetAllPublicSummonerDataByAccount(int accountID, Responder<object> responder)
        {
            _netConnection.Call(_endpoint, _summonerService, null, "getAllPublicSummonerDataByAccount", responder, new object[] { accountID });
        }

        public void GetAllSummonerDataByAccount(int accountID, Responder<object> responder)
        {
            _netConnection.Call(_endpoint, _summonerService, null, "getAllSummonerDataByAccount", responder, new object[] { accountID });
        }

        public void RetrievePlayerStatsByAccountID(int accountID, string season, Responder<object> responder)
        {
            _netConnection.Call(_endpoint, _playerStatsService, null, "retrievePlayerStatsByAccountId", responder, new object[] { accountID, season });
        }

        public void GetAggregatedStats(int accountID, string gameMode, string season, Responder<object> responder)
        {
            _netConnection.Call(_endpoint, _playerStatsService, null, "getAggregatedStats", responder, new object[] { accountID, gameMode, season });
        }

        private void _GetLoginDataPacketForUser(Responder<LoginDataPacket> responder)
        {
            _netConnection.Call(_endpoint, _clientFacadeService, null, "getLoginDataPacketForUser", responder, new object[] { });
        }

        #endregion

        #region Delegates
        public delegate void ConnectSubscriber(bool success);
        private ConnectSubscriber _connectSubscriber = null;
#endregion

        #region Server constants
        
        private const string _endpoint = "my-rtmps";

        private const string _summonerService = "summonerService";
        private const string _playerStatsService = "playerStatsService";
        private const string _clientFacadeService = "clientFacadeService";

        #endregion

        #region Configuration variables

		private ConnectionData _connectionData;
        
        #endregion

        #region Runtime variables

        public NetConnection NetConnection { get { return _netConnection; } set { _netConnection = value; } }
        private NetConnection _netConnection;

        private AuthResponse _authResponse;
        private Session _session;
        private LoginDataPacket _loginDataPacket;
        #endregion
    }
}

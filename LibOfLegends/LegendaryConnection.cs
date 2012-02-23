﻿using System;
using System.Net;
using System.IO;
using System.Threading;

using FluorineFx;
using FluorineFx.Net;
using FluorineFx.Messaging.Messages;

using com.riotgames.platform.clientfacade.domain;
using com.riotgames.platform.login;
using com.riotgames.platform.summoner;
using com.riotgames.platform.statistics;

namespace LibOfLegends
{
	public class RPCService
	{
		public RPCService(ConnectionProfile connectionData)
		{
			_connectionData = connectionData;
		}

		public void Connect(ConnectSubscriber connectSubscriber)
		{
			_connectSubscriber = connectSubscriber;

			// TODO: Run this in another thread and call back, this is a blocking operation.
			try
			{
				AuthService authService = new AuthService(_connectionData.Region.LoginQueueURL, _connectionData.Proxy.LoginQueueProxy);
				// Get an Auth token (Dumb, assumes no queueing, blocks)
				_authResponse = authService.Authenticate(_connectionData.User, _connectionData.Password);
			}
			catch (WebException e)
			{
				_connectSubscriber(false);
				return;
			}

			// Initialise our rtmps connection
			_netConnection = new NetConnection();
			_netConnection.Proxy = _connectionData.Proxy.RTMPProxy;

			// We should use AMF3 to behave as closely to the client as possible.
			_netConnection.ObjectEncoding = ObjectEncoding.AMF3;

			// Setup handlers for different network events.
			_netConnection.OnConnect += new ConnectHandler(netConnection_OnConnect);
			_netConnection.OnDisconnect += new DisconnectHandler(netConnection_OnDisconnect);
			_netConnection.NetStatus += new NetStatusHandler(netConnection_NetStatus);

			// Connect to the rtmps server
			_netConnection.Connect(_connectionData.Region.RPCURL);
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
			com.riotgames.platform.login.AuthenticationCredentials authenticationCredentials = new com.riotgames.platform.login.AuthenticationCredentials();
			authenticationCredentials.authToken = _authResponse.Token;

			authenticationCredentials.clientVersion = _connectionData.Authentication.ClientVersion;
			authenticationCredentials.domain = _connectionData.Authentication.Domain;
			authenticationCredentials.ipAddress = _connectionData.Authentication.IPAddress;
			authenticationCredentials.locale = _connectionData.Authentication.Locale;
			authenticationCredentials.oldPassword = null;
			authenticationCredentials.partnerCredentials = null;
			authenticationCredentials.securityAnswer = null;
			authenticationCredentials.password = _connectionData.Password;
			authenticationCredentials.username = _connectionData.User;

			// Add some default headers
			_netConnection.AddHeader(MessageBase.RequestTimeoutHeader, false, 60);
			_netConnection.AddHeader(MessageBase.FlexClientIdHeader, false, Guid.NewGuid().ToString());
			_netConnection.AddHeader(MessageBase.EndpointHeader, false, _endpoint);

			_netConnection.Call(_endpoint, "loginService", null, "login", new Responder<com.riotgames.platform.login.Session>(_OnLogin), authenticationCredentials);
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

		private void _OnFlexLogin(string message)
		{
			if (message == "success")
				_connectSubscriber(true);
			else
				_connectSubscriber(false);
		}

		#endregion

		#region Internal RPC

		private void GetSummonerByNameInternal(Responder<PublicSummoner> responder, object[] arguments)
		{
			_netConnection.Call(_endpoint, _summonerService, null, "getSummonerByName", responder, arguments);
		}

		private void GetRecentGamesInternal(Responder<RecentGames> responder, object[] arguments)
		{
			_netConnection.Call(_endpoint, _playerStatsService, null, "getRecentGames", responder, arguments);
		}

		public void GetAllPublicSummonerDataByAccountInternal(Responder<object> responder, object[] arguments)
		{
			_netConnection.Call(_endpoint, _summonerService, null, "getAllPublicSummonerDataByAccount", responder, arguments);
		}

		public void GetAllSummonerDataByAccountInternal(Responder<object> responder, object[] arguments)
		{
			_netConnection.Call(_endpoint, _summonerService, null, "getAllSummonerDataByAccount", responder, arguments);
		}

		public void RetrievePlayerStatsByAccountIDInternal(Responder<object> responder, object[] arguments)
		{
			_netConnection.Call(_endpoint, _playerStatsService, null, "retrievePlayerStatsByAccountId", responder, arguments);
		}

		public void GetAggregatedStatsInternal(Responder<object> responder, object[] arguments)
		{
			_netConnection.Call(_endpoint, _playerStatsService, null, "getAggregatedStats", responder, arguments);
		}

		//This call is not exposed to the outside
		private void GetLoginDataPacketForUserInternal(Responder<LoginDataPacket> responder)
		{
			_netConnection.Call(_endpoint, _clientFacadeService, null, "getLoginDataPacketForUser", responder, new object[] {});
		}

		#endregion

		#region Non-blocking RPC

		public void GetSummonerByNameAsync(string name, Responder<PublicSummoner> responder)
		{
			GetSummonerByNameInternal(responder, new object[] { name });
		}

		public void GetRecentGamesAsync(int accountID, Responder<RecentGames> responder)
		{
			GetRecentGamesInternal(responder, new object[] { accountID });
		}

		public void GetAllPublicSummonerDataByAccountAsync(int accountID, Responder<object> responder)
		{
			GetAllPublicSummonerDataByAccountInternal(responder, new object[] { accountID });
		}

		public void GetAllSummonerDataByAccountAsync(int accountID, Responder<object> responder)
		{
			GetAllSummonerDataByAccountInternal(responder, new object[] { accountID });
		}

		public void RetrievePlayerStatsByAccountIDAsync(int accountID, string season, Responder<object> responder)
		{
			RetrievePlayerStatsByAccountIDInternal(responder, new object[] { accountID, season });
		}

		public void GetAggregatedStatsAsync(int accountID, string gameMode, string season, Responder<object> responder)
		{
			GetAggregatedStatsInternal(responder, new object[] { accountID, gameMode, season });
		}

		#endregion

		#region Blocking RPC

		public PublicSummoner GetSummonerByName(string name)
		{
			return (new InternalCallContext<PublicSummoner>(GetSummonerByNameInternal, new object[] { name })).Execute();
		}

		public RecentGames GetRecentGames(int accountID)
		{
			return (new InternalCallContext<RecentGames>(GetRecentGamesInternal, new object[] { accountID })).Execute();
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

		private ConnectionProfile _connectionData;
		
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

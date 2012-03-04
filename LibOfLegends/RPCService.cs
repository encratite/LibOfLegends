using System;
using System.Net;

using FluorineFx;
using FluorineFx.Messaging.Messages;
using FluorineFx.Net;

using com.riotgames.platform.clientfacade.domain;
using com.riotgames.platform.gameclient.domain;
using com.riotgames.platform.login;
using com.riotgames.platform.statistics;
using com.riotgames.platform.summoner;

namespace LibOfLegends
{
	public class RPCService
	{
		#region Delegates

		public delegate void ConnectCallback(RPCConnectResult result);
		ConnectCallback RPCConnectCallback = null;

		#endregion

		#region Server constants

		const string EndpointString = "my-rtmps";

		const string SummonerService = "summonerService";
		const string PlayerStatsService = "playerStatsService";
		const string ClientFacadeService = "clientFacadeService";

		#endregion

		#region Configuration variables

		ConnectionProfile ConnectionData;

		#endregion

		#region Runtime variables

		public NetConnection NetConnection { get { return RPCNetConnection; } set { RPCNetConnection = value; } }
		NetConnection RPCNetConnection;

		AuthResponse AuthResponse;
		Session RPCSession;

		#endregion

		public RPCService(ConnectionProfile connectionData)
		{
			ConnectionData = connectionData;
		}

		public void Connect(ConnectCallback connectCallback)
		{
			RPCConnectCallback = connectCallback;

			// TODO: Run this in another thread and call back, this is a blocking operation.
			try
			{
				AuthService authService = new AuthService(ConnectionData.Region.LoginQueueURL, ConnectionData.Proxy.LoginQueueProxy);
				// Get an Auth token (Dumb, assumes no queueing, blocks)
				AuthResponse = authService.Authenticate(ConnectionData.User, ConnectionData.Password);
			}
			catch (WebException exception)
			{
				RPCConnectCallback(new RPCConnectResult(exception));
				return;
			}

			// Initialise our RTMPS connection
			RPCNetConnection = new NetConnection();
			RPCNetConnection.Proxy = ConnectionData.Proxy.RTMPProxy;

			// We should use AMF3 to behave as closely to the client as possible.
			RPCNetConnection.ObjectEncoding = ObjectEncoding.AMF3;

			// Setup handlers for different network events.
			RPCNetConnection.OnConnect += new ConnectHandler(NetConnectionOnConnect);
			RPCNetConnection.OnDisconnect += new DisconnectHandler(NetConnectionOnDisconnect);
			RPCNetConnection.NetStatus += new NetStatusHandler(NetConnectionNetStatus);

			// Connect to the RTMPS server
			RPCNetConnection.Connect(ConnectionData.Region.RPCURL);
		}


		#region Net connection state handlers

		void NetConnectionOnDisconnect(object sender, EventArgs eventArguments)
		{
			/// TODO: Setup a delegate to call here
		}

		void NetConnectionOnConnect(object sender, EventArgs eventArguments)
		{
			/// TODO: Check if there was a problem connecting

			// Now that we are connected call the remote login function
			AuthenticationCredentials authenticationCredentials = new AuthenticationCredentials();
			authenticationCredentials.authToken = AuthResponse.Token;

			authenticationCredentials.clientVersion = ConnectionData.Authentication.ClientVersion;
			authenticationCredentials.domain = ConnectionData.Authentication.Domain;
			authenticationCredentials.ipAddress = ConnectionData.Authentication.IPAddress;
			authenticationCredentials.locale = ConnectionData.Authentication.Locale;
			authenticationCredentials.oldPassword = null;
			authenticationCredentials.partnerCredentials = null;
			authenticationCredentials.securityAnswer = null;
			authenticationCredentials.password = ConnectionData.Password;
			authenticationCredentials.username = ConnectionData.User;

			// Add some default headers
			RPCNetConnection.AddHeader(MessageBase.RequestTimeoutHeader, false, 60);
			RPCNetConnection.AddHeader(MessageBase.FlexClientIdHeader, false, Guid.NewGuid().ToString());
			RPCNetConnection.AddHeader(MessageBase.EndpointHeader, false, EndpointString);

			RPCNetConnection.Call(EndpointString, "loginService", null, "login", new Responder<Session>(OnLogin, OnLoginFault), authenticationCredentials);
		}

		void NetConnectionNetStatus(object sender, NetStatusEventArgs eventArguments)
		{
			string level = eventArguments.Info["level"] as string;
			/// TODO: Setup a delegate to call here
		}
		#endregion

		#region LoL and Flex login
		
		void OnLogin(Session session)
		{
			// Store the session
			RPCSession = session;

			// Client header should be set to the token we received from REST authentication
			RPCNetConnection.AddHeader(MessageBase.FlexClientIdHeader, false, session.token);

			// Create the command message which will do flex authentication
			CommandMessage message = new CommandMessage();
			message.operation = CommandMessage.LoginOperation;
			message.body = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConnectionData.User + ":" + session.token));
			message.clientId = RPCNetConnection.ClientId;
			message.correlationId = null;
			message.destination = "";
			message.messageId = Guid.NewGuid().ToString();

			// Perform flex authentication.
			RPCNetConnection.Call("auth", new Responder<string>(OnFlexLogin, OnFlexLoginFault), message);
		}

		void OnLoginFault(Fault fault)
		{
			RPCConnectCallback(new RPCConnectResult(RPCConnectResultType.LoginFault, fault));
		}

		void OnFlexLogin(string message)
		{
			RPCConnectCallback(new RPCConnectResult(message));
		}

		void OnFlexLoginFault(Fault fault)
		{
			RPCConnectCallback(new RPCConnectResult(RPCConnectResultType.FlexLoginFault, fault));
		}

		#endregion

		#region Internal RPC

		void GetSummonerByNameInternal(Responder<PublicSummoner> responder, object[] arguments)
		{
			RPCNetConnection.Call(EndpointString, SummonerService, null, "getSummonerByName", responder, arguments);
		}

		void GetRecentGamesInternal(Responder<RecentGames> responder, object[] arguments)
		{
			RPCNetConnection.Call(EndpointString, PlayerStatsService, null, "getRecentGames", responder, arguments);
		}

		public void GetAllPublicSummonerDataByAccountInternal(Responder<AllPublicSummonerDataDTO> responder, object[] arguments)
		{
			RPCNetConnection.Call(EndpointString, SummonerService, null, "getAllPublicSummonerDataByAccount", responder, arguments);
		}

		public void GetAllSummonerDataByAccountInternal(Responder<AllSummonerData> responder, object[] arguments)
		{
			RPCNetConnection.Call(EndpointString, SummonerService, null, "getAllSummonerDataByAccount", responder, arguments);
		}

		public void RetrievePlayerStatsByAccountIDInternal(Responder<PlayerLifeTimeStats> responder, object[] arguments)
		{
			RPCNetConnection.Call(EndpointString, PlayerStatsService, null, "retrievePlayerStatsByAccountId", responder, arguments);
		}

		public void GetAggregatedStatsInternal(Responder<AggregatedStats> responder, object[] arguments)
		{
			RPCNetConnection.Call(EndpointString, PlayerStatsService, null, "getAggregatedStats", responder, arguments);
		}

		//This call is not exposed to the outside
		void GetLoginDataPacketForUserInternal(Responder<LoginDataPacket> responder)
		{
			RPCNetConnection.Call(EndpointString, ClientFacadeService, null, "getLoginDataPacketForUser", responder, new object[] {});
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

		public void GetAllPublicSummonerDataByAccountAsync(int accountID, Responder<AllPublicSummonerDataDTO> responder)
		{
			GetAllPublicSummonerDataByAccountInternal(responder, new object[] { accountID });
		}

		public void GetAllSummonerDataByAccountAsync(int accountID, Responder<AllSummonerData> responder)
		{
			GetAllSummonerDataByAccountInternal(responder, new object[] { accountID });
		}

		public void RetrievePlayerStatsByAccountIDAsync(int accountID, string season, Responder<PlayerLifeTimeStats> responder)
		{
			RetrievePlayerStatsByAccountIDInternal(responder, new object[] { accountID, season });
		}

		public void GetAggregatedStatsAsync(int accountID, string gameMode, string season, Responder<AggregatedStats> responder)
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

		public AllPublicSummonerDataDTO GetAllPublicSummonerDataByAccount(int accountID)
		{
			return (new InternalCallContext<AllPublicSummonerDataDTO>(GetAllPublicSummonerDataByAccountInternal, new object[] { accountID })).Execute();
		}

		public AllSummonerData GetAllSummonerDataByAccount(int accountID)
		{
			return (new InternalCallContext<AllSummonerData>(GetAllSummonerDataByAccountInternal, new object[] { accountID })).Execute();
		}

		public PlayerLifeTimeStats RetrievePlayerStatsByAccountID(int accountID, string season)
		{
			return (new InternalCallContext<PlayerLifeTimeStats>(RetrievePlayerStatsByAccountIDInternal, new object[] { accountID, season })).Execute();
		}

		public AggregatedStats GetAggregatedStats(int accountID, string gameMode, string season)
		{
			return (new InternalCallContext<AggregatedStats>(GetAggregatedStatsInternal, new object[] { accountID, gameMode, season })).Execute();
		}

		#endregion
	}
}

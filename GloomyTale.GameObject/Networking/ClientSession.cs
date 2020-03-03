/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using GloomyTale.GameObject.Networking;
using GloomyTale.Core;
using GloomyTale.Core.Handling;
using GloomyTale.Data;
using GloomyTale.Domain;
using GloomyTale.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace GloomyTale.GameObject
{
    public class ClientSession
    {
        #region Members
        private bool _isWorldServer;
        private Character _character;
        private readonly INetworkSession _client;
        private IDictionary<string[], HandlerMethodReference> _handlerMethods;
        private readonly IList<string> _waitForPacketList = new List<string>();

        // Packetwait Packets
        private int? _waitForPacketsAmount;

        public bool HealthStop;
        private readonly CommunicationServiceEvents _communicationServiceEvents = new CommunicationServiceEvents();

        private readonly char[] PACKET_SPLITTER = { (char)0xFF };

        #endregion

        #region Instantiation

        public ClientSession(INetworkSession client)
        {
            // initialize network client
            _client = client;

            // absolutely new instantiated Client has no SessionId
            SessionId = 0;

            // register for NetworkClient events
            // _client.MessageReceived += OnNetworkClientMessageReceived;

            // start observer for receiving packets
            _client.PacketReceived += Client_OnPacketReceived;

            #region Anti-Cheat Heartbeat

            if (ServerManager.Instance.IsWorldServer)
            {
                RegisterAntiCheat();
            }

            #endregion
        }

        private void Client_OnPacketReceived(object sender, string e)
        {
            try
            {
                HandlePackets(e.Split(PACKET_SPLITTER, StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Client_OnPacketReceived : ", ex);
                Disconnect();
            }
        }
        #endregion

        #region Properties

        public static ThreadSafeGenericLockedList<string> UserLog { get; set; } = new ThreadSafeGenericLockedList<string>();

        public bool IsAntiCheatAlive { get; set; }

        public IDisposable AntiCheatHeartbeatDisposable { get; set; }

        public string LastAntiCheatData { get; set; }

        public string Crc32 { get; set; }

        public Account Account { get; private set; }

        public Character Character
        {
            get
            {
                if (_character == null || !HasSelectedCharacter)
                {
                    // cant access an
                    Logger.Log.Warn("An uninitialized character should not be accessed.");
                }

                return _character;
            }
            private set => _character = value;
        }

        public long ClientId => _client.ClientId;

        public MapInstance CurrentMapInstance { get; set; }

        public IDictionary<string[], HandlerMethodReference> HandlerMethods
        {
            get => _handlerMethods ?? (_handlerMethods = new Dictionary<string[], HandlerMethodReference>());

            set => _handlerMethods = value;
        }

        public bool HasCurrentMapInstance => CurrentMapInstance != null;

        public bool HasSelectedCharacter { get; private set; }

        public bool HasSession => _client != null;

        public string IpAddress => _client.IpAddress.ToString();

        private bool _isAuthenticated;

        public bool IsAuthenticated 
        {
            get
                => _isAuthenticated;
            set
            {
                _isAuthenticated = value;
                _client.SessionId = SessionId;
            }
        }

        public bool IsConnected => _client.IsConnected;

        public bool IsDisposing
        {
            get => _client.IsDisposing;

            set => _client.IsDisposing = value;
        }

        public bool IsOnMap => CurrentMapInstance != null;

        public int LastKeepAliveIdentity { get; set; }

        public DateTime RegisterTime { get; internal set; }

        public int SessionId 
        {
            get => _client.SessionId;
            set => _client.SessionId = value;
        }

        #endregion

        #region Methods

        #region Anti-Cheat

        public void RegisterAntiCheat()
        {
            //if (!AntiCheatHelper.IsAntiCheatEnabled)
            {
                return;
            }

            IsAntiCheatAlive = true;

            AntiCheatHeartbeatDisposable = Observable.Interval(TimeSpan.FromMinutes(1))
                .Subscribe(observer =>
                {
                    if (IsAntiCheatAlive)
                    {
                        LastAntiCheatData = AntiCheatHelper.GenerateData(64);
                        //SendPacket($"ntcp_ac 0 {LastAntiCheatData} {AntiCheatHelper.Sha512(LastAntiCheatData + AntiCheatHelper.ClientKey)}");
                        IsAntiCheatAlive = false;
                    }
                    else
                    {
                        if (Account?.Authority != AuthorityType.Administrator)
                        {
                            Disconnect();
                        }
                    }
                });
        }

        public void BanForCheatUsing(int detectionCode)
        {
            /*bool isFirstTime = !DAOFactory.Instance.PenaltyLogDAO.LoadByAccount(Account.AccountId).Any(s => s.AdminName == "Anti-Cheat")
                && !DAOFactory.Instance.PenaltyLogDAO.LoadByIp(IpAddress).Any(s => s.AdminName == "Anti-Cheat");*/
            String reason ="";
            if (detectionCode == 1)
                reason = "Tried to dupe with bazar";

            PenaltyLogDTO penaltyLog = new PenaltyLogDTO
            {
                AccountId = Account.AccountId,
                Reason = reason,
                Penalty = PenaltyType.Banned,
                DateStart = DateTime.Now,
                DateEnd = DateTime.Now.AddYears(15),
                AdminName = "GloomyTale Sbirri"
            };

            Character.InsertOrUpdatePenalty(penaltyLog);
            Disconnect();
        }

        #endregion

        public void ClearLowPriorityQueue()
        {

        }

        public void Destroy()
        {
            #region Anti-Cheat Heartbeat

            AntiCheatHeartbeatDisposable?.Dispose();

            #endregion

            // unregister from WCF events
            _communicationServiceEvents.CharacterConnectedEvent -= OnOtherCharacterConnected;
            _communicationServiceEvents.CharacterDisconnectedEvent -= OnOtherCharacterDisconnected;

            // do everything necessary before removing client, DB save, Whatever
            if (HasSelectedCharacter)
            {
                Logger.Log.LogUserEvent("CHARACTER_LOGOUT", GenerateIdentity(), "");

                long characterId = Character.CharacterId;

                Character.Dispose();

                // disconnect client
                CommunicationServiceClient.Instance.DisconnectCharacter(ServerManager.Instance.WorldId, characterId);

                // unregister from map if registered
                if (CurrentMapInstance != null)
                {
                    CurrentMapInstance.UnregisterSession(characterId);
                    CurrentMapInstance = null;
                }

                ServerManager.Instance.UnregisterSession(characterId);
            }

            if (Account != null)
            {
                CommunicationServiceClient.Instance.DisconnectAccount(Account.AccountId);
            }
        }

        public void Disconnect()
        {
            // Character?.AntiBotMessageInterval?.Dispose();
            // Character?.AntiBotObservable?.Dispose();
            // Character?.SaveObs?.Dispose();
            _client.DisconnectClient();
        }

        public string GenerateIdentity()
        {
            if (Character != null)
            {
                return $"Character: {Character.Name}";
            }
            return $"Account: {Account.Name}";
        }

        public void Initialize(Type packetHandler, bool isWorldServer)
        {
            _isWorldServer = isWorldServer;

            // dynamically create packethandler references
            GenerateHandlerReferences(packetHandler, isWorldServer);
        }

        public void InitializeAccount(Account account, bool crossServer = false)
        {
            Account = account;
            if (crossServer)
            {
                CommunicationServiceClient.Instance.ConnectCrossServerAccount(ServerManager.Instance.WorldId, account.AccountId, SessionId);
            }
            else
            {
                CommunicationServiceClient.Instance.ConnectAccount(ServerManager.Instance.WorldId, account.AccountId, SessionId);
            }
            IsAuthenticated = true;
        }

        public void ReceivePacket(string packet, bool ignoreAuthority = false)
        {
            string header = packet.Split(' ')[0];
            TriggerHandler(header, $"{LastKeepAliveIdentity} {packet}", false, ignoreAuthority);
            LastKeepAliveIdentity++;
        }

        public void SendPacket(string packet)
        {
            if (!IsDisposing)
            {
                _client.SendPacket(packet);
                if (packet != null && _character != null && HasSelectedCharacter && !packet.StartsWith("cond ") && !packet.StartsWith("mv ")) SendPacket(Character.GenerateCond());
            }
        }

        public void SendPacket(PacketDefinition packet)
        {
            if (!IsDisposing)
            {
                _client.SendPacket(PacketFactory.Serialize(packet));
            }
        }

        public void SendPacketAfter(string packet, int milliseconds)
        {
            if (!IsDisposing)
            {
                Observable.Timer(TimeSpan.FromMilliseconds(milliseconds)).Subscribe(o => SendPacket(packet));
            }
        }

        public void SendPacketFormat(string packet, params object[] param)
        {
            if (!IsDisposing)
            {
                _client.SendPacketFormat(packet, param);
            }
        }

        public void SendPackets(IEnumerable<string> packets)
        {
            if (!IsDisposing)
            {
                _client.SendPackets(packets);
                if (_character != null && HasSelectedCharacter) SendPacket(Character.GenerateCond());
            }
        }

        public void SendPackets(IEnumerable<PacketDefinition> packets)
        {
            if (!IsDisposing)
            {
                packets.ToList().ForEach(s => _client.SendPacket(PacketFactory.Serialize(s)));
            }
        }

        public void SetCharacter(Character character)
        {
            Character = character;
            HasSelectedCharacter = true;

            Logger.Log.LogUserEvent("CHARACTER_LOGIN", GenerateIdentity(), "");

            // register CSC events
            _communicationServiceEvents.CharacterConnectedEvent += OnOtherCharacterConnected;
            _communicationServiceEvents.CharacterDisconnectedEvent += OnOtherCharacterDisconnected;

            // register for servermanager
            ServerManager.Instance.RegisterSession(this);Character.SetSession(this);
        }

        private void GenerateHandlerReferences(Type type, bool isWorldServer)
        {
            IEnumerable<Type> handlerTypes = !isWorldServer
                ? type.Assembly.GetTypes()
                    .Where(t => t.Name.Equals("LoginPacketHandler")) // shitty but it works, reflection?
                : type.Assembly.GetTypes().Where(p =>
                {
                    Type interfaceType = type.GetInterfaces().FirstOrDefault();
                    return interfaceType != null && !p.IsInterface && interfaceType.IsAssignableFrom(p);
                });

            // iterate thru each type in the given assembly
            foreach (Type handlerType in handlerTypes)
            {
                var handler = (IPacketHandler)Activator.CreateInstance(handlerType, this);

                // include PacketDefinition
                foreach (MethodInfo methodInfo in handlerType.GetMethods().Where(x =>
                    x.GetCustomAttributes(false).OfType<PacketAttribute>().Any() ||
                    x.GetParameters().FirstOrDefault()?.ParameterType.BaseType == typeof(PacketDefinition)))
                {
                    List<PacketAttribute> packetAttributes =
                        methodInfo.GetCustomAttributes(false).OfType<PacketAttribute>().ToList();

                    // assume PacketDefinition based handler method
                    if (packetAttributes.Count == 0)
                    {
                        var methodReference = new HandlerMethodReference(
                            DelegateBuilder.BuildDelegate<Action<object, object>>(methodInfo), handler,
                            methodInfo.GetParameters().FirstOrDefault()?.ParameterType);
                        HandlerMethods.Add(methodReference.Identification, methodReference);
                    }
                    else
                    {
                        // assume string based handler method
                        foreach (PacketAttribute packetAttribute in packetAttributes)
                        {
                            var methodReference = new HandlerMethodReference(
                                DelegateBuilder.BuildDelegate<Action<object, object>>(methodInfo), handler,
                                packetAttribute);
                            HandlerMethods.Add(methodReference.Identification, methodReference);
                        }
                    }
                }
            }
        }

        private void ProcessUnAuthedPacket(string sessionPacket)
        {
            string[] sessionParts = sessionPacket.Split(' ');
            if (sessionParts.Length == 0)
            {
                return;
            }

            if (!int.TryParse(sessionParts[0], out int lastka))
            {
                Disconnect();
            }

            LastKeepAliveIdentity = lastka;

            // set the SessionId if Session Packet arrives
            if (sessionParts.Length < 2)
            {
                return;
            }

            if (!int.TryParse(sessionParts[1].Split('\\').FirstOrDefault(), out int sessid))
            {
                return;
            }

            SessionId = sessid;
            Logger.Log.DebugFormat(Language.Instance.GetMessageFromKey("CLIENT_ARRIVED"), SessionId);

            if (!_waitForPacketsAmount.HasValue)
            {
                TriggerHandler("OpenNos.EntryPoint", string.Empty, false);
            }
        }

        /// <summary>
        /// Handle the packet received by the Client.
        /// </summary>
        private void HandlePackets(IEnumerable<string> packets)
        {
            // determine first packet
            if (_isWorldServer && SessionId == 0)
            {
                ProcessUnAuthedPacket(packets.FirstOrDefault());
                return;
            }

            foreach (string packet in packets)
            {
                string packetstring = packet.Replace('^', ' ');
                string[] packetsplit = packetstring.Split(' ');

                // wtf ???
                if (!_isWorldServer)
                {
                    string packetHeader = packetstring.Split(' ')[0];
                    if (string.IsNullOrWhiteSpace(packetHeader))
                    {
                        Disconnect();
                        return;
                    }

                    // simple messaging
                    if (packetHeader[0] == '/' || packetHeader[0] == ':' || packetHeader[0] == ';')
                    {
                        packetHeader = packetHeader[0].ToString();
                        packetstring = packet.Insert(packet.IndexOf(' ') + 2, " ");
                    }

                    TriggerHandler(packetHeader.Replace("#", ""), packetstring, false);
                    return;
                }

                // keep alive
                string nextKeepAliveRaw = packetsplit[0];
                if (!int.TryParse(nextKeepAliveRaw, out int nextKeepaliveIdentity) && nextKeepaliveIdentity != (LastKeepAliveIdentity + 1))
                {
                    Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("CORRUPTED_KEEPALIVE"), IpAddress);
                    _client.DisconnectClient();
                    return;
                }

                if (nextKeepaliveIdentity == 0)
                {
                    if (LastKeepAliveIdentity == ushort.MaxValue)
                    {
                        LastKeepAliveIdentity = nextKeepaliveIdentity;
                    }
                }
                else
                {
                    LastKeepAliveIdentity = nextKeepaliveIdentity;
                }

                if (_waitForPacketsAmount.HasValue)
                {
                    _waitForPacketList.Add(packetstring);
                    string[] packetssplit = packetstring.Split(' ');
                    if (packetssplit.Length > 3 && packetsplit[1] == "DAC")
                    {
                        _waitForPacketList.Add("0 CrossServerAuthenticate");
                    }

                    if (_waitForPacketList.Count != _waitForPacketsAmount)
                    {
                        continue;
                    }

                    _waitForPacketsAmount = null;
                    string queuedPackets = string.Join(" ", _waitForPacketList.ToArray());
                    string header = queuedPackets.Split(' ', '^')[1];
                    TriggerHandler(header, queuedPackets, true);
                    _waitForPacketList.Clear();
                    return;
                }

                if (packetsplit.Length <= 1)
                {
                    continue;
                }

                //if (packetsplit[1][0] == COMMAND_PREFIX)
                {
                    // todo Essentials plugin
                    //_commandsExecutor.HandleCommand(packet.Substring(packet.IndexOf(' ') + 1), this);
                    //return;
                }

                if (packetsplit[1].Length >= 1 &&
                    (packetsplit[1][0] == '/' || packetsplit[1][0] == ':' || packetsplit[1][0] == ';'))
                {
                    packetsplit[1] = packetsplit[1][0].ToString();
                    packetstring = packet.Insert(packet.IndexOf(' ') + 2, " ");
                }

                if (packetsplit[1] != "0")
                {
                    TriggerHandler(packetsplit[1].Replace("#", ""), packetstring, false);
                }
            }
        }

        private void OnOtherCharacterConnected(object sender, EventArgs e)
        {
            if (Character?.IsDisposed != false)
            {
                return;
            }

            Tuple<long, string> loggedInCharacter = (Tuple<long, string>)sender;

            if (Character.IsFriendOfCharacter(loggedInCharacter.Item1) && Character != null && Character.CharacterId != loggedInCharacter.Item1)
            {
                _client.SendPacket(Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("CHARACTER_LOGGED_IN"), loggedInCharacter.Item2), 10));
                _client.SendPacket(Character.GenerateFinfo(loggedInCharacter.Item1, true));
            }

            FamilyCharacter chara = Character.Family?.FamilyCharacters.Find(s => s.CharacterId == loggedInCharacter.Item1);

            if (chara != null && loggedInCharacter.Item1 != Character?.CharacterId)
            {
                _client.SendPacket(Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("CHARACTER_FAMILY_LOGGED_IN"), loggedInCharacter.Item2, Language.Instance.GetMessageFromKey(chara.Authority.ToString().ToUpper())), 10));
            }
        }

        private void OnOtherCharacterDisconnected(object sender, EventArgs e)
        {
            if (Character?.IsDisposed != false)
            {
                return;
            }

            Tuple<long, string> loggedOutCharacter = (Tuple<long, string>)sender;

            if (Character.IsFriendOfCharacter(loggedOutCharacter.Item1) && Character != null && Character.CharacterId != loggedOutCharacter.Item1)
            {
                _client.SendPacket(Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("CHARACTER_LOGGED_OUT"), loggedOutCharacter.Item2), 10));
                _client.SendPacket(Character.GenerateFinfo(loggedOutCharacter.Item1, false));
            }
        }

        private void TriggerHandler(string packetHeader, string packet, bool force, bool ignoreAuthority = false)
        {
            if (ServerManager.Instance.InShutdown)
            {
                return;
            }

            if (IsDisposing)
            {
                Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("CLIENTSESSION_DISPOSING"), packetHeader);
                return;
            }
            string[] key = HandlerMethods.Keys.FirstOrDefault(s => s.Any(m => string.Equals(m, packetHeader, StringComparison.CurrentCultureIgnoreCase)));
            HandlerMethodReference methodReference = key != null ? HandlerMethods[key] : null;
            if (methodReference == null)
            {
                Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("HANDLER_NOT_FOUND"), packetHeader);
                return;
            }

            if (methodReference.HandlerMethodAttribute != null && !force &&
                methodReference.HandlerMethodAttribute.Amount > 1 && !_waitForPacketsAmount.HasValue)
            {
                // we need to wait for more
                _waitForPacketsAmount = methodReference.HandlerMethodAttribute.Amount;
                _waitForPacketList.Add(packet != string.Empty ? packet : $"1 {packetHeader} ");
                return;
            }

            try
            {
                if (!HasSelectedCharacter &&
                    methodReference.ParentHandler.GetType().Name != "CharacterScreenPacketHandler" &&
                    methodReference.ParentHandler.GetType().Name != "LoginPacketHandler")
                {
                    return;
                }

                // call actual handler method
                if (methodReference.PacketDefinitionParameterType == null)
                {
                    methodReference.HandlerMethod(methodReference.ParentHandler, packet);
                    return;
                }

                //check for the correct authority
                if (!IsAuthenticated
                                        || Account.Authority.Equals(AuthorityType.Administrator)
                                        || methodReference.Authorities.Any(a => a.Equals(Account.Authority))
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.User)) && Account.Authority >= AuthorityType.User
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.TMOD)) && Account.Authority >= AuthorityType.TMOD && Account.Authority <= AuthorityType.BA
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.MOD)) && Account.Authority >= AuthorityType.MOD && Account.Authority <= AuthorityType.BA
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.SMOD)) && Account.Authority >= AuthorityType.SMOD && Account.Authority <= AuthorityType.BA
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.TGM)) && Account.Authority >= AuthorityType.TGM
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.GM)) && Account.Authority >= AuthorityType.GM
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.SGM)) && Account.Authority >= AuthorityType.SGM
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.GA)) && Account.Authority >= AuthorityType.GA
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.TM)) && Account.Authority >= AuthorityType.TM
                                        || methodReference.Authorities.Any(a => a.Equals(AuthorityType.CM)) && Account.Authority >= AuthorityType.CM
                                        || Account.Authority == AuthorityType.BitchNiggerFaggot && methodReference.Authorities.Any(a => a.Equals(AuthorityType.User)))
                {

                    object deserializedPacket = PacketFactory.Deserialize(packet,
                        methodReference.PacketDefinitionParameterType, IsAuthenticated);

                    if (deserializedPacket == null && !methodReference.PassNonParseablePacket)
                    {
                        Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("CORRUPT_PACKET"), packetHeader,
                            packet);
                        return;
                    }

                    methodReference.HandlerMethod(methodReference.ParentHandler, deserializedPacket);
                }
            }
            catch (Exception ex)
            {
                // disconnect if something unexpected happens
                Logger.Log.Error("Handler Error SessionId: " + SessionId, ex);
                Disconnect();
            }
        }

        #endregion
    }
}
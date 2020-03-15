using GloomyTale.Communication.RPC;
using GloomyTale.Communication;
using GloomyTale.Master.Datas;
using GloomyTale.Master.Extensions;
using GloomyTale.Master.Managers;
using Grpc.Core;
using GloomyTale.Core;
using GloomyTale.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GloomyTale.DAL.Interface;
using GloomyTale.Domain;
using GloomyTale.DAL;

namespace GloomyTale.Master
{
    public class MasterImpl : global::Master.MasterBase
    {
        private readonly WorldServerCommunicationManager _communicationManager;
        private readonly MaintenanceManager _maintenanceManager;
        private readonly SessionManager _sessionManager;
        private readonly WorldServerManager _worldManager;
        private readonly ICharacterDAO _characterDao;

        public MasterImpl(MaintenanceManager maintenanceManager, WorldServerManager worldManager, SessionManager sessionManager, WorldServerCommunicationManager communicationManager, 
            ICharacterDAO characterDao)
        {
            _maintenanceManager = maintenanceManager;
            _worldManager = worldManager;
            _sessionManager = sessionManager;
            _communicationManager = communicationManager;
            _characterDao = characterDao;
        }

        public override Task<Bool> IsAccountConnected(Long request, ServerCallContext context) => Task.FromResult(_sessionManager.IsConnectedByAccountId(request.Id).ToBool());

        public override Task<Bool> ConnectAccountOnWorld(ConnectAccountOnWorldRequest request, ServerCallContext context) =>
            Task.FromResult(_sessionManager.ConnectAccountOnWorldId(request.WorldId.ToGuid(), request.AccountId).ToBool());

        public override Task<Bool> ConnectCrossServerAccount(ConnectAccountOnWorldRequest request, ServerCallContext context)
        {
            PlayerSession account = _sessionManager.GetByAccountId(request.AccountId);
            if (account != null)
            {
                account.CanSwitchChannel = false;
                account.PreviousChannel = account.ConnectedWorld;
                account.ConnectedWorld = _worldManager.GetWorldById(request.WorldId.ToGuid());
                if (account.ConnectedWorld != null)
                {
                    return Task.FromResult(true.ToBool());
                }
            }
            return Task.FromResult(false.ToBool());
        }

        public override Task<Void> RegisterAccountLogin(RegisterAccountLoginRequest request, ServerCallContext context)
        {
            _sessionManager.ConnectAccount(request.AccountId, request.SessionId, request.AccountName);
            return Task.FromResult(new Void());
        }
        public override Task<Void> DisconnectAccount(Long request, ServerCallContext context)
        {
            _sessionManager.DisconnectByAccountId(request.Id);
            return Task.FromResult(new Void());
        }

        public override Task<Void> PulseAccount(Long request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.Id);
            if (session == null)
            {
                return Task.FromResult(new Void());
            }

            session.LastPulse = DateTime.Now;
            return Task.FromResult(new Void());
        }

        public override Task<Bool> IsCharacterConnected(IsCharacterConnectedRequest request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByCharacterId(request.WorldGroup, request.CharacterId);
            return Task.FromResult(session == null ? true.ToBool() : (session.ConnectedWorld != null).ToBool());
        }

        public override Task<Bool> ConnectCharacter(ConnectCharacterRequest request, ServerCallContext context)
        {
            WorldServer world = _worldManager.GetWorldById(request.Id.ToGuid());
            if (world == null)
            {
                return Task.FromResult(false.ToBool());
            }

            CharacterDTO character = _characterDao.LoadById(request.CharacterId);
            if (character == null)
            {
                return Task.FromResult(false.ToBool());
            }

            // fetch accountId here
            var characterSession = new CharacterSession(character.Name, character.Level, character.Gender.ToString().ToUpper(), character.Class.ToString().ToUpper());
            return Task.FromResult(_sessionManager.ConnectCharacter(world.Id, request.CharacterId, character.AccountId, characterSession).ToBool());
        }

        public override Task<Void> DisconnectCharacter(DisconnectCharacterRequest request, ServerCallContext context)
        {
            WorldServer world = _worldManager.GetWorldById(request.WorldId.ToGuid());
            if (world == null)
            {
                return Task.FromResult(new Void());
            }

            PlayerSession session = _sessionManager.GetByCharacterId(world.WorldGroup, request.CharacterId);

            _sessionManager.DisconnectByAccountId(session.AccountId);
            return Task.FromResult(new Void());
        }

        public override Task<Int> SendMessageToCharacter(MessageToCharacter request, ServerCallContext context)
        {
            WorldServer worldGroup = _worldManager.GetWorldById(request.SourceWorldId.ToGuid());
            if (worldGroup == null)
            {
                return null;
            }
            var returnValue = -1;
            WorldServer sourceWorld = _worldManager.GetWorldById(request.SourceWorldId.ToGuid());
            if (request == null || request.Message == null || sourceWorld == null)
            {
                return null;
            }

            switch (request.Type)
            {
                case messageType.Family:
                case messageType.FamilyChat:
                case messageType.Broadcast:
                    foreach (WorldServer world in _worldManager.GetWorlds().Where(w => w.WorldGroup.Equals(worldGroup.WorldGroup)))
                    {
                        ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                        worldClient.SendMessageToCharacter(new SCSCharacterMessage
                        {
                            DestinationCharacterId = request.DestinationCharacterId,
                            Message = request.Message,
                            Type = (MessageType)request.Type,
                            SourceCharacterId = request.SourceCharacterId,
                            SourceWorldChannelId = (int)request.SourceCharacterId,
                            SourceWorldId = request.SourceWorldId.ToGuid()
                        });
                    }
                    returnValue = -1;
                    return Task.FromResult(returnValue.ToInt());

                /*case messageType.PrivateChat:
                    if (request.DestinationCharacterId != 0)
                    {
                        PlayerSession receiverAccount = _sessionManager.GetByAccountId(request.DestinationCharacterId);
                        if (receiverAccount?.ConnectedWorld != null)
                        {
                            if (sourceWorld.ChannelId == 51 && receiverAccount.ConnectedWorld.ChannelId == 51
                                && DAOFactory.Instance.CharacterDAO.LoadById(request.SourceCharacterId).Faction
                                != DAOFactory.Instance.CharacterDAO.LoadById((long)request.DestinationCharacterId).Faction)
                            {
                                PlayerSession SenderAccount = _sessionManager.GetByAccountId(request.SourceCharacterId);
                                request.Message = $"talk {request.DestinationCharacterId} " + Language.Instance.GetMessageFromKey("CANT_TALK_OPPOSITE_FACTION");
                                request.DestinationCharacterId = request.SourceCharacterId;
                                SenderAccount.ConnectedWorld.
                                returnValue = -1;
                                return Task.FromResult(returnValue.ToInt());
                            }
                            else
                            {
                                receiverAccount.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(request);
                                return Task.FromResult(receiverAcc.ConnectedWorld.ChannelId.ToInt());
                            }
                        }
                    }
                    break;*/
                case messageType.Whisper:
                    if (request.DestinationCharacterId != 0)
                    {
                        PlayerSession receiverAccount = _sessionManager.GetByCharacterId(worldGroup.WorldGroup, request.DestinationCharacterId);
                        if (receiverAccount?.ConnectedWorld != null)
                        {
                            if (sourceWorld.ChannelId == 51 && receiverAccount.ConnectedWorld.ChannelId == 51
                                && DAOFactory.Instance.CharacterDAO.LoadById(request.SourceCharacterId).Faction
                                != DAOFactory.Instance.CharacterDAO.LoadById((long)request.DestinationCharacterId).Faction)
                            {
                                PlayerSession SenderAccount = _sessionManager.GetByAccountId(request.SourceCharacterId);
                                request.Message = $"say 1 {request.SourceCharacterId} 11 {Language.Instance.GetMessageFromKey("CANT_TALK_OPPOSITE_FACTION")}";
                                request.DestinationCharacterId = request.SourceCharacterId;
                                request.Type = messageType.Other;
                                ICommunicationClient world = _communicationManager.GetCommunicationClientByWorldId(receiverAccount.ConnectedWorld.Id);
                                world.SendMessageToCharacter(new SCSCharacterMessage
                                {
                                    DestinationCharacterId = request.DestinationCharacterId,
                                    Message = request.Message,
                                    Type = (MessageType)request.Type,
                                    SourceCharacterId = request.SourceCharacterId,
                                    SourceWorldChannelId = (int)request.SourceCharacterId,
                                    SourceWorldId = request.SourceWorldId.ToGuid()
                                });
                                returnValue = -1;
                                return Task.FromResult(returnValue.ToInt());
                            }
                            else
                            {
                                ICommunicationClient world = _communicationManager.GetCommunicationClientByWorldId(receiverAccount.ConnectedWorld.Id);
                                world.SendMessageToCharacter(new SCSCharacterMessage
                                {
                                    DestinationCharacterId = request.DestinationCharacterId,
                                    Message = request.Message,
                                    Type = (MessageType)request.Type,
                                    SourceCharacterId = request.SourceCharacterId,
                                    SourceWorldChannelId = (int)request.SourceCharacterId,
                                    SourceWorldId = request.SourceWorldId.ToGuid()
                                });
                                return Task.FromResult(receiverAccount.ConnectedWorld.ChannelId.ToInt());
                            }
                        }
                    }
                    break;
                case messageType.WhisperSupport:
                case messageType.WhisperGm:
                    if (request.DestinationCharacterId != 0)
                    {
                        PlayerSession account = _sessionManager.GetByCharacterId(worldGroup.WorldGroup, request.DestinationCharacterId);
                        if (account?.ConnectedWorld != null)
                        {
                            ICommunicationClient world = _communicationManager.GetCommunicationClientByWorldId(account.ConnectedWorld.Id);
                            world.SendMessageToCharacter(new SCSCharacterMessage
                            {
                                DestinationCharacterId = request.DestinationCharacterId,
                                Message = request.Message,
                                Type = (MessageType)request.Type,
                                SourceCharacterId = request.SourceCharacterId,
                                SourceWorldChannelId = (int)request.SourceCharacterId,
                                SourceWorldId = request.SourceWorldId.ToGuid()
                            });
                            return Task.FromResult(account.ConnectedWorld.ChannelId.ToInt());
                        }
                    }
                    break;

                case messageType.Shout:
                    foreach (WorldServer world in _worldManager.GetWorlds().Where(w => w.WorldGroup.Equals(worldGroup.WorldGroup)))
                    {
                        ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                        worldClient.SendMessageToCharacter(new SCSCharacterMessage
                        {
                            DestinationCharacterId = null,
                            Message = request.Message,
                            Type = (MessageType)request.Type,
                            SourceCharacterId = request.SourceCharacterId,
                            SourceWorldChannelId = (int)request.SourceCharacterId,
                            SourceWorldId = request.SourceWorldId.ToGuid()
                        });
                    }
                    returnValue = -1;
                    return Task.FromResult(returnValue.ToInt());

                /*case messageType.Other:
                    AccountConnection receiverAcc = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(request.DestinationCharacterId.Value));
                    if (receiverAcc?.ConnectedWorld != null)
                    {
                        receiverAcc.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>().SendMessageToCharacter(request);
                        return Task.FromResult(receiverAcc.ConnectedWorld.ChannelId.ToInt());
                    }
                    break;*/
            }
            return null;
        }

        public override async Task<Bool> ChangeAuthority(ChangeAuthorityRequest request, ServerCallContext context) => await base.ChangeAuthority(request, context);

        public override Task<Bool> IsLoginPermitted(AccountIdAndSessionIdRequest request, ServerCallContext context)
        {
            return Task.FromResult((_sessionManager.GetByAccountId(request.AccountId) != null).ToBool());
        }

        public override Task<Bool> IsCrossServerLoginPermitted(AccountIdAndSessionIdRequest request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.AccountId);
            if (session == null)
            {
                return Task.FromResult(false.ToBool());
            }

            return Task.FromResult(session.CanSwitchChannel.ToBool());
        }

        public override Task<Void> RegisterCrossServerLogin(RegisterSessionQuery request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.AccountId);
            if (session == null)
            {
                return Task.FromResult(new Void());
            }

            session.CanSwitchChannel = true;
            return Task.FromResult(new Void());
        }

        public override Task<Void> KickSession(AccountIdAndSessionIdRequest request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.AccountId);

            if (session == null)
            {
                return Task.FromResult(new Void());
            }

            ICommunicationClient world = _communicationManager.GetCommunicationClientByWorldId(session.ConnectedWorld.Id);
            world?.KickSession(session.AccountId, session.SessionId);
            return Task.FromResult(new Void());
        }

        public override Task<Bool> IsMasterOnline(Void request, ServerCallContext context) => Task.FromResult(true.ToBool());

        public override Task<Int> RegisterWorldServer(RegisterWorldServerRequest request, ServerCallContext context)
        {
            Guid worldId = request.RegisteredWorldServerInformations.Id.ToGuid();
            SerializableWorldServer serialized = request.RegisteredWorldServerInformations.ToSerializableWorldServer();            
            WorldServer newWorld = _worldManager.RegisterWorldServer(serialized);
            _communicationManager.CreateCommunicationClient(worldId, request.GRPCEndPoint.Ip, request.GRPCEndPoint.Port);
            Logger.Log.Info($"[SERVER_REGISTRATION] {serialized.WorldGroup}:{serialized.Id}:{serialized.ChannelId}");
            return Task.FromResult(newWorld.ChannelId.ToInt());
        }

        public override Task<Void> UnregisterWorldServer(UUID request, ServerCallContext context)
        {
            Guid worldId = request.ToGuid();
            _worldManager.UnregisterWorld(worldId);
            _communicationManager.UnregisterCommunicationClient(worldId);
            return Task.FromResult(new Void());
        }

        public override Task<Int> GetChannelIdByWorldId(UUID request, ServerCallContext context)
        {
            WorldServer world = _worldManager.GetWorldById(request.ToGuid());
            return Task.FromResult(world.ChannelId.ToInt());
        }

        public override Task<Void> Shutdown(Name request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worldServers = _worldManager.GetWorldsByWorldGroup(request.Str);
            foreach (WorldServer world in worldServers)
            {
                ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                worldClient.Shutdown();
            }

            return Task.FromResult(new Void());
        }

        public override Task<Bool> GetMaintenanceState(Void request, ServerCallContext context) => Task.FromResult(_maintenanceManager.GetMaintenanceMode().ToBool());

        public override Task<Void> SetMaintenanceState(Bool request, ServerCallContext context)
        {
            _maintenanceManager.SetMaintenanceMode(request.Boolean);
            return Task.FromResult(new Void());
        }

        public override Task<RegisteredWorldServer> GetPreviousChannelByAccountId(Long request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.Id);
            if (session == null)
            {
                return Task.FromResult(new RegisteredWorldServer());
            }

            return Task.FromResult(session.PreviousChannel.ToSerializableWorldServer().ToRegisteredWorldServer());
        }

        public override Task<RegisteredWorldServer> GetAct4ChannelInfo(Name request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worlds = _worldManager.GetWorldsByWorldGroup(request.Str).Where(s => s.Port == 3439);

            WorldServer world = worlds.FirstOrDefault(s => s.IsAct4);

            if (world != null)
            {
                return Task.FromResult(world.ToSerializableWorldServer().ToRegisteredWorldServer());
            }

            world = worlds.FirstOrDefault();

            return Task.FromResult(world.ToSerializableWorldServer().ToRegisteredWorldServer());
        }

        public override Task<RegisteredWorldServerResponse> RetrieveRegisteredWorldServers(Void request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worlds = _worldManager.GetWorlds();

            List<RegisteredWorldServer> serverList = worlds.Select(s => s.ToSerializableWorldServer().ToRegisteredWorldServer()).ToList();

            return Task.FromResult(new RegisteredWorldServerResponse
            {
                Servers = { serverList }
            });
        }

        public override Task<Void> UpdateFamily(UpdateFamilyRequest request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worldServers = _worldManager.GetWorldsByWorldGroup(request.WorldGroup);
            foreach (WorldServer world in worldServers)
            {
                ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                worldClient.UpdateFamily(request.FamilyId, request.ChangeFaction);
            }

            return Task.FromResult(new Void());
        }

        public override Task<Void> UpdateRelation(UpdateRelationQuery request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worldServers = _worldManager.GetWorldsByWorldGroup(request.WorldGroup);
            foreach (WorldServer world in worldServers)
            {
                ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                worldClient.UpdateRelation(request.RelationId);
            }

            return Task.FromResult(new Void());
        }

        public override Task<Void> UpdateBazaar(UpdateBazaarQuery request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worldServers = _worldManager.GetWorldsByWorldGroup(request.WorldGroup);
            foreach (WorldServer world in worldServers)
            {
                ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                worldClient.UpdateBazaar(request.BazaarItemId);
            }

            return Task.FromResult(new Void());
        }

        public override async Task<Void> Cleanup(Void request, ServerCallContext context) => await base.Cleanup(request, context);
    }
}

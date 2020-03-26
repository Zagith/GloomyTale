using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("game_start")]
    public class GameStartPacket
    {
        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (string.IsNullOrEmpty(packet))
            {
                return;
            }
            GameStartPacket gameStartPacket = new GameStartPacket();
            gameStartPacket.ExecuteHandler(session as ClientSession);
        }

        public static void Register() => PacketFacility.AddHandler(typeof(GameStartPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            #region System Code Lock
            /*if (Session.Character.LockCode != null)
            {
                #region Lock Code
                Session.Character.HeroChatBlocked = true;
                Session.Character.ExchangeBlocked = true;
                Session.Character.WhisperBlocked = true;
                Session.Character.Invisible = true;
                Session.Character.NoAttack = true;
                Session.Character.NoMove = true;
                Session.Character.VerifiedLock = false;
                #endregion
                Session.SendPacket(Session.Character.GenerateSay($"Your account is locked. Please, use $Unlock command.", 12));
            }
            else
            {
                #region Unlock Code
                Session.Character.HeroChatBlocked = false;
                Session.Character.ExchangeBlocked = false;
                Session.Character.WhisperBlocked = false;
                Session.Character.Invisible = false;
                Session.Character.NoAttack = false;
                Session.Character.NoMove = false;
                Session.Character.VerifiedLock = true;
                #endregion
                Session.SendPacket(Session.Character.GenerateSay($"Your account dont have a lock. If you need more security, use $SetLock and a code.", 12));
            }*/
            #endregion
            if (Session?.Character == null || Session.IsOnMap || !Session.HasSelectedCharacter)
            {
                // character should have been selected in SelectCharacter
                return;
            }

            bool shouldRespawn = false;

            if (Session.Character.MapInstance?.Map?.MapTypes != null)
            {
                if (Session.Character.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (short)MapTypeEnum.Act4)
                    && ServerManager.Instance.ChannelId != 51)
                {
                    if (ServerManager.Instance.IsAct4Online())
                    {
                        Session.Character.ChangeChannel(ServerManager.Instance.Configuration.Act4IP, ServerManager.Instance.Configuration.Act4Port, 2);
                        return;
                    }

                    shouldRespawn = true;
                }
            }

            Session.CurrentMapInstance = Session.Character.MapInstance;

            if (ServerManager.Instance.Configuration.SceneOnCreate
                && Session.Character.GeneralLogs.CountLinq(s => s.LogType == "Connection") < 2)
            {
                Session.SendPacket("scene 40");
            }

            Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("WELCOME_SERVER"), ServerManager.Instance.ServerGroup), 10));
            Session.Character.hasVerifiedSecondPassword = true;            
            Session.Character.PinAsk = Observable.Interval(TimeSpan.FromSeconds(15)).Subscribe(x =>
            {
                if (!Session.Character.hasVerifiedSecondPassword)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo("Please enter your pin with $Pw. If you have no pin, use $SetPw"));
                }
            });
            Session.SendPacket(UserInterfaceHelper.GenerateInfo("Please enter your pin with $Pw. If you don't have a pin, use $SetPw"));
            Session.Character.LoadSpeed();
            Session.Character.LoadSkills();
            Session.SendPacket(Session.Character.GenerateTit());
            Session.SendPacket(Session.Character.GenerateSpPoint());
            Session.SendPacket("rsfi 1 1 0 9 0 9");

            Session.Character.Quests?.Where(q => q?.Quest?.TargetMap != null).ToList()
                .ForEach(qst => Session.SendPacket(qst.Quest.TargetPacket()));

            if (Session.Character.Hp <= 0 && (!Session.Character.IsSeal || ServerManager.Instance.ChannelId != 51))
            {
                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
            }
            else
            {
                if (shouldRespawn)
                {
                    RespawnMapTypeDTO resp = Session.Character.Respawn;
                    short x = (short)(resp.DefaultX + ServerManager.RandomNumber(-3, 3));
                    short y = (short)(resp.DefaultY + ServerManager.RandomNumber(-3, 3));
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, resp.DefaultMapId, x, y);
                }
                else
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId);
                }
            }

            Session.SendPacket(Session.Character.GenerateSki());
            Session.SendPacket(
                $"fd {Session.Character.Reputation} 0 {(int)Session.Character.Dignity} {Math.Abs(Session.Character.GetDignityIco())}");
            Session.SendPacket(Session.Character.GenerateFd());
            Session.SendPacket("rage 0 250000");
            Session.SendPacket("rank_cool 0 0 18000");
            ItemInstance specialistInstance = Session.Character.Inventory.LoadBySlotAndType(8, InventoryType.Wear);

            StaticBonusDTO medal = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);

            if (medal != null)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LOGIN_MEDAL"), 12));
            }

            if (Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBasket))
            {
                Session.SendPacket("ib 1278 1");
            }

            if (Session.Character.MapInstance?.Map?.MapTypes?.Any(m => m.MapTypeId == (short)MapTypeEnum.CleftOfDarkness) == true)
            {
                Session.SendPacket("bc 0 0 0");
            }

            if (specialistInstance != null)
            {
                Session.SendPacket(Session.Character.GenerateSpPoint());
            }

            Session.SendPacket("scr 0 0 0 0 0 0");
            for (int i = 0; i < 10; i++)
            {
                Session.SendPacket($"bn {i} {Language.Instance.GetMessageFromKey($"BN{i}")}");
            }

            Session.SendPacket(Session.Character.GenerateExts());
            Session.SendPacket(Session.Character.GenerateMlinfo());
            Session.SendPacket(UserInterfaceHelper.GeneratePClear());

            Session.SendPacket(Session.Character.GeneratePinit());
            Session.SendPackets(Session.Character.GeneratePst());
            Session.SendPacket(Session.Character.GenerateTitle());
            Session.SendPacket("zzim");
            Session.SendPacket(
                $"twk 1 {Session.Character.CharacterId} {Session.Account.Name} {Session.Character.Name} shtmxpdlfeoqkr");

            long? familyId = DAOFactory.FamilyCharacterDAO.LoadByCharacterId(Session.Character.CharacterId)?.FamilyId;
            if (familyId.HasValue)
            {
                Session.Character.Family = ServerManager.Instance.FamilyList[familyId.Value];
            }

            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null)
            {
                if (Session.Character.Faction != (FactionType)Session.Character.Family.FamilyFaction)
                {
                    Session.Character.Faction
                        = (FactionType)Session.Character.Family.FamilyFaction;
                }

                Session.SendPacket(Session.Character.GenerateGInfo());
                Session.SendPackets(Session.Character.GetFamilyHistory());
                Session.SendPacket(Session.Character.GenerateFamilyMember());
                Session.SendPacket(Session.Character.GenerateFamilyMemberMessage());
                Session.SendPacket(Session.Character.GenerateFamilyMemberExp());

                if (!string.IsNullOrWhiteSpace(Session.Character.Family.FamilyMessage))
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo("--- Family Message ---\n" +
                                                         Session.Character.Family.FamilyMessage));
                }
            }
            Session.SendPacket(Session.Character.GetSqst());
            Session.SendPacket("act6");
            Session.SendPacket(Session.Character.GenerateFaction());
            Session.SendPackets(Session.Character.GenerateScP());
            Session.SendPackets(Session.Character.GenerateScN());
#pragma warning disable 618
            Session.Character.GenerateStartupInventory();
#pragma warning restore 618

            Session.SendPacket(Session.Character.GenerateGold());
            Session.SendPackets(Session.Character.GenerateQuicklist());

            string clinit = "clinit";
            string flinit = "flinit";
            string kdlinit = "kdlinit";
            foreach (CharacterDTO character in ServerManager.Instance.TopComplimented)
            {
                clinit +=
                    $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Compliment}|{character.Name}";
            }

            foreach (CharacterDTO character in ServerManager.Instance.TopReputation)
            {
                flinit +=
                    $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Reputation}|{character.Name}";
            }

            foreach (CharacterDTO character in ServerManager.Instance.TopPoints)
            {
                kdlinit +=
                    $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Act4Points}|{character.Name}";
            }

            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateTitleInfo());
            Session.SendPacket(Session.Character.GenerateFinit());
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(clinit);
            Session.SendPacket(flinit);
            Session.SendPacket(kdlinit);

            Session.Character.LastPVPRevive = DateTime.Now;

            IEnumerable<PenaltyLogDTO> warning = DAOFactory.PenaltyLogDAO.LoadByAccount(Session.Character.AccountId)
                .Where(p => p.Penalty == PenaltyType.Warning);
            if (warning.Any())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(
                    string.Format(Language.Instance.GetMessageFromKey("WARNING_INFO"), warning.Count())));
            }

            // finfo - friends info
            Session.Character.LoadMail();
            Session.Character.LoadSentMail();
            Session.Character.DeleteTimeout();

            if (Session.Character.Quests.Any())
            {
                Session.SendPacket(Session.Character.GenerateQuestsPacket());
            }

            if (Session.Character.IsSeal)
            {
                if (ServerManager.Instance.ChannelId == 51)
                {
                    Session.Character.SetSeal();
                }
                else
                {
                    Session.Character.IsSeal = false;
                }
            }

            #region Multi account detection

            bool trapTriggered = false;
            bool possibleUnregisteredException = false;
            long[][] connections = CommunicationServiceClient.Instance.RetrieveOnlineCharacters(Session.Character.CharacterId);
            foreach (long[] connection in connections)
            {
                if (connection != null)
                {
                    CharacterDTO characterDTO = DAOFactory.CharacterDAO.LoadById(connection[0]);
                    if (characterDTO != null)
                    {
                        MultiAccountExceptionDTO exception = DAOFactory.MultiAccountExceptionDAO.LoadByAccount(characterDTO.AccountId);
                        if (exception == null && connections.Length > 3)
                        {
                            Session.Disconnect();
                            trapTriggered = true;
                        }
                        if (exception != null && connections.Length > exception.ExceptionLimit)
                        {
                            Session.Disconnect();
                            possibleUnregisteredException = true;
                        }
                    }
                }
            }
            if (possibleUnregisteredException)
            {
                foreach (ClientSession team in ServerManager.Instance.Sessions.Where(s =>
                s.Account.Authority >= AuthorityType.GM))
                {
                    if (team.HasSelectedCharacter)
                    {
                        team.SendPacket(team.Character.GenerateSay(
                            string.Format("Possible unregistered exception detected for user: " + Session.Character.Name + ", CharacterId: " + Session.Character.CharacterId), 12));
                    }
                }
            }
            if (trapTriggered)
            {
                foreach (ClientSession team in ServerManager.Instance.Sessions.Where(s =>
                s.Account.Authority >= AuthorityType.GM))
                {
                    if (team.HasSelectedCharacter)
                    {
                        team.SendPacket(team.Character.GenerateSay(
                            string.Format("Possible multi account abusing user: " + Session.Character.Name + ", CharacterId: " + Session.Character.CharacterId), 12));
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}

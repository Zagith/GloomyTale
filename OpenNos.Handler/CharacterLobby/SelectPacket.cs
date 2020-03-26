using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.CharacterLobby
{
    [PacketHeader("select")]
    public class SelectPacket
    {
        #region Properties

        public byte Slot { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 3)
            {
                return;
            }
            SelectPacket selectPacket = new SelectPacket();
            if (byte.TryParse(packetSplit[2], out byte slot))
            {
                selectPacket.Slot = slot;
                selectPacket.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(SelectPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            try
            {
                #region Validate Session

                if (Session?.Account == null
                    || Session.HasSelectedCharacter)
                {
                    return;
                }

                #endregion

                #region Load Character

                CharacterDTO characterDTO = DAOFactory.CharacterDAO.LoadBySlot(Session.Account.AccountId, Slot);

                if (characterDTO == null)
                {
                    return;
                }

                Character character = new Character(characterDTO);

                #endregion

                #region Unban Character

                if (ServerManager.Instance.BannedCharacters.Contains(character.CharacterId))
                {
                    ServerManager.Instance.BannedCharacters.RemoveAll(s => s == character.CharacterId);
                }

                #endregion

                #region Initialize Character

                character.Initialize();

                character.MapInstanceId = ServerManager.GetBaseMapInstanceIdByMapId(character.MapId);
                character.PositionX = character.MapX;
                character.PositionY = character.MapY;
                character.Authority = Session.Account.Authority;

                Session.SetCharacter(character);

                #endregion

                #region Load General Logs

                character.GeneralLogs = new ThreadSafeGenericList<GeneralLogDTO>();
                character.GeneralLogs.AddRange(DAOFactory.GeneralLogDAO.LoadByAccount(Session.Account.AccountId)
                    .Where(s => s.LogType == "DailyReward" || s.CharacterId == character.CharacterId).ToList());

                #endregion

                #region Reset SpPoint

                if (!Session.Character.GeneralLogs.Any(s => s.Timestamp == DateTime.Now && s.LogData == "World" && s.LogType == "Connection"))
                {
                    Session.Character.SpAdditionPoint += (int)(Session.Character.SpPoint / 100D * 20D);
                    Session.Character.SpPoint = 10000;
                }

                #endregion

                #region Other Character Stuffs

                Session.Character.Respawns = DAOFactory.RespawnDAO.LoadByCharacter(Session.Character.CharacterId).ToList();
                Session.Character.StaticBonusList = DAOFactory.StaticBonusDAO.LoadByCharacterId(Session.Character.CharacterId).ToList();
                Session.Character.Titles = DAOFactory.CharacterTitleDAO.LoadByCharacterId(Session.Character.CharacterId).ToList();
                Session.Character.LoadInventory();
                Session.Character.LoadQuicklists();
                Session.Character.GenerateMiniland();

                #endregion

                #region Quests

                //if (!DAOFactory.CharacterQuestDAO.LoadByCharacterId(Session.Character.CharacterId).Any(s => s.IsMainQuest)
                //    && !DAOFactory.QuestLogDAO.LoadByCharacterId(Session.Character.CharacterId).Any(s => s.QuestId == 1997))
                //{
                //    CharacterQuestDTO firstQuest = new CharacterQuestDTO
                //    {
                //        CharacterId = Session.Character.CharacterId,
                //        QuestId = 1997,
                //        IsMainQuest = true
                //    };

                //    DAOFactory.CharacterQuestDAO.InsertOrUpdate(firstQuest);
                //}

                DAOFactory.CharacterQuestDAO.LoadByCharacterId(Session.Character.CharacterId).ToList()
                    .ForEach(qst => Session.Character.Quests.Add(new CharacterQuest(qst)));

                #endregion

                #region Fix Partner Slots

                if (character.MaxPartnerCount < 3)
                {
                    character.MaxPartnerCount = 3;
                }

                #endregion

                #region Load Mates

                DAOFactory.MateDAO.LoadByCharacterId(Session.Character.CharacterId).ToList().ForEach(s =>
                {
                    Mate mate = new Mate(s)
                    {
                        Owner = Session.Character
                    };

                    mate.GenerateMateTransportId();
                    mate.Monster = ServerManager.GetNpcMonster(s.NpcMonsterVNum);

                    Session.Character.Mates.Add(mate);
                });

                #endregion

                #region Load Permanent Buff

                Session.Character.LastPermBuffRefresh = DateTime.Now;

                #endregion

                #region CharacterLife

                Session.Character.Life = Observable.Interval(TimeSpan.FromMilliseconds(300))
                    .Subscribe(x => Session.Character.CharacterLife());

                #endregion

                #region Load Amulet

                Observable.Timer(TimeSpan.FromSeconds(1))
                    .Subscribe(o =>
                    {
                        ItemInstance amulet = Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Amulet, InventoryType.Wear);

                        if (amulet?.ItemDeleteTime != null || amulet?.DurabilityPoint > 0)
                        {
                            Session.Character.AddBuff(new Buff(62, Session.Character.Level), Session.Character.BattleEntity);
                        }
                    });

                #endregion

                #region Load Static Buff

                foreach (StaticBuffDTO staticBuff in DAOFactory.StaticBuffDAO.LoadByCharacterId(Session.Character.CharacterId))
                {
                    if (staticBuff.CardId != 319 /* Wedding */)
                    {
                        Session.Character.AddStaticBuff(staticBuff);
                    }
                }

                #endregion

                #region Enter the World

                Session.Character.GeneralLogs.Add(new GeneralLogDTO
                {
                    AccountId = Session.Account.AccountId,
                    CharacterId = Session.Character.CharacterId,
                    IpAddress = Session.IpAddress,
                    LogData = "World",
                    LogType = "Connection",
                    Timestamp = DateTime.Now
                });

                Session.SendPacket("OK");

                CommunicationServiceClient.Instance.ConnectCharacter(ServerManager.Instance.WorldId, character.CharacterId);

#warning TODO remove this comment for allow 2auth system
                /*Session.Character.HasGodMode = true;
                Session.Character.Invisible = true;
                Session.Character.InvisibleGm = true;*/
                character.Channel = ServerManager.Instance;

                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error("Failed selecting the character.", ex);
            }
            finally
            {
                // Suspicious activity detected -- kick!
                if (Session != null && (!Session.HasSelectedCharacter || Session.Character == null))
                {
                    Session.Disconnect();
                }
            }
        }

        #endregion
    }
}

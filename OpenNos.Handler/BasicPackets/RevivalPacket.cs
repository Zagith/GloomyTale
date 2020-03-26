using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("revival")]
    public class RevivalPacket
    {
        #region Properties

        public byte Type { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 3)
            {
                return;
            }
            RevivalPacket packetDefinition = new RevivalPacket();
            if (byte.TryParse(packetSplit[2], out byte type))
            {
                packetDefinition.Type = type;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(RevivalPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            if (Session.Character.Hp > 0)
            {
                return;
            }

            switch (Type)
            {
                case 0:
                    switch (Session.CurrentMapInstance.MapInstanceType)
                    {
                        case MapInstanceType.LodInstance:
                            const int saver = 1211;
                            if (Session.Character.Inventory.CountItem(saver) < 1)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_SAVER"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                            }
                            else
                            {
                                Session.Character.Inventory.RemoveItemAmount(saver);
                                Session.Character.Hp = (int)Session.Character.HPLoad();
                                Session.Character.Mp = (int)Session.Character.MPLoad();
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }

                            break;

                        case MapInstanceType.Act4Demetra:
                        case MapInstanceType.Act4Zanarkand:
                        case MapInstanceType.Act4Orias:
                        case MapInstanceType.Act4Viserion:
                            if (Session.Character.Reputation < Session.Character.Level * 10)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_REPUT"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                            }
                            else
                            {
                                //Session.Character.GetReputation(Session.Character.Level * -10);
                                Session.Character.Hp = (int)Session.Character.HPLoad();
                                Session.Character.Mp = (int)Session.Character.MPLoad();
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }

                            break;

                        case MapInstanceType.CaligorInstance:
                            Session.Character.Hp = (int)Session.Character.HPLoad();
                            Session.Character.Mp = (int)Session.Character.MPLoad();
                            short x = 0;
                            short y = 0;
                            switch (Session.Character.Faction)
                            {
                                case FactionType.Angel:
                                    x = 50;
                                    y = 172;
                                    break;
                                case FactionType.Demon:
                                    x = 130;
                                    y = 172;
                                    break;
                            }
                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.MapInstance.MapInstanceId, x, y);
                            Session.Character.AddBuff(new Buff(169, Session.Character.Level), Session.Character.BattleEntity);
                            break;

                        default:
                            const int seed = 1012;
                            if (Session.Character.Inventory.CountItem(seed) < 10 && Session.Character.Level > 20)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("NOT_ENOUGH_POWER_SEED"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                                Session.SendPacket(
                                    Session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_SEED_SAY"), 0));
                            }
                            else
                            {
                                if (Session.Character.Level > 20)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("SEED_USED"), 10), 10));
                                    Session.Character.Inventory.RemoveItemAmount(seed, 10);
                                    Session.Character.Hp = (int)(Session.Character.HPLoad() / 2);
                                    Session.Character.Mp = (int)(Session.Character.MPLoad() / 2);
                                    Session.Character.AddBuff(new Buff(44, Session.Character.Level), Session.Character.BattleEntity);
                                }
                                else
                                {
                                    Session.Character.Hp = (int)Session.Character.HPLoad();
                                    Session.Character.Mp = (int)Session.Character.MPLoad();
                                }

                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateTp());
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }
                            break;
                    }

                    break;

                case 1:
                    ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        if (Session.Character.Level > 20)
                        {
                            Session.Character.AddBuff(new Buff(44, Session.Character.Level), Session.Character.BattleEntity);
                        }
                    }
                    break;

                case 2:
                    if ((Session.CurrentMapInstance == ServerManager.Instance.ArenaInstance || Session.CurrentMapInstance == ServerManager.Instance.FamilyArenaInstance) &&
                        Session.Character.Gold >= 100)
                    {
                        Session.Character.Hp = (int)Session.Character.HPLoad();
                        Session.Character.Mp = (int)Session.Character.MPLoad();
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateTp());
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                        Session.SendPacket(Session.Character.GenerateStat());
                        Session.Character.Gold -= 100;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.Character.LastPVPRevive = DateTime.Now;
                        Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(observer =>
                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PVP_ACTIVE"), 10)));
                    }
                    else
                    {
                        ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                    }

                    break;
            }
            Session.Character.BattleEntity.SendBuffsPacket();
        }

        #endregion
    }
}

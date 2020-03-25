﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.CommandPackets
{
    [PacketHeader("$RemoveMonster", PassNonParseablePacket = true, Authority = AuthorityType.GA)]
    public class RemoveMobPacket
    {
        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            RemoveMobPacket packetDefinition = new RemoveMobPacket();
            packetDefinition.ExecuteHandler(session as ClientSession, packet);
        }

        public static void Register() => PacketFacility.AddHandler(typeof(RemoveMobPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$RemoveNpcMonster";

        private void ExecuteHandler(ClientSession Session, string packet)
        {
            if (Session.HasCurrentMapInstance)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[RemoveMob]NpcMonsterId: {Session.Character.LastNpcMonsterId}");
                LogHelper.Instance.InsertCommandLog(Session.Character.CharacterId, packet, Session.IpAddress);
                MapMonster monster = Session.CurrentMapInstance.GetMonsterById(Session.Character.LastNpcMonsterId);
                MapNpc npc = Session.CurrentMapInstance.GetNpc(Session.Character.LastNpcMonsterId);
                if (monster != null)
                {
                    int distance = GameObject.Map.GetDistance(new MapCell
                    {
                        X = Session.Character.PositionX,
                        Y = Session.Character.PositionY
                    }, new MapCell
                    {
                        X = monster.MapX,
                        Y = monster.MapY
                    });
                    if (distance > 5)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("TOO_FAR")), 11));
                        return;
                    }

                    if (monster.IsAlive)
                    {
                        Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster,
                            monster.MapMonsterId));
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("MONSTER_REMOVED"), monster.MapMonsterId,
                                monster.Monster.Name[Session.Account.Language], monster.MapId, monster.MapX, monster.MapY), 12));
                        Session.CurrentMapInstance.RemoveMonster(monster);
                        Session.CurrentMapInstance.RemovedMobNpcList.Add(monster);
                        if (DAOFactory.MapMonsterDAO.LoadById(monster.MapMonsterId) != null)
                        {
                            DAOFactory.MapMonsterDAO.DeleteById(monster.MapMonsterId);
                        }
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("MONSTER_NOT_ALIVE")), 11));
                    }
                }
                else if (npc != null)
                {

                    int distance = GameObject.Map.GetDistance(new MapCell
                    {
                        X = Session.Character.PositionX,
                        Y = Session.Character.PositionY
                    }, new MapCell
                    {
                        X = npc.MapX,
                        Y = npc.MapY
                    });
                    if (distance > 5)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("TOO_FAR")), 11));
                        return;
                    }

                    if (!npc.IsMate && !npc.IsDisabled && !npc.IsProtected)
                    {
                        Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Npc, npc.MapNpcId));
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NPCMONSTER_REMOVED"), npc.MapNpcId,
                                npc.Npc.Name[Session.Account.Language], npc.MapId, npc.MapX, npc.MapY), 12));
                        Session.CurrentMapInstance.RemoveNpc(npc);
                        Session.CurrentMapInstance.RemovedMobNpcList.Add(npc);
                        if (DAOFactory.ShopDAO.LoadByNpc(npc.MapNpcId) != null)
                        {
                            DAOFactory.ShopDAO.DeleteByNpcId(npc.MapNpcId);
                        }

                        if (DAOFactory.MapNpcDAO.LoadById(npc.MapNpcId) != null)
                        {
                            DAOFactory.MapNpcDAO.DeleteById(npc.MapNpcId);
                        }
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NPC_CANNOT_BE_REMOVED")), 11));
                    }
                }
            }
        }

        #endregion
    }
}
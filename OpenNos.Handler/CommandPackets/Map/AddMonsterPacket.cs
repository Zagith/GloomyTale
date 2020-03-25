﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.CommandPackets.Map
{
    [PacketHeader("$AddMonster" , Authority = AuthorityType.GA)]
    public class AddMonsterPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public bool IsMoving { get; set; }

        public short MonsterVNum { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (!(session is ClientSession sess))
            {
                return;
            }
            if (packetSplit.Length < 4)
            {
                sess.SendPacket(sess.Character.GenerateSay(ReturnHelp(), 10));
                return;
            }
            AddMonsterPacket packetDefinition = new AddMonsterPacket();
            if (short.TryParse(packetSplit[2], out short vnum))
            {
                packetDefinition._isParsed = true;
                packetDefinition.MonsterVNum = vnum;
                packetDefinition.IsMoving = packetSplit[3] == "1";
            }
            packetDefinition.ExecuteHandler(sess);
            LogHelper.Instance.InsertCommandLog(sess.Character.CharacterId, packet, sess.IpAddress);
        }

        public static void Register() => PacketFacility.AddHandler(typeof(AddMonsterPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$AddMonster VNUM MOVE";

        private void ExecuteHandler(ClientSession Session)
        {
            if (_isParsed)
            {
                Logger.LogUserEvent("GMCOMMAND", Session.GenerateIdentity(),
                    $"[AddMonster]NpcMonsterVNum: {MonsterVNum} IsMoving: {IsMoving}");
                if (!Session.HasCurrentMapInstance)
                {
                    return;
                }

                NpcMonster npcmonster = ServerManager.GetNpcMonster(MonsterVNum);
                if (npcmonster == null)
                {
                    return;
                }

                MapMonsterDTO monst = new MapMonsterDTO
                {
                    MonsterVNum = MonsterVNum,
                    MapY = Session.Character.PositionY,
                    MapX = Session.Character.PositionX,
                    MapId = Session.Character.MapInstance.Map.MapId,
                    Position = Session.Character.Direction,
                    IsMoving = IsMoving,
                    MapMonsterId = ServerManager.Instance.GetNextMobId()
                };
                if (!DAOFactory.MapMonsterDAO.DoesMonsterExist(monst.MapMonsterId))
                {
                    DAOFactory.MapMonsterDAO.Insert(monst);
                    if (DAOFactory.MapMonsterDAO.LoadById(monst.MapMonsterId) is MapMonsterDTO monsterDTO)
                    {
                        MapMonster monster = new MapMonster(monsterDTO);
                        monster.Initialize(Session.CurrentMapInstance);
                        Session.CurrentMapInstance.AddMonster(monster);
                        Session.CurrentMapInstance?.Broadcast(monster.GenerateIn());
                    }
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}
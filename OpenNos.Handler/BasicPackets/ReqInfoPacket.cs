using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("req_info")]
    public class ReqInfoPacket
    {
        #region Properties

        public int? MateVNum { get; set; }

        public long TargetVNum { get; set; }

        public byte Type { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 4)
            {
                return;
            }
            ReqInfoPacket packetDefinition = new ReqInfoPacket();
            if (byte.TryParse(packetSplit[2], out byte type) && long.TryParse(packetSplit[3], out long targetVNum))
            {
                packetDefinition.Type = type;
                packetDefinition.TargetVNum = targetVNum;
                packetDefinition.MateVNum = packetSplit.Length >= 5
                    && int.TryParse(packetSplit[4], out int mateVNum) ? mateVNum : (int?)null;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(ReqInfoPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            if (Session.Character != null)
            {
                switch(Type)
                {
                    case 6:
                        {
                            if (MateVNum.HasValue)
                            {
                                Mate mate = Session.CurrentMapInstance?.Sessions?.FirstOrDefault(s => s?.Character?.Mates != null && s.Character.Mates.Any(o => o.MateTransportId == MateVNum.Value))?
                                    .Character.Mates.FirstOrDefault(m => m.MateTransportId == MateVNum.Value);

                                if (mate != null)
                                {
                                    Session.SendPacket(mate.GenerateEInfo());
                                }
                            }
                        }
                        break;
                    case 5:
                        {
                            NpcMonster npc = ServerManager.GetNpcMonster((short)TargetVNum);

                            if (Session.CurrentMapInstance?.GetMonsterById(Session.Character.LastNpcMonsterId)
                                is MapMonster monster && monster.Monster?.OriginalNpcMonsterVNum == TargetVNum)
                            {
                                npc = ServerManager.GetNpcMonster(monster.Monster.NpcMonsterVNum);
                            }

                            if (npc != null)
                            {
                                Session.SendPacket(npc.GenerateEInfo(Session.Account.Language));
                            }
                        }
                        break;
                    case 12:
                        {
                            if (Session.Character.Inventory != null)
                            {
                                Session.SendPacket(Session.Character.Inventory.LoadBySlotAndType((short)TargetVNum, InventoryType.Equipment)?.GenerateReqInfo());
                            }
                        }
                        break;
                    default:
                        {
                            if (ServerManager.Instance.GetSessionByCharacterId(TargetVNum)?.Character is Character character)
                            {
                                Session.SendPacket(character.GenerateReqInfo());
                            }
                        }
                        break;
                }
            }
        }

        #endregion
    }
}

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("rstart")]
    public class RaidStartPacket
    {
        #region Properties

        public byte Type { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 2)
            {
                return;
            }
            RaidStartPacket packetDefinition = new RaidStartPacket
            {
                Type = packetSplit.Length >= 3 && byte.TryParse(packetSplit[2], out byte type)
                ? type
                : (byte)0
            };
            packetDefinition.ExecuteHandler(session as ClientSession);
        }

        public static void Register() => PacketFacility.AddHandler(typeof(RaidStartPacket), HandlePacket);

        private void ExecuteHandler(ClientSession Session)
        {
            if (Session.Character.Timespace != null)
            {
                if (Type == 1 && Session.Character.Timespace.InstanceBag != null && Session.Character.Timespace.InstanceBag.Lock == false)
                {
                    if (Session.Character.Timespace.SpNeeded?[(byte)Session.Character.Class] != 0)
                    {
                        ItemInstance specialist = Session.Character.Inventory?.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
                        if (specialist == null || specialist.ItemVNum != Session.Character.Timespace.SpNeeded?[(byte)Session.Character.Class])
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TS_SP_NEEDED"), 0));
                            return;
                        }
                    }
                    Session.Character.Timespace.InstanceBag.Lock = true;
                    PReqPacket.HandlePacket(Session, new PReqPacket().ToString());
                    Session.Character.Timespace._mapInstanceDictionary.ToList().SelectMany(s => s.Value.Sessions).Where(s => s.Character?.Timespace != null).ToList().ForEach(s =>
                    {
                        s.Character.GeneralLogs.Add(new GeneralLogDTO
                        {
                            AccountId = s.Account.AccountId,
                            CharacterId = s.Character.CharacterId,
                            IpAddress = s.IpAddress,
                            LogData = s.Character.Timespace.Id.ToString(),
                            LogType = "InstanceEntry",
                            Timestamp = DateTime.Now
                        });
                    });
                }
            }
        }

        #endregion
    }
}

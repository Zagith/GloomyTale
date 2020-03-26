using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Handler.BasicPackets
{
    [PacketHeader("pcl")]
    public class GetGiftPacket
    {
        #region Properties

        public int GiftId { get; set; }

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
            GetGiftPacket packetDefinition = new GetGiftPacket();
            if (byte.TryParse(packetSplit[2], out byte type) && int.TryParse(packetSplit[3], out int giftId))
            {
                packetDefinition.Type = type;
                packetDefinition.GiftId = giftId;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(GetGiftPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
            int giftId = GiftId;
            if (session.Character.MailList.ContainsKey(giftId))
            {
                MailDTO mail = session.Character.MailList[giftId];
                if (Type == 4 && mail.AttachmentVNum != null)
                {
                    if (session.Character.Inventory.CanAddItem((short)mail.AttachmentVNum))
                    {
                        ItemInstance newInv = session.Character.Inventory.AddNewToInventory((short)mail.AttachmentVNum,
                                mail.AttachmentAmount, Upgrade: mail.AttachmentUpgrade,
                                Rare: (sbyte)mail.AttachmentRarity, Design: mail.AttachmentDesign)
                            .FirstOrDefault();
                        if (newInv != null)
                        {
                            if (newInv.Rare != 0)
                            {
                                bool isPartner = (newInv.ItemVNum >= 990 && newInv.ItemVNum <= 992) || (newInv.ItemVNum >= 995 && newInv.ItemVNum <= 997);
                                newInv.SetRarityPoint(isPartner);
                            }

                            session.SendPacket(session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("ITEM_GIFTED"), newInv.Item.Name,
                                    mail.AttachmentAmount), 12));

                            DAOFactory.MailDAO.DeleteById(mail.MailId);

                            session.SendPacket($"parcel 2 1 {giftId}");

                            session.Character.MailList.Remove(giftId);
                        }
                    }
                    else
                    {
                        session.SendPacket("parcel 5 1 0");
                        session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"),
                                0));
                    }
                }
                else if (Type == 5)
                {
                    session.SendPacket($"parcel 7 1 {giftId}");

                    if (DAOFactory.MailDAO.LoadById(mail.MailId) != null)
                    {
                        DAOFactory.MailDAO.DeleteById(mail.MailId);
                    }

                    if (session.Character.MailList.ContainsKey(giftId))
                    {
                        session.Character.MailList.Remove(giftId);
                    }
                }
            }
        }

        #endregion
    }
}

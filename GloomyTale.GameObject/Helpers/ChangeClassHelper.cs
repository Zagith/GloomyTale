using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Helpers
{
    public class ChangeClassHelper
    {
        public void ChangeClass(ClientSession session, ItemInstance inv)
        {
            if (session.Character.HasShopOpened || session.Character.InExchangeOrTrade)
            {
                session.Character.Dispose();
            }
            if (session.Character.IsChangingMapInstance)
            {
                return;
            }

            if (ServerManager.Instance.ChannelId == 51)
            {
                return;
            }

            if (session.Character.LastSkillUse.AddSeconds(20) < DateTime.Now || session.Character.LastDefence.AddSeconds(20) < DateTime.Now)
            {
                if (session.Character.Inventory.All(i => i.Type != InventoryType.Wear))
                {
                    switch (inv.Item.EffectValue)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            session.Character.ChangeClass((ClassType)inv.Item.EffectValue, false, noItem: false);
                            break;
                        case 4:
                            if (session.Character.Level > 79)
                            {
                                session.Character.ChangeClass((ClassType)inv.Item.EffectValue, false, noItem: false);
                            }
                            else
                            {
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("LOW_LEVEL"), 0));
                            }
                            break;
                        default:
                            return;
                    }
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                }
                else
                {
                    session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(
                            Language.Instance.GetMessageFromKey("EQ_NOT_EMPTY"), 0));
                }
            }
            else
            {
                session.SendPacket(
                    session.Character.GenerateSay(
                        Language.Instance.GetMessageFromKey("CANT_USE_THAT_IN_BATTLE"), 10));
            }
        }

        public void SequenzialChangeClass(ClientSession session, bool vip = false)
        {
            if (session.Character.HasShopOpened || session.Character.InExchangeOrTrade)
            {
                session.Character.Dispose();
            }
            if (session.Character.IsChangingMapInstance)
            {
                return;
            }

            if (ServerManager.Instance.ChannelId == 51)
            {
                return;
            }

            StaticBonusDTO vipBonus =
                session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.VIP);
            if (vip == true && vipBonus == null)
            {
                session.SendPacket(session.Character.GenerateSay("You need a Vip packet to use this item.", 11));
                return;
            }

            if (session.Character.LastSkillUse.AddSeconds(20) < DateTime.Now || session.Character.LastDefence.AddSeconds(20) < DateTime.Now)
            {
                if (session.Character.Inventory.All(i => i.Type != InventoryType.Wear))
                {
                    var i = 0;
                    switch (session.Character.Class)
                    {
                        case ClassType.Adventurer:
                            i = 1;
                            break;
                        case ClassType.Swordsman:
                            i = 2;
                            break;
                        case ClassType.Archer:
                            i = 3;
                            break;
                        case ClassType.Magician:
                            if (session.Character.Level > 79)
                            {
                                i = 4;
                            }
                            else
                            {
                                i = 0;
                            }
                            break;
                        default:
                            i = 0;
                            break;
                    }

                    session.Character.ChangeClass((ClassType)i, false, noItem: false);
                }
                else
                {
                    session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(
                            Language.Instance.GetMessageFromKey("EQ_NOT_EMPTY"), 0));
                }
            }
            else
            {
                session.SendPacket(
                    session.Character.GenerateSay(
                        Language.Instance.GetMessageFromKey("CANT_USE_THAT_IN_BATTLE"), 10));
            }
        }
        #region Singleton

        private static ChangeClassHelper _instance;

        public static ChangeClassHelper Instance => _instance ?? (_instance = new ChangeClassHelper());
        #endregion
    }
}

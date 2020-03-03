using GloomyTale.Core;
using GloomyTale.Domain;
using GloomyTale.GameObject.ComponentEntities.Extensions;
using GloomyTale.GameObject.Items.Instance;
using GloomyTale.GameObject.Networking;

namespace GloomyTale.GameObject.Helpers
{
    public class CustomHelper
    {
        #region Method
        
        public void SpeedPerfection(ClientSession Session, SpecialistInstance specialistInstance, ItemInstance inv = null)
        {
            short[] upsuccess = { 50, 40, 30, 20, 10 };

            int[] goldprice = { 5000, 10000, 20000, 50000, 100000 };
            byte[] stoneprice = { 1, 2, 3, 4, 5 };
            short stonevnum;
            byte upmode = 1;
            short SpDamage = 0;
            short SpDefence = 0;
            short SpElement = 0;
            short SpHP = 0;
            short SpFire = 0;
            short SpWater = 0;
            short SpLight = 0;
            short SpDark = 0;
            short Fallimenti = 0;
            short Successi = 0;

            switch (specialistInstance.Item.Morph)
            {
                case 2:
                case 6:
                case 9:
                case 12:
                    stonevnum = 2514;
                    break;

                case 3:
                case 4:
                case 14:
                    stonevnum = 2515;
                    break;

                case 5:
                case 11:
                case 15:
                    stonevnum = 2516;
                    break;

                case 10:
                case 13:
                case 7:
                    stonevnum = 2517;
                    break;

                case 17:
                case 18:
                case 19:
                    stonevnum = 2518;
                    break;

                case 20:
                case 21:
                case 22:
                    stonevnum = 2519;
                    break;

                case 23:
                case 24:
                case 25:
                    stonevnum = 2520;
                    break;

                case 26:
                case 27:
                case 28:
                    stonevnum = 2521;
                    break;

                default:
                    return;
            }

            while (Session.Character.Inventory.CountItem(stonevnum) > 0)
            {
                if (specialistInstance.SpStoneUpgrade > 99)
                {
                    break;
                }
                if (specialistInstance.SpStoneUpgrade > 80)
                {
                    upmode = 5;
                }
                else if (specialistInstance.SpStoneUpgrade > 60)
                {
                    upmode = 4;
                }
                else if (specialistInstance.SpStoneUpgrade > 40)
                {
                    upmode = 3;
                }
                else if (specialistInstance.SpStoneUpgrade > 20)
                {
                    upmode = 2;
                }

                if (Session.Character.Gold < goldprice[upmode - 1])
                {
                    break;
                }
                if (Session.Character.Inventory.CountItem(stonevnum) < stoneprice[upmode - 1])
                {
                    break;
                }
                int rnd = ServerManager.RandomNumber();
                if (rnd < upsuccess[upmode - 1])
                {
                    byte type = (byte)ServerManager.RandomNumber(0, 16), count = 1;
                    if (upmode == 4)
                    {
                        count = 2;
                    }
                    if (upmode == 5)
                    {
                        count = (byte)ServerManager.RandomNumber(3, 6);
                    }
                    if (type < 3)
                    {
                        specialistInstance.SpDamage += count;
                        SpDamage += count;
                    }
                    else if (type < 6)
                    {
                        specialistInstance.SpDefence += count;
                        SpDefence += count;
                    }
                    else if (type < 9)
                    {
                        specialistInstance.SpElement += count;
                        SpElement += count;
                    }
                    else if (type < 12)
                    {
                        specialistInstance.SpHP += count;
                        SpHP += count;
                    }
                    else if (type == 12)
                    {
                        specialistInstance.SpFire += count;
                        SpFire += count;
                    }
                    else if (type == 13)
                    {
                        specialistInstance.SpWater += count;
                        SpWater += count;
                    }
                    else if (type == 14)
                    {
                        specialistInstance.SpLight += count;
                        SpLight += count;
                    }
                    else if (type == 15)
                    {
                        specialistInstance.SpDark += count;
                        SpDark += count;
                    }
                    specialistInstance.SpStoneUpgrade++;
                    Successi++;
                }
                else
                {
                    Fallimenti++;
                }
                Session.SendPacket(specialistInstance.GenerateInventoryAdd());
                Session.Character.Gold -= goldprice[upmode - 1];
                Session.SendPacket(Session.Character.GenerateGold());
                Session.Character.Inventory.RemoveItemAmount(stonevnum, stoneprice[upmode - 1]);
            }
            if (Successi > 0 || Fallimenti > 0)
            {
                if (inv != null)
                {
                    Session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                }
                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(VisualType.Player, Session.Character.CharacterId, 3005), Session.Character.MapX, Session.Character.MapY);
                Session.SendPacket(Session.Character.GenerateSay("-------------Perfection Result-------------", 11));
                Session.SendPacket(Session.Character.GenerateSay("Success: " + Successi, 11));
                Session.SendPacket(Session.Character.GenerateSay("Fail: " + Fallimenti, 11));
                Session.SendPacket(Session.Character.GenerateSay("Attack: " + SpDamage, 11));
                Session.SendPacket(Session.Character.GenerateSay("Defence: " + SpDefence, 11));
                Session.SendPacket(Session.Character.GenerateSay("HP: " + SpHP, 11));
                Session.SendPacket(Session.Character.GenerateSay("Element: " + SpElement, 11));
                Session.SendPacket(Session.Character.GenerateSay("Fire: " + SpFire, 11));
                Session.SendPacket(Session.Character.GenerateSay("Water: " + SpWater, 11));
                Session.SendPacket(Session.Character.GenerateSay("Ligth: " + SpLight, 11));
                Session.SendPacket(Session.Character.GenerateSay("Dark': " + SpDark, 11));
                Session.SendPacket(Session.Character.GenerateSay("-----------------------------------------------", 11));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay("You don't have enough perfection's stones or gold", 10));
            }      
        }

        public void RemovePerfection(ClientSession session, SpecialistInstance SP, ItemInstance inv = null)
        {
            SP.SpFire = 0;
            SP.SpWater = 0;
            SP.SpLight = 0;
            SP.SpDark = 0;
            SP.SpDamage = 0;
            SP.SpDefence = 0;
            SP.SpHP = 0;
            SP.SpElement = 0;
            SP.SpStoneUpgrade = 0;
            session.SendPacket(SP.GenerateInventoryAdd());
            if (inv != null)
            {
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
            }
            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }
        #endregion

        #region Singleton

        private static CustomHelper _instance;

        public static CustomHelper Instance => _instance ?? (_instance = new CustomHelper());
        #endregion
    }
}

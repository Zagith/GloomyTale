using OpenNos.Data;
using OpenNos.Domain;
using System.Linq;

namespace OpenNos.GameObject.Helpers
{
    public static class SideHelper
    {
        public static bool SideFriendRequirements(ClientSession Session, ClientSession session)
        {
            if (Session.Character.Reputation < (long)SideReputPortalType.Side2 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side2)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 2", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side3 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side3)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 3", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side4 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side4)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 4", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side5 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side5)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 5", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side6 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side6)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 6", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side7 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side7)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 7", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side8 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side8)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 8", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side9 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side9)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 9", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side10 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side10)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 10", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side11 && session.CurrentMapInstance?.Map.Side == (int)SideReputMapType.Side11)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 11", 10));
                return false;
            }
            if (Session.Character.Inventory.CountItem(15120) < 1 && (session.CurrentMapInstance?.Map.MapId == 9990 || session.CurrentMapInstance?.Map.MapId == 46))
            {
                Session.SendPacket(Session.Character.GenerateSay($"This map requires the object: Key of the Eternal Redemption!", 10));
                return false;
            }
            StaticBonusDTO vipBonus =
                Session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.VIP);
            if ((vipBonus == null && Session.Character.Inventory.CountItem(15124) < 1) && session.CurrentMapInstance?.Map.MapId == 106)
            {
                Session.SendPacket(Session.Character.GenerateSay($"This map requires the object: Key of the Monthly Pass and VIP bonus!", 10));
            }
            return true;
        }

        public static bool SidePortalRequirement(ClientSession Session, PortalDTO portal)
        {
            if (portal.RequiredItem != 0)
            {
                if (Session.Character.Inventory.CountItem(portal.RequiredItem) == 0)
                {
                    Session.SendPacket(Session.Character.GenerateSay("This map requires the object: " + portal.NomeOggetto + "!", 10));
                    return false;
                }
            }
            StaticBonusDTO vipBonus =
                Session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.VIP);
            if (portal.DestinationMapId == 106 && vipBonus == null)
            {
                Session.SendPacket(Session.Character.GenerateSay($"This map requires the object: " + portal.NomeOggetto + "and VIP bonus!", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side1 && portal.Side == (int)SideReputMapType.Side1)
            {
                if (Session.Character.Level < 25)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 25", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 1", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side2 && portal.Side == (int)SideReputMapType.Side2)
            {
                if (Session.Character.Level < 35)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 35", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 2", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side3 && portal.Side == (int)SideReputMapType.Side3)
            {
                if (Session.Character.Level < 45)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 45", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 3", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side4 && portal.Side == (int)SideReputMapType.Side4)
            {
                if (Session.Character.Level < 55)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 55", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 4", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side5 && portal.Side == (int)SideReputMapType.Side5)
            {
                if (Session.Character.Level < 65)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 65", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 5", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side6 && portal.Side == (int)SideReputMapType.Side6)
            {
                if (Session.Character.Level < 75)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 75", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 6", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side7 && portal.Side == (int)SideReputMapType.Side7)
            {
                if (Session.Character.Level < 85)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 85", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 7", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side8 && portal.Side == (int)SideReputMapType.Side8)
            {
                if (Session.Character.Level < 90)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 90", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 8", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side9 && portal.Side == (int)SideReputMapType.Side9)
            {
                if (Session.Character.Level < 93)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 93", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 9", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side10 && portal.Side == (int)SideReputMapType.Side10)
            {
                if (Session.Character.Level < 93)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 93", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 10", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side10 && portal.Side == (int)SideReputMapType.Side11)
            {
                if (Session.Character.Level < 95)
                {
                    Session.SendPacket(Session.Character.GenerateSay($"You need the Level 95", 10));
                    return false;
                }
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 10", 10));
                return false;
            }
            return true;
        }
    }
}

using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Helpers
{
    public static class SideHelper
    {
        public static bool SideFriendRequirements(ClientSession Session, ClientSession session)
        {
            if (Session.Character.Reputation < (long)SideReputPortalType.Side2 && session.CurrentMapInstance.Map.Side == (int)SideReputMapType.Side2)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 2", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side3 && session.CurrentMapInstance.Map.Side == (int)SideReputMapType.Side3)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 3", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side4 && session.CurrentMapInstance.Map.Side == (int)SideReputMapType.Side4)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 4", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side5 && session.CurrentMapInstance.Map.Side == (int)SideReputMapType.Side5)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 5", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side6 && session.CurrentMapInstance.Map.Side == (int)SideReputMapType.Side6)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 6", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side7 && session.CurrentMapInstance.Map.Side == (int)SideReputMapType.Side7)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 7", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side8 && session.CurrentMapInstance.Map.Side == (int)SideReputMapType.Side8)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 8", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side9 && session.CurrentMapInstance.Map.Side == (int)SideReputMapType.Side9)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 9", 10));
                return false;
            }
            if (Session.Character.Reputation < (long)SideReputPortalType.Side10 && session.CurrentMapInstance.Map.Side == (int)SideReputMapType.Side10)
            {
                Session.SendPacket(Session.Character.GenerateSay($"You need the Side Set 10", 10));
                return false;
            }
            return true;
        }
    }
}

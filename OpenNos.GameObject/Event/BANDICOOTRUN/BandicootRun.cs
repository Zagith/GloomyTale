using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Event.BANDICOOTRUN
{
    public static class BandicootRun
    {
        public static void GenerateBandicootRun()
        {
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("BANDICOOTRUN_OPEN_REGISTER")), 0));
            ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("BANDICOOTRUN_OPEN_REGISTER")), 1));
            ServerManager.Instance.CanRegisterBandicootRun = true;
            Thread.Sleep(1 * 40 * 100);
            ServerManager.Instance.CanRegisterBandicootRun = false;
            Thread.Sleep(1 * 100);
            ServerManager.Instance.EventInWaiting = true;
            Thread.Sleep(1 * 100);
            ServerManager.Instance.Sessions.Where(s => s.Character?.IsWaitingForEvent == false).ToList().ForEach(s => s.SendPacket("esf"));
            ServerManager.Instance.EventInWaiting = false;
            IEnumerable<BandicootMember> sessions = ServerManager.Instance.BandicootMembersRegistered.Where(s => s.Session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance);

            ServerManager.Instance.BandicootMembersRegistered = new List<BandicootMember>();
            ServerManager.Instance.RainbowBattleMembers = new List<RainbowBattleMember>();

            List<Tuple<MapInstance, byte>> maps = new List<Tuple<MapInstance, byte>>();
            MapInstance map = ServerManager.GenerateMapInstance(2004, MapInstanceType.EventGameInstance, new InstanceBag());
            maps.Add(new Tuple<MapInstance, byte>(map, 1));
           /* if (map != null)
            {
                foreach (BandicootMember sess in sessions)
                {
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(sess, map.MapInstanceId);
                }

                ServerManager.Instance.Sessions.Where(s => s.Character != null).ToList().ForEach(s => s.Character.IsWaitingForEvent = false);
                ServerManager.Instance.StartedEvents.Remove(EventType.METEORITEGAME);

            }

            if (map.Sessions.Count() < MiniPlayerForStart)
            {
                map.Broadcast(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANTBATTLE_NOT_ENOUGH_PLAYERS"), 0));
                EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(5), new EventContainer(map, EventActionType.DISPOSEMAP, null));
                return;
            }
            MeteoriteGameThread task = new MeteoriteGameThread();
            Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(X => task.Run(map));*/
        }
    }
}

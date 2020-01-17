/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using GloomyTale.DAL;
using GloomyTale.Data;
using GloomyTale.Domain;
using System;
using System.Linq;
using GloomyTale.GameObject.Networking;
using GloomyTale.GameObject.Networking;

namespace GloomyTale.GameObject.Event
{
    public static class MinilandRefresh
    {
        #region Methods

        public static void GenerateMinilandEvent()
        {
            ServerManager.Instance.SaveAll();
            foreach (CharacterDTO chara in DAOFactory.Instance.CharacterDAO.LoadAll())
            {
                GeneralLogDTO gen = DAOFactory.Instance.GeneralLogDAO.LoadByAccount(null).LastOrDefault(s => s.LogData == nameof(MinilandRefresh) && s.LogType == "World" && s.Timestamp.Day == DateTime.Now.Day);
                int count = DAOFactory.Instance.GeneralLogDAO.LoadByAccount(chara.AccountId).Count(s => s.LogData == "MINILAND" && s.Timestamp > DateTime.Now.AddDays(-1) && s.CharacterId == chara.CharacterId);

                ClientSession Session = ServerManager.Instance.GetSessionByCharacterId(chara.CharacterId);
                if (Session != null)
                {
                    //Session.Character.GetReputation(2 * count);
                    Session.Character.MinilandPoint = 2000;
                }
                else if (CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, chara.CharacterId))
                {
                    if (gen == null)
                    {
                        //chara.Reputation += 2 * count;
                    }
                    chara.MinilandPoint = 2000;
                    CharacterDTO chara2 = chara;
                    DAOFactory.Instance.CharacterDAO.InsertOrUpdate(ref chara2);
                }
            }
            DAOFactory.Instance.GeneralLogDAO.Insert(new GeneralLogDTO { LogData = nameof(MinilandRefresh), LogType = "World", Timestamp = DateTime.Now });
            ServerManager.Instance.StartedEvents.Remove(EventType.MINILANDREFRESHEVENT);
        }

        #endregion
    }
}
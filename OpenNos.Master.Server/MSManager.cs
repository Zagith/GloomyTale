﻿/*
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

using OpenNos.Core;
using OpenNos.Master.Library.Data;
using OpenNos.SCS.Communication.ScsServices.Service;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace OpenNos.Master.Server
{
    internal class MSManager
    {
        #region Members

        private static MSManager _instance;

        #endregion

        #region Instantiation

        public MSManager()
        {
            //DiscordServers = new List<DiscordServer>();
            WorldServers = new List<WorldServer>();
            LoginServers = new List<IScsServiceClient>();
            ConnectedAccounts = new ThreadSafeGenericList<AccountConnection>();
            AuthentificatedClients = new ThreadSafeGenericLockedList<long>();
            ConfigurationObject = new ConfigurationObject
            {
                RateXP = int.Parse(ConfigurationManager.AppSettings["RateXp"]),
                RateHeroicXP = int.Parse(ConfigurationManager.AppSettings["RateHeroicXp"]),
                RateDrop = int.Parse(ConfigurationManager.AppSettings["RateDrop"]),
                MaxGold = long.Parse(ConfigurationManager.AppSettings["MaxGold"]),
                RateGoldDrop = int.Parse(ConfigurationManager.AppSettings["GoldRateDrop"]),
                RateGold = int.Parse(ConfigurationManager.AppSettings["RateGold"]),
                RateReputation = int.Parse(ConfigurationManager.AppSettings["RateReputation"]),
                RateFairyXP = int.Parse(ConfigurationManager.AppSettings["RateFairyXp"]),
                PartnerSpXp = long.Parse(ConfigurationManager.AppSettings["PartnerSpXp"]),
                MaxLevel = byte.Parse(ConfigurationManager.AppSettings["MaxLevel"]),
                MaxJobLevel = byte.Parse(ConfigurationManager.AppSettings["MaxJobLevel"]),
                MaxSPLevel = byte.Parse(ConfigurationManager.AppSettings["MaxSPLevel"]),
                MaxHeroLevel = byte.Parse(ConfigurationManager.AppSettings["MaxHeroLevel"]),
                HeroicStartLevel = byte.Parse(ConfigurationManager.AppSettings["HeroicStartLevel"]),
                MaxUpgrade = byte.Parse(ConfigurationManager.AppSettings["MaxUpgrade"]),
                SceneOnCreate = bool.Parse(ConfigurationManager.AppSettings["SceneOnCreate"]),
                SessionLimit = int.Parse(ConfigurationManager.AppSettings["SessionLimit"]),
                WorldInformation = bool.Parse(ConfigurationManager.AppSettings["WorldInformation"]),
                IsAntiCheatEnabled = bool.Parse(ConfigurationManager.AppSettings["IsAntiCheatEnabled"]),
                AntiCheatClientKey = ConfigurationManager.AppSettings["AntiCheatClientKey"],
                AntiCheatServerKey = ConfigurationManager.AppSettings["AntiCheatServerKey"],
                Act4IP = ConfigurationManager.AppSettings["Act4IP"],
                Act4Port = int.Parse(ConfigurationManager.AppSettings["Act4Port"]),
                MallBaseURL = ConfigurationManager.AppSettings["MallBaseURL"],
                MallAPIKey = ConfigurationManager.AppSettings["MallAPIKey"],
                UseLogService = bool.Parse(ConfigurationManager.AppSettings["UseLogService"]),
                QuestDropRate = int.Parse(ConfigurationManager.AppSettings["QuestDropRate"]),
                HalloweenEvent = bool.Parse(ConfigurationManager.AppSettings["HalloweenEvent"]),
                ChristmasEvent = bool.Parse(ConfigurationManager.AppSettings["ChristmasEvent"]),
                BackpackSize = byte.Parse(ConfigurationManager.AppSettings["BackpackSize"]),
                GlacernonPercentRatePvm = byte.Parse(ConfigurationManager.AppSettings["GlacernonPercentRatePvm"]),
                GlacernonPercentRatePvp = byte.Parse(ConfigurationManager.AppSettings["GlacernonPercentRatePvp"]),
            };
        }

        #endregion

        #region Properties

        public static MSManager Instance => _instance ?? (_instance = new MSManager());

        public ThreadSafeGenericLockedList<long> AuthentificatedClients { get; set; }

        public ConfigurationObject ConfigurationObject { get; set; }

        public ThreadSafeGenericList<AccountConnection> ConnectedAccounts { get; set; }

        public List<IScsServiceClient> LoginServers { get; set; }

        public List<WorldServer> WorldServers { get; set; }

        public Tuple<int, int> Act4Stat { get; set; }

        #endregion
    }
}
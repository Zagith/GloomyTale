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

using OpenNos.DAL.DAO;
using OpenNos.DAL.Interface;

namespace OpenNos.DAL
{
    public static class DAOFactory
    {
        #region Members

        private static IAccountDAO _accountDAO;
        private static IBazaarItemDAO _bazaarItemDAO;
        private static IBCardDAO _bcardDAO;
        private static IBoxItemDAO _boxItemDAO;
        private static ICardDAO _cardDAO;
        private static ICellonOptionDAO _cellonOptionDAO;
        private static ICharacterDAO _characterDAO;
        private static ICharacterRelationDAO _characterRelationDAO;
        private static ICharacterSkillDAO _characterSkillDAO;
        private static ICharacterTitlesDAO _characterTitleDAO;
        private static ICharacterQuestDAO _characterQuestDAO;
        private static IComboDAO _comboDAO;
        private static IDropDAO _dropDAO;
        private static IFamilyCharacterDAO _familyCharacterDAO;
        private static IFamilyDAO _familyDAO;
        private static IFamilyLogDAO _familyLogDAO;
        private static IGeneralLogDAO _generalLogDAO;
        private static IItemDAO _itemDAO;
        private static IItemInstanceDAO _itemInstanceDAO;
        private static ILogCommandsDAO _logCommandsDAO;
        private static ILogChatDAO _logChatDAO;
        private static ILogDropDAO _logDropDAO;
        private static ILogPutItemDAO _logPutItemDAO;
        private static IMailDAO _mailDAO;
        private static IMaintenanceLogDAO _maintenanceLogDAO;
        private static IMapDAO _mapDAO;
        private static IMapMonsterDAO _mapMonsterDAO;
        private static IMapNpcDAO _mapNpcDAO;
        private static IMapTypeDAO _mapTypeDAO;
        private static IMapTypeMapDAO _mapTypeMapDAO;
        private static IMateDAO _mateDAO;
        private static IMinigameLogDAO _minigameLogDAO;
        private static IMinilandObjectDAO _minilandObjectDAO;
        private static INpcMonsterDAO _npcMonsterDAO;
        private static INpcMonsterSkillDAO _npcMonsterSkillDAO;
        private static IPartnerSkillDAO _partnerSkillDAO;
        private static IPenaltyLogDAO _penaltyLogDAO;
        private static IPortalDAO _portalDAO;
        private static IQuestDAO _questDAO;
        private static IQuestLogDAO _questLogDAO;
        private static IQuestRewardDAO _questRewardDAO;
        private static IQuestObjectiveDAO _questObjectiveDAO;
        private static IQuicklistEntryDAO _quicklistEntryDAO;
        private static IRecipeDAO _recipeDAO;
        private static IRecipeItemDAO _recipeItemDAO;
        private static IRecipeListDAO _recipeListDAO;
        private static IRespawnDAO _respawnDAO;
        private static IRespawnMapTypeDAO _respawnMapTypeDAO;
        private static IRollGeneratedItemDAO _rollGeneratedItemDAO;
        private static IRuneEffectDAO _runeEffectDAO;
        private static IScriptedInstanceDAO _scriptedInstanceDAO;
        private static IShellEffectDAO _shellEffectDAO;
        private static IShopDAO _shopDAO;
        private static IShopItemDAO _shopItemDAO;
        private static IShopSkillDAO _shopSkillDAO;
        private static ISkillDAO _skillDAO;
        private static IStaticBonusDAO _staticBonusDAO;
        private static IStaticBuffDAO _staticBuffDAO;
        private static ITeleporterDAO _teleporterDAO;
        private static II18NItemDAO _i18NItemDAO;
        private static II18NCardDAO _i18NCardDAO;
        private static II18NNpcMonsterDAO _i18NNpcMonsterDAO;
        private static II18NSkillDAO _i18NSkillDAO;
        private static II18NMapDAO _i18NMapDAO;
        private static IFortuneWheelDAO _fortuneWheelDAO;
        private static II18NShopNameDAO _i18nShopDAO;
        private static ITimeSpaceLogDAO _timespacelogDAO;
        private static ITrueOrFalseDAO _trueOrFalseDAO;
        private static IMultiAccountExceptionDAO _multiAccountExceptionDAO;
        private static IPvPLogDAO _pvpLogDAO;
        private static IRaidLogDAO _raidLogDAO;
        private static IUpgradeLogDAO _upgradeLogDAO;
        private static ILevelLogDAO _levelLogDAO;
        private static II18NTorFDAO _I18NTorFDAO;
        private static IAchievementsDAO _IAchievementsDAO;
        private static ICharacterAchievementDAO _ICharacterAchievementDAO;

        #endregion

        #region Properties

        public static IAccountDAO AccountDAO => _accountDAO ?? (_accountDAO = new AccountDAO());

        public static IBazaarItemDAO BazaarItemDAO => _bazaarItemDAO ?? (_bazaarItemDAO = new BazaarItemDAO());

        public static IBCardDAO BCardDAO => _bcardDAO ?? (_bcardDAO = new BCardDAO());

        public static IBoxItemDAO BoxItemDAO => _boxItemDAO ?? (_boxItemDAO = new BoxItemDAO());

        public static ICardDAO CardDAO => _cardDAO ?? (_cardDAO = new CardDAO());

        public static ICellonOptionDAO CellonOptionDAO => _cellonOptionDAO ?? (_cellonOptionDAO = new CellonOptionDAO());

        public static ICharacterDAO CharacterDAO => _characterDAO ?? (_characterDAO = new CharacterDAO());

        public static ICharacterRelationDAO CharacterRelationDAO => _characterRelationDAO ?? (_characterRelationDAO = new CharacterRelationDAO());

        public static ICharacterSkillDAO CharacterSkillDAO => _characterSkillDAO ?? (_characterSkillDAO = new CharacterSkillDAO());

        public static ICharacterTitlesDAO CharacterTitleDAO => _characterTitleDAO ?? (_characterTitleDAO = new CharacterTitleDAO());

        public static ICharacterQuestDAO CharacterQuestDAO => _characterQuestDAO ?? (_characterQuestDAO = new CharacterQuestDAO());

        public static IComboDAO ComboDAO => _comboDAO ?? (_comboDAO = new ComboDAO());

        public static IDropDAO DropDAO => _dropDAO ?? (_dropDAO = new DropDAO());

        public static IFamilyCharacterDAO FamilyCharacterDAO => _familyCharacterDAO ?? (_familyCharacterDAO = new FamilyCharacterDAO());

        public static IFamilyDAO FamilyDAO => _familyDAO ?? (_familyDAO = new FamilyDAO());

        public static IFamilyLogDAO FamilyLogDAO => _familyLogDAO ?? (_familyLogDAO = new FamilyLogDAO());

        public static IGeneralLogDAO GeneralLogDAO => _generalLogDAO ?? (_generalLogDAO = new GeneralLogDAO());

        public static IItemDAO ItemDAO => _itemDAO ?? (_itemDAO = new ItemDAO());

        public static IItemInstanceDAO ItemInstanceDAO => _itemInstanceDAO ?? (_itemInstanceDAO = new ItemInstanceDAO());

        public static IMailDAO MailDAO => _mailDAO ?? (_mailDAO = new MailDAO());

        public static IMaintenanceLogDAO MaintenanceLogDAO => _maintenanceLogDAO ?? (_maintenanceLogDAO = new MaintenanceLogDAO());

        public static ILogCommandsDAO LogCommandsDAO => _logCommandsDAO ?? (_logCommandsDAO = new LogCommandsDAO());

        public static ILogChatDAO LogChatDAO => _logChatDAO ?? (_logChatDAO = new LogChatDAO());

        public static ILogDropDAO LogDropDAO => _logDropDAO ?? (_logDropDAO = new LogDropDAO());

        public static ILogPutItemDAO LogPutItemDAO => _logPutItemDAO ?? (_logPutItemDAO = new LogPutItemDAO());

        public static IMapDAO MapDAO => _mapDAO ?? (_mapDAO = new MapDAO());

        public static IMapMonsterDAO MapMonsterDAO => _mapMonsterDAO ?? (_mapMonsterDAO = new MapMonsterDAO());

        public static IMapNpcDAO MapNpcDAO => _mapNpcDAO ?? (_mapNpcDAO = new MapNpcDAO());

        public static IMapTypeDAO MapTypeDAO => _mapTypeDAO ?? (_mapTypeDAO = new MapTypeDAO());

        public static IMapTypeMapDAO MapTypeMapDAO => _mapTypeMapDAO ?? (_mapTypeMapDAO = new MapTypeMapDAO());

        public static IMateDAO MateDAO => _mateDAO ?? (_mateDAO = new MateDAO());

        public static IMinigameLogDAO MinigameLogDAO => _minigameLogDAO ?? (_minigameLogDAO = new MinigameLogDAO());

        public static IMinilandObjectDAO MinilandObjectDAO => _minilandObjectDAO ?? (_minilandObjectDAO = new MinilandObjectDAO());

        public static INpcMonsterDAO NpcMonsterDAO => _npcMonsterDAO ?? (_npcMonsterDAO = new NpcMonsterDAO());

        public static INpcMonsterSkillDAO NpcMonsterSkillDAO => _npcMonsterSkillDAO ?? (_npcMonsterSkillDAO = new NpcMonsterSkillDAO());

        public static IPartnerSkillDAO PartnerSkillDAO => _partnerSkillDAO ?? (_partnerSkillDAO = new PartnerSkillDAO());

        public static IPenaltyLogDAO PenaltyLogDAO => _penaltyLogDAO ?? (_penaltyLogDAO = new PenaltyLogDAO());

        public static IPortalDAO PortalDAO => _portalDAO ?? (_portalDAO = new PortalDAO());

        public static IQuestDAO QuestDAO => _questDAO ?? (_questDAO = new QuestDAO());

        public static IQuestLogDAO QuestLogDAO => _questLogDAO ?? (_questLogDAO = new QuestLogDAO());

        public static IQuestRewardDAO QuestRewardDAO => _questRewardDAO ?? (_questRewardDAO = new QuestRewardDAO());

        public static IQuestObjectiveDAO QuestObjectiveDAO => _questObjectiveDAO ?? (_questObjectiveDAO = new QuestObjectiveDAO());

        public static IQuicklistEntryDAO QuicklistEntryDAO => _quicklistEntryDAO ?? (_quicklistEntryDAO = new QuicklistEntryDAO());

        public static IRecipeDAO RecipeDAO => _recipeDAO ?? (_recipeDAO = new RecipeDAO());

        public static IRecipeItemDAO RecipeItemDAO => _recipeItemDAO ?? (_recipeItemDAO = new RecipeItemDAO());

        public static IRecipeListDAO RecipeListDAO => _recipeListDAO ?? (_recipeListDAO = new RecipeListDAO());

        public static IRespawnDAO RespawnDAO => _respawnDAO ?? (_respawnDAO = new RespawnDAO());

        public static IRespawnMapTypeDAO RespawnMapTypeDAO => _respawnMapTypeDAO ?? (_respawnMapTypeDAO = new RespawnMapTypeDAO());

        public static IRollGeneratedItemDAO RollGeneratedItemDAO => _rollGeneratedItemDAO ?? (_rollGeneratedItemDAO = new RollGeneratedItemDAO());

        public static IRuneEffectDAO RuneEffectDAO => _runeEffectDAO ?? (_runeEffectDAO = new RuneEffectDAO());

        public static IScriptedInstanceDAO ScriptedInstanceDAO => _scriptedInstanceDAO ?? (_scriptedInstanceDAO = new ScriptedInstanceDAO());

        public static IShellEffectDAO ShellEffectDAO => _shellEffectDAO ?? (_shellEffectDAO = new ShellEffectDAO());

        public static IShopDAO ShopDAO => _shopDAO ?? (_shopDAO = new ShopDAO());

        public static IShopItemDAO ShopItemDAO => _shopItemDAO ?? (_shopItemDAO = new ShopItemDAO());

        public static IShopSkillDAO ShopSkillDAO => _shopSkillDAO ?? (_shopSkillDAO = new ShopSkillDAO());

        public static ISkillDAO SkillDAO => _skillDAO ?? (_skillDAO = new SkillDAO());

        public static IStaticBonusDAO StaticBonusDAO => _staticBonusDAO ?? (_staticBonusDAO = new StaticBonusDAO());

        public static IStaticBuffDAO StaticBuffDAO => _staticBuffDAO ?? (_staticBuffDAO = new StaticBuffDAO());

        public static ITeleporterDAO TeleporterDAO => _teleporterDAO ?? (_teleporterDAO = new TeleporterDAO());

        public static II18NItemDAO I18NItemDAO => _i18NItemDAO ?? (_i18NItemDAO = new I18NItemDAO());

        public static II18NCardDAO I18NCardDAO => _i18NCardDAO ?? (_i18NCardDAO = new I18NCardDAO());

        public static II18NNpcMonsterDAO I18NNpcMonsterDAO => _i18NNpcMonsterDAO ?? (_i18NNpcMonsterDAO = new I18NNpcMonsterDAO());

        public static II18NSkillDAO I18NSkillDAO => _i18NSkillDAO ?? (_i18NSkillDAO = new I18NSkillDAO());

        public static II18NMapDAO I18NMapDAO => _i18NMapDAO ?? (_i18NMapDAO = new I18NMapDAO());

        public static II18NShopNameDAO I18NShopNameDAO => _i18nShopDAO ?? (_i18nShopDAO = new I18NShopNameDAO());

        public static IFortuneWheelDAO FortuneWheelDAO => _fortuneWheelDAO ?? (_fortuneWheelDAO = new FortuneWheelDAO());

        public static ITimeSpaceLogDAO TimeSpaceLogDAO => _timespacelogDAO ?? (_timespacelogDAO = new TimeSpaceLogDAO());

        public static ITrueOrFalseDAO TrueOrFalseDAO => _trueOrFalseDAO ?? (_trueOrFalseDAO = new TrueOrFalseDAO());

        public static IMultiAccountExceptionDAO MultiAccountExceptionDAO => _multiAccountExceptionDAO ?? (_multiAccountExceptionDAO = new MultiAccountExceptionDAO());

        public static IPvPLogDAO PvPLogDAO => _pvpLogDAO ?? (_pvpLogDAO = new PvPLogDAO());

        public static IRaidLogDAO RaidLogDAO => _raidLogDAO ?? (_raidLogDAO = new RaidLogDAO());

        public static ILevelLogDAO LevelLogDAO => _levelLogDAO ?? (_levelLogDAO = new LevelLogDAO());

        public static IUpgradeLogDAO UpgradeLogDAO => _upgradeLogDAO ?? (_upgradeLogDAO = new UpgradeLogDAO());

        public static II18NTorFDAO I18NTorFDAO => _I18NTorFDAO ?? (_I18NTorFDAO = new I18NTorFDAO());

        public static IAchievementsDAO AchievementsDAO => _IAchievementsDAO ?? (_IAchievementsDAO = new AchievementsDAO());

        public static ICharacterAchievementDAO CharacterAchievementDAO => _ICharacterAchievementDAO ?? (_ICharacterAchievementDAO = new CharacterAchievementDAO());
        #endregion
    }
}
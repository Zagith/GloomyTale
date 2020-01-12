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

using GloomyTale.DAL.Interface;

namespace GloomyTale.DAL
{
    public class DAOFactory
    {

        public DAOFactory (IAccountDAO _accountDAO,IBazaarItemDAO _bazaarItemDAO,IBCardDAO _bcardDAO,IBoxItemDAO _boxItemDAO,
            ICardDAO _cardDAO,ICellonOptionDAO _cellonOptionDAO,ICharacterDAO _characterDAO,ICharacterRelationDAO _characterRelationDAO,
            ICharacterSkillDAO _characterSkillDAO,ICharacterTitlesDAO _characterTitleDAO,
            ICharacterQuestDAO _characterQuestDAO,IComboDAO _comboDAO,IDropDAO _dropDAO, IFamilyCharacterDAO _familyCharacterDAO,
            IFamilyDAO _familyDAO, IFamilyLogDAO _familyLogDAO,IGeneralLogDAO _generalLogDAO,IItemDAO _itemDAO,IItemInstanceDAO _itemInstanceDAO, 
            ILogCommandsDAO _logCommandsDAO, ILogChatDAO _logChatDAO, ILogDropDAO _logDropDAO, ILogPutItemDAO _logPutItemDAO, IMailDAO _mailDAO, 
            IMaintenanceLogDAO _maintenanceLogDAO, IMapDAO _mapDAO, IMapMonsterDAO _mapMonsterDAO, IMapNpcDAO _mapNpcDAO,IMapTypeDAO _mapTypeDAO,
            IMapTypeMapDAO _mapTypeMapDAO,IMateDAO _mateDAO,IMinigameLogDAO _minigameLogDAO,IMinilandObjectDAO _minilandObjectDAO,
            INpcMonsterDAO _npcMonsterDAO,INpcMonsterSkillDAO _npcMonsterSkillDAO,IPartnerSkillDAO _partnerSkillDAO,IPenaltyLogDAO _penaltyLogDAO,IPortalDAO _portalDAO,
            IQuestDAO _questDAO,IQuestLogDAO _questLogDAO,IQuestRewardDAO _questRewardDAO,IQuestObjectiveDAO _questObjectiveDAO,
            IQuicklistEntryDAO _quicklistEntryDAO,IRecipeDAO _recipeDAO,IRecipeItemDAO _recipeItemDAO,IRecipeListDAO _recipeListDAO,
            IRespawnDAO _respawnDAO,IRespawnMapTypeDAO _respawnMapTypeDAO,IRollGeneratedItemDAO _rollGeneratedItemDAO,
            IScriptedInstanceDAO _scriptedInstanceDAO, IShellEffectDAO _shellEffectDAO,IShopDAO _shopDAO,IShopItemDAO _shopItemDAO,
            IShopSkillDAO _shopSkillDAO,ISkillDAO _skillDAO,IStaticBonusDAO _staticBonusDAO,IStaticBuffDAO _staticBuffDAO,
            ITeleporterDAO _teleporterDAO,II18NItemDAO _i18NItemDAO, II18NCardDAO _i18NCardDAO,II18NNpcMonsterDAO _i18NNpcMonsterDAO,
            II18NSkillDAO _i18NSkillDAO, II18NMapDAO _i18NMapDAO, IFortuneWheelDAO _fortuneWheelDAO,II18NShopNameDAO _i18nShopDAO,ITimeSpaceLogDAO _timespacelogDAO)
        {
            AccountDAO = _accountDAO;
            BazaarItemDAO = _bazaarItemDAO;
            BCardDAO = _bcardDAO;
            BoxItemDAO = _boxItemDAO;
            CardDAO = _cardDAO;
            CellonOptionDAO = _cellonOptionDAO;
            CharacterDAO = _characterDAO;
            CharacterRelationDAO = _characterRelationDAO;
            CharacterSkillDAO = _characterSkillDAO;
            CharacterTitleDAO = _characterTitleDAO;
            CharacterQuestDAO = _characterQuestDAO;
            ComboDAO = _comboDAO;
            DropDAO = _dropDAO;
            FamilyCharacterDAO = _familyCharacterDAO;
            FamilyDAO = _familyDAO;
            FamilyLogDAO = _familyLogDAO;
            GeneralLogDAO = _generalLogDAO;
            ItemDAO = _itemDAO;
            ItemInstanceDAO = _itemInstanceDAO;
            MailDAO = _mailDAO;
            MaintenanceLogDAO = _maintenanceLogDAO;
            LogCommandsDAO = _logCommandsDAO;
            LogChatDAO = _logChatDAO;
            LogDropDAO = _logDropDAO;
            LogPutItemDAO = _logPutItemDAO;
            MapDAO = _mapDAO;
            MapMonsterDAO = _mapMonsterDAO;
            MapNpcDAO = _mapNpcDAO;
            MapTypeDAO = _mapTypeDAO;
            MapTypeMapDAO = _mapTypeMapDAO;
            MateDAO = _mateDAO;
            MinigameLogDAO = _minigameLogDAO;
            MinilandObjectDAO = _minilandObjectDAO;
            NpcMonsterDAO = _npcMonsterDAO;
            NpcMonsterSkillDAO = _npcMonsterSkillDAO;
            PartnerSkillDAO = _partnerSkillDAO;
            PenaltyLogDAO = _penaltyLogDAO;
            PortalDAO = _portalDAO;
            QuestDAO = _questDAO;
            QuestLogDAO = _questLogDAO;
            QuestRewardDAO = _questRewardDAO;
            QuestObjectiveDAO = _questObjectiveDAO;
            QuicklistEntryDAO = _quicklistEntryDAO;
            RecipeDAO = _recipeDAO;
            RecipeItemDAO = _recipeItemDAO;
            RecipeListDAO = _recipeListDAO;
            RespawnDAO = _respawnDAO;
            RespawnMapTypeDAO = _respawnMapTypeDAO;
            RollGeneratedItemDAO = _rollGeneratedItemDAO;
            ScriptedInstanceDAO = _scriptedInstanceDAO;
            ShellEffectDAO = _shellEffectDAO;
            ShopDAO = _shopDAO;
            ShopItemDAO = _shopItemDAO;
            ShopSkillDAO = _shopSkillDAO;
            SkillDAO = _skillDAO;
            StaticBonusDAO = _staticBonusDAO;
            StaticBuffDAO = _staticBuffDAO;
            TeleporterDAO = _teleporterDAO;
            I18NItemDAO = _i18NItemDAO;
            I18NCardDAO = _i18NCardDAO;
            I18NNpcMonsterDAO = _i18NNpcMonsterDAO;
            I18NSkillDAO = _i18NSkillDAO;
            I18NMapDAO = _i18NMapDAO;
            I18NShopNameDAO = _i18nShopDAO;
            FortuneWheelDAO = _fortuneWheelDAO;
            TimeSpaceLogDAO = _timespacelogDAO;
        }

        #region Properties
        public IAccountDAO AccountDAO { get; }

        public IBazaarItemDAO BazaarItemDAO { get; }

        public IBCardDAO BCardDAO { get; }

        public IBoxItemDAO BoxItemDAO { get; }

        public ICardDAO CardDAO { get; }

        public ICellonOptionDAO CellonOptionDAO { get; }

        public ICharacterDAO CharacterDAO { get; }

        public ICharacterRelationDAO CharacterRelationDAO { get; }

        public ICharacterSkillDAO CharacterSkillDAO { get; }

        public ICharacterTitlesDAO CharacterTitleDAO { get; }

        public ICharacterQuestDAO CharacterQuestDAO { get; }

        public IComboDAO ComboDAO { get; }

        public IDropDAO DropDAO { get; }

        public IFamilyCharacterDAO FamilyCharacterDAO { get; }

        public IFamilyDAO FamilyDAO { get; }

        public IFamilyLogDAO FamilyLogDAO { get; }

        public IGeneralLogDAO GeneralLogDAO { get; }

        public IItemDAO ItemDAO { get; }

        public IItemInstanceDAO ItemInstanceDAO { get; }

        public IMailDAO MailDAO { get; }

        public IMaintenanceLogDAO MaintenanceLogDAO { get; }

        public ILogCommandsDAO LogCommandsDAO { get; }

        public ILogChatDAO LogChatDAO { get; }

        public ILogDropDAO LogDropDAO { get; }

        public ILogPutItemDAO LogPutItemDAO { get; }

        public IMapDAO MapDAO { get; }

        public IMapMonsterDAO MapMonsterDAO { get; }

        public IMapNpcDAO MapNpcDAO { get; }

        public IMapTypeDAO MapTypeDAO { get; }

        public IMapTypeMapDAO MapTypeMapDAO { get; }

        public IMateDAO MateDAO { get; }

        public IMinigameLogDAO MinigameLogDAO { get; }

        public IMinilandObjectDAO MinilandObjectDAO { get; }

        public INpcMonsterDAO NpcMonsterDAO { get; }

        public INpcMonsterSkillDAO NpcMonsterSkillDAO { get; }

        public IPartnerSkillDAO PartnerSkillDAO { get; }

        public IPenaltyLogDAO PenaltyLogDAO { get; }

        public IPortalDAO PortalDAO { get; }

        public IQuestDAO QuestDAO { get; }

        public IQuestLogDAO QuestLogDAO { get; }

        public IQuestRewardDAO QuestRewardDAO { get; }

        public IQuestObjectiveDAO QuestObjectiveDAO { get; }

        public IQuicklistEntryDAO QuicklistEntryDAO { get; }

        public IRecipeDAO RecipeDAO { get; }

        public IRecipeItemDAO RecipeItemDAO { get; }

        public IRecipeListDAO RecipeListDAO { get; }

        public IRespawnDAO RespawnDAO { get; }

        public IRespawnMapTypeDAO RespawnMapTypeDAO { get; }

        public IRollGeneratedItemDAO RollGeneratedItemDAO { get; }

        public IScriptedInstanceDAO ScriptedInstanceDAO { get; }

        public IShellEffectDAO ShellEffectDAO { get; }

        public IShopDAO ShopDAO { get; }

        public IShopItemDAO ShopItemDAO { get; }

        public IShopSkillDAO ShopSkillDAO { get; }

        public ISkillDAO SkillDAO { get; }

        public IStaticBonusDAO StaticBonusDAO { get; }

        public IStaticBuffDAO StaticBuffDAO { get; }

        public ITeleporterDAO TeleporterDAO { get; }

        public II18NItemDAO I18NItemDAO { get; }

        public II18NCardDAO I18NCardDAO { get; }

        public II18NNpcMonsterDAO I18NNpcMonsterDAO { get; }

        public II18NSkillDAO I18NSkillDAO { get; }

        public II18NMapDAO I18NMapDAO { get; }

        public II18NShopNameDAO I18NShopNameDAO { get; }

        public IFortuneWheelDAO FortuneWheelDAO { get; }

        public ITimeSpaceLogDAO TimeSpaceLogDAO { get; }

        #endregion
    }
}
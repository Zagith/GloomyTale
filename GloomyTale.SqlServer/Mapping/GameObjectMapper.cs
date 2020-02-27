using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Configuration;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Entities;
using GloomyTale.Data;
using BoxInstance = GloomyTale.GameObject.Items.Instance.BoxInstance;
using SpecialistInstance = GloomyTale.GameObject.Items.Instance.SpecialistInstance;
using WearableInstance = GloomyTale.GameObject.Items.Instance.WearableInstance;

namespace GloomyTale.SqlServer.Mapping
{
    public class GameObjectMapper : IMapper
    {
        private readonly IMapper _mapper;
        public GameObjectMapper()
        {
            var cfg = new MapperConfigurationExpression();


            cfg.CreateMap<ItemInstance, ItemInstanceDTO>()
                .As<GameObject.Items.Instance.ItemInstance>();
            // GameObject to EF
            cfg.CreateMap<GameObject.Items.Instance.ItemInstance, ItemInstance>()
                .ForMember(s => s.Item, opts => opts.Ignore());

            // EF to GameObject
            cfg.CreateMap<ItemInstance, GameObject.Items.Instance.ItemInstance>()
                .ForMember(s => s.Item, opts => opts.Ignore());

            MapItemInstance<BoxInstance>(cfg);
            MapItemInstance<SpecialistInstance>(cfg);
            MapItemInstance<WearableInstance>(cfg);

            AddMapping<Account, AccountDTO>(cfg);
            // character
            AddMapping<Character, CharacterDTO, GameObject.Character>(cfg);
            AddMapping<CharacterRelation, CharacterRelationDTO>(cfg);
            AddMapping<CharacterSkill, CharacterSkillDTO, GameObject.CharacterSkill>(cfg);
            AddMapping<CharacterQuest, CharacterQuestDTO, GameObject.CharacterQuest>(cfg);
            AddMapping<CharacterTitle, CharacterTitleDTO>(cfg);

            // combo
            AddMapping<Combo, ComboDTO>(cfg);
            // drop
            AddMapping<Drop, DropDTO>(cfg);
            AddMapping<GeneralLog, GeneralLogDTO>(cfg);
            // item
            AddMapping<Item, ItemDTO>(cfg);
            AddMapping<BazaarItem, BazaarItemDTO>(cfg);
            AddMapping<Mail, MailDTO>(cfg);
            AddMapping<RollGeneratedItem, RollGeneratedItemDTO>(cfg);

            // map
            AddMapping<Map, MapDTO>(cfg);
            AddMapping<MapMonster, MapMonsterDTO, GameObject.MapMonster>(cfg);
            AddMapping<MapNpc, MapNpcDTO, GameObject.MapNpc>(cfg);

            // family
            AddMapping<Family, FamilyDTO, GameObject.Family>(cfg);
            AddMapping<FamilyCharacter, FamilyCharacterDTO, GameObject.FamilyCharacter>(cfg);
            AddMapping<FamilyLog, FamilyLogDTO>(cfg);

            AddMapping<MapType, MapTypeDTO>(cfg);
            AddMapping<MapTypeMap, MapTypeMapDTO>(cfg);


            // npc monster
            AddMapping<NpcMonster, NpcMonsterDTO, GameObject.NpcMonster>(cfg);
            AddMapping<NpcMonsterSkill, NpcMonsterSkillDTO, GameObject.NpcMonsterSkill>(cfg);

            // penalty
            AddMapping<PenaltyLog, PenaltyLogDTO>(cfg);

            // portal
            AddMapping<Portal, PortalDTO, GameObject.Portal>(cfg);

            // quests
            AddMapping<Quest, QuestDTO, GameObject.Quest>(cfg);
            AddMapping<QuestLog, QuestLogDTO>(cfg);
            AddMapping<QuestReward, QuestRewardDTO>(cfg);
            AddMapping<QuestObjective, QuestObjectiveDTO>(cfg);

            // quicklist
            AddMapping<QuicklistEntry, QuicklistEntryDTO>(cfg);

            // recipes
            AddMapping<Recipe, RecipeDTO, GameObject.Recipe>(cfg);
            AddMapping<RecipeItem, RecipeItemDTO>(cfg);

            // miniland
            AddMapping<MinilandObject, MinilandObjectDTO>(cfg);
            AddMapping<MinilandObject, GameObject.MapDesignObject>(cfg);

            // respawn
            AddMapping<Respawn, RespawnDTO>(cfg);
            AddMapping<RespawnMapType, RespawnMapTypeDTO>(cfg);

            // shop
            AddMapping<Shop, ShopDTO, GameObject.Shop>(cfg);
            AddMapping<ShopItem, ShopItemDTO>(cfg);
            AddMapping<ShopSkill, ShopSkillDTO>(cfg);

            // cards
            AddMapping<Card, CardDTO, GameObject.Card>(cfg);

            // bcards
            AddMapping<BCard, BCardDTO, GameObject.BCard>(cfg);

            // skills
            AddMapping<Skill, SkillDTO, GameObject.Skill>(cfg);

            // mates
            AddMapping<Mate, MateDTO, GameObject.Mate>(cfg);

            // mates
            AddMapping<Teleporter, TeleporterDTO>(cfg);

            // static bonus
            AddMapping<StaticBonus, StaticBonusDTO>(cfg);
            AddMapping<StaticBuff, StaticBuffDTO>(cfg);

            // scripted instance
            AddMapping<ScriptedInstance, ScriptedInstanceDTO, GameObject.ScriptedInstance>(cfg);

            // logs
            AddMapping<LogChat, LogChatDTO>(cfg);
            AddMapping<LogCommands, LogCommandsDTO>(cfg);
            AddMapping<LogDrop, LogDropDTO>(cfg);
            AddMapping<LogPutItem, LogPutItemDTO>(cfg);
            AddMapping<TimeSpacesLog, TimeSpacesLogDTO>(cfg);

            _mapper = new MapperConfiguration(cfg).CreateMapper();
        }

        private static void AddMapping<TEntity, TDto>(IProfileExpression cfg) where TDto : MappingBaseDTO
        {
            // GameObject -> Entity
            cfg.CreateMap<TDto, TEntity>();

            // Entity -> GameObject
            cfg.CreateMap<TEntity, TDto>()
                .AfterMap((_, dest) => dest.Initialize());
        }

        private static void AddMapping<TEntity, TDto, TGameObject>(IProfileExpression cfg) where TDto : MappingBaseDTO where TGameObject : TDto
        {
            // GameObject -> Entity
            cfg.CreateMap<TDto, TEntity>();

            // Entity -> GameObject
            cfg.CreateMap<TEntity, TDto>()
                .AfterMap((_, dest) => dest.Initialize())
                .As<TGameObject>();
        }

        private static void MapItemInstance<TGameObject>(IProfileExpression cfg) where TGameObject : GameObject.Items.Instance.ItemInstance
        {
            // GameObject -> Entity
            cfg.CreateMap<TGameObject, ItemInstance>()
                .ForMember(s => s.Item, opts => opts.Ignore())
                .IncludeBase<GameObject.Items.Instance.ItemInstance, ItemInstance>();

            // Entity -> GameObject
            cfg.CreateMap<ItemInstance, TGameObject>()
                .IncludeBase<ItemInstance, GameObject.Items.Instance.ItemInstance >()
                .As<TGameObject>();

            // Entity -> GameObject
            cfg.CreateMap<ItemInstance, ItemInstanceDTO>().As<TGameObject>();
        }

        public TDestination Map<TDestination>(object source) => _mapper.Map<TDestination>(source);

        public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> opts) => _mapper.Map<TDestination>(source, opts);

        public TDestination Map<TSource, TDestination>(TSource source) => _mapper.Map<TSource, TDestination>(source);

        public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts) => _mapper.Map(source, opts);

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination) => _mapper.Map(source, destination);

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts) => _mapper.Map(source, destination, opts);

        public object Map(object source, Type sourceType, Type destinationType) => _mapper.Map(source, sourceType, destinationType);

        public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts) => _mapper.Map(source, sourceType, destinationType, opts);

        public object Map(object source, object destination, Type sourceType, Type destinationType) => _mapper.Map(source, destination, sourceType, destinationType);

        public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts) => _mapper.Map(source, destination, sourceType, destinationType, opts);

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, object parameters = null, params Expression<Func<TDestination, object>>[] membersToExpand) => _mapper.ProjectTo(source, parameters, membersToExpand);

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, IDictionary<string, object> parameters, params string[] membersToExpand) => _mapper.ProjectTo<TDestination>(source, parameters, membersToExpand);

        public IConfigurationProvider ConfigurationProvider => _mapper.ConfigurationProvider;

        public Func<Type, object> ServiceCtor => _mapper.ServiceCtor;
    }
}

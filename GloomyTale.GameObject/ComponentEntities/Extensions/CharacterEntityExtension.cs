using GloomyTale.DAL;
using GloomyTale.Data;
using GloomyTale.Domain;
using GloomyTale.GameObject.ComponentEntities.Interfaces;
using GloomyTale.GameObject.Networking;
using GloomyTale.GameObject.Packets.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GloomyTale.GameObject.ComponentEntities.Extensions
{
    public static class CharacterEntityExtension
    {
        public static GoldPacket GenerateGold(this ICharacterEntity characterEntity)
        {
            return new GoldPacket { Gold = characterEntity.Gold, Unknown = 0};
        }

        public static TitleInfoPacket GenerateTitInfo(this ICharacterEntity visualEntity)
        {
            var visibleTitle = visualEntity.Titles.FirstOrDefault(s => s.Visible)?.TitleType;
            var effectiveTitle = visualEntity.Titles.FirstOrDefault(s => s.Active)?.TitleType;
            return new TitleInfoPacket
            {
                VisualId = visualEntity.VisualId,
                EffectiveTitle = effectiveTitle ?? 0,
                VisualType = visualEntity.VisualType,
                VisibleTitle = visibleTitle ?? 0,
            };
        }

        public static TitlePacket GenerateTitle(this ICharacterEntity visualEntity)
        {
            var data = visualEntity.Titles.Select(s => new TitleSubPacket
            {
                TitleId = (short)(s.TitleType - 9300),
                TitleStatus = (byte)((s.Visible ? 2 : 0) + (s.Active ? 4 : 0) + 1)
            }).ToList();
            return new TitlePacket
            {
                Data = data.Any() ? data : null
            };
        }

        public static ExtsPacket GenerateExts(this ICharacterEntity visualEntity)
        {
            return new ExtsPacket
            {
                EquipmentExtension = (byte)(visualEntity.Inventory.Expensions[InventoryType.Equipment] + ServerManager.Instance.BackpackSize),
                MainExtension = (byte)(visualEntity.Inventory.Expensions[InventoryType.Main] + ServerManager.Instance.BackpackSize),
                EtcExtension = (byte)(visualEntity.Inventory.Expensions[InventoryType.Etc] + ServerManager.Instance.BackpackSize)
            };
        }

        public static ClPacket GenerateInvisible(this ICharacterEntity visualEntity)
        {
            return new ClPacket
            {
                VisualId = visualEntity.VisualId,
                Camouflage = visualEntity.Camouflage,
                Invisible = visualEntity.Invisible
            };
        }

        public static BlinitPacket GenerateBlinit(this ICharacterEntity visualEntity)
        {
            var subpackets = new List<BlinitSubPacket>();
            var blackList = DAOFactory.Instance.CharacterRelationDAO.LoadAll().Where(
                    s => s.CharacterId == visualEntity.VisualId && s.RelationType == CharacterRelationType.Blocked);
            foreach (var relation in blackList)
            {
                if (relation.CharacterId == visualEntity.VisualId)
                {
                    continue;
                }
                
                subpackets.Add(new BlinitSubPacket
                {
                    RelatedCharacterId = relation.CharacterId,
                    CharacterName = visualEntity.Name
                });
            }

            return new BlinitPacket { SubPackets = subpackets };
        }

        public static FinitPacket GenerateFinit(this ICharacterEntity visualEntity)
        {
            var subpackets = new List<FinitSubPacket>();
            foreach (CharacterRelationDTO relation in visualEntity.CharacterRelations.ToList().Where(c => c.RelationType == CharacterRelationType.Friend || c.RelationType == CharacterRelationType.Spouse))
            {
                long id = relation.RelatedCharacterId == visualEntity.VisualId ? relation.CharacterId : relation.RelatedCharacterId;
                var account = CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, id);
                var name = DAOFactory.Instance.CharacterDAO.LoadById(id).Name;
                subpackets.Add(new FinitSubPacket
                {
                    CharacterId = relation.CharacterId,
                    RelationType = relation.RelationType,
                    IsOnline = account,
                    CharacterName = name
                });
            }
            return new FinitPacket { SubPackets = subpackets };
        }

        public static IconPacket GenerateIcon(this ICharacterEntity visualEntity, byte iconType, short iconParameter)
        {
            return new IconPacket
            {
                VisualType = visualEntity.VisualType,
                VisualId = visualEntity.VisualId,
                IconParameter = iconParameter,
                IconType = iconType
            };
        }

        public static ServerGetPacket GenerateGet(this ICharacterEntity visualEntity, long itemId)
        {
            return new ServerGetPacket
            {
                VisualType = visualEntity.VisualType,
                VisualId = visualEntity.VisualId,
                ItemId = itemId
            };
        }
    }
}

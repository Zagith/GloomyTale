using GloomyTale.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.ComponentEntities.Interfaces
{
    public interface ICharacterEntity : INamedEntity
    {
        //bool FriendRequestBlocked { get; set; }

        //AuthorityType Authority { get; }

        //short MapId { get; set; }

        //GenderType Gender { get; }

        //HairStyleType HairStyle { get; }

        //HairColorType HairColor { get; }

        ClassType Class { get; }

        //InEquipmentSubPacket Equipment { get; }

        //int ReputIcon { get; }

        //int DignityIcon { get; }

        //bool Camouflage { get; }

       //bool Invisible { get; }

        //IChannel Channel { get; }

        //bool GroupRequestBlocked { get; }

        /*ConcurrentDictionary<long, long> GroupRequestCharacterIds { get; }
        UpgradeRareSubPacket WeaponUpgradeRareSubPacket { get; }

        UpgradeRareSubPacket ArmorUpgradeRareSubPacket { get; }*/

        long Gold { get; }

        //long BankGold { get; }

        /*IInventoryService Inventory { get; }

        RegionType AccountLanguage { get; }

        List<StaticBonusDto> StaticBonusList { get; set; }

        List<TitleDto> Titles { get; set; }*/

        //bool IsDisconnecting { get; }

        /*void GenerateMail(IEnumerable<MailData> data);

        void SendPacket(IPacket packetDefinition);

        void SendPackets(IEnumerable<IPacket> packetDefinitions);*/

        //void LeaveGroup();

        //void JoinGroup(Group group);

        //void Save();

        //void SetJobLevel(byte level);

        //void SetHeroLevel(byte level);

        //void SetReputation(long reput);

        //void SetGold(long gold);

        //void AddGold(long gold);

        //void RemoveGold(long gold);

        //void AddBankGold(long bankGold);

        //void RemoveBankGold(long bankGold);

        //void ChangeClass(ClassType classType);

        //void ChangeMap(short mapId, short mapX, short mapY);
    }
}

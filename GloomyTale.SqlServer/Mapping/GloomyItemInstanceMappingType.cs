using GloomyTale.DAL.DAO;
using GloomyTale.DAL.EF;
using GloomyTale.DAL.EF.Entities;
using System;
using System.Collections.Generic;

namespace GloomyTale.SqlServer.Mapping
{
    public class GloomyItemInstanceMappingType : ItemInstanceDAO.IItemInstanceMappingTypes
    {
        public List<(Type, Type)> Types { get; } = new List<(Type, Type)>
        {
            (typeof(GameObject.Items.Instance.BoxInstance), typeof(BoxInstance)),
            (typeof(GameObject.Items.Instance.ItemInstance), typeof(ItemInstance)),
            (typeof(GameObject.Items.Instance.SpecialistInstance), typeof(SpecialistInstance)),
            (typeof(GameObject.Items.Instance.WearableInstance), typeof(WearableInstance)),
        };
    }
}

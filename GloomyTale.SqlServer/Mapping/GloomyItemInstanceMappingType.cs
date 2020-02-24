using GloomyTale.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.SqlServer.Mapping
{
    public class GloomyItemInstanceMappingType // : ItemInstanceDAO.IItemInstanceMappingTypes
    {
        public List<(Type, Type)> Types { get; } = new List<(Type, Type)>
        {
            (typeof(GameObject.Item.Instance.BoxInstance), typeof(BoxInstance)),
            (typeof(GameObject.Item.Instance.ItemInstance), typeof(ItemInstance)),
            (typeof(GameObject.Item.Instance.SpecialistInstance), typeof(SpecialistInstance)),
            (typeof(GameObject.Item.Instance.WearableInstance), typeof(WearableInstance)),
        };
    }
}

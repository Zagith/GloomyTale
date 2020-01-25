using OpenNos.Data;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject
{
    public class CharacterTitle : CharacterTitleDTO
    {
        private Item _item;

        public Item Item => _item ?? (_item = ServerManager.GetItem(TitleType));

    }
}

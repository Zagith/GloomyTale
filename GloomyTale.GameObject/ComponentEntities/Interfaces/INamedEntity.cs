using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.ComponentEntities.Interfaces
{
    public interface INamedEntity : IAliveEntity
    {
        //Group Group { get; set; }

        string Name { get; }

        //long LevelXp { get; set; }
    }
}

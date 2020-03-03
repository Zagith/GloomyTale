using GloomyTale.Domain;
using System;

namespace GloomyTale.GameObject.ComponentEntities.Interfaces
{
    public interface IVisualEntity
    {
        VisualType VisualType { get; }

        //short VNum { get; }

        long VisualId { get; }

        /*byte Direction { get; set; }

        Guid MapInstanceId { get; }

        MapInstance MapInstance { get; }

        short PositionX { get; set; }

        short PositionY { get; set; }*/
    }
}

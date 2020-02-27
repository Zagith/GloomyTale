using GloomyTale.Data.Interfaces;
using GloomyTale.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.GameObject.Items.Instance
{
    public class BoxInstance : SpecialistInstance, IBoxInstance
    {
        #region Members

        private Random _random;

        #endregion

        #region Instantiation

        public BoxInstance() => _random = new Random();

        public BoxInstance(Guid id)
        {
            Id = id;
            _random = new Random();
        }

        #endregion

        #region Properties

        public short HoldingVNum { get; set; }

        public MateType MateType { get; set; }

        #endregion
    }
}

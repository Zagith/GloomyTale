using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.Master.Datas
{
    public class CharacterSession
    {
        public CharacterSession(string name, int level, string gender, string playerClass)
        {
            Name = name;
            Level = level;
            Gender = gender;
            Class = playerClass;
        }

        public string Name { get; }
        public int Level { get; }
        public string Gender { get; }
        public string Class { get; }
    }
}

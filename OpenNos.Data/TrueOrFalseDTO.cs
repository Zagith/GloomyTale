using Mapster;
using OpenNos.Data.Base;
using OpenNos.Data.I18N;
using OpenNos.Data.Interfaces;
using System;

namespace OpenNos.Data
{
    [Serializable]
    public class TrueOrFalseDTO : IStaticDto
    {
        public short TrueOrFalseId { get; set; }

        [I18NFrom(typeof(I18NTorFDto))]
        public I18NString Name { get; set; } = new I18NString();
        [AdaptMember("Name")]
        public string NameI18NKey { get; set; }

        public bool Answer { get; set; }

        public short QuestionType { get; set; }
    }
}

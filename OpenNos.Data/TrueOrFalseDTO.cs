using System;

namespace OpenNos.Data
{
    [Serializable]
    public class TrueOrFalseDTO
    {
        public short TrueOrFalseId { get; set; }

        public string Question { get; set; }

        public bool Answer { get; set; }

        public short QuestionType { get; set; }
    }
}

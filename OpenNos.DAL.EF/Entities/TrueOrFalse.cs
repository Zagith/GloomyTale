using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF.Entities
{
    public class TrueOrFalse
    {
        [Key]
        public short TrueOrFalseId { get; set; }

        public string Question { get; set; }

        public bool Answer { get; set; }

        public short QuestionType { get; set; }
    }
}

using OpenNos.DAL.EF.Base;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF.Entities
{
    public class TrueOrFalse
    {
        [Key]
        public short TrueOrFalseId { get; set; }

        [MaxLength(255)]
        [I18NString(typeof(I18NTorF))]
        public string Name { get; set; }

        public bool Answer { get; set; }

        public short QuestionType { get; set; }
    }
}

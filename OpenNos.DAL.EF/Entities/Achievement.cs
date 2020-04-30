using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.EF.Entities
{
    public class Achievement
    {
        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AchievementId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public int AchievementType { get; set; }

        public int Data { get; set; }

        public byte LevelMin { get; set; }

        public byte LevelMax { get; set; }

        public bool IsDaily { get; set; }

        public byte Category { get; set; }

        #endregion
    }
}

﻿using GloomyTale.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloomyTale.DAL.EF
{
    public class LogChat
    {
        [Key]
        public long LogId { get; set; }

        public virtual Character Character { get; set; }

        public long? CharacterId { get; set; }

        public ChatType ChatType { get; set; }

        [MaxLength(255)]
        public string ChatMessage { get; set; }

        [MaxLength(255)]
        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}

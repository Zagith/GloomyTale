using System;
using System.Collections.Generic;
using System.Text;

namespace GloomyTale.ToolKit.Importers
{
    public class ImportConfiguration
    {
        public string Folder { get; set; }
        public string Lang { get; set; }
        public List<string[]> Packets { get; set; }
    }
}

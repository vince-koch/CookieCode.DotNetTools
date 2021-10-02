using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPack
{
    public class PackageSource
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public int ProtocolVersion { get; set; }
    }
}

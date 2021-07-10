using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSandbox.Entities
{
    public class VATRate
    {
        public string Country { get; set; }
        public string Code { get; set; }
        public int VAT { get; set; }
    }
}

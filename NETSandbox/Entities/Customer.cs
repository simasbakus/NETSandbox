﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSandbox.Entities
{
    class Customer
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public bool IsIndividual { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSandbox.Entities
{
    public interface IServiceProvider
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public bool VATPayer { get; set; }
    }
}

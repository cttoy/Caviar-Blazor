﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models
{
    public interface ITableTemplate
    {
        public Task<bool> Validate();
    }
}
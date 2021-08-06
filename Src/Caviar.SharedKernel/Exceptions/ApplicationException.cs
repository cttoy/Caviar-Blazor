﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string errorMsg)
            : base(errorMsg)
        {
        }
    }
}
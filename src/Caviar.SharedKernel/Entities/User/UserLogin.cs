﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities.User
{
    public class UserLogin
    {
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "RequiredErrorMsg")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    [DisplayName("角色用户组")]
    public class SysRoleUserGroup : SysBaseEntity, IBaseEntity,IView
    {
        public int UserGroupId { get; set; }

        public int RoleId { get; set; }
    }
}

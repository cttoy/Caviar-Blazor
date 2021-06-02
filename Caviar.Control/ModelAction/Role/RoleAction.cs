﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caviar.Models.SystemData;

namespace Caviar.Control.Role
{
    public partial class RoleAction
    {
        partial void PartialModelToViewModel(ref bool isContinue, PageData<SysRole> model, ref PageData<ViewRole> outModel)
        {
            outModel = CommonHelper.AToB<PageData<ViewRole>, PageData<SysRole>>(model);
            if (outModel.Total != 0)
            {
                var viewMenus = new List<ViewRole>().ListAutoAssign(model.Rows);
                outModel.Rows = viewMenus.ListToTree();
            }
            isContinue = false;
        }

        public async Task<List<SysRole>> GetCurrentRoles()
        {
            List<SysRole> roles = new List<SysRole>();
            if (BC.Id>0)
            {
                //获取当前用户角色
                var userRoles = BC.DC.GetEntityAsync<SysRoleLogin>(u => u.UserId == BC.Id);
                foreach (var item in userRoles)
                {
                    roles.Add(item.Role);
                }
            }
            //获取未登录角色
            var noRole = await BC.DC.GetEntityAsync<SysRole>(CaviarConfig.NoLoginRoleGuid);
            roles.Add(noRole);
            return roles;
        }
    }
}
﻿using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control.Permission
{
    public partial class PermissionAction
    {
        /// <summary>
        /// 获取当前权限
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<List<SysPermission>> GetCurrentPermissions(List<SysRole> roles)
        {
            List<SysPermission> permissions = new List<SysPermission>();
            foreach (var item in roles)
            {
                var permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.RoleId == item.Id);
                permissions.AddRange(permission);
            }
            return permissions;
        }
        /// <summary>
        /// 设置角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="menuIds"></param>
        /// <returns></returns>
        public async Task SetRoleMenu(int roleId,int[] menuIds)
        {
            var menus = await GetRoleMenu(roleId);
            if (menus == null) return;
            var Ids = menus.Where(u=>u.IsDisable==false).Select(u => u.Id);
            var addIds = menuIds.Except(Ids).ToArray();
            var deleteIds = Ids.Except(menuIds).ToArray();
            List<SysPermission> addSysPermission = new List<SysPermission>();
            foreach (var item in addIds)
            {
                var permission = new SysPermission()
                {
                    PermissionId = item,
                    PermissionType = PermissionType.Menu,
                    RoleId = roleId
                };
                addSysPermission.Add(permission);
            }
            List<SysPermission> deleteSysPermission = new List<SysPermission>();
            foreach (var item in deleteIds)
            {
                var permission = await BC.DC.GetFirstEntityAsync<SysPermission>(u => u.RoleId == roleId && u.PermissionId == item && u.PermissionType == PermissionType.Menu);
                deleteSysPermission.Add(permission);
            }
            await BC.DC.DeleteEntityAsync(deleteSysPermission, IsDelete: true);//该权限不需要保存，直接彻底删除
            await BC.DC.AddEntityAsync(addSysPermission);
        }
        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<SysMenu>> GetRoleMenu(int roleId)
        {
            List<SysMenu> menus = new List<SysMenu>();
            IEnumerable<SysPermission> permission;
            if (roleId == 0)
            {
                permission = BC.UserData.Permissions.Where(u => u.PermissionType == PermissionType.Menu);
            }
            else
            {
                permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.RoleId==roleId && u.PermissionType == PermissionType.Menu);
            }
            var allMneus = BC.UserData.Menus;
            if (BC.IsAdmin)
            {
                allMneus = await BC.DC.GetAllAsync<SysMenu>();
            }
            foreach (var item in allMneus)
            {
                var menu = permission.FirstOrDefault(u => u.PermissionId == item.Id);
                if (menu == null) 
                {
                    item.IsDisable = true;
                }
                else
                {
                    item.IsDisable = false;
                }
                menus.Add(item);
            }
            menus.OrderBy(u => u.Number);
            return menus;
        }
        /// <summary>
        /// 获取角色字段
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<SysModelFields>> GetRoleFields(string fullName,int roleId = 0)
        {
            if (string.IsNullOrEmpty(fullName)) return null;
            IEnumerable<SysPermission> permission;
            if (roleId == 0)
            {
                permission = BC.UserData.Permissions.Where(u => u.PermissionType == PermissionType.Field);
            }
            else{
                permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.RoleId == roleId && u.PermissionType == PermissionType.Field);
            }
            var fields = await BC.DC.GetEntityAsync<SysModelFields>(u => u.FullName == fullName);
            foreach (var item in fields)
            {
                if (permission.FirstOrDefault(u => u.PermissionId == item.Id)!=null)
                {
                    item.IsDisable = false;
                }
            }
            return fields;
        }
        /// <summary>
        /// 设置角色字段
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="roleId"></param>
        /// <param name="modelFields"></param>
        /// <returns></returns>
        public async Task SetRoleFields(string fullName, int roleId, List<SysModelFields> modelFields)
        {
            if (string.IsNullOrEmpty(fullName) || roleId == 0) return;
            var permission = await BC.DC.GetEntityAsync<SysPermission>(u => u.RoleId == roleId && u.PermissionType == PermissionType.Field);
            var fields = await BC.DC.GetEntityAsync<SysModelFields>(u => u.FullName == fullName);
            foreach (var item in modelFields)
            {
                var field = fields.FirstOrDefault(u => u.TypeName == item.TypeName);
                if (field == null) continue;
                var perm = permission.FirstOrDefault(u => u.PermissionType == PermissionType.Field && u.PermissionId == field.Id);
                field.Width = item.Width;
                field.Number = item.Number;
                field.IsPanel = item.IsPanel;
                if (!string.IsNullOrEmpty(item.DisplayName))
                {
                    field.DisplayName = item.DisplayName;
                }
                await BC.DC.UpdateEntityAsync(field);
                if (item.IsDisable)
                {
                    if (perm != null)
                    {
                        await BC.DC.DeleteEntityAsync(perm,IsDelete:true);
                    }

                }
                else
                {
                    if (perm == null)
                    {
                        perm = new SysPermission()
                        {
                            PermissionType = PermissionType.Field,
                            PermissionId = field.Id,
                            RoleId = roleId,
                        };
                        await BC.DC.AddEntityAsync(perm);
                    }
                }
            }
        }
    }
}

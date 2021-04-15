﻿using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Control
{
    public partial class SysUserLoginAction : SysUserLogin
    {

        public virtual string Login()
        {

            if (string.IsNullOrEmpty(Password) || Password.Length != 64) return "用户名或密码错误";
            SysUserLogin userLogin = null;
            if (!string.IsNullOrEmpty(UserName))
            {
                userLogin = BaseControllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.UserName == UserName && u.Password == Password).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(PhoneNumber))
            {
                userLogin = BaseControllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.PhoneNumber == PhoneNumber && u.Password == Password).FirstOrDefault();
            }
            if (userLogin == null) return "用户名或密码错误";
            this.AutoAssign(userLogin);
            BaseControllerModel.UserName = UserName;
            BaseControllerModel.Id = Id;
            BaseControllerModel.IsLogin = true;
            return "登录成功，欢迎回来";
        }

        public virtual bool Register(out string msg)
        {
            var count = BaseControllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.UserName == UserName).Count();
            if (count > 0)
            {
                msg = "该用户名已经被注册！";
                return false;
            }
            count = BaseControllerModel.DataContext.GetEntityAsync<SysUserLogin>(u => u.PhoneNumber == PhoneNumber).Count();
            if (count > 0)
            {
                msg = "该手机号已经被注册！";
                return false;
            }
            count = BaseControllerModel.DataContext.AddEntityAsync(this).Result;
            if (count <= 0)
            {
                msg = "注册账号失败！";
                return false;
            }
            msg = "账号注册成功";
            return true;
        }
    }
}
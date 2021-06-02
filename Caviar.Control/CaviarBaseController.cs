﻿using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using Caviar.Models;
using Microsoft.Extensions.Primitives;
using System.Web;
using Caviar.Control.Role;
using System.Threading.Tasks;
using Caviar.Control.Permission;
using Caviar.Control.Menu;

namespace Caviar.Control
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public partial class CaviarBaseController : Controller
    {
        IBaseControllerModel _controllerModel;
        public IBaseControllerModel BC
        {
            get
            {
                if (_controllerModel == null)
                {
                    _controllerModel = HttpContext.RequestServices.GetService<BaseControllerModel>();
                }
                return _controllerModel;
            }
        }
        


        Stopwatch stopwatch = new Stopwatch();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            stopwatch.Start();
            base.OnActionExecuting(context);
            //获取ip地址
            BC.Current_Ipaddress = context.HttpContext.GetUserIp();
            //获取完整Url
            BC.Current_AbsoluteUri = context.HttpContext.Request.GetAbsoluteUri();
            //获取请求路径
            BC.Current_Action = context.HttpContext.Request.Path.Value;
            //请求上下文
            BC.HttpContext = HttpContext;

            if (HttpContext.Request.Headers.TryGetValue("UsreToken", out StringValues value))
            {
                string base64url = value[0];
                string base64 = HttpUtility.UrlDecode(base64url);
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                UserToken userToken = JsonConvert.DeserializeObject<UserToken>(json);
                var token = CaviarConfig.GetUserToken(userToken);
                if (token != userToken.Token)
                {
                    context.Result = ResultUnauthorized("您的登录已过期，请重新登录");
                    return;
                }
                var outTime = (userToken.CreateTime.AddMinutes(CaviarConfig.TokenDuration) - DateTime.Now);
                if (outTime.TotalSeconds <= 0)
                {
                    context.Result = ResultUnauthorized("您的登录已过期，请重新登录");
                    return;
                }
                BC.UserToken = userToken;
            }
            GetRolePermission();
            var IsVerification = ActionVerification();
            if (!IsVerification)
            {
                context.Result = ResultForbidden("对不起，您还没有获得该权限");
                return;
            }

            if (context.ActionArguments.Count > 0)
            {
                foreach (var ArgumentsItem in context.ActionArguments)
                {
                    if(ArgumentsItem.Value is IBaseModel)
                    {
                        ((IBaseModel)ArgumentsItem.Value).BC = BC;
                    }
                }
            }
        }
        /// <summary>
        /// 获取用户角色和权限
        /// 可以做缓存，未做
        /// </summary>
        /// <returns></returns>
        void GetRolePermission()
        {
            var roleAction = CreateModel<RoleAction>();
            BC.Roles = roleAction.GetCurrentRoles().Result;
            var permissionAction = CreateModel<PermissionAction>();
            BC.Permissions = permissionAction.GetCurrentPermissions(BC.Roles);
        } 

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            stopwatch.Stop();
            //日志记录，这里应该想一个更好的办法
            
        }
        
        #region 创建模型
        protected virtual T CreateModel<T>() where T : class, IBaseModel
        {
            var entity = BC.HttpContext.RequestServices.GetRequiredService<T>();
            entity.BC = BC;
            return entity;
        }
        #endregion



        protected virtual bool ActionVerification()
        {
            if (CaviarConfig.IsDebug) return true;

            return true;
        }

        #region  日志消息
        protected void LoggerMsg<T>(string msg, string action = "", LogLevel logLevel = LogLevel.Information, bool IsSucc = true)
        {
            BC.GetLogger<T>().LogInformation($"用户：{BC.UserName} 访问地址：{BC.Current_AbsoluteUri} 访问ip：{BC.Current_Ipaddress} 执行时间：{stopwatch.Elapsed} 执行结果：{IsSucc} 执行消息：{msg}");
        }
        #endregion


        #region 消息回复
        private ResultMsg _resultMsg;
        protected ResultMsg ResultMsg
        {
            get
            {
                if (_resultMsg == null)
                {
                    _resultMsg = HttpContext.RequestServices.GetRequiredService<ResultMsg>();
                }
                return _resultMsg;
            }
        }

        protected virtual IActionResult ResultOK()
        {
            return Ok(ResultMsg);
        }

        protected virtual IActionResult ResultOK(string title)
        {
            ResultMsg.Title = title;
            return Ok(ResultMsg);
        }

        protected virtual IActionResult ResultOK(ResultMsg resultMsg)
        {
            return Ok(resultMsg);
        }



        protected virtual IActionResult ResultError(int status, string title, string detail)
        {
            ResultMsg.Status = status;
            ResultMsg.Title = title;
            ResultMsg.Detail = detail;
            return Ok(ResultMsg);
        }
        protected virtual IActionResult ResultError(int status, string title, string detail, IDictionary<string, string[]> errors)
        {
            ResultMsg.Status = status;
            ResultMsg.Title = title;
            ResultMsg.Detail = detail;
            ResultMsg.Errors = errors;
            return Ok(ResultMsg);
        }

        protected virtual IActionResult ResultError(int status, string title)
        {
            ResultMsg.Status = status;
            ResultMsg.Title = title;
            return Ok(ResultMsg);
        }

        protected virtual IActionResult ResultError(ResultMsg resultMsg)
        {
            return Ok(resultMsg);
        }


        /// <summary>
        /// 返回（无权限）
        /// </summary>
        /// <returns></returns>
        protected virtual IActionResult ResultForbidden(string title)
        {
            return ResultError(403, title);
        }
        /// <summary>
        /// 返回此结果定位到登录界面（登录失效）
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        protected virtual IActionResult ResultUnauthorized(string title)
        {
            return ResultError(401, title);
        }

        protected virtual IActionResult ResultErrorMsg(string title, string detail)
        {
            return ResultError(406, title, detail);
        }
        protected virtual IActionResult ResultErrorMsg(string title)
        {
            return ResultError(406, title);
        }
        #endregion


    }
}
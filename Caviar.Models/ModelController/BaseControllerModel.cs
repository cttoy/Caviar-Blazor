﻿using Caviar.Models.SystemData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models
{
    [DIInject(InjectType.SCOPED)]
    public class BaseControllerModel : IBaseControllerModel
    {
        /// <summary>
        /// 数据上下文
        /// </summary>
        public IDataContext DataContext => HttpContext.RequestServices.GetRequiredService<IDataContext>();
        /// <summary>
        /// 获取日志记录
        /// </summary>
        public ILogger<T> GetLogger<T>() => HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
        /// <summary>
        /// 当前请求路径
        /// </summary>
        public string Current_Action { get; set; }
        /// <summary>
        /// 当前请求ip地址
        /// </summary>
        public string Current_Ipaddress { get; set; }
        /// <summary>
        /// 当前请求的完整Url
        /// </summary>
        public string Current_AbsoluteUri { get; set; }

        public HttpContext HttpContext { get; set; }

        public string UserName { get; set; } = "未登录用户";

        public int Id { get; set; }

        public bool IsLogin { get; set; } = false;

        public string PhoneNumber { get; set; }
    }
}
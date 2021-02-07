﻿using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using AntDesign;

namespace Caviar.UI.Shared
{
    partial class MainLayout
    {
        [Inject]
        IConfiguration Configuration { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }
        /// <summary>
        /// logo图片地址
        /// </summary>
        string LogoImgSrc;

        string LogoImg;
        string LogoImgIco;

        /// <summary>
        /// 面包屑数据同步
        /// </summary>
        public MenuItem BreadcrumbItemNav;

        protected override void OnInitialized()
        {
            LogoImg = Configuration["Logo:LogoPath:logo"];
            LogoImgIco = Configuration["Logo:LogoPath:logo-Ico"];
            LogoImgSrc = LogoImg;
            base.OnInitialized();
        }

        bool _collapsed;
        bool Collapsed
        {
            set
            {
                if (value)
                {
                    LogoImgSrc = LogoImgIco;
                }
                else
                {
                    LogoImgSrc = LogoImg;
                }
                _collapsed = value;
            }
            get
            {
                return _collapsed;
            }
        }
        

        void Toggle()
        {
            Collapsed = !Collapsed;
        }

        void OnCollapse(bool collapsed)
        {
            this.Collapsed = collapsed;
        }
    }
}

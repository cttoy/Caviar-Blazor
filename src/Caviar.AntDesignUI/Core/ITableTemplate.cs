﻿// Copyright (c) BeiYinZhiNian (1031622947@qq.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: http://www.caviar.wang/ or https://gitee.com/Cherryblossoms/caviar.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Caviar.AntDesignUI.Core
{
    public interface ITableTemplate
    {
        [Parameter]
        public string CurrentUrl { get; set; }
        public Task<bool> Validate();
    }
}

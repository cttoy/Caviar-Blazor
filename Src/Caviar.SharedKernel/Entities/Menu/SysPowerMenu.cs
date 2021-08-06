﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    [DisplayName("SysMenu")]
    public partial class SysMenu : SysBaseEntity, IBaseEntity
    {
        [DisplayName("MenuName")]
        [Required(ErrorMessage = "RequiredErrorMsg")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string MenuName { get; set; }
        [DisplayName("MenuType")]
        public MenuType MenuType { get; set; }

        [DisplayName("TargetType")]
        public TargetType TargetType { get; set; }
        [DisplayName("Url")]
        [StringLength(1024, ErrorMessage = "LengthErrorMsg")]
        public string Url { get; set; }

        [DisplayName("Icon")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string Icon { get; set; }

        [DisplayName("ParentId")]
        public int ParentId { get; set; }

        [DisplayName("ButtonPosition")]
        public ButtonPosition ButtonPosition { get; set; }

        [DisplayName("IsDoubleTrue")]
        public bool IsDoubleTrue { get; set; }
    }


}
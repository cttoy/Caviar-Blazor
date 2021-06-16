﻿using AntDesign;
using Caviar.Models.SystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.AntDesignPages.Pages.UserGroup
{
    public partial class DataTemplate
    {
        partial void PratialOnInitializedAsync(ref bool IsContinue)
        {
            GetParents();
        }

        private List<ViewUserGroup> Parents = new List<ViewUserGroup>();
        string ParentName { get; set; } = "无上层目录";

        async void GetParents()
        {
            var result = await Http.GetJson<PageData<ViewUserGroup>>("UserGroup/Index?pageSize=100");
            if (result.Status != 200) return;
            if (DataSource.ParentId > 0)
            {
                List<ViewUserGroup> listData = new List<ViewUserGroup>();
                result.Data.Rows.TreeToList(listData);
                var parent = listData.SingleOrDefault(u => u.Id == DataSource.ParentId);
                if (parent != null)
                {
                    ParentName = parent.Name;
                }
            }
            Parents = result.Data.Rows;
            StateHasChanged();
        }



        void EventRecord(TreeEventArgs<ViewUserGroup> args)
        {
            ParentName = args.Node.Title;
            DataSource.ParentId = int.Parse(args.Node.Key);
        }

        void RemoveRecord()
        {
            ParentName = "无上层目录";
            DataSource.ParentId = 0;
        }
    }
}
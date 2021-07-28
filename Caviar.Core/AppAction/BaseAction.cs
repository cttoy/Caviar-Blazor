﻿using Caviar.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core.ModelAction
{
    public partial class BaseAction<T,ViewT> : ActionResult, IBaseAction<T, ViewT> where T : class, IBaseModel,new()  where ViewT: class,T, new()
    {
        public IInteractor Interactor { get; set; }

        public ResultMsg ResultMsg { get; set; } = new ResultMsg();
        public virtual async Task<ResultMsg> AddEntity(T entity)
        {
            try
            {
                var count = await Interactor.DbContext.AddEntityAsync(entity);
                if (count > 0)
                {
                    return Ok();
                }
                throw new Exception("添加失败，添加结果：" + count);
            }
            catch(Exception e)
            {
                return Error("添加数据失败", e.Message);
            }
        }
       
        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultMsg> DeleteEntity(T entity)
        {
            try
            {
                if (entity.Uid == CaviarConfig.SysAdminRoleGuid)
                {
                    return Error("不能删除管理员角色");
                }
                else if(entity.Uid == CaviarConfig.NoLoginRoleGuid)
                {
                    return Error("不能默认用户组");
                }
                else if (entity.Uid == CaviarConfig.UserAdminGuid)
                {
                    return Error("不能删除默认用户");
                }
                var count = await Interactor.DbContext.DeleteEntityAsync(entity);
                if (count > 0)
                {
                    return Ok();
                }
                return Error("删除失败，删除结果：" + count);
            }
            catch (Exception e)
            {
                return Error("删除数据失败", e.Message);
            }
        }
        
        /// <summary>
        /// 修改指定实体
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultMsg> UpdateEntity(T entity)
        {
            try
            {
                var count = await Interactor.DbContext.UpdateEntityAsync(entity);
                if (count > 0)
                {
                    return Ok();
                }
                throw new Exception("修改失败，删除结果：" + count);
            }
            catch (Exception e)
            {
                return Error("修改数据失败", e.Message);
            }
        }
        
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultMsg<PageData<ViewT>>> GetPages(Expression<Func<T, bool>> where, int pageIndex, int pageSize, bool isOrder = true, bool isNoTracking = true)
        {
            var pages = await Interactor.DbContext.GetPageAsync(where, u => u.Number, pageIndex, pageSize, isOrder, isNoTracking);
            var list = ToViewModel(pages.Rows);
            PageData<ViewT> viewPage = new PageData<ViewT>(list);
            viewPage.PageIndex = pages.PageIndex;
            viewPage.PageSize = pages.PageSize;
            viewPage.Total = pages.Total;
            return Ok(viewPage);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<ResultMsg> DeleteEntity(List<T> menus)
        {
            try
            {
                var count = await Interactor.DbContext.DeleteEntityAsync(menus);
                if (count > 0)
                {
                    return Ok();
                }
                throw new Exception("删除失败，删除结果：" + count);
            }
            catch (Exception e)
            {
                return Error("删除数据失败", e.Message);
            }
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public virtual async Task<ResultMsg> UpdateEntity(List<SysMenu> menus)
        {
            try
            {
                var count = await Interactor.DbContext.UpdateEntityAsync(menus);
                if (count > 0)
                {
                    return Ok();
                }
                throw new Exception("修改失败，删除结果：" + count);
            }
            catch (Exception e)
            {
                return Error("修改数据失败", e.Message);
            }
        }
        /// <summary>
        /// 通用模糊查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual ResultMsg<PageData<ViewT>> FuzzyQuery(ViewQuery query)
        {
            var fields = Interactor.UserData.ModelFields.Where(u => u.BaseTypeName == typeof(T).Name).ToList();
            if (fields == null) return Error<PageData<ViewT>>("没有对该对象的查询权限");
            var assemblyList = CommonlyHelper.GetAssembly();
            Type type = null;
            foreach (var item in assemblyList)
            {
                type = item.GetTypes().SingleOrDefault(u => u.Name.ToLower() == typeof(T).Name.ToLower());
                if (type != null) break;
            }
            if (type == null) return default;
            List<SqlParameter> parameters = new List<SqlParameter>();
            var queryField = "";
            if (query.QueryData.Count > 0)
            {
                queryField = "and (";
                var i = 0;
                foreach (var item in query.QueryData)
                {
                    var field = fields.FirstOrDefault(u => u.TypeName == item.Key);
                    if (field == null) return default;
                    queryField += $" {item.Key} LIKE @{item.Key}Query ";
                    parameters.Add(new SqlParameter($"@{item.Key}Query", "%" + item.Value + "%"));
                    i++;
                    if (i < query.QueryData.Count)
                    {
                        queryField += " and ";
                    }
                }
                queryField += ")";
            }
            var from = CommonlyHelper.GetCavBaseType(type)?.Name;
            string sql = $"select top(20)* from {from} where IsDelete=0 " + queryField;
            if (query.StartTime != null)
            {
                sql += $" and CreatTime>=@StartTime ";
                parameters.Add(new SqlParameter("@StartTime", query.StartTime));
            }
            if (query.EndTime != null)
            {
                sql += $" and CreatTime<=@EndTime ";
                parameters.Add(new SqlParameter("@EndTime", query.EndTime));
            }
            var data = Interactor.DbContext.SqlQuery(sql, parameters.ToArray());
            var model = data.ToList<T>(type);
            var viewModel = ToViewModel(model);
            var pages = new PageData<ViewT>(viewModel);
            pages.Total = viewModel.Count;
            pages.PageSize = viewModel.Count;
            return Ok(pages);
        }

        /// <summary>
        /// 数据转换
        /// 需要达到一个model转为viewModel效果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual List<ViewT> ToViewModel(List<T> model)
        {
            model.AToB(out List<ViewT> outModel);
            return outModel;
        }

        public virtual ViewT ToViewModel(T model)
        {
            var list = ToViewModel(new List<T>() { model });
            return list.FirstOrDefault();
        }

        public virtual async Task<ResultMsg<ViewT>> GetEntity(Guid guid)
        {
            var entity = await Interactor.DbContext.GetEntityAsync<T>(guid);
            var viewModel = ToViewModel(entity);
            return Ok(viewModel);
        }

        public virtual async Task<ResultMsg<ViewT>> GetEntity(int id)
        {
            var entity = await Interactor.DbContext.GetEntityAsync<T>(id);
            var viewModel = ToViewModel(entity);
            return Ok(viewModel);
        }
    }
}
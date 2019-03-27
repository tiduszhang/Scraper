/*----------------------------------------------------------------

// Copyright (C) 2016 南京思创信息技术有限公司 版权所有。
//
// 文件名：.cs
// 文件功能描述：
//
/// @author zhangsx
/// @date 2016/12/05 11:18:19
//
//----------------------------------------------------------------*/
using MVVM.Controls;
using MVVM.Model;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// 分页实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageData<T> : EntityBase
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex
        {
            get
            {
                return this.GetValue(o => o.PageIndex);
            }
            set
            {
                this.SetValue(o => o.PageIndex, value);
            }
        }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize
        {
            get
            {
                return this.GetValue(o => o.PageSize);
            }
            set
            {
                this.SetValue(o => o.PageSize, value);
            }
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageCount
        {
            get
            {
                return this.GetValue(o => o.TotalPageCount);
            }
            set
            {
                this.SetValue(o => o.TotalPageCount, value);
            }
        }

        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount
        {
            get
            {
                return this.GetValue(o => o.TotalCount);
            }
            set
            {
                this.SetValue(o => o.TotalCount, value);
            }
        }

        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<T> QueryData
        {
            get
            {
                return this.GetValue(o => o.QueryData);
            }
            set
            {
                this.SetValue(o => o.QueryData, value);
            }
        }
         
        /// <summary>
        /// 把分页信息设置到PageViewModel中
        /// </summary>
        /// <param name="pageViewModel"></param>
        public void FillPage(PageViewModel<T> pageViewModel)
        {
            pageViewModel.PageIndex = this.PageIndex;
            pageViewModel.TotalCount = this.TotalCount;
            pageViewModel.TotalPageCount = this.TotalPageCount;
        }
         
    }
}
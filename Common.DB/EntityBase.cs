using MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    public abstract class EntityBase : NotifyPropertyBase
    {
        /// <summary>
        /// 实体类基类
        /// </summary>
        public EntityBase()
        {
            //IsNew = true;
        }

        /// <summary>
        /// 序号
        /// </summary>
        public virtual int Sequence { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public virtual string ID
        {
            get
            {
                return this.GetValue(o => o.ID);
            }
            set
            {
                this.SetValue(o => o.ID, value);
            }
        }
    }
}
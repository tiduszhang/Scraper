using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace MVVM.Model
{
    /// <summary>
    /// 属性短名称
    /// </summary>
    public class ShortNameData : NotifyPropertyBase
    {
        /// <summary>
        /// 需要验证的类
        /// </summary>
        [ScriptIgnore]
        public NotifyPropertyBase NotifyProperty
        {
            get
            {
                return this.GetValue(o => o.NotifyProperty);
            }
            set
            {
                this.SetValue(o => o.NotifyProperty, value);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ShortNameData()
        {
        }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="propertyName"> </param>
        /// <returns> </returns>>
        [ScriptIgnore]
        public string this[string propertyName]
        {
            get
            {
                string propertyNameValue = "";
                if (NotifyProperty != null)
                {
                    propertyNameValue = NotifyProperty.GetPropertyShortName(propertyName);
                }
                return propertyNameValue;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (NotifyProperty != null)
            {
                NotifyProperty.Dispose();
            }
            base.Dispose();
        }
    }
}

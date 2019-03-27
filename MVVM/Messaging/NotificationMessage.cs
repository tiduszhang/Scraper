using MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.Messaging
{
    /// <summary>
    /// 互通消息对象
    /// </summary>
    public class NotificationMessage : NotifyBaseModel
    {
        /// <summary>
        /// 消息关键字
        /// </summary>
        public string Key
        {
            get
            {
                return this.GetValue(o => o.Key);
            }
            set
            {
                this.SetValue(o => o.Key, value);
            }
        }

        /// <summary>
        /// 消息数据内容
        /// </summary>
        public object Data
        {
            get
            {
                return this.GetValue(o => o.Data);
            }
            set
            {
                this.SetValue(o => o.Data, value);
            }
        }
         
    }
}

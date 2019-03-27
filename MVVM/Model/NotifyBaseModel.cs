using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using Common;

namespace MVVM.Model
{
    /// <summary>
    /// 实体基类，可以实现属性修改事件通知
    /// 附加功能：
    /// 1、定义属性验证
    /// 2、定义属性名称
    /// 3、定义属性描述
    /// </summary>
    public class NotifyBaseModel : NotifyPropertyBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NotifyBaseModel()
        {
            this.Error = new ValidationErrorData
            {
                NotifyProperty = this
            };

            this.DisplayName = new DisplayNameData
            {
                NotifyProperty = this
            };

            this.ShortName = new ShortNameData
            {
                NotifyProperty = this
            };

            this.Prompt = new PromptData
            {
                NotifyProperty = this
            };

            this.Description = new DescriptionData
            {
                NotifyProperty = this
            };
        }

        /// <summary>
        /// 错误信息，通过IDataErrorInfo实现
        /// </summary>
        [ScriptIgnore]
        public ValidationErrorData Error
        {
            get
            {
                return this.GetValue(o => o.Error);
            }
            set
            {
                this.SetValue(o => o.Error, value);
            }
        }

        /// <summary>
        /// 属性名称，可以通过DisplayAttribute标注属性名称
        /// </summary>
        [ScriptIgnore]
        public DisplayNameData DisplayName
        {
            get
            {
                return this.GetValue(o => o.DisplayName);
            }
            set
            {
                this.SetValue(o => o.DisplayName, value);
            }
        }

        /// <summary>
        /// 属性短名称，可以通过DisplayAttribute标注属性名称
        /// </summary>
        [ScriptIgnore]  
        public ShortNameData ShortName
        {
            get
            {
                return this.GetValue(o => o.ShortName);
            }
            set
            {
                this.SetValue(o => o.ShortName, value);
            }
        }

        /// <summary>
        /// 属性水印提示，可以通过DisplayAttribute标注属性名称
        /// </summary>
        [ScriptIgnore]
        public PromptData Prompt
        {
            get
            {
                return this.GetValue(o => o.Prompt);
            }
            set
            {
                this.SetValue(o => o.Prompt, value);
            }
        }

        /// <summary>
        /// 属性描述，可以通过DisplayAttribute标注属性描述
        /// </summary>
        [ScriptIgnore]
        public DescriptionData Description
        {
            get
            {
                return this.GetValue(o => o.Description);
            }
            set
            {
                this.SetValue(o => o.Description, value);
            }
        }

        /// <summary>
        /// 消息内容，所有错误消息内容。
        /// </summary>
        [ScriptIgnore]
        public virtual string ErrorMessage
        {
            get
            {
                return this.GetValue(o => o.ErrorMessage);
            }
            set
            {
                this.SetValue(o => o.ErrorMessage, value);
            }
        }

        /// <summary>
        /// 是否验证通过
        /// </summary>
        [ScriptIgnore]
        public virtual bool IsValid
        {
            get
            {
                return this.GetValue(o => o.IsValid);
            }
            set
            {
                this.SetValue(o => o.IsValid, value);
            }
        }


        /// <summary>
        /// 验证属性
        /// </summary>
        /// <returns> 验证是否通过，若通过则返回 true，否则为false。 </returns>
        public virtual bool Valid()
        {
            this.ErrorMessage = "";
            Type tp = this.GetType();
            PropertyInfo[] pis = tp.GetProperties();
            IsValid = true;
            foreach (PropertyInfo pi in pis)
            {
                try
                {
                    if (pi.Name.ToUpper() != "ITEM")
                    {
                        string strError = this.Error.Valid(pi.Name);
                        if (!String.IsNullOrWhiteSpace(strError))
                        {
                            IsValid = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            if (!IsValid)
            {
                this.ErrorMessage = this.Error.Error;
            }
            return IsValid;
        }

        /// <summary>
        /// 验证属性
        /// </summary>
        /// <param name="Valided"> 先执行属性特性验证。再执行相关验证方法。 </param>
        /// <returns> 验证是否通过，若通过则返回 true，否则为false。 </returns>
        public virtual bool Valid(Func<object, string> Valided)
        {
            return Valid<NotifyBaseModel>(Valided);
        }

        /// <summary>
        /// 验证属性
        /// </summary>
        /// <typeparam name="T"> 当前对象泛型 </typeparam>
        /// <param name="Valided"> 先执行属性特性验证。再执行相关验证方法。 </param>
        /// <returns> 验证是否通过，若通过则返回 true，否则为false。 </returns>
        public bool Valid<T>(Func<T, string> Valided) where T : class
        {
            if (!Valid())
            {
                return false;
            }
            string message = Valided.Invoke(this as T);
            if (!String.IsNullOrWhiteSpace(message))
            {
                this.ErrorMessage = message;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (Error != null)
            {
                Error.Dispose();
            }
            if (DisplayName != null)
            {
                DisplayName.Dispose();
            }
            if (ShortName != null)
            {
                ShortName.Dispose();
            }
            if (Prompt != null)
            {
                Prompt.Dispose();
            }
            if (Description != null)
            {
                Description.Dispose();
            }
            base.Dispose();
        }
    }
}
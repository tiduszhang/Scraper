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
    /// 验证信息数据
    /// </summary>
    public class ValidationErrorData : NotifyPropertyBase, IDataErrorInfo
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
        public ValidationErrorData()
        {
        }

        /// <summary>
        /// 属性和属性验证值
        /// </summary>
        private Dictionary<string, string> _ErrorDictionary = new Dictionary<string, string>();

        /// <summary>
        /// 错误信息
        /// </summary>
        [ScriptIgnore]
        public string Error
        {
            get
            {
                string strError = "";
                if (_ErrorDictionary.Count > 0)
                {
                    strError = String.Join(System.Environment.NewLine, _ErrorDictionary.Values.ToArray());
                }
                _ErrorDictionary.Clear();
                return strError;
            }
        }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="columnName"> </param>
        /// <returns> </returns>>
        [ScriptIgnore]
        public string this[string columnName]
        {
            get
            {
                string strError = "";
                //string strError = this.NotifyProperty.ValidateProperty(columnName);
                //if (!String.IsNullOrWhiteSpace(strError))
                //{
                //    if (!_ErrorDictionary.ContainsKey(columnName))
                //    {
                //        _ErrorDictionary.Add(columnName, strError);
                //    }
                //    else
                //    {
                //        _ErrorDictionary[columnName] = strError;
                //    }
                //}
                //else
                //{
                //    if (!_ErrorDictionary.ContainsKey(columnName))
                //    {
                //        _ErrorDictionary.Remove(columnName);
                //    }
                //}
                if (!_ErrorDictionary.ContainsKey(columnName))
                {
                    strError = _ErrorDictionary[columnName];
                }
                return strError;
            }
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <returns></returns>
        public string Valid(string columnName)
        {
            string strError = "";
            if (NotifyProperty != null)
            {
                strError = this.NotifyProperty.ValidateProperty(columnName);
                if (!String.IsNullOrWhiteSpace(strError))
                {
                    if (!_ErrorDictionary.ContainsKey(columnName))
                    {
                        _ErrorDictionary.Add(columnName, strError);
                    }
                    else
                    {
                        _ErrorDictionary[columnName] = strError;
                    }
                }
                else
                {
                    if (!_ErrorDictionary.ContainsKey(columnName))
                    {
                        _ErrorDictionary.Remove(columnName);
                    }
                }
            }

            return strError;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (_ErrorDictionary != null && _ErrorDictionary.Count > 0)
            {
                _ErrorDictionary.Clear();
            }
            if (NotifyProperty != null)
            {
                NotifyProperty.Dispose();
            }
            base.Dispose();
        }
    }
}
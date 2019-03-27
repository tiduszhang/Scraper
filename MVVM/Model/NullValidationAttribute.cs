using System;

using System.ComponentModel.DataAnnotations;

namespace MVVM.Model
{
    /// <summary>
    /// 验证空数据特性
    /// </summary>
    public class NullValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// 验证方法
        /// </summary>
        /// <param name="value"> </param>
        /// <returns> </returns>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (NotTrim)
            {
                return !String.IsNullOrEmpty(value.ToString());
            }
            else
            {
                return !String.IsNullOrWhiteSpace(value.ToString());
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public override string FormatErrorMessage(string name)
        {
            if (String.IsNullOrWhiteSpace(ErrorMessage))
            {
                ErrorMessage = "请输入" + name + "！";
            }
            return ErrorMessage;
        }

        /// <summary>
        /// 是否需要去除头尾空格
        /// </summary>
        public bool NotTrim { get; set; }
    }
}
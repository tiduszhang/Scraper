using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MVVM.Model
{
    /// <summary>
    /// 验证模块-用于界面绑定验证实现
    /// </summary>
    public class NotifyBaseModelValidationRule : ValidationRule
    {
        /// <summary>
        /// 验证方法
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is NotifyBaseModel)
            {
                return new ValidationResult((value as NotifyBaseModel).Valid(), (value as NotifyBaseModel).ErrorMessage);
            }
            return ValidationResult.ValidResult;
            //throw new NotImplementedException();
        }
    }
}

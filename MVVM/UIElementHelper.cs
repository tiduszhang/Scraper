using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MVVM
{
    /// <summary>
    /// 界面帮助类
    /// </summary>
    public static class UIElementHelper
    { 
        /// <summary>
        /// 初始化界面实体验证
        /// </summary>
        public static void InIValidation<T>(this FrameworkElement frameworkElement, T value)
        {
            //移除验证模版
            frameworkElement.SetValue(System.Windows.Controls.Validation.ErrorTemplateProperty, null);
            //绑定验证机制
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding() { Source = value };
            binding.ValidatesOnDataErrors = true;
            binding.UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.Explicit;
            binding.ValidationRules.Add(new Model.NotifyBaseModelValidationRule() { ValidatesOnTargetUpdated = true });
            frameworkElement.SetBinding(FrameworkElement.DataContextProperty, binding);
        }

    }
}

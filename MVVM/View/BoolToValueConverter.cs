using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MVVM.View
{
    /// <summary>
    /// 布尔值显示值转换器
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToValueConverter : IValueConverter
    {
        /// <summary>
        /// True值
        /// </summary>
        public string TrueValue { get; set; }

        /// <summary>
        /// False值
        /// </summary>
        public string FalseValue { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BoolToValueConverter()
        {
            // set defaults
            TrueValue = "true";
            FalseValue = "false";
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="value"> </param>
        /// <param name="targetType"> </param>
        /// <param name="parameter"> </param>
        /// <param name="culture"> </param>
        /// <returns> </returns>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="value"> </param>
        /// <param name="targetType"> </param>
        /// <param name="parameter"> </param>
        /// <param name="culture"> </param>
        /// <returns> </returns>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }
    }
}
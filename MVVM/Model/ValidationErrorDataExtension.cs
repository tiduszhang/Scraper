using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MVVM.Model
{
    /// <summary>
    /// 数据验证扩展
    /// </summary>
    public static class ValidationErrorDataExtension
    {
        /// <summary>
        /// 验证方法
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="obj"> </param>
        /// <param name="propertyName"> </param>
        /// <returns> </returns>
        public static string ValidateProperty<T>(this T obj, string propertyName) where T : NotifyPropertyBase
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return string.Empty;
            }
            Type tp = obj.GetType();
            PropertyInfo pi = tp.GetProperty(propertyName);
            var value = pi.GetValue(obj, null);
            object[] Attributes = pi.GetCustomAttributes(false);
            string strErrorMessage = "";
            if (Attributes != null && Attributes.Length > 0)
            {
                foreach (object attribute in Attributes)
                {
                    if (attribute is ValidationAttribute)
                    {
                        try
                        {
                            ValidationAttribute vAttribute = attribute as ValidationAttribute;
                            if (!vAttribute.IsValid(value))
                            {
                                strErrorMessage = !String.IsNullOrWhiteSpace(vAttribute.ErrorMessage) ? vAttribute.ErrorMessage : vAttribute.GetValidationResult(value, new ValidationContext(value, null, null)).ErrorMessage;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                }
            }
            return strErrorMessage;
        }
    }
}
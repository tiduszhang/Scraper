using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common;

namespace MVVM.Model
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class NotifyPropertyBaseExtension
    {
        /// <summary>
        /// 属性访问
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <typeparam name="U"> </typeparam>
        /// <param name="t"> </param>
        /// <param name="exp"> </param>
        /// <returns> </returns>
        public static U GetValue<T, U>(this T t, Expression<Func<T, U>> exp) where T : NotifyPropertyBase
        {
            string _pN = GetPropertyName(exp);
            return t.GetPropertyValue<U>(_pN);
        }

        /// <summary>
        /// 属性访问
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <typeparam name="U"> </typeparam>
        /// <param name="t"> </param>
        /// <param name="exp"> </param>
        /// <param name="value"> </param>
        public static void SetValue<T, U>(this T t, Expression<Func<T, U>> exp, U value) where T : NotifyPropertyBase
        {
            string _pN = GetPropertyName(exp);
            t.SetPropertyValue<U>(_pN, value);
        }

        /// <summary>
        /// 得到属性名称
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <typeparam name="U"> </typeparam>
        /// <param name="exp"> </param>
        /// <returns> </returns>
        private static string GetPropertyName<T, U>(Expression<Func<T, U>> exp)
        {
            string _pName = "";
            if (exp.Body is MemberExpression)
            {
                _pName = (exp.Body as MemberExpression).Member.Name;
            }
            else if (exp.Body is UnaryExpression)
            {
                _pName = ((exp.Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            return _pName;
        } 
    }
}
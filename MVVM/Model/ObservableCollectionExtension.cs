using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using System.Linq;

namespace MVVM.Model
{
    /// <summary>
    /// 用于实现模型层绑定数据基类-集合数据
    /// </summary>
    public static class ObservableCollectionExtension
    {
        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Exists<T>(this ObservableCollection<T> collection, Predicate<T> predicate)
        {
            bool exists = false;

            foreach (T item in collection)
            {
                exists = predicate.Invoke(item);

                if (exists)
                {
                    break;
                }
            }
            return exists;
        }

        /// <summary>
        /// 遍历数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this ObservableCollection<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action.Invoke(item);
            }
        }

        /// <summary>
        /// 查找第一个符合条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Find<T>(this ObservableCollection<T> collection, Predicate<T> predicate)
        {
            foreach (T item in collection)
            {
                if (predicate.Invoke(item))
                {
                    return item;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 查找所有符合条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IList<T> FindAll<T>(this ObservableCollection<T> collection, Predicate<T> predicate)
        {
            IList<T> result = new List<T>();
            collection.ForEach(item =>
            {
                if (predicate.Invoke(item))
                {
                    result.Add(item);
                }
            });
            return result;
        }

        /// <summary>
        ///复制绑定对象到列表对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="lstresult"></param>
        /// <returns></returns>
        public static void CopyToList<T>(this ObservableCollection<T> collection, IList<T> lstresult)
        {
            collection.ForEach(o =>
            {
                lstresult.Add(o);
            });
        }

        /// <summary>
        /// 将界面对象列表转换成JSON字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string ToJson<T>(this ObservableCollection<T> collection)
        {
            return collection.ToList().JsStringify();
        }

        /// <summary>
        /// 将字符串转换成界面对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static void CopyToObservableCollection<T>(this string value, ObservableCollection<T> collection)
        {
            IList<T> lstresult = value.JsParse<List<T>>();
            lstresult.CopyToObservableCollection(collection);
        }

        /// <summary>
        /// 复制列表到绑定列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static void CopyToObservableCollection<T>(this IList<T> value, ObservableCollection<T> collection)
        {
            if (value != null)
            {
                foreach (T o in value)
                {
                    collection.Add(o);
                }
            }
        }

        /// <summary>
        /// 验证序列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static System.ComponentModel.DataAnnotations.ValidationResult IsValid<T>(this ObservableCollection<T> value) where T : NotifyBaseModel
        {
            if (value != null)
            {
                foreach (T o in value)
                {
                    if (!o.Valid())
                    {
                        return new System.ComponentModel.DataAnnotations.ValidationResult(o.ErrorMessage);
                    }
                }
            }
            return System.ComponentModel.DataAnnotations.ValidationResult.Success;
        }
    }
}
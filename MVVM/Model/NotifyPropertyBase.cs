using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MVVM.Model
{
    /// <summary>
    /// 基类
    /// </summary>
    public class NotifyPropertyBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// 属性和属性值
        /// </summary>
        private Dictionary<object, object> _ValueDictionary = new Dictionary<object, object>();

        #region 根据属性名得到属性值

        /// <summary>
        /// 属性访问
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="propertyName"> </param>
        /// <returns> </returns>
        public virtual T GetPropertyValue<T>(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("invalid " + propertyName);
            object _propertyValue;
            if (!_ValueDictionary.TryGetValue(propertyName, out _propertyValue))
            {
                _propertyValue = default(T);
                _ValueDictionary.Add(propertyName, _propertyValue);
            }
            return (T)_propertyValue;
        }

        #endregion 根据属性名得到属性值

        #region 设置指定的属性值

        /// <summary>
        /// 属性访问
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="propertyName"> </param>
        /// <param name="value"> </param>
        public virtual void SetPropertyValue<T>(string propertyName, T value)
        {
            if (!_ValueDictionary.ContainsKey(propertyName) || _ValueDictionary[propertyName] != (object)value)
            {
                _ValueDictionary[propertyName] = value;
            }
            OnPropertyChanged(propertyName);
        }

        #endregion 设置指定的属性值

        /// <summary>
        /// 构造函数
        /// </summary>
        public NotifyPropertyBase()
        {
        }

        #region INotifyPropertyChanged

        /// <summary>
        /// 属性修改
        /// </summary>
        /// <param name="propertyName"> </param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            if (_ValueDictionary != null && _ValueDictionary.Count > 0)
            {
                _ValueDictionary.Clear();
            }
        }

        /// <summary>
        /// 属性修改
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

    }
}
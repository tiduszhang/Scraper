using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static class ObjectTools
    {
        /// <summary>
        /// 设置属性区分大小写。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Parameter"></param>
        public static void SetProperty(this object value, string PropertyName, object Parameter = null)
        {
            if (!String.IsNullOrWhiteSpace(PropertyName))
            {
                var type = value.GetType();
                var property = type.GetProperty(PropertyName);
                if (property != null)
                {
                    property.SetValue(value, Parameter, null);
                }
            }
        }

        /// <summary>
        /// 获取属性区分大小写。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static object GetProperty(this object value, string PropertyName)
        {
            if (!String.IsNullOrWhiteSpace(PropertyName))
            {
                var type = value.GetType();
                var property = type.GetProperty(PropertyName);
                if (property != null)
                {
                    return property.GetValue(value, null);
                }
            }
            return null;
        }

        /// <summary>
        /// 注册事件区分大小写。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="EventName"></param>
        /// <param name="action"></param>
        public static void RegistEvent(this object value, string EventName, Delegate action)
        {
            if (String.IsNullOrWhiteSpace(EventName) && action != null)
            {
                var type = value.GetType();
                var eventInfo = type.GetEvent(EventName);
                if (eventInfo != null)
                {
                    eventInfo.AddEventHandler(value, action);
                }
            }
        }
        /// <summary>
        /// 注销事件区分大小写。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="EventName"></param>
        /// <param name="action"></param>
        public static void UnRegistEvent(this object value, string EventName, Delegate action)
        {
            if (String.IsNullOrWhiteSpace(EventName) && action != null)
            {
                var type = value.GetType();
                var eventInfo = type.GetEvent(EventName);
                if (eventInfo != null)
                {
                    eventInfo.RemoveEventHandler(value, action);
                }
            }
        }

        /// <summary>
        /// 执行方法区分大小写。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="methodName"></param>
        /// <param name="Parameter">方法参数，需要与方法定义参数类型和顺序保持一致。</param>
        /// <returns></returns> 
        public static object InvokeMethod(this object value, string methodName, params object[] Parameter)
        {
            var type = value.GetType();
            var method = type.GetMethod(methodName);//方法
            if (method != null)
            {
                if (Parameter != null && Parameter.Length > 0)//参数列表
                {
                    var methodParameters = method.GetParameters();
                    if (methodParameters != null && methodParameters.Length > 0)
                    {
                        if (methodParameters.Length == Parameter.Length)
                        {
                            bool isOk = true;
                            for (int i = 0; i < methodParameters.Length; i++)//检查方法参数
                            {
                                if (Parameter[i] != null)
                                {
                                    if (methodParameters[i].ParameterType == Parameter[i].GetType()
                                        || methodParameters[i].ParameterType.IsAssignableFrom(Parameter[i].GetType())
                                        || Parameter[i].GetType().IsSubclassOf(methodParameters[i].ParameterType))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        isOk = false;
                                        break;
                                    }
                                }
                            }
                            if (isOk)
                            {
                                return method.Invoke(value, Parameter);
                            }
                            else
                            {
#if DEBUG  
                                throw new Exception("参数与当前调用的方法不匹配！");
#endif
                            }
                        }
                    }
                }
                else
                {
                    return method.Invoke(value, null);
                }
            }
            return null;
        }

    }
}

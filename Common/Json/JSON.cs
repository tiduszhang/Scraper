using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Common
{
    /// <summary>
    /// 解析JSON，仿Javascript风格 
    /// </summary>
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    public static class JSON
    {
        /// <summary>
        /// 将字符串转换成二进制 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(this string value)
        {
            return Encoding.Default.GetBytes(value);
        }

        /// <summary>
        /// 将Object转换成二进制 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(this object value)
        {
            return value.JsStringify().ConvertToBytes();
        }

        /// <summary>
        /// 将二进制转换成对象 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertToObject<T>(this byte[] value) where T : class
        {
            return value.ConvertToString().JsParse<T>();
        }
        
        /// <summary>
        /// 将字符串转换成Json对象 
        /// </summary>
        /// <typeparam name="T"> 泛型对象 </typeparam>
        /// <param name="value"> 字符串 </param>
        /// <returns> 泛型对象 </returns>
        public static T JsParse<T>(this string value)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<T>(value);
        }

        /// <summary>
        /// 将对象转换成Json字符串 
        /// </summary>
        /// <param name="value"> 对象 </param>
        /// <returns> Json字符串 </returns>
        public static string JsStringify(this object value)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string str = js.Serialize(value);
            str = Regex.Replace(str, @"\\/Date\((\d+)\)\\/", match =>
            {
                DateTime dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                dt = dt.ToLocalTime();
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            });
            return str;
        }

        /// <summary>
        /// 加载JSON格式文件 
        /// </summary>
        /// <typeparam name="T"> 泛型类 </typeparam>
        /// <param name="Path"> 文件路径 </param>
        /// <param name="FileName"> 文件名 </param>
        /// <returns> 泛型实体 </returns>
        public static T LoadJsonFile<T>(this string Path, string FileName)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            string jsonFile = System.IO.Path.GetFullPath(Path + @"\" + FileName);
            //T objT = default(T);
            //string strJson = File.ReadAllText(jsonFile, Encoding.UTF8);
            //if (String.IsNullOrWhiteSpace(strJson))
            //{
            //    strJson = objT.JsStringify();
            //    File.WriteAllText(jsonFile, strJson, Encoding.UTF8);
            //}
            //else
            //{
            //    objT = strJson.JsParse<T>();
            //} 
            return jsonFile.LoadJsonFile<T>();
        }

        /// <summary>
        /// 加载JSON格式文件 
        /// </summary>
        /// <typeparam name="T"> 泛型类 </typeparam>
        /// <param name="FilePath"> 文件全路径 </param>
        /// <returns> 泛型实体 </returns>
        public static T LoadJsonFile<T>(this string FilePath)
        {
            T objT = default(T);
            if (File.Exists(FilePath))
            {
                string strJson = File.ReadAllText(FilePath);
                if (!String.IsNullOrWhiteSpace(strJson))
                {
                    objT = strJson.JsParse<T>();
                }
            }
            return objT;
        }

        /// <summary>
        /// 加载JSON格式文件 
        /// </summary>
        /// <typeparam name="T"> 泛型类 </typeparam>
        /// <param name="Path"> 文件全路径 </param> 
        /// <returns> 泛型实体 </returns>
        public static List<T> LoadJsonFiles<T>(this string Path)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            string[] strFiles = Directory.GetFiles(Path);
            if (strFiles != null && strFiles.Length > 0)
            {
                List<T> lstObj = new List<T>();
                foreach (string file in strFiles)
                {
                    try
                    {
                        lstObj.Add(file.LoadJsonFile<T>());
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
                return lstObj;
            }
            return null;
        }

        /// <summary>
        /// 加载JSON格式文件 
        /// </summary>
        /// <typeparam name="T"> 泛型类 </typeparam>
        /// <param name="Path"> 文件路径 </param> 
        /// <param name="searchPattern">   </param> 
        /// <returns> 泛型实体 </returns>
        public static List<T> LoadJsonFiles<T>(this string Path, string searchPattern)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            string[] strFiles = Directory.GetFiles(Path, searchPattern);
            if (strFiles != null && strFiles.Length > 0)
            {
                List<T> lstObj = new List<T>();
                foreach (string file in strFiles)
                {
                    try
                    {
                        T obj = file.LoadJsonFile<T>();
                        if (obj != null)
                        {
                            lstObj.Add(obj);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
                return lstObj;
            }
            return null;
        }

        /// <summary>
        /// 加载JSON格式文件 
        /// </summary>
        /// <typeparam name="T"> 泛型类 </typeparam>
        /// <param name="Path"> 文件路径 </param> 
        /// <param name="searchPattern">   </param>
        /// <param name="searchOption">   </param> 
        /// <returns> 泛型实体 </returns>
        public static List<T> LoadJsonFiles<T>(this string Path, string searchPattern, SearchOption searchOption)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            string[] strFiles = Directory.GetFiles(Path, searchPattern, searchOption);
            if (strFiles != null && strFiles.Length > 0)
            {
                List<T> lstObj = new List<T>();
                foreach (string file in strFiles)
                {
                    try
                    {
                        lstObj.Add(file.LoadJsonFile<T>());
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
                return lstObj;
            }
            return null;
        }


        /// <summary>
        /// 保存JSON文本 
        /// </summary>
        /// <typeparam name="T"> 泛型类 </typeparam>
        /// <param name="value"> 数据对象 </param>
        /// <param name="Path"> 文件路径 </param>
        /// <param name="FileName"> 文件名 </param>
        public static void SaveJsonFile<T>(this T value, string Path, string FileName)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            if (value != null)
            {
                string strJson = value.JsStringify();
                File.WriteAllText(System.IO.Path.GetFullPath(Path + @"\" + FileName), strJson, Encoding.UTF8);
            }
        }


        /// <summary>
        /// 转换成动态类型对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static dynamic ConvertToDynamic(this string value)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<object>(value);
            //serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            //return serializer.Deserialize(value, typeof(object));
        }

        /// <summary>
        /// 转换成动态类型对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static dynamic ConvertToDynamicObject(this string value)
        {
            var serializer = new JavaScriptSerializer();
            //return serializer.Deserialize<object>(value);
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            return serializer.Deserialize<dynamic>(value);
        }

        /// <summary>
        /// 对象转换-要求对象类型必须可序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertToType<T>(this object value)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.ConvertToType<T>(value);
        }

        /// <summary>
        /// 深度复制-要求对象类型必须可序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Copy<T>(this T value)
        {
            return value.JsStringify().JsParse<T>();
        }


    }
}
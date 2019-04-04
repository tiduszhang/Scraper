/*----------------------------------------------------------------

// Copyright (C) 2016 南京思创信息技术有限公司 版权所有。
//
// 文件名：.cs
// 文件功能描述：
//
/// @author zhangsx
/// @date 2016/12/05 11:18:19
//
//----------------------------------------------------------------*/
using STSdb4.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>
    /// 数据操作
    /// </summary>
    public static class NoSQLHelper
    {
        /// <summary>
        /// 数据引擎列表
        /// </summary>
        private static Dictionary<string, IStorageEngine> Engines = new Dictionary<string, IStorageEngine>();

        /// <summary>
        /// 数据引擎列表
        /// </summary>
        private static Dictionary<string, ITable<int, string>> Tables = new Dictionary<string, ITable<int, string>>();

        /// <summary>
        /// 数据库存储位置
        /// </summary>
        public static string DBPath = "";

        /// <summary>
        /// 释放引擎
        /// </summary>
        public static void DisposEngines()
        {
            lock (Engines)
            {
                if (Engines != null && Engines.Count > 0)
                {
                    foreach (string key in Engines.Keys)
                    {
                        Engines[key].Commit();
                        Engines[key].Heap.Close();
                        //Engines[key].Close();
                        //Engines[key].Dispose();
                    }
                    Engines.Clear();
                    GC.Collect();
                }
            }
            "释放数据库引擎".WriteToLog();
        }

        /// <summary>
        /// 获取存储引擎
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        private static IStorageEngine GetStorageEngine<T>(string MarkId = "")
        {
            IStorageEngine engine = null;
            string strKey = typeof(T).Name;
            var engineData = new KeyValuePair<string, IStorageEngine>();
            lock (Engines)
            {
                engineData = Engines.FirstOrDefault(o => o.Key == strKey + MarkId);
            }
            if (engineData.Value == null)
            {
                var dbFile = GetDB<T>(MarkId);
                try
                {
                    //engine = STSdb.FromStream(System.IO.File.Open(dbFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite));
                    engine = STSdb.FromFile(dbFile);
                    //engine = STSdb.FromMemory(); 
                    var table = engine.OpenXTable<int, string>(strKey);
                    table.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    ("数据库出错，异常信息：" + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
                    engine.Heap.Close();
                    System.Threading.Thread.Sleep(100);
                    System.IO.File.Move(dbFile, (dbFile + "_" + DateTime.Now.ToString("yyyyMMhhddmmss") + ".bak"));
                    System.Threading.Thread.Sleep(100);
                    engine = STSdb.FromFile(dbFile);
                }
                lock (Engines)
                {
                    Engines.Add(strKey + MarkId, engine);
                }
            }
            else
            {
                engine = engineData.Value;
            }
            return engine;
        }

        /// <summary>
        /// 获取表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="engine"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        private static ITable<int, string> GetTable<T>(this IStorageEngine engine, string MarkId) where T : EntityBase
        {
            ITable<int, string> table = null;
            string strKey = typeof(T).Name;
            var tableData = new KeyValuePair<string, ITable<int, string>>();
            lock (Tables)
            {
                tableData = Tables.FirstOrDefault(o => o.Key == strKey + MarkId);
            }
            if (tableData.Value == null)
            {
                table = engine.OpenXTable<int, string>(strKey);
                lock (Tables)
                {
                    Tables.Add(strKey + MarkId, table);
                }
            }
            else
            {
                table = tableData.Value;
            }
            //table = engine.OpenXTable<int, string>(strKey);
            return table;
        }

        /// <summary>
        /// 获取数据库存储路径
        /// </summary>
        /// <returns></returns>
        public static string GetDBPath()
        {
            string path = DBPath;
            if (String.IsNullOrWhiteSpace(path))
            {
                path = WorkPath.ApplicationWorkPath + @"\DB\";
            }
            DBPath = path;
            if (!System.IO.Directory.Exists(DBPath))
            {
                System.IO.Directory.CreateDirectory(DBPath);
            }
            return DBPath;//目前一个实体一个库分开存储
        }

        /// <summary>
        /// 获取数据库存储路径
        /// </summary>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static string GetDB<T>(string MarkId = "")
        {
            string path = GetDBPath();

            return path + typeof(T).Name + MarkId + ".db";//目前一个实体一个库分开存储
        }

        /// <summary>
        /// 加载所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static List<T> LoadALL<T>(string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            return table.Select(o => o.Value.JsParse<T>()).ToList();
        }

        /// <summary>
        /// 根据ID加载数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static T LoadById<T>(string Id, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var objexist = table.FirstOrDefault(o => o.Value.JsParse<T>().ID == Id);
            if (!String.IsNullOrWhiteSpace(objexist.Value))
            {
                return objexist.Value.JsParse<T>();
            }
            return null;
        }

        /// <summary>
        /// 获取数据表中第一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static T First<T>(Func<T, bool> func = null, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            KeyValuePair<int, string> data = new KeyValuePair<int, string>();
            if (func != null)
            {
                var predicate = new Func<KeyValuePair<int, string>, bool>(o => func(o.Value.JsParse<T>()));
                data = table.FirstOrDefault(predicate);
                if (!String.IsNullOrWhiteSpace(data.Value))
                {
                    return data.Value.JsParse<T>();
                }
            }
            else
            {
                data = table.FirstOrDefault();
                if (!String.IsNullOrWhiteSpace(data.Value))
                {
                    return data.Value.JsParse<T>();
                }
            }
            return default(T);
        }

        /// <summary>
        ///  获取数据表中后一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static T FindNext<T>(this T value, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            //System.Console.WriteLine("数据表中有：" + table.Count() + "条记录！");
            //var objexist = table.FirstOrDefault(o => o.Key == value.Sequence);
            //if (!String.IsNullOrWhiteSpace(objexist.Value))
            {
                var nextValue = table.FindAfter(value.Sequence);
                if (nextValue != null)
                {
                    return nextValue.Value.Value.JsParse<T>();
                }
            }
            return default(T);
        }

        /// <summary>
        ///  获取数据表中前一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static T FindPrev<T>(this T value, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            //var objexist = table.FirstOrDefault(o => o.Key == value.Sequence);
            //if (!String.IsNullOrWhiteSpace(objexist.Value))
            {
                var nextValue = table.FindBefore(value.Sequence);
                if (nextValue != null)
                {
                    return nextValue.Value.Value.JsParse<T>();
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取数据表中最后一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static T Last<T>(Func<T, bool> func = null, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            KeyValuePair<int, string> data = new KeyValuePair<int, string>();
            if (func != null)
            {
                var predicate = new Func<KeyValuePair<int, string>, bool>(o => func(o.Value.JsParse<T>()));
                data = table.LastOrDefault(predicate);
                if (!String.IsNullOrWhiteSpace(data.Value))
                {
                    return data.Value.JsParse<T>();
                }
            }
            else
            {
                data = table.LastOrDefault();
                if (!String.IsNullOrWhiteSpace(data.Value))
                {
                    return data.Value.JsParse<T>();
                }
            }
            return default(T);
        }

        /// <summary>
        /// 根据条件加载数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static List<T> LoadByCondition<T>(Func<T, bool> func, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            return table.Where(o => func(o.Value.JsParse<T>())).Select(o => o.Value.JsParse<T>()).ToList();
        }

        /// <summary>
        /// 根据条件分页加载数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="func"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static PageData<T> LoadByPage<T>(int PageIndex, int PageSize, Func<T, bool> func = null, string MarkId = "") where T : EntityBase
        {
            PageData<T> pagedata = new PageData<T>();
            pagedata.PageIndex = PageIndex;
            pagedata.PageSize = PageSize;

            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            if (func != null)
            {
                pagedata.TotalCount = (int)table.Where(o => func(o.Value.JsParse<T>())).Count();
            }
            else
            {
                pagedata.TotalCount = (int)table.Count();
            }
            if (pagedata.TotalCount > 0)
            {
                if (pagedata.PageSize <= 0)
                {
                    pagedata.PageSize = 1;
                }
                pagedata.TotalPageCount = (int)Math.Ceiling((double)(pagedata.TotalCount / pagedata.PageSize));
                if (pagedata.PageIndex > pagedata.TotalPageCount)
                {
                    pagedata.PageIndex = pagedata.TotalPageCount;
                }
                else if (pagedata.PageIndex < 0)
                {
                    pagedata.PageIndex = 1;
                }
                if (func != null)
                {
                    pagedata.QueryData = table.Where(o => func(o.Value.JsParse<T>())).Skip((pagedata.PageIndex - 1) * pagedata.PageSize).Take(pagedata.PageSize).Select(o => o.Value.JsParse<T>()).ToList();
                }
                else
                {
                    pagedata.QueryData = table.Skip((pagedata.PageIndex - 1) * pagedata.PageSize).Take(pagedata.PageSize).Select(o => o.Value.JsParse<T>()).ToList();
                }
            }
            else
            {
                pagedata.TotalCount = 0;
                pagedata.TotalPageCount = 0;
                pagedata.PageIndex = 0;
                pagedata.QueryData = null;
            }
            return pagedata;
        }

        /// <summary>
        /// 分页加载所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="PageIndex"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static T LoadOneByPage<T>(int PageIndex, Func<T, bool> func = null, string MarkId = "") where T : EntityBase
        {
            var pageData = LoadByPage<T>(PageIndex, 1, func, MarkId);
            if (pageData != null && pageData.QueryData != null && pageData.QueryData.Count > 0)
            {
                return pageData.QueryData[0];
            }
            return null;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="MarkId"></param>
        public static void Delete<T>(this T value, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            //var objexist = table.TryGetOrDefault(value.Sequence, "");
            //if (!String.IsNullOrWhiteSpace(objexist))//修改
            //{
            table.Delete(value.Sequence);
            //}
            engine.Commit();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="MarkId"></param>
        public static void Delete<T>(this List<T> values, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            //var objexist = table.TryGetOrDefault(value.Sequence, "");
            //if (!String.IsNullOrWhiteSpace(objexist))//修改
            //{
            //    table.Delete(objexist.JsParse<T>().Sequence);
            //}
            foreach (T value in values)
            {
                table.Delete(value.Sequence);
                System.Threading.Thread.Sleep(10);
            }
            engine.Commit();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="MarkId"></param>
        public static void Save<T>(this T value, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            int iMaxKey = 0;
            if (value.Sequence == 0)
            {
                if (table.Count() > 0)
                {
                    iMaxKey = table.Max(o => o.Key);
                }
                //新增
                iMaxKey++;
                value.Sequence = iMaxKey;
            }
            table[value.Sequence] = value.JsStringify();
            engine.Commit();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="MarkId"></param>
        public static void Save<T>(this List<T> values, string MarkId = "") where T : EntityBase
        {
            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            int iMaxKey = 0;
            var selecExist = values.Where(o => o.Sequence > 0);
            if (selecExist != null && selecExist.Count() > 0)
            {
                foreach (T value in selecExist)
                {
                    table[value.Sequence] = value.JsStringify();
                }
            }
            var selecNotExist = values.Where(o => o.Sequence == 0);
            if (selecNotExist != null && selecNotExist.Count() > 0)
            {
                if (table.Count() > 0)
                {
                    iMaxKey = table.Max(o => o.Key);
                }
                foreach (T value in selecNotExist)
                {
                    //新增
                    iMaxKey++;
                    value.Sequence = iMaxKey;
                    table[value.Sequence] = value.JsStringify();
                }
            }
            engine.Commit();
        }

        /// <summary>
        /// 获取数据所在数据库的索引，返回-1标识没找到。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static int GetIndex<T>(this T value, string MarkId = "") where T : EntityBase
        {
            int iIndex = -1;

            IStorageEngine engine = GetStorageEngine<T>(MarkId);
            //var table = engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            var table = engine.GetTable<T>(MarkId); //engine.OpenXTable<int, string>(typeof(T).Name + MarkId);
            //var data = table.TryGetOrDefault(value.Sequence, "");
            //if (!String.IsNullOrWhiteSpace(data))
            //{
            iIndex = table.Select((o, index) => o.Key == value.Sequence ? index : -1).FirstOrDefault(o => o != -1);
            //}

            return iIndex;
        }
    }
}
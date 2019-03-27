using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Common
{
    /// <summary>
    /// FileName: Function.cs
    /// CLRVersion: 4.0.30319.42000
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// Corporation:
    /// Description:    
    /// </summary> 
    public static class FunctionTools
    {
        /// <summary>
        /// 将二进制转换成字符串 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToString(this byte[] value)
        {
            return Encoding.Default.GetString(value);
            //return System.BitConverter.ToString(value);
            //return Encoding.UTF8.GetString(value);
        }

        /// <summary>
        /// 将二进制转换成字符串 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToUTF8String(this byte[] value)
        {
            return Encoding.UTF8.GetString(value);
            //return System.BitConverter.ToString(value);
            //return Encoding.UTF8.GetString(value);
        }

        /// <summary>
        /// 压缩 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] value)
        {
            MemoryStream outStream = new MemoryStream();
            using (MemoryStream intStream = new MemoryStream(value))
            {
                using (GZipStream Compress =
                    new GZipStream(outStream,
                    CompressionMode.Compress))
                {
                    intStream.CopyTo(Compress);
                }
            }
            return outStream.ToArray();
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Decompress(this byte[] value)
        {
            byte[] result = null;
            MemoryStream compressedStream = new MemoryStream(value);
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream Decompress = new GZipStream(compressedStream,
                        CompressionMode.Decompress))
                {
                    Decompress.CopyTo(outStream);
                    result = outStream.ToArray();
                }
            }
            return result;
        }

        /// <summary>
        /// 验证字符串是否为IP地址 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPAddress(this string ip)
        {
            if (string.IsNullOrEmpty(ip) || ip.Length < 7 || ip.Length > 15) return false;

            string regformat = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$";

            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);

            return regex.IsMatch(ip);
        }


        /// <summary>
        /// WeeksOfYear
        /// </summary>
        /// <param name="dTime"></param>
        /// <returns></returns>
        public static int WeeksOfYear(this DateTime dTime)
        {
            try
            {
                var dayOfYear = dTime.DayOfYear;
                var tempDate = new DateTime(dTime.Year, 1, 1);
                var tempDayOfWeek = (int)tempDate.DayOfWeek;
                tempDayOfWeek = tempDayOfWeek == 0 ? 7 : tempDayOfWeek;
                var index = (int)dTime.DayOfWeek;
                index = index == 0 ? 7 : index;
                DateTime retStartDay = dTime.AddDays(-(index - 1));
                DateTime retEndDay = dTime.AddDays(7 - index);
                var weekIndex = (int)Math.Ceiling(((double)dayOfYear + tempDayOfWeek - 1) / 7);
                if (retStartDay.Year < retEndDay.Year)
                {
                    weekIndex = 1;
                }

                return weekIndex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targe"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<List<T>> CreateList<T>(this List<T> targe, int size)
        {
            List<List<T>> listArr = new List<List<T>>();
            //获取被拆分的数组个数  
            int arrSize = targe.Count % size == 0 ? targe.Count / size : targe.Count / size + 1;
            for (int i = 0; i < arrSize; i++)
            {
                List<T> sub = new List<T>();
                //把指定索引数据放入到list中  
                for (int j = i * size; j <= size * (i + 1) - 1; j++)
                {
                    if (j <= targe.Count - 1)
                    {
                        sub.Add(targe[j]);
                    }
                }
                listArr.Add(sub);
            }
            return listArr;
        }
    }
}
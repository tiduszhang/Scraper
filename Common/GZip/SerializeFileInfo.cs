using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common
{

    /// <summary>
    /// 文件序列化信息
    /// </summary>
    [Serializable]
    public class SerializeFileInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        string fileName;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get
            {
                return fileName;
            }
            private set
            {
                fileName = value;
            }
        }

        /// <summary>
        /// 文件内容
        /// </summary>
        byte[] fileBuffer;
        /// <summary>
        /// 文件内容
        /// </summary>
        public byte[] FileBuffer
        {
            get
            {
                return fileBuffer;
            }
            private set
            {
                fileBuffer = value;
            }
        }

        /// <summary>
        /// 文件相对路径
        /// </summary>
        string path;
        /// <summary>
        /// 文件相对路径
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }
        /// <summary>
        /// 文件相对路径
        /// </summary>
        SerializeFileInfoType type;
        /// <summary>
        /// 文件相对路径
        /// </summary>
        public SerializeFileInfoType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static SerializeFileInfo LoadFile(string filepath, SerializeFileInfoType Type = SerializeFileInfoType.File)
        {
            var SerializeFileInfo = new SerializeFileInfo(); 
            SerializeFileInfo.Type = Type;
            if (Type == SerializeFileInfoType.Directory)
            {
                return SerializeFileInfo;
            }
            SerializeFileInfo.FileBuffer = File.ReadAllBytes(filepath);
            SerializeFileInfo.FileName = System.IO.Path.GetFileName(filepath);
            return SerializeFileInfo;
        }

        /// <summary>
        /// 将文件内容保存到指定目录
        /// </summary>
        /// <param name="dirPath"></param>
        public void SaveAs(string dirPath)
        {
            var filePath = System.IO.Path.GetFullPath(dirPath + @"\" + this.Path);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            if (Type == SerializeFileInfoType.Directory)
            {
                return;
            }
            string newName = System.IO.Path.GetFullPath(filePath + @"\" + this.FileName);
            using (FileStream fs = new FileStream(newName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(this.FileBuffer, 0, this.FileBuffer.Length);
                //fs.Close();
                //using (Stream stream = new MemoryStream(f.FileBuffer))
                //{
                //    byte[] bytes = new byte[4096];
                //    int n;
                //    while ((n = stream.Read(bytes, 0, bytes.Length)) != 0)
                //    {
                //        fs.Write(bytes, 0, n);
                //    }
                //}

            }
        }

        /// <summary>
        /// 压缩文件内容信息类型
        /// </summary>
        public enum SerializeFileInfoType
        {
            /// <summary>
            /// 文件类型
            /// </summary>
            File,
            /// <summary>
            /// 文件夹类型
            /// </summary>
            Directory
        }
    }


    /// <summary>
    /// 序列化文件信息扩展类
    /// </summary>
    public static class SerializeFileInfoExtensions
    {
        /// <summary>
        /// 转换成内存流
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Stream ToStream<T>(this List<T> value) where T : class
        {
            IFormatter formatter = new BinaryFormatter();
            Stream s = new MemoryStream();
            formatter.Serialize(s, value);
            s.Position = 0;
            return s;
        }


        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static List<T> ToObjectList<T>(this Stream s)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (List<T>)b.Deserialize(s);
        }

    }


}

using System;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// Zip压缩包处理-文件夹压缩解压缩处理
    /// 注意：本压缩规则采用非标准Zip规则
    /// </summary>
    public class GZipCompress
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GZipCompress()
        {

        }

        /// <summary>
        /// 压缩时为需要压缩的文件夹，解压缩时是解压到的文件夹，需要绝对路径
        /// </summary>
        public string DirPath { get; set; }

        /// <summary>
        /// 压缩式是输出文件名，解压缩时是输入文件名，需要绝对路径
        /// </summary>
        public string ZipFileName { get; set; }

        /// <summary>
        /// 执行消息
        /// </summary>
        public Action<string> Message { get; set; }

        /// <summary>
        /// 处理文件分块大小
        /// </summary>
        private int bufferSize = 1024 * 10;

        /// <summary>
        /// 对目标文件夹进行压缩，将压缩结果保存为指定文件
        /// </summary> 
        public void Compress()
        {
            if (!Directory.Exists(DirPath))
            {
                Message?.Invoke("目标文件夹不存在！");
                return;
            }

            var files = Directory.GetFiles(DirPath, "*", SearchOption.AllDirectories);
            //if (files == null || files.Length <= 0)
            //{
            //    Message?.Invoke("目标文件夹内没有文件！");
            //    return;
            //}

            //准备文件 
            Message?.Invoke("正在准备文件！"); 
            var list = new List<SerializeFileInfo>();
            foreach (string f in files)
            {
                SerializeFileInfo sfi = SerializeFileInfo.LoadFile(f);
                //Message?.Invoke("正在准备文件" + sfi.FileName + "！");
                sfi.Path = f.Substring(DirPath.Length, f.Length - DirPath.Length - sfi.FileName.Length);
                list.Add(sfi);
            }

            //解决文件夹问题-一般不会太多
            var dirs = Directory.GetDirectories(DirPath, "*", SearchOption.AllDirectories);
            if (dirs != null && dirs.Length > 0)
            {
                foreach (string d in dirs)
                {
                    if (Directory.GetFiles(d, "*", SearchOption.AllDirectories).Length == 0)//当文件夹内没用文件时启用
                    {
                        SerializeFileInfo sfi = SerializeFileInfo.LoadFile(d, SerializeFileInfo.SerializeFileInfoType.Directory);
                        sfi.Path = d.Substring(DirPath.Length, d.Length - DirPath.Length);
                        list.Add(sfi);
                    }
                }
            } 
            var source = list.ToStream();

            //开始压缩 
            Message?.Invoke("正在压缩文件！");
            using (Stream destination = new FileStream(ZipFileName, FileMode.Create, FileAccess.Write))
            {
                using (GZipStream output = new GZipStream(destination, CompressionMode.Compress))
                {
                    byte[] bytes = new byte[bufferSize];
                    int n;
                    while ((n = source.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        Message?.Invoke("正在压缩文件！已完成" + (source.Position * 100 / source.Length).ToString("0.##") + "%");
                        output.Write(bytes, 0, n);
                    }
                }
                destination.Close();
            }

            Message?.Invoke("压缩已完成！"); 

            GC.Collect();
        }


        /// <summary>
        /// 对目标压缩文件解压缩，将内容解压缩到指定文件夹
        /// </summary> 
        public void DeCompress()
        {
            Message?.Invoke("正在解压文件！");
            //开始解压
            List<SerializeFileInfo> sfi = null;
            using (Stream source = File.OpenRead(ZipFileName))
            {
                using (Stream destination = new MemoryStream())
                {
                    using (GZipStream input = new GZipStream(source, CompressionMode.Decompress, true))
                    {
                        byte[] bytes = new byte[bufferSize];
                        int n;
                        while ((n = input.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            Message?.Invoke("正在解压文件！已完成" + (source.Position * 100 / source.Length).ToString("0.##") + "%" );
                            destination.Write(bytes, 0, n);
                        }
                    }
                    destination.Flush();
                    destination.Position = 0;
                    sfi = destination.ToObjectList<SerializeFileInfo>();
                }

                source.Close(); 
            }

            Message?.Invoke("正在保存文件！");
            //保存文件
            if (sfi != null)
            {
                foreach (var sf in sfi)
                {
                    //Message?.Invoke("正在保存文件" + sf.FileName + "！");
                    sf.SaveAs(DirPath);
                }
            }

            Message?.Invoke("解压已完成！"); 
            GC.Collect();
        }



    }

}
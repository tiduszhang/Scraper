using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 图像处理工具扩展
    /// </summary>
    public static class ImageTools
    {
        /// <summary>
        /// 将二进制流转换成图片 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapImage ByteArrayToBitmapImage(this byte[] byteArray)
        {
            System.Windows.Media.Imaging.BitmapImage bmp = null;

            try
            {
                bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new System.IO.MemoryStream(byteArray);
                bmp.EndInit();
                bmp.Freeze();
            }
            catch
            {
                bmp = null;
            }

            return bmp;
        }

        /// <returns></returns>
        /// <summary>
        /// 从Bitmap转换成BitmapSource 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapSource ChangeBitmapToBitmapSource(this System.Drawing.Bitmap bmp)
        {
            System.Windows.Media.Imaging.BitmapSource returnSource;
            try
            {
                IntPtr hBitmap = bmp.GetHbitmap();
                returnSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                if (!Win32Helper.DeleteObject(hBitmap))//记得要进行内存释放。否则会有内存不足的报错。
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;
        }

        /// <summary>
        /// 从bitmap转换成ImageSource 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static System.Windows.Media.ImageSource ChangeBitmapToImageSource(this System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            System.Windows.Media.ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            if (!Win32Helper.DeleteObject(hBitmap))//记得要进行内存释放。否则会有内存不足的报错。
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static System.Windows.Media.ImageSource ChangeIconToImageSource(this System.Drawing.Icon icon)
        {
            IntPtr hBitmap = icon.Handle;
            System.Windows.Media.ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
            hBitmap,
            System.Windows.Int32Rect.Empty,
            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            if (!Win32Helper.DeleteObject(hBitmap))//记得要进行内存释放。否则会有内存不足的报错。
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return imageSource;
        }

        /// <summary>
        /// 将图片转换成二进制 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this System.Drawing.Image image)
        {
            byte[] bValue = null;
            try
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bValue = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                ("图片内存转换成二进制出现异常，异常信息：" + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
                bValue = null;
            }

            return bValue;
        }


        /// <summary>
        /// 将图片转换成二进制 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static System.Drawing.Image ToImage(this byte[] value)
        {
            System.Drawing.Image image = null;
            try
            {
                System.IO.Stream ms = new System.IO.MemoryStream(value);
                image = System.Drawing.Image.FromStream(ms, false, false);
            }
            catch (Exception ex)
            {
                ("二进制转换成图片出现异常，异常信息：" + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
                image = null;
            }

            return image;
        }

        /// <summary>
        /// 创建水印文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="horizontalResolution"></param>
        /// <param name="verticalResolution"></param>
        public static void CreatWotermarkFile(this string fileName, string text, int width, int height, float horizontalResolution, float verticalResolution)
        {
            if (System.IO.File.Exists(fileName))
            {
                return;
            }

            System.Drawing.Bitmap img = new System.Drawing.Bitmap(width, height);
            img.SetResolution(horizontalResolution, verticalResolution);
            img.MakeTransparent();
            using (var gImage = System.Drawing.Graphics.FromImage(img))
            {
                //生成字体
                //var font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.Name, fpSize, System.Drawing.FontStyle.Bold);
                //var font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.Name, Convert.ToInt32((img.Width / 50)), System.Drawing.FontStyle.Bold);
                var font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.Name, width / 50, System.Drawing.FontStyle.Bold);
                //测量文字长度
                var size = gImage.MeasureString(text, font);

                ("水印大小计算结果：size.Width->" + size.Width + " , size.Height ->" + size.Height).WriteToLog();
                //生成一张图片
                using (System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)size.Width + 10, (int)size.Height + 10))
                {
                    using (System.Drawing.Graphics gText = System.Drawing.Graphics.FromImage(image))
                    {
                        //绘制背景
                        gText.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(120, System.Drawing.Color.Black)), 0, 0, image.Width, image.Height);
                        //绘制文字
                        gText.DrawString(text, font, System.Drawing.Brushes.Yellow, 5, 5);

                        ////绘制背景
                        //g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(120, System.Drawing.Color.Black)), (img.Width - size.Width - 50 - 5), ((img.Height / 50) - 5), size.Width + 10, size.Height + 10);
                        ////绘制文字
                        //g.DrawString(Text, font, System.Drawing.Brushes.Yellow, (img.Width - size.Width - 50), (img.Height / 50));
                    }

                    gImage.DrawImage(image, (img.Width - image.Width - 20), (img.Height / 50));
                }
            }
            img.Save(fileName);
            img.Dispose();
            GC.Collect();
        }

        /// <summary>
        /// 向图片增加水印
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="watermarkFileName"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static void DrawWatermark(this string fileName, string watermarkFileName, string text = "")
        {
            try
            {
                var bFile = System.IO.File.ReadAllBytes(fileName);

                if (bFile == null || bFile.Length <= 0)
                {
                    ("不能为空文件[" + fileName + "]增加水印。").WriteToLog("", log4net.Core.Level.Error);
                    return;
                }
                var img = bFile.ToImage();

                if (!String.IsNullOrWhiteSpace(text))
                {
                    watermarkFileName.CreatWotermarkFile(text, img.Width, img.Height, img.HorizontalResolution, img.VerticalResolution);
                }
                if (System.IO.File.Exists(watermarkFileName))
                {
                    var bWatermarkFile = System.IO.File.ReadAllBytes(watermarkFileName);
                    var imgWatermark = bWatermarkFile.ToImage();

                    //todo:合并图片
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
                    {
                        g.DrawImage(imgWatermark, 0, 0);
                        //g.DrawImage(imgWatermark, new System.Drawing.Rectangle(0, 0, imgWatermark.Width, imgWatermark.Height), new System.Drawing.Rectangle(0, 0, imgWatermark.Width, imgWatermark.Height), System.Drawing.GraphicsUnit.Pixel);
                    }

                    img.Save(fileName);

                    imgWatermark.Dispose();
                    img.Dispose();
                }

            }
            catch (Exception ex)
            {
                ("[" + fileName + "]增加水印出现异常，异常信息：" + ex.ToString()).WriteToLog("", log4net.Core.Level.Error);
            }

            GC.Collect();
        }

        /// <summary>
        /// 按比例缩放图片
        /// </summary>
        /// <param name="SourceImage"></param>
        /// <param name="TargetWidth"></param>
        /// <param name="TargetHeight"></param>
        /// <returns></returns>
        public static System.Drawing.Image ZoomPicture(this System.Drawing.Image SourceImage, int TargetWidth, int TargetHeight)
        {
            int IntWidth; //新的图片宽
            int IntHeight; //新的图片高
            try
            {
                System.Drawing.Imaging.ImageFormat format = SourceImage.RawFormat;
                System.Drawing.Bitmap SaveImage = new System.Drawing.Bitmap(TargetWidth, TargetHeight);
                SaveImage.MakeTransparent();
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(SaveImage);
                //g.Clear(System.Drawing.Color.White);

                //计算缩放图片的大小 http://www.cnblogs.com/roucheng/

                if (SourceImage.Width > TargetWidth && SourceImage.Height <= TargetHeight)//宽度比目的图片宽度大，长度比目的图片长度小
                {
                    IntWidth = TargetWidth;
                    IntHeight = (IntWidth * SourceImage.Height) / SourceImage.Width;
                }
                else if (SourceImage.Width <= TargetWidth && SourceImage.Height > TargetHeight)//宽度比目的图片宽度小，长度比目的图片长度大
                {
                    IntHeight = TargetHeight;
                    IntWidth = (IntHeight * SourceImage.Width) / SourceImage.Height;
                }
                else if (SourceImage.Width <= TargetWidth && SourceImage.Height <= TargetHeight) //长宽比目的图片长宽都小
                {
                    IntHeight = SourceImage.Width;
                    IntWidth = SourceImage.Height;
                }
                else//长宽比目的图片的长宽都大
                {
                    IntWidth = TargetWidth;
                    IntHeight = (IntWidth * SourceImage.Height) / SourceImage.Width;
                    if (IntHeight > TargetHeight)//重新计算
                    {
                        IntHeight = TargetHeight;
                        IntWidth = (IntHeight * SourceImage.Width) / SourceImage.Height;
                    }
                }

                g.DrawImage(SourceImage, (TargetWidth - IntWidth) / 2, (TargetHeight - IntHeight) / 2, IntWidth, IntHeight);
                SourceImage.Dispose();

                return SaveImage;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return null;
        }

        /// <summary>
        /// 制作图片的缩略图
        /// </summary>
        /// <param name="originalImage">原图</param>
        /// <param name="width">缩略图的宽（像素）</param>
        /// <param name="height">缩略图的高（像素）</param>
        /// <param name="mode">缩略方式</param>
        /// <returns>缩略图</returns>
        /// <remarks>
        ///        <paramref name="mode"/>：
        ///            <para>HW：指定的高宽缩放（可能变形）</para>
        ///            <para>HWO：指定高宽缩放（可能变形）（过小则不变）</para>
        ///            <para>W：指定宽，高按比例</para>
        ///            <para>WO：指定宽（过小则不变），高按比例</para>
        ///            <para>H：指定高，宽按比例</para>
        ///            <para>HO：指定高（过小则不变），宽按比例</para>
        ///            <para>CUT：指定高宽裁减（不变形）</para>
        /// </remarks>
        public static System.Drawing.Image MakeThumbnail(this System.Drawing.Image originalImage, int width, int height, ThumbnailMode mode)
        {
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;


            switch (mode)
            {
                case ThumbnailMode.UsrHeightWidth: //指定高宽缩放（可能变形）
                    break;
                case ThumbnailMode.UsrHeightWidthBound: //指定高宽缩放（可能变形）（过小则不变）
                    if (originalImage.Width <= width && originalImage.Height <= height)
                    {
                        return originalImage;
                    }
                    if (originalImage.Width < width)
                    {
                        towidth = originalImage.Width;
                    }
                    if (originalImage.Height < height)
                    {
                        toheight = originalImage.Height;
                    }
                    break;
                case ThumbnailMode.UsrWidth: //指定宽，高按比例
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailMode.UsrWidthBound: //指定宽（过小则不变），高按比例
                    if (originalImage.Width <= width)
                    {
                        return originalImage;
                    }
                    else
                    {
                        toheight = originalImage.Height * width / originalImage.Width;
                    }
                    break;
                case ThumbnailMode.UsrHeight: //指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailMode.UsrHeightBound: //指定高（过小则不变），宽按比例
                    if (originalImage.Height <= height)
                    {
                        return originalImage;
                    }
                    else
                    {
                        towidth = originalImage.Width * height / originalImage.Height;
                    }
                    break;
                case ThumbnailMode.Cut: //指定高宽裁减（不变形）
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            return originalImage.GetThumbnailImage(towidth, toheight, null, IntPtr.Zero);
        }

        /// <summary>
        /// 缩略图制作模式
        /// </summary>
        public enum ThumbnailMode
        {
            /// <summary>
            /// 指定的高宽缩放（可能变形）
            /// </summary>
            UsrHeightWidth,
            /// <summary>
            /// 指定高宽缩放（可能变形）（过小则不变）
            /// </summary>
            UsrHeightWidthBound,
            /// <summary>
            /// 指定宽，高按比例
            /// </summary>
            UsrWidth,
            /// <summary>
            /// 指定宽（过小则不变），高按比例
            /// </summary>
            UsrWidthBound,
            /// <summary>
            /// 指定高，宽按比例
            /// </summary>
            UsrHeight,
            /// <summary>
            /// 指定高（过小则不变），宽按比例
            /// </summary>
            UsrHeightBound,
            /// <summary>
            /// 指定高宽裁减（不变形）
            /// </summary>
            Cut,
            /// <summary>
            /// 未知
            /// </summary>
            NONE,
        }
    }
}

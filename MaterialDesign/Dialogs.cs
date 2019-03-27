using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaterialDesign
{
    /// <summary>
    /// MetroDialog样式
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public static class MetroDialogSettings
    {
        /// <summary>
        /// 样式
        /// </summary>
        private static ResourceDictionary DialogDictionary = new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml") };

        /// <summary>
        /// 默认样式
        /// </summary>
        public static MahApps.Metro.Controls.Dialogs.MetroDialogSettings MetroDialogDefault
        {
            get
            {
                return new MahApps.Metro.Controls.Dialogs.MetroDialogSettings
                {
                    CustomResourceDictionary = DialogDictionary,
                    AffirmativeButtonText = "确定",
                    //SuppressDefaultResources = true
                };
            }
        }

        /// <summary>
        /// 确定取消样式
        /// </summary>
        public static MahApps.Metro.Controls.Dialogs.MetroDialogSettings MetroDialogOKCancel
        {
            get
            {
                return new MahApps.Metro.Controls.Dialogs.MetroDialogSettings
                {
                    CustomResourceDictionary = DialogDictionary,
                    AffirmativeButtonText = "确定",
                    NegativeButtonText = "取消",
                    //SuppressDefaultResources = true
                };
            }
        }

        /// <summary>
        /// 是否样式
        /// </summary>
        public static MahApps.Metro.Controls.Dialogs.MetroDialogSettings MetroDialogYesNo
        {
            get
            {
                return new MahApps.Metro.Controls.Dialogs.MetroDialogSettings
                {
                    CustomResourceDictionary = DialogDictionary,
                    AffirmativeButtonText = "是",
                    NegativeButtonText = "否",
                    //SuppressDefaultResources = true
                };
            }
        }
    }

}

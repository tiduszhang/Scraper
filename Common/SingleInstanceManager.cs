using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// FileName: SingleInstanceManager.cs
    /// CLRVersion: 4.0.30319.42000
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// Corporation:
    /// Description: 
    /// Using VB bits to detect single instances and process accordingly:
    /// * OnStartup is fired when the first instance loads
    /// * OnStartupNextInstance is fired when the application is re-run again
    /// NOTE: it is redirected to this instance thanks to IsSingleInstance
    /// </summary> 
    public class SingleInstanceManager : WindowsFormsApplicationBase
    {
        /// <summary>
        /// 运行程序
        /// </summary>
        /// <typeparam name="T">WPF 程序指定启动程序类，Winform 应用程序指定主窗口</typeparam>
        /// <param name="args">启动参数</param>
        /// <param name="IsSingleInstance">启动参数</param>
        public static void Launcher<T>(string[] args, bool IsSingleInstance = true)
        {
            //AppDomain appDomain = WorkPath.CreateDomain("AppDomain" + System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

            //System.Reflection.Assembly assembly = appDomain.Load(typeof(SingleInstanceManager).Assembly.GetName());
            //AppDomain.Unload(appDomain);
            //Type typeManager = assembly.GetType(typeof(SingleInstanceManager).FullName);
            SingleInstanceManager singleInstanceManager = new SingleInstanceManager(); //typeManager.GetConstructor(new Type[] { }).Invoke(null) as SingleInstanceManager;
            Type typeSingleInstanceManager = singleInstanceManager.GetType();
            singleInstanceManager.IsSingleInstance = !IsSingleInstance;

            if (typeof(T).BaseType == typeof(System.Windows.Application))//WPF程序
            {
                System.Reflection.PropertyInfo propertyInfo = typeSingleInstanceManager.GetProperty("WPFApplication");

                Type typeApp = typeof(T);
                System.Windows.Application application = typeApp.GetConstructor(new Type[] { }).Invoke(null) as System.Windows.Application;
                propertyInfo.SetValue(singleInstanceManager, application, null);
                //singleInstanceManager.WPFApplication = application;
            }
            else if (typeof(T).BaseType == typeof(System.Windows.Forms.Form))//Winform程序
            {
                System.Reflection.PropertyInfo propertyInfo = typeSingleInstanceManager.GetProperty("MainWindow");

                Type typeApp = typeof(T);
                System.Windows.Forms.Form mainwindow = typeApp.GetConstructor(new Type[] { }).Invoke(null) as System.Windows.Forms.Form;
                propertyInfo.SetValue(singleInstanceManager, mainwindow, null);
                //singleInstanceManager.MainWindow = mainwindow;
            }

            //System.Reflection.MethodInfo methodInfo = typeSingleInstanceManager.GetMethod("Run");
            try
            {
                singleInstanceManager.Run(args);
                //methodInfo.Invoke(singleInstanceManager, new object[] { args });
            }
            catch (Exception ex)
            {
                ex.ToString().WriteToLog("", log4net.Core.Level.Info);
            }
        }


        /// <summary>
        /// 应用程序集
        /// </summary>
        public System.Windows.Application WPFApplication { get; set; }

        /// <summary>
        /// Winform主窗口
        /// </summary>
        public System.Windows.Forms.Form MainWindow { get; set; }

        /// <summary>
        /// 构造函数初始化为单实例应用程序
        /// </summary>
        public SingleInstanceManager()
        {
            this.IsSingleInstance = true;
        }

        /// <summary>
        /// 程序启动
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            try
            {
                if (WPFApplication != null)
                {
                    WPFApplication.Run();
                }
                else if (MainWindow != null)
                {
                    System.Windows.Forms.Application.Run(MainWindow);
                }
            }
            catch (Exception ex)
            {
                ex.ToString().WriteToLog("", log4net.Core.Level.Error);
                System.Windows.MessageBox.Show("系统异常，请联系管理员！", "提示信息", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error, System.Windows.MessageBoxResult.OK);
                System.Environment.Exit(System.Environment.ExitCode);
            }
            return false;
        }

        /// <summary>
        /// 程序已经被启动显示当前主窗口
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            // Subsequent launches
            base.OnStartupNextInstance(eventArgs);

            if (WPFApplication != null)
            {
                System.Windows.WindowState windowState = WPFApplication.MainWindow.WindowState;
                WPFApplication.MainWindow.WindowState = System.Windows.WindowState.Minimized;
                WPFApplication.MainWindow.Show();
                WPFApplication.MainWindow.Activate();
                WPFApplication.MainWindow.Focus();
                WPFApplication.MainWindow.WindowState = windowState;
            }
            else if (this.ApplicationContext.MainForm != null)
            {
                System.Windows.Forms.FormWindowState formWindowState = this.ApplicationContext.MainForm.WindowState;
                this.ApplicationContext.MainForm.WindowState = System.Windows.Forms.FormWindowState.Minimized;
                this.ApplicationContext.MainForm.Show();
                this.ApplicationContext.MainForm.Activate();
                this.ApplicationContext.MainForm.Focus();
                this.ApplicationContext.MainForm.WindowState = formWindowState;
            }
        }
    }

}

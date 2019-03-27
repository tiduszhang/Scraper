using Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MVVM.ViewModel
{
    /// <summary>
    /// 最大化
    /// </summary>
    public class WindowMaximizedCommand : ICommand
    {
        private Window w = null;

        /// <summary>
        /// 判断是否可以执行
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        public bool CanExecute(object parameter)
        {
            if (parameter is Window)
            {
                if (w == null)
                {
                    w = (parameter as Window);
                }
                if (w.WindowState == WindowState.Normal)
                {
                    Button btnMax = w.FindChild<Button>(o =>
                      {
                          return o.Name == "btnMax";
                      });
                    btnMax.Visibility = Visibility.Visible;
                    Button btnNormal = w.FindChild<Button>(o =>
                    {
                        return o.Name == "btnNormal";
                    });
                    btnNormal.Visibility = Visibility.Collapsed;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 事件
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="parameter"> </param>
        public void Execute(object parameter)
        {
            w.WindowState = WindowState.Maximized;
        }
    }
}
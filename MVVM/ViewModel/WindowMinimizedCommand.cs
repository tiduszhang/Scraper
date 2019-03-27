using System;
using System.Windows;
using System.Windows.Input;

namespace MVVM.ViewModel
{
    /// <summary>
    /// 最小化
    /// </summary>
    public class WindowMinimizedCommand : ICommand
    {
        /// <summary>
        /// 判断是否可执行
        /// </summary>
        /// <param name="parameter"> </param>
        /// <returns> </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="parameter"> </param>
        public void Execute(object parameter)
        {
            if (parameter is Window)
            {
                Window w = (parameter as Window);
                w.WindowState = WindowState.Minimized;
            }
        }
    }
}
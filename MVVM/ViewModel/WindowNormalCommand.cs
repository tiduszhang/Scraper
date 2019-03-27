using Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MVVM.ViewModel
{
    /// <summary>
    /// 正常
    /// </summary>
    public class WindowNormalCommand : ICommand
    {
        private Window w = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (parameter is Window)
            {
                if (w == null)
                {
                    w = (parameter as Window);
                }
                if (w.WindowState == WindowState.Maximized)
                {
                    Button btnMax = w.FindChild<Button>(o =>
                    {
                        return o.Name == "btnMax";
                    });
                    btnMax.Visibility = Visibility.Collapsed;
                    Button btnNormal = w.FindChild<Button>(o =>
                    {
                        return o.Name == "btnNormal";
                    });
                    btnNormal.Visibility = Visibility.Visible;
                    return true;
                }
            }
            return false;
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
        ///
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            w.WindowState = WindowState.Normal;
        }
    }
}
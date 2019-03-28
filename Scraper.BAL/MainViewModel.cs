using MVVM.Model;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Scraper.BAL
{
    /// <summary>
    /// 
    /// </summary>
    public class MainViewModel : NotifyBaseModel
    {
        public ProxyViewModel ProxyViewModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MainViewModel()
        {
            ProxyViewModel = new ProxyViewModel();
        }

    }
}

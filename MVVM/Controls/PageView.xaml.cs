using MVVM.Model;
using MVVM.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;

namespace MVVM.Controls
{
    /// <summary>
    /// PageView.xaml 的交互逻辑
    /// </summary>
    public partial class PageView : UserControl
    {
        /// <summary>
        /// 分页控件
        /// </summary>
        public PageView()
        {
            InitializeComponent();
            this.Loaded += PageViewViewModel_Loaded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageViewViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsInDesignMode())
            {
                return;
            }
            if (PageViewModel == null)
            {
                PageViewModel = new PageModel();
            }
        }

        /// <summary>
        /// 绑定声明
        /// </summary>
        public static readonly DependencyProperty PageModelDependencyProperty = DependencyProperty.Register("PageViewModel", typeof(PageModel), typeof(PageView));

        /// <summary>
        /// 分页实体
        /// </summary>
        public PageModel PageViewModel
        {
            get
            {
                return this.GetValue(PageModelDependencyProperty) as PageModel;
            }
            set
            {
                this.SetValue(PageModelDependencyProperty, value);
            }
        }

        /// <summary>
        /// 分页事件
        /// </summary>
        public ICommand ChangePage
        {
            get
            { 
                return new DelegateCommand<Button>(button =>
                {
                    if (this.PageViewModel == null)
                    {
                        return;
                    }
                    switch (button.Name)
                    {
                        case "btnFirst":
                            this.PageViewModel.PageIndex = 1;
                            break;

                        case "btnPrev":
                            this.PageViewModel.PageIndex--;
                            break;

                        case "btnNext":
                            this.PageViewModel.PageIndex++;
                            break;

                        case "btnLast":
                            this.PageViewModel.PageIndex = this.PageViewModel.TotalPageCount;
                            break;

                        case "btnGo":
                            this.PageViewModel.PageIndex = this.PageViewModel.PageIndex;
                            break;

                        default:
                            break;
                    }
                    //绑定方法
                    this.PageViewModel.ChangePage();
                });
            }
        }
    }
}
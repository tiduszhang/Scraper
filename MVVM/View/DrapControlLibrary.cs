using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System;
using System.Linq;
using MVVM.ViewModel;
using Common;

namespace MVVM.View
{
    /// <summary>
    /// 将状态信息和零个或更多个 ICommand 封装到一个可附加的对象中。
    /// </summary>
    public class DragInCanvasBehavior : Behavior<UIElement>
    {
        /// <summary>
        /// 画板
        /// </summary>
        private Canvas canvas;
         
        /// <summary>
        /// 控件所在窗口
        /// </summary>
        private Window window;
         
        /// <summary>
        /// 在行为附加到 AssociatedObject 后调用。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
             
            if (window == null)
            {
                if (this.AssociatedObject != null)
                {
                    window = WPFUITools.GetOwnerWindow<Window>(this.AssociatedObject);
                    if (window != null)
                    {
                        window.SizeChanged += window_SizeChanged;
                    }
                }
            }

            // Hook up event handlers.
            this.AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            this.AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
        }

        /// <summary>
        /// 在行为与其 AssociatedObject 分离时（但在它实际发生之前）调用。
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            // Detach event handlers.
            this.AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            this.AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
        }

        // Keep track of when the element is being dragged.
        private bool isDragging = false;

        // When the element is clicked, record the exact position
        // where the click is made.
        private Point mouseOffset;

        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Find the canvas.
            if (canvas == null) canvas = VisualTreeHelper.GetParent(this.AssociatedObject) as Canvas;

            // Dragging mode begins.
            isDragging = true;

            // Get the position of the click relative to the element
            // (so the top-left corner of the element is (0,0).
            mouseOffset = e.GetPosition(AssociatedObject);

            // Capture the mouse. This way you'll keep receiveing
            // the MouseMove event even if the user jerks the mouse
            // off the element.
            AssociatedObject.CaptureMouse();
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Get the position of the element relative to the Canvas.
                //获取坐标偏移量
                Point point = e.GetPosition(canvas);

                //设置左边
                double left = point.X - mouseOffset.X;

                //设置顶部
                double top = point.Y - mouseOffset.Y;

                //设置位置
                SetLocation(left, top);
            }
        }

        /// <summary>
        /// 窗口改变大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //设置左边
            double left = Convert.ToDouble(AssociatedObject.GetValue(Canvas.LeftProperty));

            //设置顶部
            double top = Convert.ToDouble(AssociatedObject.GetValue(Canvas.TopProperty));

            //设置位置
            SetLocation(left, top);
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        private void SetLocation(double left, double top)
        {
            if (canvas == null)
            {
                return;
            }

            //设置左边
            if (left <= 0)
            {
                left = 0;
            }

            //设置顶边
            if (top <= 0)
            {
                top = 0;
            }

            //设置右边
            double width = Convert.ToDouble(AssociatedObject.GetValue(Canvas.ActualWidthProperty));
            double canWidth = canvas.ActualWidth;
            double right = left + width;
            if (right >= canWidth)
            {
                left = canWidth - width;
            }

            //设置底边
            double height = Convert.ToDouble(AssociatedObject.GetValue(Canvas.ActualHeightProperty));
            double canHeight = canvas.ActualHeight;
            double bottom = top + height;
            if (bottom >= canHeight)
            {
                top = canHeight - height;
            }

            // Move the element.
            //设置位置
            AssociatedObject.SetValue(Canvas.TopProperty, top);
            AssociatedObject.SetValue(Canvas.LeftProperty, left);
        }


        /// <summary>
        /// 鼠标左键释放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                AssociatedObject.ReleaseMouseCapture();
                isDragging = false;
            }
        }
    }
}
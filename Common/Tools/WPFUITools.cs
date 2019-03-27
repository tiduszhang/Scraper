using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Common
{
    /// <summary>
    /// UI工具类
    /// </summary>
    public static class WPFUITools
    {
        /// <summary>
        /// 是否为设计模式 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsInDesignMode(this FrameworkElement obj)
        {
            return (bool)System.ComponentModel.DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        }

        /// <summary>
        /// 验证内容
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <returns></returns>
        public static bool Validation(this FrameworkElement frameworkElement)
        {
            frameworkElement.GetBindingExpression(FrameworkElement.DataContextProperty).UpdateSource();
            return frameworkElement.GetBindingExpression(FrameworkElement.DataContextProperty).ValidateWithoutUpdate();
        }

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="VisualObject">A direct or indirect child of the queried item.</param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, a null reference is being returned.</returns>
        public static T FindVisualParent<T>(this DependencyObject VisualObject) where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(VisualObject);

            // we’ve reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            // check if the parent matches the type we’re looking for 
            if (parentObject is T)
            {
                return parentObject as T;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        /// <summary>
        /// 查找打开的窗口
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetOpenWindow<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().FirstOrDefault()
               : Application.Current.Windows.OfType<T>().FirstOrDefault(w => w.Name == name);
        }

        /// <summary>
        /// 查找打开且激活的窗口
        /// </summary>
        /// <returns></returns>
        public static T GetOpenWindow<T>() where T : Window
        {
            return Application.Current.Windows.OfType<T>().FirstOrDefault(w => w.IsActive);
        }

        /// <summary>
        /// 查找控件所在窗口
        /// </summary>
        /// <returns></returns>
        public static T GetOwnerWindow<T>(this DependencyObject dependencyObject) where T : Window
        {
            return Window.GetWindow(dependencyObject) as T;
        }
         
        /// <summary>
        /// 获得子控件
        /// </summary>
        /// <typeparam name="T">要获得控件类名</typeparam>
        /// <param name="obj">当前控件名</param>
        /// <param name="name">要查询子控件名</param>
        /// <returns>要获得控件类名</returns>
        public static T GetChildObject<T>(this DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;


            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);


                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }


        /// <summary>
        /// Find the first child of the specified type (the child must exist) by walking down the
        /// logical/visual trees Will throw an exception if a matching child does not exist. If
        /// you're not sure, use the TryFindChild method instead.
        /// </summary>
        /// <typeparam name="T"> The type of child you want to find </typeparam>
        /// <param name="parent"> The dependency object whose children you wish to scan </param>
        /// <returns> The first descendant of the specified type </returns>
        /// <remarks> usage: myWindow.FindChild<StackPanel>() </StackPanel> </remarks>
        public static T FindChild<T>(this DependencyObject parent)
            where T : DependencyObject
        {
            return parent.FindChild<T>(child => true);
        }

        /// <summary>
        /// Find the first child of the specified type (the child must exist) by walking down the
        /// logical/visual trees, which meets the specified criteria Will throw an exception if a
        /// matching child does not exist. If you're not sure, use the TryFindChild method instead.
        /// </summary>
        /// <typeparam name="T"> The type of child you want to find </typeparam>
        /// <param name="parent"> The dependency object whose children you wish to scan </param>
        /// <param name="predicate">
        /// The child object is selected if the predicate evaluates to true
        /// </param>
        /// <returns> The first matching descendant of the specified type </returns>
        /// <remarks>
        /// usage: myWindow.FindChild<StackPanel>( child =&gt; child.Name == "myPanel" ) </StackPanel>
        /// </remarks>
        public static T FindChild<T>(this DependencyObject parent, Func<T, bool> predicate)
            where T : DependencyObject
        {
            return parent.FindChildren<T>(predicate).First();
        }

        /// <summary>
        /// Use this overload if the child you're looking may not exist.
        /// </summary>
        /// <typeparam name="T"> The type of child you're looking for </typeparam>
        /// <param name="parent"> The dependency object whose children you wish to scan </param>
        /// <param name="foundChild">
        /// out param - the found child dependencyobject, null if the method returns false
        /// </param>
        /// <returns> True if a child was found, else false </returns>
        public static bool TryFindChild<T>(this DependencyObject parent, out T foundChild)
            where T : DependencyObject
        {
            return parent.TryFindChild<T>(child => true, out foundChild);
        }

        /// <summary>
        /// Use this overload if the child you're looking may not exist.
        /// </summary>
        /// <typeparam name="T"> The type of child you're looking for </typeparam>
        /// <param name="parent"> The dependency object whose children you wish to scan </param>
        /// <param name="predicate">
        /// The child object is selected if the predicate evaluates to true
        /// </param>
        /// <param name="foundChild">
        /// out param - the found child dependencyobject, null if the method returns false
        /// </param>
        /// <returns> True if a child was found, else false </returns>
        public static bool TryFindChild<T>(this DependencyObject parent, Func<T, bool> predicate, out T foundChild)
            where T : DependencyObject
        {
            foundChild = null;
            var results = parent.FindChildren<T>(predicate);
            if (results.Count() == 0)
                return false;
            foundChild = results.First();
            return true;
        }

        /// <summary>
        /// Get a list of descendant dependencyobjects of the specified type and which meet the
        /// criteria as specified by the predicate
        /// </summary>
        /// <typeparam name="T"> The type of child you want to find </typeparam>
        /// <param name="parent"> The dependency object whose children you wish to scan </param>
        /// <param name="predicate">
        /// The child object is selected if the predicate evaluates to true
        /// </param>
        /// <returns> The first matching descendant of the specified type </returns>
        /// <remarks>
        /// usage: myWindow.FindChildren<StackPanel>( child =&gt; child.Name == "myPanel" ) </StackPanel>
        /// </remarks>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject parent, Func<T, bool> predicate)
            where T : DependencyObject
        {
            var children = new List<DependencyObject>();
            if ((parent is Visual) || (parent is Visual3D))
            {
                var visualChildrenCount = VisualTreeHelper.GetChildrenCount(parent);
                for (int childIndex = 0; childIndex < visualChildrenCount; childIndex++)
                    children.Add(VisualTreeHelper.GetChild(parent, childIndex));
            }
            foreach (var logicalChild in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
                if (!children.Contains(logicalChild))
                    children.Add(logicalChild);
            foreach (var child in children)
            {
                var typedChild = child as T;
                if ((typedChild != null) && predicate.Invoke(typedChild))
                    yield return typedChild;
                foreach (var foundDescendant in FindChildren(child, predicate))
                    yield return foundDescendant;
            }
            yield break;
        }

        /// <summary>
        /// 执行按钮Click事件
        /// </summary>
        /// <param name="sender"></param>
        public static void PerformClick(this System.Windows.Controls.Primitives.ButtonBase sender)
        {
            //System.Windows.Automation.Peers.ButtonAutomationPeer bam = new System.Windows.Automation.Peers.ButtonAutomationPeer(button);
            //System.Windows.Automation.Provider.IInvokeProvider iip = bam.GetPattern(System.Windows.Automation.Peers.PatternInterface.Invoke) as System.Windows.Automation.Provider.IInvokeProvider;
            //iip.Invoke();
            sender.RaiseEvent(System.Windows.Controls.Primitives.ButtonBase.ClickEvent);
        }

        /// <summary>
        /// 执行注册的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void RaiseEvent(this System.Windows.UIElement sender, RoutedEvent e)
        {
            //System.Windows.Automation.Peers.ButtonAutomationPeer bam = new System.Windows.Automation.Peers.ButtonAutomationPeer(button);
            //System.Windows.Automation.Provider.IInvokeProvider iip = bam.GetPattern(System.Windows.Automation.Peers.PatternInterface.Invoke) as System.Windows.Automation.Provider.IInvokeProvider;
            //iip.Invoke();
            sender.RaiseEvent(new RoutedEventArgs(e));
        }

    }
}

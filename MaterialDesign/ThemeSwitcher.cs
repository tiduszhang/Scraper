using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MaterialDesign
{
    /// <summary>
    /// FileName: ThemeSwitcher.cs
    /// CLRVersion: 4.0.30319.42000
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// Corporation:
    /// Description:    
    /// </summary>
    public class ThemeSwitcher
    {
        /// <summary>
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public static ResourceDictionary GetThemeResourceDictionary(ThemeEnum theme)
        {
            switch (theme)
            {
                //==================== CLASSIC ======================================
                //C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\PresentationClassic.dll
                //classic
                case ThemeEnum.CLASSIC:
                    return GetThemeResourceDictionary(
                            "/PresentationClassic, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/classic.xaml",
                            UriKind.Relative);
                //==================== ROYALE ======================================
                //C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\PresentationRoyale.dll
                //royale.normalcolor
                case ThemeEnum.ROYALE:
                    return GetThemeResourceDictionary(
                            "/PresentationRoyale, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/royale.normalcolor.xaml",
                            UriKind.Relative);
                //==================== LUNA ======================================
                //C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\PresentationLuna.dll
                //luna.normalcolor
                case ThemeEnum.LUNA:
                    return GetThemeResourceDictionary(
                            "/PresentationLuna, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/luna.normalcolor.xaml",
                            UriKind.Relative);
                //luna.homestead
                case ThemeEnum.LUNA_HOMESTEAD:
                    return GetThemeResourceDictionary(
                            "/PresentationLuna, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/luna.homestead.xaml",
                            UriKind.Relative);
                //luna.metallic
                case ThemeEnum.LUNA_METALLIC:
                    return GetThemeResourceDictionary(
                            "/PresentationLuna, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/luna.metallic.xaml",
                            UriKind.Relative);
                //==================== AERO ======================================
                //C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\PresentationAero.dll
                //aero.normalcolor
                case ThemeEnum.AERO:
                    return GetThemeResourceDictionary(
                            "/PresentationAero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/aero.normalcolor.xaml",
                            UriKind.Relative);

                //==================== METRO ======================================
                //Themes.Metro.dll
                //Skin
                case ThemeEnum.METRO:
                default:
                    return GetThemeResourceDictionary(
                            "/MaterialDesign;component/Skin.xaml",
                            UriKind.RelativeOrAbsolute);
            }
            //return Application.Current.Resources;
        }

        /// <summary>
        /// </summary>
        /// <param name="themeurl"></param>
        /// <param name="UriKind"></param>
        /// <returns></returns>
        public static ResourceDictionary GetThemeResourceDictionary(string themeurl, UriKind UriKind)
        {
            var uri = new Uri(themeurl, UriKind);
            return Application.LoadComponent(uri) as ResourceDictionary;
        }

        /// <summary>
        /// 设置主题 
        /// </summary>
        /// <param name="ThemeEnum"></param>
        public static void SetTheme(ThemeEnum ThemeEnum)
        {
            Application.Current.Resources.MergedDictionaries.Add(GetThemeResourceDictionary(ThemeEnum));
        }

        /// <summary>
        /// </summary>
        /// <param name="themeurl"> Theme引用地址 如："/Themes.Metro;component/Skin.xaml" </param>
        /// <param name="element"> FrameworkElement对象 </param>
        public static void SwitchTheme(string themeurl, FrameworkElement element)
        {
            element.Resources.MergedDictionaries.Add(GetThemeResourceDictionary(themeurl, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// </summary>
        /// <param name="theme"> Theme枚举 </param>
        /// <param name="element"> FrameworkElement对象 </param>
        public static void SwitchTheme(ThemeEnum theme, FrameworkElement element)
        {
            element.Resources.MergedDictionaries.Add(GetThemeResourceDictionary(theme));
        }

        //public static void UnloadTheme(ThemeEnum theme,FrameworkContentElement element) {
        //    element.Resources.MergedDictionaries.Remove(GetThemeResourceDictionary(theme));
        //}
    }
}
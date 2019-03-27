using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaterialDesign
{
    /// <summary>
    /// FileName: ThemeEnum.cs
    /// CLRVersion: 4.0.30319.42000
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// Corporation:
    /// Description:    
    /// </summary>
    [Flags]
    public enum ThemeEnum
    {
        /// <summary>
        /// Classic 
        /// </summary>
        CLASSIC = 1,

        /// <summary>
        /// Royale 
        /// </summary>
        ROYALE = 2,

        /// <summary>
        /// Luna 
        /// </summary>
        LUNA = 4,

        /// <summary>
        /// Luna 
        /// </summary>
        LUNA_HOMESTEAD = 8,

        /// <summary>
        /// Luna 
        /// </summary>
        LUNA_METALLIC = 16,

        /// <summary>
        /// Aero 
        /// </summary>
        AERO = 32,

        /// <summary>
        /// Metro 
        /// </summary>
        METRO = 64
    }
}
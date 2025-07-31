/// -------------------------------------------------------------------------------
/// AppEngine Framework
///
/// Copyright (C) 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemAssembly = System.Reflection.Assembly;
using SystemFieldInfo = System.Reflection.FieldInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace AppEngine
{
    /// <summary>
    /// 程序库接口类，用于对程序启动所需的模块进行定义及加载管理
    /// </summary>
    public static class AppLibrary
    {
        /// <summary>
        /// 程序库类型枚举定义
        /// </summary>
        public enum LibraryType : byte
        {
            Unknown = 0,
            Clib = 11,
            Nova = 12,
            Basic = 13,
            Gimp = 14,
            Agen = 21,
            Api = 22,
            Game = 31,
            GameHotfix = 32,
            World = 41,
            WorldHotfix = 42,
        }

        /// <summary>
        /// 程序库工作类型枚举定义
        /// </summary>
        public enum LibraryWorkingType : byte
        {
            Unknown = 0,
            /// <summary>
            /// 内核层
            /// </summary>
            Kernel = 1,
            /// <summary>
            /// 中间层
            /// </summary>
            Intermediate = 2,
            /// <summary>
            /// 服务层
            /// </summary>
            Service = 3,
        }

        ///// <summary>
        ///// 依赖库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.Clib, LibraryWorkingType.Kernel)]
        //public const string ClibAssemblyName = "Clib";

        ///// <summary>
        ///// 核心库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.Nova, LibraryWorkingType.Kernel)]
        //public const string NovaAssemblyName = "Nova";

        ///// <summary>
        ///// 基础库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.Basic, LibraryWorkingType.Kernel)]
        //public const string BasicAssemblyName = "Basic";

        ///// <summary>
        ///// 导入库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.Gimp, LibraryWorkingType.Intermediate)]
        //public const string GimpAssemblyName = "Gimp";

        ///// <summary>
        ///// 配置库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.Agen, LibraryWorkingType.Service)]
        //public const string AgenAssemblyName = "Agen";

        ///// <summary>
        ///// Api库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.Api, LibraryWorkingType.Service, RuntimeHotfix = true)]
        //public const string ApiAssemblyName = "Api";

        ///// <summary>
        ///// 游戏基础库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.Game, LibraryWorkingType.Service)]
        //public const string GameAssemblyName = "Game";

        ///// <summary>
        ///// 游戏逻辑库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.GameHotfix, LibraryWorkingType.Service, RuntimeHotfix = true)]
        //public const string GameHotfixAssemblyName = "GameHotfix";

        ///// <summary>
        ///// 世界基础库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.World, LibraryWorkingType.Service)]
        //public const string WorldAssemblyName = "World";

        ///// <summary>
        ///// 世界逻辑库程序集名称
        ///// </summary>
        //[LibraryTypeBinding(LibraryType.WorldHotfix, LibraryWorkingType.Service, RuntimeHotfix = true)]
        //public const string WorldHotfixAssemblyName = "WorldHotfix";

        /// <summary>
        /// 程序库的代码文件目录
        /// </summary>
        public const string LibraryCodePath = "Assets/Sources";
        /// <summary>
        /// 程序库的链接文件目录
        /// </summary>
        public const string LibraryDylinkPath = "Assets/_Resources";
        /// <summary>
        /// 程序库的资源文件目录
        /// </summary>
        public const string LibraryBinaryPath = "Assets/_Resources";

        /// <summary>
        /// 程序库启动接口函数
        /// </summary>
        public static void Startup()
        {
            // 初始化程序库中所有程序集类型
            InitLibraryAssemblyTypes();
        }

        /// <summary>
        /// 程序库关闭接口函数
        /// </summary>
        public static void Shutdown()
        {
            // 清理程序库中所有程序集类型
            CleanupAllLibraryAssemblyTypes();
        }

        #region 程序库属性关联操作接口

        /// <summary>
        /// 程序库类型绑定属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        private sealed class LibraryTypeBindingAttribute : SystemAttribute
        {
            /// <summary>
            /// 程序库类型标识
            /// </summary>
            private LibraryType _libraryType;
            /// <summary>
            /// 程序库工作类型标识
            /// </summary>
            private LibraryWorkingType _libraryWorkingType;

            /// <summary>
            /// 运行时热重载状态标识
            /// </summary>
            public bool RuntimeHotfix = false;

            public LibraryType LibraryType => _libraryType;
            public LibraryWorkingType LibraryWorkingType => _libraryWorkingType;

            public LibraryTypeBindingAttribute(LibraryType libraryType, LibraryWorkingType libraryWorkingType)
            {
                _libraryType = libraryType;
                _libraryWorkingType = libraryWorkingType;
            }
        }

        /// <summary>
        /// 程序库描述信息类，用于记录其状态及资源信息
        /// </summary>
        private sealed class LibraryInfo
        {
            /// <summary>
            /// 程序库名称
            /// </summary>
            private readonly string _libraryName;
            /// <summary>
            /// 程序库类型标识
            /// </summary>
            private readonly LibraryType _libraryType;
            /// <summary>
            /// 程序库工作类型标识
            /// </summary>
            private LibraryWorkingType _libraryWorkingType;
            /// <summary>
            /// 程序库启用状态
            /// </summary>
            private bool _isEnabled;
            /// <summary>
            /// 程序库热重载启用状态
            /// </summary>
            private bool _isRuntimeHotfix;
            /// <summary>
            /// 程序库对应加载的程序集
            /// </summary>
            private SystemAssembly _assembly;

            /// <summary>
            /// 程序库名称获取函数
            /// </summary>
            public string LibraryName => _libraryName;
            /// <summary>
            /// 程序库类型标识获取函数
            /// </summary>
            public LibraryType LibraryType => _libraryType;
            /// <summary>
            /// 程序库工作类型获取函数
            /// </summary>
            public LibraryWorkingType LibraryWorkingType => _libraryWorkingType;
            /// <summary>
            /// 程序库启用状态获取/设置函数
            /// </summary>
            public bool Enabled { get { return _isEnabled; } set { _isEnabled = value; } }
            /// <summary>
            /// 程序库热重载启用状态获取函数
            /// </summary>
            public bool RuntimeHotfix => _isRuntimeHotfix;
            /// <summary>
            /// 程序库对应加载的程序集获取/设置函数
            /// </summary>
            public SystemAssembly Assembly { get { return _assembly; } set { _assembly = value; } }

            public LibraryInfo(string libraryName, LibraryType libraryType)
            {
                _libraryName = libraryName;
                _libraryType = libraryType;
            }

            public LibraryInfo(string libraryName, LibraryType libraryType, LibraryTypeBindingAttribute attribute) : this(libraryName, libraryType)
            {
                _libraryWorkingType = attribute.LibraryWorkingType;

                _isRuntimeHotfix = attribute.RuntimeHotfix;
            }
        }

        /// <summary>
        /// 程序库名称与类型映射集合
        /// </summary>
        private static IDictionary<string, LibraryType> _libraryTypeMapping = null;
        /// <summary>
        /// 程序库类型与描述对象映射集合
        /// </summary>
        private static IDictionary<LibraryType, LibraryInfo> _libraryInfoMapping = null;

        /// <summary>
        /// 初始化程序库中所有的程序集类型
        /// </summary>
        private static void InitLibraryAssemblyTypes()
        {
            _libraryTypeMapping = new Dictionary<string, LibraryType>();
            _libraryInfoMapping = new Dictionary<LibraryType, LibraryInfo>();

            SystemType classType = typeof(AppLibrary);
            SystemFieldInfo[] fields = classType.GetFields(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static | SystemBindingFlags.FlattenHierarchy);
            for (int n = 0; n < fields.Length; ++n)
            {
                SystemFieldInfo field = fields[n];

                LibraryTypeBindingAttribute attribute = field.GetCustomAttribute<LibraryTypeBindingAttribute>();
                if (null == attribute)
                {
                    // 该字段没有程序库类型绑定属性，不属于程序库标识变量
                    continue;
                }

                if (!(field.IsLiteral && false == field.IsInitOnly))
                {
                    // 非const常量属性定义
                    AppLogger.Error("The library binding field '{0}' must be const type, please checked it before doing this operation.", field.Name);
                    continue;
                }

                LibraryType libraryType = attribute.LibraryType;

                AppLogger.Assert(false == _libraryTypeMapping.ContainsKey(field.Name));
                _libraryTypeMapping.Add(field.Name, libraryType);

                LibraryInfo libraryInfo = new LibraryInfo(field.Name, libraryType, attribute);

                AppLogger.Assert(false == _libraryInfoMapping.ContainsKey(libraryType));
                _libraryInfoMapping.Add(libraryType, libraryInfo);
            }
        }

        /// <summary>
        /// 清理程序库中所有的程序集类型
        /// </summary>
        private static void CleanupAllLibraryAssemblyTypes()
        {
            _libraryTypeMapping?.Clear();
            _libraryTypeMapping = null;

            _libraryInfoMapping?.Clear();
            _libraryInfoMapping = null;
        }

        #endregion
    }
}

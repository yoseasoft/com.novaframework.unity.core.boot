/// -------------------------------------------------------------------------------
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

namespace NovaFramework
{
    /// <summary>
    /// 程序库接口类，用于对程序启动所需的模块进行定义及加载管理
    /// </summary>
    public static class AppLibrary
    {
        /// <summary>
        /// 程序集名称列表
        /// </summary>
        static IReadOnlyList<string> _assemblyNames = null;
        /// <summary>
        /// 可加载程序集名称列表
        /// </summary>
        static IReadOnlyList<string> _loadableAssemblyNames = null;
        /// <summary>
        /// 可重载程序集名称列表
        /// </summary>
        static IReadOnlyList<string> _reloadableAssemblyNames = null;
        /// <summary>
        /// 元数据链接库名称列表
        /// </summary>
        static IReadOnlyList<string> _aotLibraryNames = null;

        /// <summary>
        /// 获取当前系统注册的全部程序集名称
        /// </summary>
        /// <returns>返回全部程序集的名称列表</returns>
        public static IReadOnlyList<string> GetAllAssemblyNames()
        {
            if (null == _assemblyNames)
            {
                _assemblyNames = DynamicLibrary.GetAllAssemblyNames((info) =>
                {
                    // 教程相关程序模块的过滤
                    if (!AppConfigures.Instance.tutorialMode && info.IsContainsTag(LibraryTag.Tutorial))
                        return false;

                    return true;
                });
            }

            return _assemblyNames;
        }

        /// <summary>
        /// 获取当前系统注册的全部可加载程序集名称
        /// </summary>
        /// <returns>返回全部可加载程序集的名称列表</returns>
        public static IReadOnlyList<string> GetAllLoadableAssemblyNames()
        {
            if (null == _loadableAssemblyNames)
            {
                _loadableAssemblyNames = DynamicLibrary.GetAllAssemblyNames((info) =>
                {
                    // 跳过内核或插件库
                    if (false == info.IsContainsTag(LibraryTag.Game))
                        return false;

                    // 教程相关程序模块的过滤
                    if (!AppConfigures.Instance.tutorialMode && info.IsContainsTag(LibraryTag.Tutorial))
                        return false;

                    return true;
                });
            }

            return _loadableAssemblyNames;
        }

        /// <summary>
        /// 获取当前系统注册的全部可重载程序集名称
        /// </summary>
        /// <returns>返回全部可重载程序集的名称列表</returns>
        public static IReadOnlyList<string> GetAllReloadableAssemblyNames()
        {
            if (null == _reloadableAssemblyNames)
            {
                _reloadableAssemblyNames = DynamicLibrary.GetAllPlayableAssemblyNames((info) =>
                {
                    if (!AppConfigures.Instance.tutorialMode && info.IsContainsTag(LibraryTag.Tutorial))
                        return false;

                    if (!info.IsContainsTag(LibraryTag.Hotfix))
                        return false;

                    return true;
                });
            }

            return _reloadableAssemblyNames;
        }

        /// <summary>
        /// 获取当前系统注册的全部元数据链接库名称
        /// </summary>
        /// <returns>返回全部元数据链接库的名称列表</returns>
        public static IReadOnlyList<string> GetAllGenericAotNames()
        {
            if (null == _aotLibraryNames)
            {
                _aotLibraryNames = DynamicLibrary.GetAllGenericAotNames();
            }

            return _aotLibraryNames;
        }
    }
}

/// -------------------------------------------------------------------------------
/// CoreEngine Framework
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

namespace CoreEngine
{
    /// <summary>
    /// 程序库接口类，用于对程序启动所需的模块进行定义及加载管理
    /// </summary>
    public static class AppLibrary
    {
        /// <summary>
        /// 程序集名称列表
        /// </summary>
        static IList<string> _assemblyNames = null;
        /// <summary>
        /// 可重载程序集名称列表
        /// </summary>
        static IList<string> _reloadableAssemblyNames = null;

        /// <summary>
        /// 获取当前系统注册的全部程序集名称
        /// </summary>
        /// <returns>返回全部程序集的名称列表</returns>
        public static IList<string> GetAllAssemblyNames()
        {
            if (null == _assemblyNames)
            {
                _assemblyNames = DynamicLibrary.GetAllAssemblyNames((info) =>
                {
                    // 教程相关程序模块的过滤
                    if (!AppSettings.Instance.tutorialMode && info.tutorial)
                        return false;

                    return true;
                });
            }

            return _assemblyNames;
        }

        /// <summary>
        /// 获取当前系统注册的全部可重载程序集名称
        /// </summary>
        /// <returns>返回全部可重载程序集的名称列表</returns>
        public static IList<string> GetAllReloadableAssemblyNames()
        {
            if (null == _reloadableAssemblyNames)
            {
                _reloadableAssemblyNames = DynamicLibrary.GetAllReloadableAssemblyNames((info) =>
                {
                    if (!AppSettings.Instance.tutorialMode && info.tutorial)
                        return false;

                    return true;
                });
            }

            return _reloadableAssemblyNames;
        }
    }
}

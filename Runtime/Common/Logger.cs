/// -------------------------------------------------------------------------------
/// CoreEngine Framework
///
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

namespace CoreEngine
{
    /// <summary>
    /// 程序日志接口类，用于在程序中提供日志输出接口函数
    /// </summary>
    internal static class Logger
    {
        /// <summary>
        /// 普通信息输出日志接口函数
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Info(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// 普通信息输出日志接口函数
        /// </summary>
        /// <param name="format">格式化文本</param>
        /// <param name="args">参数列表</param>
        public static void Info(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        /// <summary>
        /// 警告信息输出日志接口函数
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Warn(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        /// <summary>
        /// 警告信息输出日志接口函数
        /// </summary>
        /// <param name="format">格式化文本</param>
        /// <param name="args">参数列表</param>
        public static void Warn(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(format, args);
        }

        /// <summary>
        /// 错误信息输出日志接口函数
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        /// <summary>
        /// 错误信息输出日志接口函数
        /// </summary>
        /// <param name="format">格式化文本</param>
        /// <param name="args">参数列表</param>
        public static void Error(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }

        /// <summary>
        /// 断言调试接口函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        public static void Assert(bool condition)
        {
            UnityEngine.Debug.Assert(condition);
        }
    }
}

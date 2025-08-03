/// -------------------------------------------------------------------------------
/// CoreEngine Editor Framework
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

namespace CoreEngine.Editor
{
    /// <summary>
    /// 编辑器配置参数管理类，用于记录编辑环境下的参数设置
    /// </summary>
    public static class EditorConfigure
    {
        /// <summary>
        /// 设置一个字符串值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetString(string key, string value)
        {
            UserSettings.SetString(key, value);
        }

        /// <summary>
        /// 获取一个字符串值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static string GetString(string key)
        {
            return UserSettings.GetString(key);
        }

        /// <summary>
        /// 设置一个布尔值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetBool(string key, bool value)
        {
            UserSettings.SetBool(key, value);
        }

        /// <summary>
        /// 获取一个布尔值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static bool GetBool(string key)
        {
            return UserSettings.GetBool(key);
        }

        /// <summary>
        /// 设置一个整型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetInt(string key, int value)
        {
            UserSettings.SetInt(key, value);
        }

        /// <summary>
        /// 获取一个整型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static int GetInt(string key)
        {
            return UserSettings.GetInt(key);
        }

        /// <summary>
        /// 设置一个长整型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetLong(string key, long value)
        {
            UserSettings.SetLong(key, value);
        }

        /// <summary>
        /// 获取一个长整型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static long GetLong(string key)
        {
            return UserSettings.GetLong(key);
        }

        /// <summary>
        /// 设置一个浮点型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetFloat(string key, float value)
        {
            UserSettings.SetFloat(key, value);
        }

        /// <summary>
        /// 获取一个浮点型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static float GetFloat(string key)
        {
            return UserSettings.GetFloat(key);
        }

        /// <summary>
        /// 设置一个日期型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetDateTime(string key, System.DateTime dateTime)
        {
            UserSettings.SetDateTime(key, dateTime);
        }

        /// <summary>
        /// 获取一个日期型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static System.DateTime GetDateTime(string key)
        {
            return UserSettings.GetDateTime(key);
        }
    }
}

/// -------------------------------------------------------------------------------
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using UnityEngine.SceneManagement;

namespace NovaFramework
{
    /// <summary>
    /// 辅助工具接口类，用于提供一些辅助工具方法
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// 检测当前运行环境下是否只有一个场景，且该场景中没有任何游戏对象实例
        /// </summary>
        /// <returns>若符合空场景条件则返回true，否则返回false</returns>
        public static bool IsEmptyScene()
        {
            // 获取场景数量
            int sceneCount = SceneManager.sceneCount;
            // 场景数量大于1，则认为当前不是空场景
            if (sceneCount > 1)
            {
                return false;
            }

            // 获取当前活动场景
            Scene currentScene = SceneManager.GetActiveScene();
            // 获取场景中的根游戏对象数量
            int rootCount = currentScene.rootCount;
            // 如果没有根游戏对象，则认为当前场景为空
            return 0 == rootCount;
        }

        /// <summary>
        /// 检测当前活动场景名称是否为指定名称
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>若场景名称一致则返回true，否则返回false</returns>
        public static bool IsActiveSceneName(string sceneName)
        {
            return SceneManager.GetActiveScene().name.ToLower() == sceneName.ToLower();
        }
    }
}

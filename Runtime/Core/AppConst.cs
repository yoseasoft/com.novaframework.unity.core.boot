/// -------------------------------------------------------------------------------
/// CoreEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 应用程序常量设置类，用于统一管理当前模块中所需要用到的常量定义
    /// </summary>
    public static class AppConst
    {
        /// <summary>
        /// 主调度器的场景名称
        /// </summary>
        public const string AppControllerSceneName = "main";
        /// <summary>
        /// 主调度器的对象节点名称
        /// </summary>
        public const string AppControllerGameObjectName = "MainApp";
        /// <summary>
        /// 主调度器的引擎控制器类名称
        /// </summary>
        public const string AppControllerClassName = "GameEngine.EngineController";

        /// <summary>
        /// 核心链接库名称
        /// </summary>
        public const string NovaDylinkName = "Nova.Engine";

        /// <summary>
        /// 基础链接库名称
        /// </summary>
        public const string BasicDylinkName = "Nova.Basic";

        /// <summary>
        /// 配置链接库名称
        /// </summary>
        public const string AgenDylinkName = "Agen";

        /// <summary>
        /// Api链接库名称
        /// </summary>
        public const string ApiDylinkName = "Api";

        /// <summary>
        /// 游戏基础链接库名称
        /// </summary>
        public const string GameDylinkName = "Game";

        /// <summary>
        /// 游戏逻辑链接库名称
        /// </summary>
        public const string GameHotfixDylinkName = "GameHotfix";

        /// <summary>
        /// 世界基础链接库名称
        /// </summary>
        public const string WorldDylinkName = "World";

        /// <summary>
        /// 世界逻辑链接库名称
        /// </summary>
        public const string WorldHotfixDylinkName = "WorldHotfix";

        /// <summary>
        /// 链接库的资源目录
        /// </summary>
        public const string DylinkAssetPath = "Assets/_Resources/Code";
    }
}

/// -------------------------------------------------------------------------------
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using UnityEngine;

namespace NovaFramework
{
    /// <summary>
    /// 应用程序的基础配置，用于应用正式启动的参数设置
    /// </summary>
    // [CreateAssetMenu(fileName = "AppSettings", menuName = "Nova Framework/Application Settings")] // 创建后可以不再显示在右键菜单
    public class AppSettings : ScriptableObject
    {
        // ----------------------------------------------------------------------------------------------------
        [Header("应用程序运行模式")]

        [FieldLabelName("编辑模式")]
        [Tooltip("仅当此模式打开后可以通过路径方式访问未打包的原始资源")]
        public bool editorMode = false;

        [FieldLabelName("调试模式")]
        [Tooltip("仅当此模式打开后可以启用调试窗口组件和输出调试信息")]
        public bool debugMode = false;

        [EnumLabelName("调试级别")]
        [Tooltip("输出调试信息的分级设置，可以通过该分级控制输出信息内容")]
        public DebugLevelType debugLevel = DebugLevelType.Debug;

        [FieldLabelName("加密模式")]
        [Tooltip("当此模式打开后，将对程序中的所有资源进行解码访问")]
        public bool cryptMode = false;

        [FieldLabelName("离线模式")]
        [Tooltip("当此模式打开后，程序将不再执行在线更新流程访问")]
        public bool offlineMode = false;

        [FieldLabelName("链接模式")]
        [Tooltip("当此模式打开后，程序将通过链接库的方式启动(需要断点调试可关闭该模式)")]
        public bool dylinkMode = false;

        [FieldLabelName("热载模式")]
        [Tooltip("当此模式打开后，程序将支持程序集的热重载操作(需要同时开启链接模式才能正常使用)")]
        public bool hotfixMode = false;

        // ----------------------------------------------------------------------------------------------------
        [Header("应用程序系统参数")]

        [FieldLabelName("应用名称")]
        [Tooltip("应用程序的唯一名称，将应用在安装环境下的目录与文件命名")]
        public string applicationName = "unknown";

        [FieldLabelName("应用编码")]
        [Tooltip("应用程序的唯一编码，通常应用在后台以区分不同的安装程序")]
        public int applicationCode = 0;

        [FieldLabelName("刷新帧数")]
        [Tooltip("应用程序的逻辑刷新帧数")]
        public int frameRate = 0;

        [FieldLabelName("动画速率")]
        [Tooltip("应用程序的动画刷新帧率")]
        public int animationRate = 0;

        [FieldLabelName("分辨率宽度")]
        [Tooltip("分辨率宽度")]
        public int designResolutionWidth = 0;

        [FieldLabelName("分辨率高度")]
        [Tooltip("分辨率高度")]
        public int designResolutionHeight = 0;

        // ----------------------------------------------------------------------------------------------------
        [Header("自定义环境变量")]

        public List<CustomizeEnvironmentVariable> customizeEnvironmentVariables = new();

        /// <summary>
        /// AppSettings示例
        /// </summary>
        public static AppSettings Instance
        {
            get
            {
                AppSettings settings = Resources.Load<AppSettings>(nameof(AppSettings));
                if (settings == null)
                {
                    settings = CreateInstance<AppSettings>();
                    Logger.Error("Could not found any AppSettings assets, please create one instance in resources directory.");
                }

                return settings;
            }
        }
    }

    /// <summary>
    /// 自定义环境变量键值管理对象类
    /// </summary>
    [System.Serializable]
    public class CustomizeEnvironmentVariable
    {
        [FieldLabelName("键")]
        [Tooltip("自定义环境变量的映射键信息")]
        public string key;

        [FieldLabelName("值")]
        [Tooltip("自定义环境变量的映射值信息")]
        public string value;
    }

    /// <summary>
    /// 调试级别的类型定义
    /// </summary>
    public enum DebugLevelType : int
    {
        [Header("调试模式")]
        Debug = 0,

        [Header("信息模式")]
        Info = 1,

        [Header("警告模式")]
        Warning = 2,

        [Header("错误模式")]
        Error = 3,

        [Header("崩溃模式")]
        Fatal = 4,
    }
}

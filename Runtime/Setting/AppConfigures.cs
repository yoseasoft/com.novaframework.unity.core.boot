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

using UnityEngine;

namespace NovaFramework
{
    /// <summary>
    /// 应用程序的基础配置，用于应用正式启动的参数设置
    /// </summary>
    // [CreateAssetMenu(fileName = "AppConfigures", menuName = "Nova Framework/Application Configures")] // 创建后可以不再显示在右键菜单
    public class AppConfigures : ScriptableObject
    {
        // ----------------------------------------------------------------------------------------------------
        [Header("应用程序运行设置")]

        [FieldLabelName("游戏入口")]
        [Tooltip("应用程序业务层入口，必须填写该名称后才可以正常启动业务逻辑")]
        public string gameEntryName = null;

        [FieldLabelName("休眠模式")]
        [Tooltip("当此模式打开后程序将不会进入休眠状态")]
        public bool screenNeverSleep = false;

        [EnumLabelName("网络消息包头长度")]
        [Tooltip("应用程序网络通信的消息包头长度设置，必须设置该选项后才可以进行网络通信")]
        public NetworkMessageHeaderSizeType networkMessageHeaderSize = NetworkMessageHeaderSizeType.Unknown;

        // ----------------------------------------------------------------------------------------------------
        [Header("应用程序日志设置")]

        [Tooltip("日志通道开启的类型组合，可以同时开启多种不同类型的通道用于日志输出")]
        public LogChannelType[] logChannel = null;

        [FieldLabelName("日志使用自定义颜色")]
        [Tooltip("日志文本颜色使用自定义设置，具体颜色值请参考编辑窗口定义")]
        public bool logUsingCustomColor = false;

        [FieldLabelName("日志使用系统颜色")]
        [Tooltip("日志文本颜色使用系统设置，具体颜色值请参考编辑窗口定义")]
        public bool logUsingSystemColor = false;

        [FieldLabelName("日志分组过滤模式")]
        [Tooltip("当此模式打开后，日志的分组输出将默认启动，并根据过滤清单对指定的分组进行单独过滤处理")]
        public bool logUsingGroupFilter = false;

        // ----------------------------------------------------------------------------------------------------
        [Header("应用程序调试设置")]

        [FieldLabelName("调试窗口模式")]
        [Tooltip("当此模式打开后，程序将自动挂载调试窗口组件，支持查看程序运行时的动态分析数据")]
        public bool debuggerWindowMode = false;

        [FieldLabelName("自动统计模式")]
        [Tooltip("当此模式打开后，程序将自动对实体对象进行统计，并在调试窗口中显示其统计信息")]
        public bool autoStatisticsMode = false;

        // ----------------------------------------------------------------------------------------------------
        [Header("应用程序教程设置")]

        [FieldLabelName("教程模式")]
        [Tooltip("当此模式打开后，程序将跳转到演示案例环境下进行启动")]
        public bool tutorialMode = false;

        [FieldLabelName("教程案例")]
        [Tooltip("通过选择教程示例，程序运行后将在对应的案例环境下进行启动")]
        public TutorialSampleType tutorialSampleType = TutorialSampleType.Unknown;

        /// <summary>
        /// AppConfigures示例
        /// </summary>
        public static AppConfigures Instance
        {
            get
            {
                AppConfigures configures = Resources.Load<AppConfigures>(nameof(AppConfigures));
                if (configures == null)
                {
                    configures = CreateInstance<AppConfigures>();
                    Logger.Error("Could not found any AppConfigures assets, please create one instance in resources directory.");
                }

                return configures;
            }
        }
    }

    /// <summary>
    /// 网络消息包头长度类型的枚举定义
    /// </summary>
    public enum NetworkMessageHeaderSizeType : byte
    {
        [Header("未知")]
        Unknown = 0,

        [Header("2字节")]
        Header2 = 2,

        [Header("4字节")]
        Header4 = 4,
    }

    /// <summary>
    /// 日志输出通道设置，你可以设置以下类型：<br/>
    /// 1 - 控制台<br/>
    /// 2 - 编辑器<br/>
    /// 4 - 视图窗口<br/>
    /// 8 - 本地文件<br/>
    /// 若需要同时对多个类型的通道进行输出，则可以将多个类型或操作后的值设置到该参数上<br/>
    /// 例如，需要同时开启“控制台”和“本地文件”，则可以将该参数设置为：9<br/>
    /// 需要特别声明，编辑器模式仅在编辑器环境下生效，非编辑器环境该标识将自动清除<br/>
    /// <br/>
    /// 这里需要注意，如果设置了本地文件选项，建议同步配置本地文件的存放路径及文件名称<br/>
    /// 如果没有配置名称，则默认使用临时目录下，以应用程序+“_log”作为日志文件的名称
    /// </summary>
    public enum LogChannelType : int
    {
        [Header("未知")]
        Unknown = 0,

        [Header("控制台")]
        Console = 1,

        [Header("编辑器")]
        Editor = 2,

        [Header("视图窗口")]
        Window = 4,

        [Header("本地文件")]
        File = 8,
    }

    /// <summary>
    /// 教程示例的类型定义
    /// </summary>
    public enum TutorialSampleType : int
    {
        [Header("未知")]
        Unknown = 0,

        [Header("文本格式化")]
        TextFormat,

        [Header("符号解析")]
        SymbolParser,

        [Header("构建动态调用")]
        DynamicInvokeGenerator,

        [Header("控制反转")]
        InversionOfControl,

        [Header("对象生命周期")]
        ObjectLifecycle,

        [Header("转发通知")]
        DispatchCall,

        [Header("状态转换")]
        StateTransition,

        [Header("依赖注入")]
        DependencyInject,

        [Header("配置表达式")]
        ConfigureExpression,

        [Header("性能分析")]
        PerformanceAnalysis,
    }
}

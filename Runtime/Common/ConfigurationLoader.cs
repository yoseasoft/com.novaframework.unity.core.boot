/// -------------------------------------------------------------------------------
/// Copyright (C) 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using System.IO;

using Cysharp.Threading.Tasks;

namespace NovaFramework
{
    /// <summary>
    /// 程序配置加载器类，用于对程启动所需的相关配置参数进行统一加载管理
    /// </summary>
    internal static class ConfigurationLoader
    {
        /// <summary>
        /// 加载应用配置的设置信息
        /// </summary>
        // public static async UniTask<IReadOnlyDictionary<string, string>> LoadEnvironmentSettings()
        public static IReadOnlyDictionary<string, string> LoadEnvironmentSettings()
        {
            IDictionary<string, string> vars = new Dictionary<string, string>();

            // 系统配置数据
            AppSettings settings = AppSettings.Instance;

            // 运行模式
#if UNITY_EDITOR
            vars.Add(nameof(settings.EditorMode), settings.EditorMode.ToString());
#else
            vars.Add(nameof(settings.EditorMode), "false");
#endif

#if DEBUG
            vars.Add(nameof(settings.DebugMode), settings.DebugMode.ToString());
#else
            vars.Add(nameof(settings.DebugMode), "false");
#endif
            vars.Add(nameof(settings.DebugLevel), ((int) settings.DebugLevel).ToString());
            vars.Add(nameof(settings.CryptMode), settings.CryptMode.ToString());
            vars.Add(nameof(settings.OfflineMode), settings.OfflineMode.ToString());
            vars.Add(nameof(settings.DylinkMode), settings.DylinkMode.ToString());
            vars.Add(nameof(settings.HotfixMode), settings.HotfixMode.ToString());

            // 系统参数
            vars.Add(nameof(settings.ApplicationName), settings.ApplicationName.ToString());
            vars.Add(nameof(settings.ApplicationCode), settings.ApplicationCode.ToString());
            vars.Add(nameof(settings.FrameRate), settings.FrameRate.ToString());
            vars.Add(nameof(settings.AnimationRate), settings.AnimationRate.ToString());
            vars.Add(nameof(settings.DesignResolutionWidth), settings.DesignResolutionWidth.ToString());
            vars.Add(nameof(settings.DesignResolutionHeight), settings.DesignResolutionHeight.ToString());

            // 自定义环境变量
            foreach (CustomizeEnvironmentVariable v in settings.CustomizeEnvironmentVariables)
            {
                vars.Add(v.key, v.value);
            }

            // 应用配置数据
            AppConfigures configures = AppConfigures.Instance;

            // 运行设置
            vars.Add(nameof(configures.GameEntryName), configures.GameEntryName.ToString());
            vars.Add(nameof(configures.ScreenNeverSleep), configures.ScreenNeverSleep.ToString());
            vars.Add(nameof(configures.NetworkMessageHeaderSize), ((int) configures.NetworkMessageHeaderSize).ToString());

            // 日志设置
            int logChannel = 0;
            for (int n = 0; null != configures.LogChannel && n < configures.LogChannel.Length; ++n)
            {
                logChannel |= (int) configures.LogChannel[n];
            }

            vars.Add(nameof(configures.LogChannel), logChannel.ToString());

#if DEBUG
            vars.Add(nameof(configures.LogUsingCustomColor), configures.LogUsingCustomColor.ToString());
            vars.Add(nameof(configures.LogUsingSystemColor), configures.LogUsingSystemColor.ToString());
            vars.Add(nameof(configures.LogUsingGroupFilter), configures.LogUsingGroupFilter.ToString());
#else
            vars.Add(nameof(configures.LogUsingCustomColor), "false");
            vars.Add(nameof(configures.LogUsingSystemColor), "false");
            vars.Add(nameof(configures.LogUsingGroupFilter), "false");
#endif

            // 调试设置
#if DEBUG
            vars.Add(nameof(configures.DebuggerWindowMode), configures.DebuggerWindowMode.ToString());
            vars.Add(nameof(configures.AutoStatisticsMode), configures.AutoStatisticsMode.ToString());
#else
            vars.Add(nameof(configures.DebuggerWindowMode), "false");
            vars.Add(nameof(configures.AutoStatisticsMode), "false");
#endif

            // 教程设置
#if DEBUG
            vars.Add(nameof(configures.TutorialMode), configures.TutorialMode.ToString());
#else
            vars.Add(nameof(configures.TutorialMode), "false");
#endif
            if ((int) configures.TutorialSampleType > 0)
            {
                // 合法配置教程案例
                vars.Add(nameof(configures.TutorialSampleType), configures.TutorialSampleType.ToString());
            }

            // await LoadConfigureSettingsVariables(vars);

            return new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(vars);
        }

        #region 属性文件加载、解析处理接口函数

        /// <summary>
        /// 加载配置参数数据
        /// </summary>
        /// <param name="variables">环境参数集合</param>
        private static async UniTask LoadConfigureSettingsVariables(IDictionary<string, string> variables)
        {
            string text;

            text = await LoadConfigureFileFromStreamAssetPath("Settings/app_conf.properties");
            if (false == ResolvePropertiesInfoFromText(text, variables))
            {
                Logger.Error("Cannot resolve properties from target file, loaded environment settings failed.");
            }

            text = await LoadConfigureFileFromStreamAssetPath("Settings/channel_conf.properties");
            if (false == ResolvePropertiesInfoFromText(text, variables))
            {
                Logger.Error("Cannot resolve properties from target file, loaded environment settings failed.");
            }
        }

        /// <summary>
        /// 从目标文件中加载配置资源
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>返回加载成功的文本字符串，若加载失败返回null</returns>
        private static async UniTask<string> LoadConfigureFileFromStreamAssetPath(string path)
        {
            string localProtocol;
            UnityEngine.RuntimePlatform pf = UnityEngine.Application.platform;
            if (pf != UnityEngine.RuntimePlatform.OSXEditor && pf != UnityEngine.RuntimePlatform.OSXPlayer && pf != UnityEngine.RuntimePlatform.IPhonePlayer)
                localProtocol = pf is UnityEngine.RuntimePlatform.WindowsEditor or UnityEngine.RuntimePlatform.WindowsPlayer ? "file:///" : string.Empty;
            else
                localProtocol = "file://";

            string configureFilePath = localProtocol + Path.Combine(UnityEngine.Application.streamingAssetsPath, path);
            UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(configureFilePath);
            await request.SendWebRequest();
            if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Logger.Error("获取目标配置文件‘{0}’失败，错误原因：{1}！", path, request.error);
                return null;
            }

            return request.downloadHandler.text;
        }

        /// <summary>
        /// 从字符串中解析属性信息
        /// </summary>
        /// <param name="text">字符串文本</param>
        /// <param name="variables">属性集合</param>
        /// <returns>字符串文本解析成功返回true，否则返回false</returns>
        private static bool ResolvePropertiesInfoFromText(string text, IDictionary<string, string> variables)
        {
            Logger.Assert(null != variables);

            string line;
            StringReader reader = new StringReader(text);
            while (null != (line = reader.ReadLine()))
            {
                // 截掉空白字符
                line = line.Trim();

                if (string.IsNullOrEmpty(line) || line.StartsWith('#'))
                {
                    // 注释行，直接跳过
                    continue;
                }

                string[] str_list = line.Split('=');
                if (str_list.Length != 2)
                {
                    // 文本格式错误
                    Logger.Warn("Invalid property content with text '{0}', parsing it failed.", line);
                    return false;
                }

                string key = str_list[0].Trim();
                string value = str_list[1].Trim();
                if (variables.ContainsKey(key))
                {
                    Logger.Warn("The property key '{0}' was already exist, repeat added it failed.", key);
                    // variables.Remove(key);
                    return false;
                }

                variables.Add(key, value);
            }
            reader.Close();

            return true;
        }

        #endregion
    }
}

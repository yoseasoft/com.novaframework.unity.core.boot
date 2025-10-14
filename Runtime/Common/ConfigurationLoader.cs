/// -------------------------------------------------------------------------------
/// CoreEngine Framework
///
/// Copyright (C) 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Collections.Generic;

using Cysharp.Threading.Tasks;

namespace CoreEngine
{
    /// <summary>
    /// 程序配置加载器类，用于对程启动所需的相关配置参数进行统一加载管理
    /// </summary>
    internal static class ConfigurationLoader
    {
        /// <summary>
        /// 加载环境配置的设置信息
        /// </summary>
        // public static async UniTask<IReadOnlyDictionary<string, string>> LoadEnvironmentSettings()
        public static IReadOnlyDictionary<string, string> LoadEnvironmentSettings()
        {
            IDictionary<string, string> vars = new Dictionary<string, string>();

            // 系统配置数据
            AppSettings settings = AppSettings.Instance;

            // 运行模式
#if UNITY_EDITOR
            vars.Add(nameof(settings.editorMode), settings.editorMode.ToString());
#else
            vars.Add(nameof(settings.editorMode), "false");
#endif

#if DEBUG
            vars.Add(nameof(settings.debugMode), settings.debugMode.ToString());
#else
            vars.Add(nameof(settings.debugMode), "false");
#endif
            vars.Add(nameof(settings.debugLevel), ((int) settings.debugLevel).ToString());
            vars.Add(nameof(settings.cryptMode), settings.cryptMode.ToString());
            vars.Add(nameof(settings.offlineMode), settings.offlineMode.ToString());
            vars.Add(nameof(settings.dylinkMode), settings.dylinkMode.ToString());

            // 系统参数
            vars.Add(nameof(settings.applicationName), settings.applicationName.ToString());
            vars.Add(nameof(settings.applicationCode), settings.applicationCode.ToString());
            vars.Add(nameof(settings.frameRate), settings.frameRate.ToString());
            vars.Add(nameof(settings.animationRate), settings.animationRate.ToString());
            vars.Add(nameof(settings.designResolutionWidth), settings.designResolutionWidth.ToString());
            vars.Add(nameof(settings.designResolutionHeight), settings.designResolutionHeight.ToString());

            // 自定义环境变量
            foreach (CustomizeEnvironmentVariable v in settings.customizeEnvironmentVariables)
            {
                vars.Add(v.key, v.value);
            }

            // 应用配置数据
            AppConfigures configures = AppConfigures.Instance;

            // 运行设置
            vars.Add(nameof(configures.screenNeverSleep), configures.screenNeverSleep.ToString());

            // 日志设置
            int logChannel = 0;
            for (int n = 0; null != configures.logChannel && n < configures.logChannel.Length; ++n)
            {
                logChannel |= (int) configures.logChannel[n];
            }

            vars.Add(nameof(configures.logChannel), logChannel.ToString());

#if DEBUG
            vars.Add(nameof(configures.logUsingCustomColor), configures.logUsingCustomColor.ToString());
            vars.Add(nameof(configures.logUsingSystemColor), configures.logUsingSystemColor.ToString());
#else
            vars.Add(nameof(configures.logUsingCustomColor), "false");
            vars.Add(nameof(configures.logUsingSystemColor), "false");
#endif

            // 教程设置
#if DEBUG
            vars.Add(nameof(configures.tutorialMode), configures.tutorialMode.ToString());
#else
            vars.Add(nameof(configures.tutorialMode), "false");
#endif
            if ((int) configures.tutorialSampleType > 0)
            {
                // 合法配置教程案例
                vars.Add(nameof(configures.tutorialSampleType), configures.tutorialSampleType.ToString());
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
            string text = null;

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

            string configureFilePath = localProtocol + System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, path);
            UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(configureFilePath);
            await request.SendWebRequest();
            if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Logger.Error("获取目标配置文件‘{%s}’失败，错误原因：{%s}！", path, request.error);
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

            string line = null;
            System.IO.StringReader reader = new System.IO.StringReader(text);
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
                    Logger.Warn("Invalid property content with text '{%s}', parsing it failed.", line);
                    return false;
                }

                string key = str_list[0].Trim();
                string value = str_list[1].Trim();
                if (variables.ContainsKey(key))
                {
                    Logger.Warn("The property key '{%s}' was already exist, repeat added it failed.", key);
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

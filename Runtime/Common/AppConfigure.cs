/// -------------------------------------------------------------------------------
/// AppEngine Framework
///
/// Copyright (C) 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace AppEngine
{
    /// <summary>
    /// 程序配置管理类，用于对程启动所需的相关配置参数进行统一加载及配置
    /// </summary>
    public static class AppConfigure
    {
        /// <summary>
        /// 加载环境配置的设置信息
        /// </summary>
        internal static async UniTask<IReadOnlyDictionary<string, string>> LoadEnvironmentSettings()
        {
            IDictionary<string, string> vars = new Dictionary<string, string>();

            AppSettings settings = AppSettings.Instance;

            // SetProperty

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

            // SetVariable(应用设置)
            vars.Add(nameof(settings.applicationName), settings.applicationName.ToString());
            vars.Add(nameof(settings.applicationCode), settings.applicationCode.ToString());
            vars.Add(nameof(settings.frameRate), settings.frameRate.ToString());
            vars.Add(nameof(settings.animationRate), settings.animationRate.ToString());
            vars.Add(nameof(settings.designResolutionWidth), settings.designResolutionWidth.ToString());
            vars.Add(nameof(settings.designResolutionHeight), settings.designResolutionHeight.ToString());

            // 示例相关
#if DEBUG
            vars.Add(nameof(settings.tutorialMode), settings.tutorialMode.ToString());
#else
            vars.Add(nameof(settings.tutorialMode), "false");
#endif
            vars.Add(nameof(settings.tutorialSampleType), settings.tutorialSampleType.ToString());

            foreach (CustomizeEnvironmentVariable v in settings.customizeEnvironmentVariables)
            {
                vars.Add(v.key, v.value);
            }

            await LoadConfigureSettingsVariables(vars);

            return new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(vars);
        }

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
                AppLogger.Error("Cannot resolve properties from target file, loaded environment settings failed.");
            }

            text = await LoadConfigureFileFromStreamAssetPath("Settings/channel_conf.properties");
            if (false == ResolvePropertiesInfoFromText(text, variables))
            {
                AppLogger.Error("Cannot resolve properties from target file, loaded environment settings failed.");
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
                AppLogger.Error("获取目标配置文件‘{%s}’失败，错误原因：{%s}！", path, request.error);
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
            AppLogger.Assert(null != variables);

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
                    AppLogger.Warn("Invalid property content with text '{%s}', parsing it failed.", line);
                    return false;
                }

                string key = str_list[0].Trim();
                string value = str_list[1].Trim();
                if (variables.ContainsKey(key))
                {
                    AppLogger.Warn("The property key '{%s}' was already exist, repeat added it failed.", key);
                    // variables.Remove(key);
                    return false;
                }

                variables.Add(key, value);
            }
            reader.Close();

            return true;
        }
    }
}

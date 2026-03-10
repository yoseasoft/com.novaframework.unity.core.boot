/// -------------------------------------------------------------------------------
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;
using UnityEditor;
using UnityEngine;

namespace NovaFramework.Editor.Preference
{
    /// <summary>
    /// 安装完成后自动导出配置的处理器
    /// </summary>
    public class SettingsExportStep : InstallationStep
    {
        const string AppSettingsAssetUrl = @"Assets/Resources/AppSettings.asset";
        const string AppConfigureAssetUrl = @"Assets/Resources/AppConfigures.asset";

        public void Install(Action onComplete = null, Action<string> addLog = null)
        {
            addLog?.Invoke("开始执行安装后配置资产创建");

            CreateAndSaveSettingAsset();
            addLog?.Invoke("已创建 AppSettings.asset");

            CreateAndSaveConfigureAsset();
            addLog?.Invoke("已创建 AppConfigures.asset");

            addLog?.Invoke("配置资产创建完成");

            // 调用完成回调
            onComplete?.Invoke();
        }

        /// <summary>
        /// 创建并保存‘AppSettings’资产文件
        /// </summary>
        private AppSettings CreateAndSaveSettingAsset()
        {
            return AssetDatabaseUtils.CreateScriptableObjectAsset<AppSettings>(AppSettingsAssetUrl, (asset) =>
            {
                asset.EditorMode = true;
                asset.DebugMode = true;
                asset.DebugLevel = DebugLevelType.Debug;

                asset.ApplicationName = Application.productName;
                asset.ApplicationCode = 0;
                asset.FrameRate = 20;
                asset.AnimationRate = 60;
                asset.DesignResolutionWidth = 1920;
                asset.DesignResolutionHeight = 1080;
            });
        }

        /// <summary>
        /// 创建并保存‘AppConfigures’资产文件
        /// </summary>
        private AppConfigures CreateAndSaveConfigureAsset()
        {
            return AssetDatabaseUtils.CreateScriptableObjectAsset<AppConfigures>(AppConfigureAssetUrl, (asset) =>
            {
                asset.GameEntryName = @"Game.GameWorld";
                asset.NetworkMessageHeaderSize = NetworkMessageHeaderSizeType.Header2;
                asset.LogChannel = new LogChannelType[] { LogChannelType.Console, LogChannelType.Editor };
            });
        }

        public void Uninstall(Action onComplete = null)
        {
            Debug.Log("PostInstallConfigurationExporter: 执行卸载操作");

            // 卸载逻辑（如果需要）
            AssetDatabase.DeleteAsset(AppSettingsAssetUrl);
            AssetDatabase.DeleteAsset(AppConfigureAssetUrl);

            // 调用完成回调
            onComplete?.Invoke();
        }
    }
}

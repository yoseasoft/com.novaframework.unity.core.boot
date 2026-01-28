/// -------------------------------------------------------------------------------
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

namespace NovaFramework.Editor
{
    /// <summary>
    /// 设置文件编辑工具类
    /// </summary>
    static class SettingsEditor
    {
        const string AppSettingsAssetUrl = @"Assets/Resources/AppSettings.asset";
        const string AppConfigureAssetUrl = @"Assets/Resources/AppConfigures.asset";

        /// <summary>
        /// 创建并保存‘AppSettings’资产文件
        /// </summary>
        public static void CreateAndSaveSettingAsset()
        {
            AssetDatabaseUtils.CreateScriptableObjectAsset<AppSettings>(AppSettingsAssetUrl, (asset) =>
            {
                asset.editorMode = true;
                asset.debugMode = true;
                asset.debugLevel = DebugLevelType.Debug;

                asset.applicationName = Application.productName;
                asset.applicationCode = 0;
                asset.frameRate = 20;
                asset.animationRate = 60;
                asset.designResolutionWidth = 1920;
                asset.designResolutionHeight = 1080;
            });
        }

        /// <summary>
        /// 创建并保存‘AppConfigures’资产文件
        /// </summary>
        public static void CreateAndSaveConfigureAsset()
        {
            AssetDatabaseUtils.CreateScriptableObjectAsset<AppConfigures>(AppConfigureAssetUrl, (asset) =>
            {
                asset.gameEntryName = @"Game.GameWorld";

                asset.logChannel = new LogChannelType[] { LogChannelType.Console, LogChannelType.Editor };
            });
        }
    }
}

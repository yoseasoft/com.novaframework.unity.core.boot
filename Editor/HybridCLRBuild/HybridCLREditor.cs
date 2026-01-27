/// -------------------------------------------------------------------------------
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

using System.IO;
using UnityEngine;
using UnityEditor;

using HybridCLR.Editor.Settings;
using GooAsset;

namespace NovaFramework.Editor
{
    /// <summary>
    /// HybridCLR工具类
    /// </summary>
    public static class HybridCLREditor
    {
        /// <summary>
        /// 将AOT库拷贝到资源目录
        /// </summary>
        public static void CopyAotDlls()
        {
            string fromDir = Path.Combine(HybridCLRSettings.Instance.strippedAOTDllOutputRootDir, EditorUserBuildSettings.activeBuildTarget.ToString());
            string toDir = Path.Combine(EnvironmentPath.GetPath(ResourcePathType.AotLibraryPath), Utility.Platform.CurrentPlatformName);

            if (Directory.Exists(toDir))
            {
                Directory.Delete(toDir, true);
            }
            Directory.CreateDirectory(toDir);

            foreach (string aotFileName in HybridCLRSettings.Instance.patchAOTAssemblies)
            {
                File.Copy(Path.Combine(fromDir, aotFileName), Path.Combine(toDir, $"{aotFileName}.bytes"), true);
            }

            AssetDatabase.Refresh();

            Debug.Log("Copy Finished");
        }
    }
}

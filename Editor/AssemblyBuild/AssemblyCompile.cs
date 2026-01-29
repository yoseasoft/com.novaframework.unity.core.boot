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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Player;

namespace NovaFramework.Editor
{
    public static class AssemblyCompile
    {
        /// <summary>
        /// Unity手动编译生成dll的所在目录
        /// </summary>
        const string BuildOutputDir = "Temp/Bin/Debug";

        const string LastCompileTimeLabelName = @"AssemblyLastCompileTime";

        const string DataLabelForAutoCompileStatus = @"AssemblyAutoCompileStatus";
        const string DataLabelForUniqueCompileStatus = @"AssemblyUniqueCompileStatus";

        /// <summary>
        /// Unity线程的同步上下文
        /// </summary>
        static SynchronizationContext UnitySynchronizationContext { get; set; }

        /// <summary>
        /// 最后编译时间
        /// </summary>
        internal static long LastCompileTimeTick
        {
            get => EditorConfigure.GetLong(LastCompileTimeLabelName);
            private set => EditorConfigure.SetLong(LastCompileTimeLabelName, value);
        }

        /// <summary>
        /// 是否自动编译
        /// </summary>
        public static bool IsAutoCompile
        {
            get => EditorConfigure.GetBool(DataLabelForAutoCompileStatus);
            set => EditorConfigure.SetBool(DataLabelForAutoCompileStatus, value);
        }

        /// <summary>
        /// 启用唯一程序集编译方式(给最终打包后的程序集热重载用, 打包机需要勾选)
        /// </summary>
        public static bool IsUniqueCompile
        {
            get => EditorConfigure.GetBool(DataLabelForUniqueCompileStatus);
            set => EditorConfigure.SetBool(DataLabelForUniqueCompileStatus, value);
        }

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// 运行Unity的编译
        /// </summary>
        static void RunUnityCompile()
        {
            // 运行时编译需要先设置为UnitySynchronizationContext, 编译完再还原为CurrentContext
            SynchronizationContext lastSynchronizationContext = Application.isPlaying ? SynchronizationContext.Current : null;
            SynchronizationContext.SetSynchronizationContext(UnitySynchronizationContext);

            try
            {
                Directory.CreateDirectory(BuildOutputDir);
                BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
                ScriptCompilationSettings scriptCompilationSettings = new()
                {
                    group = group,
                    target = target,
                    extraScriptingDefines = new[] { "GAME_COMPILE" },
                    options = EditorUserBuildSettings.development ? ScriptCompilationOptions.DevelopmentBuild : ScriptCompilationOptions.None
                };
                PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, BuildOutputDir);
                EditorUtility.ClearProgressBar();
            }
            finally
            {
                if (lastSynchronizationContext != null)
                {
                    SynchronizationContext.SetSynchronizationContext(lastSynchronizationContext);
                }
            }
        }

        /// <summary>
        /// 获取指定程序集的源码目录地址
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns>返回源码目录地址</returns>
        static string GetSourceCodePathByAssemblyName(string assemblyName)
        {
            return Path.Combine(EnvironmentPath.GetPath(ResourcePathType.SourceCodePath), assemblyName);
        }

        /// <summary>
        /// 获取指定程序集的最后编译时间的标签名称
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns>返回最后编译的标签名称</returns>
        static string GetLastCompileTimeLabelNameByAssemblyName(string assemblyName)
        {
            return $"{LastCompileTimeLabelName}_{assemblyName}";
        }

        /// <summary>
        /// 编译成dll
        /// </summary>
        public static void CompileDlls()
        {
            string compileTimeStr = DateTime.Now.Ticks.ToString();

            IReadOnlyList<string> hotfixDllNames = AppLibrary.GetAllReloadableAssemblyNames();

            // 将可热重载的dll改成唯一名字
            if (IsUniqueCompile)
            {
                RenameHotfixDllByTime(hotfixDllNames, compileTimeStr);
            }

            RunUnityCompile();

            CopyDll(hotfixDllNames, compileTimeStr);

            // 热重载dll的编译时间记录
            foreach (string assemblyName in hotfixDllNames)
            {
                string lastCompileLabelName = GetLastCompileTimeLabelNameByAssemblyName(assemblyName);
                long lastCompileTime = File.GetLastWriteTime(DynamicLibrary.GetBinaryLibraryFilePathByAssemblyName(assemblyName)).Ticks;

                EditorConfigure.SetLong(lastCompileLabelName, lastCompileTime);
            }

            // 获取最新的编译文件修改时间视为最后编译时间
            string libraryPath = EnvironmentPath.GetPath(ResourcePathType.LinkLibraryPath);
            string[] filePaths = Directory.GetFiles(libraryPath, "*.bytes");
            long lastWriteTimeTick = filePaths.Select(path => File.GetLastWriteTime(path).Ticks).Prepend(0).Max();
            LastCompileTimeTick = lastWriteTimeTick;

            // 运行中热重载
            if (Application.isPlaying)
            {
                AppStart.ReloadAssembliesAsync().Forget();
            }

            // 还原临时修改的dll名称
            if (IsUniqueCompile)
            {
                RevertHotfixDllName(hotfixDllNames, compileTimeStr);
            }
        }

        /// <summary>
        /// 重命名可以热重载的dll(因HybridCLR不允许加载重复名字的dll, 故改成唯一名字)
        /// </summary>
        static void RenameHotfixDllByTime(IReadOnlyList<string> assemblyNames, string compileTimeStr)
        {
            foreach (string assemblyName in assemblyNames)
            {
                RenameHotfixLibraryName(assemblyName, compileTimeStr);
            }
        }

        /// <summary>
        /// 将热重载dll命名重置
        /// </summary>
        static void RevertHotfixDllName(IReadOnlyList<string> assemblyNames, string compileTimeStr)
        {
            bool isRevert = false;

            foreach (string assemblyName in assemblyNames)
            {
                if (RevertHotfixLibraryName(assemblyName, compileTimeStr))
                {
                    isRevert = true;
                }
            }

            // 改变程序集后, 再跑一次编译, 保证文件立即编译完成, 避免进入游戏后再编译造成报错
            if (isRevert)
            {
                RunUnityCompile();

                // 删除之前临时编译出来的项目文件
                foreach (string assemblyName in assemblyNames)
                {
                    File.Delete($"{assemblyName}_{compileTimeStr}.csproj");
                }
            }
        }

        /// <summary>
        /// 复制成bytes
        /// </summary>
        static void CopyDll(IReadOnlyList<string> reloadableAssemblyNames, string compileTimeStr)
        {
            string libraryPath = EnvironmentPath.GetPath(ResourcePathType.LinkLibraryPath);
            if (!Directory.Exists(libraryPath))
            {
                Directory.CreateDirectory(libraryPath);
            }

            IReadOnlyList<string> assemblyNames = AppLibrary.GetAllAssemblyNames();

            foreach (string dllName in assemblyNames)
            {
                string dllFileName = dllName;

                if (IsUniqueCompile && reloadableAssemblyNames.Contains(dllName))
                {
                    dllFileName = $"{dllName}_{compileTimeStr}";
                }

                string dllFilePath = Path.Combine(BuildOutputDir, $"{dllFileName}.dll");
                string pdbFilePath = Path.Combine(BuildOutputDir, $"{dllFileName}.pdb");
                if (!File.Exists(dllFilePath) || !File.Exists(pdbFilePath))
                {
                    continue;
                }

                byte[] fromDllBytes = File.ReadAllBytes(dllFilePath);
                byte[] fromPdbBytes = File.ReadAllBytes(pdbFilePath);
                File.WriteAllBytes(Path.Combine(libraryPath, $"{dllName}.dll.bytes"), AppSecret.Encrypt(fromDllBytes));
                File.WriteAllBytes(Path.Combine(libraryPath, $"{dllName}.pdb.bytes"), AppSecret.Encrypt(fromPdbBytes));
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 重命名热重载的程序包
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="compileTimeStr">时间字符串</param>
        /// <returns>若重命名成功返回true，否则返回false</returns>
        static bool RenameHotfixLibraryName(string assemblyName, string compileTimeStr)
        {
            string lastCompileTimeLabelName = GetLastCompileTimeLabelNameByAssemblyName(assemblyName);
            long previousCompileTime = EditorConfigure.GetLong(lastCompileTimeLabelName);
            long lastCompileTime = File.GetLastWriteTime(DynamicLibrary.GetBinaryLibraryFilePathByAssemblyName(assemblyName)).Ticks;
            bool needCompile = previousCompileTime != lastCompileTime;

            string sourceCodePath = GetSourceCodePathByAssemblyName(assemblyName);
            if (!needCompile)
            {
                string[] filePaths = Directory.GetFiles(sourceCodePath, "*.cs", SearchOption.AllDirectories);
                long lastWriteTimeTick = filePaths.Select(path => File.GetLastWriteTime(path).Ticks).Prepend(0).Max();
                needCompile = previousCompileTime < lastWriteTimeTick;
            }

            if (needCompile)
            {
                string asmdefPath = Path.Combine(sourceCodePath, $"{assemblyName}.asmdef");
                if (File.Exists(asmdefPath))
                {
                    string json = File.ReadAllText(asmdefPath);
                    json = json.Replace($"\"name\": \"{assemblyName}\",", $"\"name\": \"{assemblyName}_{compileTimeStr}\",");
                    File.WriteAllText(asmdefPath, json);
                    AssetDatabase.ImportAsset(asmdefPath);
                    AssetDatabase.Refresh();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 重置热重载的程序包名称
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="compileTimeStr">时间字符串</param>
        /// <returns>若重置名称成功返回true，否则返回false</returns>
        static bool RevertHotfixLibraryName(string assemblyName, string compileTimeStr)
        {
            string sourceCodePath = GetSourceCodePathByAssemblyName(assemblyName);
            string asmdefFilePath = Path.Combine(sourceCodePath, $"{assemblyName}.asmdef");
            if (File.Exists(asmdefFilePath))
            {
                string json = File.ReadAllText(asmdefFilePath);
                string nameWithTime = $"\"name\": \"{assemblyName}_{compileTimeStr}\",";
                if (json.Contains(nameWithTime))
                {
                    json = json.Replace(nameWithTime, $"\"name\": \"{assemblyName}\",");
                    File.WriteAllText(asmdefFilePath, json);
                    AssetDatabase.ImportAsset(asmdefFilePath);
                    AssetDatabase.Refresh();

                    return true;
                }
            }

            return false;
        }
    }
}

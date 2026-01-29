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

using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace NovaFramework.Editor
{
    static class AssemblyEditor
    {
        /// <summary>
        /// 本地脚本编译文件的存储目录
        /// </summary>
        static string LocalScriptAssembliesPath
        {
            get
            {
                return $"{Application.dataPath}/../Library/ScriptAssemblies";
            }
        }

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            EditorApplication.playModeStateChanged += change =>
            {
                switch (change)
                {
                    case PlayModeStateChange.ExitingEditMode:
                    {
                        OnExitingEditMode();
                        break;
                    }
                    case PlayModeStateChange.ExitingPlayMode:
                    {
                        OnExitingPlayMode();
                        break;
                    }
                }
            };
        }

        /// <summary>
        /// 退出编辑模式时处理(即将进入运行模式)
        /// </summary>
        static void OnExitingEditMode()
        {
            if (!AppSettings.Instance.DylinkMode)
            {
                return;
            }

            // 若开启了自动编译, 先处理自动编译逻辑
            ProcessAutoCompile();

            // EnableDll模式时, 屏蔽掉Library的dll(通过改文件后缀方式屏蔽), 仅使用AppStart.CodeAssetPath下的dll
            DisableDynamicDlls();
        }

        /// <summary>
        /// 退出运行模式时处理(即将进入编辑模式)
        /// 还原Library里面屏蔽掉的dll(HybridCLR或者非EnableDll模式都会用到这个目录下的dll, 故需要还原)
        /// </summary>
        static void OnExitingPlayMode()
        {
            string[] filePaths = Directory.GetFiles(LocalScriptAssembliesPath, "*.DISABLE");
            foreach (string filePath in filePaths)
            {
                string fileNameWithoutDisable = Path.GetFileNameWithoutExtension(filePath);
                File.Move(filePath, Path.Combine(LocalScriptAssembliesPath, fileNameWithoutDisable));
            }
        }

        /// <summary>
        /// 处理自动编译逻辑
        /// </summary>
        static void ProcessAutoCompile()
        {
            if (!AssemblyCompile.IsAutoCompile)
            {
                return;
            }

            bool needCompile = false;

            // 先判断最后编译文件修改时间和记录的编译时间是否一致, 不一致也重新编译(因svn相关操作可能会改动)
            string libraryPath = EnvironmentPath.GetPath(ResourcePathType.LinkLibraryPath);
            string[] filePaths = Directory.GetFiles(libraryPath, "*.bytes");
            long lastWriteTimeTick = filePaths.Select(path => File.GetLastWriteTime(path).Ticks).Prepend(0).Max();
            if (AssemblyCompile.LastCompileTimeTick != lastWriteTimeTick)
            {
                needCompile = true;
            }

            if (!needCompile)
            {
                // 然后判断代码文件时间是否大于编译时间, 若代码文件更加新, 则代表需要编译
                string sourceCodePath = EnvironmentPath.GetPath(ResourcePathType.SourceCodePath);
                filePaths = Directory.GetFiles(sourceCodePath, "*.cs", SearchOption.AllDirectories);
                lastWriteTimeTick = filePaths.Select(path => File.GetLastWriteTime(path).Ticks).Prepend(0).Max();
                if (AssemblyCompile.LastCompileTimeTick > lastWriteTimeTick)
                {
                    return;
                }
            }

            // 在ExitingEditMode时编译需要先关闭游戏, 编译完成再打开游戏, 不然继续运行下去会报错, 报错原因可能是编译时SynchronizationContext切换的问题
            EditorApplication.isPlaying = false;
            AssemblyCompile.CompileDlls();
            EditorApplication.isPlaying = true;
        }

        /// <summary>
        /// 屏蔽掉Library的dll(通过改文件后缀方式屏蔽), 使用代码动态加载
        /// </summary>
        static void DisableDynamicDlls()
        {
            IReadOnlyList<string> assemblyNames = AppLibrary.GetAllAssemblyNames();
            foreach (string dll in assemblyNames)
            {
                // 因编辑器工具需要引用, 编辑器下跳过加载配置表库, 使用Unity默认加载, 故此处不进行屏蔽
                LibraryInfo info = DynamicLibrary.GetLibraryInfoByAssemblyName(dll);
                // if (dll is AppStart.AgenDllName)
                if (info.IsContainsTag(LibraryTag.Shared))
                {
                    continue;
                }

                string dllFile = Path.Combine(LocalScriptAssembliesPath, $"{dll}.dll");
                if (File.Exists(dllFile))
                {
                    string dllDisableFile = Path.Combine(LocalScriptAssembliesPath, $"{dll}.dll.DISABLE");
                    if (File.Exists(dllDisableFile))
                    {
                        File.Delete(dllDisableFile);
                    }

                    File.Move(dllFile, dllDisableFile);
                }

                string pdbFile = Path.Combine(LocalScriptAssembliesPath, $"{dll}.pdb");
                if (File.Exists(pdbFile))
                {
                    string pdbDisableFile = Path.Combine(LocalScriptAssembliesPath, $"{dll}.pdb.DISABLE");
                    if (File.Exists(pdbDisableFile))
                    {
                        File.Delete(pdbDisableFile);
                    }

                    File.Move(pdbFile, pdbDisableFile);
                }
            }
        }
    }
}

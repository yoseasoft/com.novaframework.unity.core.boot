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

#if UNITY_WEBGL && !UNITY_EDITOR
#define USE_WEBGL_RUNTIME_CONTEXT
#else
#define USE_HYBRIDCLR_RUNTIME_CONTEXT
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

using GooAsset;

namespace NovaFramework
{
    /// <summary>
    /// 应用程序启动流程管理类，用于对应用程序启动流程及脚本加载提供接口函数
    /// </summary>
    public static class AppStart
    {
        /// <summary>
        /// 配置、协议库名
        /// </summary>
        // public const string AgenDllName = "Agen";

        /// <summary>
        /// 名字对应的程序集
        /// </summary>
        static readonly Dictionary<string, Assembly> _name2Assembly = new();

        /// <summary>
        /// 主控制器实例
        /// </summary>
        private static AppController appController;

        /// <summary>
        /// 获取已加载的程序集
        /// </summary>
        public static Assembly GetLoadedAssembly(string name)
        {
            return _name2Assembly.GetValueOrDefault(name);
        }

        [RuntimeInitializeOnLoadMethod]
        static void Start()
        {
            // Main场景才运行
            if (false == Utils.IsEmptyScene() || false == Utils.IsActiveSceneName(AppDefine.AppControllerSceneName))
            {
                Logger.Error("当前运行环境必须为空场景，且场景名称需要定义为‘{0}’！", AppDefine.AppControllerSceneName);
                return;
            }

            StartAsync().Forget();
        }

        /// <summary>
        /// 运行启动流程
        /// </summary>
        static async UniTaskVoid StartAsync()
        {
            // 资源管理初始化
            await AssetManagement.InitAsync().Task;

            InitRuntimeEnvironments();

            await LoadAssembliesAsync();

            StartEngine();
        }

        /// <summary>
        /// 初始化运行时环境
        /// </summary>
        static void InitRuntimeEnvironments()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("system_environments");
            if (null == textAsset)
            {
                Logger.Error("加载系统环境配置文件失败！");
                return;
            }

            SystemEnvironmentDataWrapper systemEnvironmentDataWrapper = JsonUtility.FromJson<SystemEnvironmentDataWrapper>(textAsset.text);
            systemEnvironmentDataWrapper.AutoRegisterDatas();
            Resources.UnloadAsset(textAsset);
        }

        /// <summary>
        /// 异步加载程序集
        /// </summary>
        static async UniTask LoadAssembliesAsync()
        {
#if USE_WEBGL_RUNTIME_CONTEXT
            await UniTask.WaitForSeconds(1);
            LoadAssembliesFromCurrentDomain();
#elif USE_HYBRIDCLR_RUNTIME_CONTEXT
            await LoadMetadataAsync();

            if (Application.isEditor && !AppSettings.Instance.DylinkMode)
            {
                LoadAssembliesFromCurrentDomain();
                return;
            }

            await LoadAssembliesFromAssetsAsync();
#endif
        }

        /// <summary>
        /// 添加基础运行控制组件
        /// </summary>
        static void StartEngine()
        {
            Assembly assembly = _name2Assembly[DynamicLibrary.ExternalControlEntranceName];
            Type assemblyType = assembly?.GetType(AppDefine.AppControllerClassName);
            if (null == assemblyType)
            {
                Logger.Error($"加载Type:{AppDefine.AppControllerClassName}失败");
                return;
            }

            IReadOnlyDictionary<string, string> vars = ConfigurationLoader.LoadEnvironmentSettings();

            UnityEngine.GameObject gameObject = new UnityEngine.GameObject { name = AppDefine.AppControllerGameObjectName };
            appController = gameObject.AddComponent<AppController>();

            // 确保该脚本不会被移除
            UnityEngine.Object.DontDestroyOnLoad(appController);

            assemblyType.GetMethod("OnAssemblyLoaded", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { _name2Assembly, false });

            assemblyType.GetMethod("Start", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { appController, vars });
        }

        static void ReloadEngine()
        {
            Assembly assembly = _name2Assembly[DynamicLibrary.ExternalControlEntranceName];
            Type assemblyType = assembly?.GetType(AppDefine.AppControllerClassName);
            if (null == assemblyType)
            {
                Logger.Error($"加载Type:{AppDefine.AppControllerClassName}失败");
                return;
            }

            assemblyType.GetMethod("OnAssemblyLoaded", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { _name2Assembly, true });

            assemblyType.GetMethod("Reload", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { 1 });
        }

        public static void ReloadConfigure()
        {
            Assembly assembly = _name2Assembly[DynamicLibrary.ExternalControlEntranceName];
            Type assemblyType = assembly?.GetType(AppDefine.AppControllerClassName);
            if (null == assemblyType)
            {
                Logger.Error($"加载Type:{AppDefine.AppControllerClassName}失败");
                return;
            }

            assemblyType.GetMethod("OnAssemblyLoaded", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { _name2Assembly, true });

            assemblyType.GetMethod("Reload", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { 2 });
        }

#if USE_HYBRIDCLR_RUNTIME_CONTEXT
        /// <summary>
        /// 异步补充元数据
        /// </summary>
        static async UniTask LoadMetadataAsync()
        {
            if (Application.isEditor)
            {
                return;
            }

            IReadOnlyList<string> aotDllNames = AppLibrary.GetAllGenericAotNames();
            var dllAssets = new Asset[aotDllNames.Count];
            for (int i = 0; i < aotDllNames.Count; i++)
            {
                string filePath = EnvironmentPath.GetFilePath(ResourcePathType.AotLibraryPath, Utility.Platform.CurrentPlatformName, $"{aotDllNames[i]}.bytes");
                var asset = AssetManagement.LoadAssetAsync(filePath, typeof(TextAsset));
                dllAssets[i] = asset;
            }
            await UniTask.WaitUntil(() => { return dllAssets.All(asset => asset.IsDone); });

            foreach (var asset in dllAssets)
            {
                byte[] assBytes = (asset.result as TextAsset)?.bytes;
                HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(assBytes, HybridCLR.HomologousImageMode.SuperSet);
                asset.Release();
            }
            Logger.Info($"补元数加载完成  ======  aot dll load  ---- count:{dllAssets.Length}");
        }

        /// <summary>
        /// 从当前运行环境加载Dll
        /// </summary>
        static void LoadAssembliesFromCurrentDomain()
        {
            IReadOnlyList<string> assemblyNames = AppLibrary.GetAllAssemblyNames();
            foreach (string assemblyName in assemblyNames)
            {
                _name2Assembly.Add(assemblyName, Assembly.Load(assemblyName));
            }
        }

        /// <summary>
        /// 从资源中加载Dll
        /// </summary>
        static async UniTask LoadAssembliesFromAssetsAsync()
        {
            Dictionary<string, Asset> name2DllAssets = new();
            IReadOnlyList<string> assemblyNames = AppLibrary.GetAllAssemblyNames();
            string libraryPath = EnvironmentPath.GetPath(ResourcePathType.LinkLibraryPath);
            foreach (string dllName in assemblyNames)
            {
                string fileName = $"{dllName}.dll";
                Asset asset = AssetManagement.LoadAssetAsync($"{libraryPath}/{fileName}.bytes", typeof(TextAsset));
                name2DllAssets.Add(fileName, asset);

                fileName = $"{dllName}.pdb";
                asset = AssetManagement.LoadAssetAsync($"{libraryPath}/{fileName}.bytes", typeof(TextAsset));
                name2DllAssets.Add(fileName, asset);
            }

            Asset[] dllAssets = name2DllAssets.Values.ToArray();
            await UniTask.WaitUntil(() => { return dllAssets.All(asset => asset.IsDone); });

            foreach (string dllName in assemblyNames)
            {
#if UNITY_EDITOR
                // 因编辑器工具需要引用, 编辑器下跳过加载配置表库, 使用Unity默认加载
                LibraryInfo info = DynamicLibrary.GetLibraryInfoByAssemblyName(dllName);
                // if (dllName == AgenDllName)
                if (info.IsContainsTag(LibraryTag.Shared))
                {
                    _name2Assembly.Add(dllName, Assembly.Load(dllName));
                    continue;
                }
#endif

                byte[] assBytes = AppSecret.Decrypt((name2DllAssets[$"{dllName}.dll"].result as TextAsset).bytes);
                byte[] pdbBytes = AppSecret.Decrypt((name2DllAssets[$"{dllName}.pdb"].result as TextAsset).bytes);
                _name2Assembly.Add(dllName, Assembly.Load(assBytes, pdbBytes));
            }

            foreach (Asset asset in dllAssets)
            {
                asset.Release();
            }
        }
#endif

        /// <summary>
        /// 重载所有程序集并进行通知
        /// </summary>
        public async static UniTaskVoid ReloadAssembliesAsync()
        {
            await ReloadAssembliesFromAssetsAsync();

            ReloadEngine();
        }

        /// <summary>
        /// 重载程序集
        /// </summary>
        public async static UniTask ReloadAssembliesFromAssetsAsync()
        {
            IReadOnlyList<string> assemblyNames = AppLibrary.GetAllReloadableAssemblyNames();
            string libraryPath = EnvironmentPath.GetPath(ResourcePathType.LinkLibraryPath);

            for (int n = 0; n < assemblyNames.Count; ++n)
            {
                string assemblyName = assemblyNames[n];

                Asset dllAsset = AssetManagement.LoadAssetAsync($"{libraryPath}/{assemblyName}.dll.bytes", typeof(TextAsset));
                Asset pdbAsset = AssetManagement.LoadAssetAsync($"{libraryPath}/{assemblyName}.pdb.bytes", typeof(TextAsset));
                await dllAsset;
                await pdbAsset;
                byte[] dllBytes = AppSecret.Decrypt((dllAsset.result as TextAsset).bytes);
                byte[] pdbBytes = AppSecret.Decrypt((pdbAsset.result as TextAsset).bytes);
                _name2Assembly[assemblyName] = Assembly.Load(dllBytes, pdbBytes);
            }
        }

#if USE_WEBGL_RUNTIME_CONTEXT
        /// <summary>
        /// 从当前运行环境加载Dll
        /// </summary>
        static void LoadAssembliesFromCurrentDomain()
        {
            // WebGL中，程序集已经编译到WebAssembly中
            // 只需要从当前域获取已加载的程序集
            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // 手动添加需要的程序集
            foreach (Assembly assembly in allAssemblies)
            {
                string simpleName = assembly.GetName().Name;
                if (!_name2Assembly.ContainsKey(simpleName))
                {
                    _name2Assembly.Add(simpleName, assembly);
                }
            }
        }
#endif
    }
}

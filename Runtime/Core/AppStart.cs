/// -------------------------------------------------------------------------------
/// CoreEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using GooAsset;

using SystemType = System.Type;

namespace CoreEngine
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
            if (false == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToLower().Equals(AppDefine.AppControllerSceneName))
            {
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

            await LoadAssembliesAsync();

            StartEngine();
        }

        /// <summary>
        /// 异步加载程序集
        /// </summary>
        static async UniTask LoadAssembliesAsync()
        {
            await LoadMetadataAsync();

            if (Application.isEditor && !AppSettings.Instance.dylinkMode)
            {
                LoadAssembliesFromCurrentDomain();
                return;
            }

            await LoadAssembliesFromAssetsAsync();
        }

        /// <summary>
        /// 添加基础运行控制组件
        /// </summary>
        static void StartEngine()
        {
            Assembly assembly = _name2Assembly[DynamicLibrary.ExternalControlEntranceName];
            SystemType assemblyType = assembly?.GetType(AppDefine.AppControllerClassName);
            if (assemblyType is null)
            {
                Debug.LogError($"加载Type:{AppDefine.AppControllerClassName}失败");
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
            SystemType assemblyType = assembly?.GetType(AppDefine.AppControllerClassName);
            if (assemblyType is null)
            {
                Debug.LogError($"加载Type:{AppDefine.AppControllerClassName}失败");
                return;
            }

            assemblyType.GetMethod("OnAssemblyLoaded", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { _name2Assembly, true });

            assemblyType.GetMethod("Reload", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { 1 });
        }

        public static void ReloadConfigure()
        {
            Assembly assembly = _name2Assembly[DynamicLibrary.ExternalControlEntranceName];
            SystemType assemblyType = assembly?.GetType(AppDefine.AppControllerClassName);
            if (assemblyType is null)
            {
                Debug.LogError($"加载Type:{AppDefine.AppControllerClassName}失败");
                return;
            }

            assemblyType.GetMethod("OnAssemblyLoaded", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { _name2Assembly, true });

            assemblyType.GetMethod("Reload", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, new object[] { 2 });
        }

        /// <summary>
        /// 异步补充元数据
        /// </summary>
        static async UniTask LoadMetadataAsync()
        {
            if (Application.isEditor)
            {
                return;
            }

            IList<string> aotDllNames = AppLibrary.GetAllGenericAotNames();
            var dllAssets = new Asset[aotDllNames.Count];
            for (int i = 0; i < aotDllNames.Count; i++)
            {
                string filePath = SystemPath.GetFilePath(ResourcePathType.MonoAotPath, Utility.Platform.CurrentPlatformName, $"{aotDllNames[i]}.bytes");
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
            Debug.Log($"补元数加载完成  ======  aot dll load  ---- count:{dllAssets.Length}");
        }

        /// <summary>
        /// 从当前运行环境加载Dll
        /// </summary>
        static void LoadAssembliesFromCurrentDomain()
        {
            IList<string> assemblyNames = AppLibrary.GetAllAssemblyNames();
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
            IList<string> assemblyNames = AppLibrary.GetAllAssemblyNames();
            string libraryPath = SystemPath.GetPath(ResourcePathType.LinkLibraryPath);
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
                CoreEngine.LibraryInfo info = CoreEngine.DynamicLibrary.GetLibraryInfoByAssemblyName(dllName);
                // if (dllName == AgenDllName)
                if (info.IsContainsTag(CoreEngine.LibraryTag.Shared))
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
            IList<string> assemblyNames = AppLibrary.GetAllReloadableAssemblyNames();
            string libraryPath = SystemPath.GetPath(ResourcePathType.LinkLibraryPath);

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
    }
}

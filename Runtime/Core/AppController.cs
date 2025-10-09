/// -------------------------------------------------------------------------------
/// CoreEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace CoreEngine
{
    /// <summary>
    /// 生命周期总入口
    /// </summary>
    [UnityEngine.DisallowMultipleComponent]
    public class AppController : UnityEngine.MonoBehaviour
    {
        private System.Action onStart;
        private System.Action onUpdate;
        private System.Action onLateUpdate;
        private System.Action onDestroy;
        private System.Action<bool> onApplicationFocus;
        private System.Action<bool> onApplicationPause;
        private System.Action onApplicationQuit;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            onStart?.Invoke();
        }

        void Update()
        {
            onUpdate?.Invoke();
        }

        void LateUpdate()
        {
            onLateUpdate?.Invoke();
        }

        void OnDestroy()
        {
            onDestroy?.Invoke();
        }

        void OnApplicationFocus(bool focus)
        {
            onApplicationFocus?.Invoke(focus);
        }

        void OnApplicationPause(bool pause)
        {
            onApplicationPause?.Invoke(pause);
        }

        void OnApplicationQuit()
        {
            onApplicationQuit?.Invoke();
        }
    }
}

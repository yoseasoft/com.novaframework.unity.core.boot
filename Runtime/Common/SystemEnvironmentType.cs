/// -------------------------------------------------------------------------------
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

using System;
using System.Collections.Generic;
using System.Text;

namespace NovaFramework
{
    /// <summary>
    /// 系统环境数据模型容器类
    /// </summary>
    [Serializable]
    internal sealed class SystemEnvironmentDataWrapper
    {
        public List<Serialization.SerializedVariableObject> variables = new ();
        public List<Serialization.SerializedLibraryObject> modules = new ();
        public List<string> aot_libraries = new ();

        public void AutoRegisterDatas()
        {
            Dictionary<string, string> vars = new Dictionary<string, string>();
            foreach (Serialization.SerializedVariableObject variableObject in variables)
            {
                vars.Add(variableObject.key, variableObject.value);
            }
            EnvironmentVariables.Instance.SetValue(vars);

            for (int n = 0; n < modules.Count; ++n)
            {
                Serialization.SerializedLibraryObject module = modules[n];
                DynamicLibrary.Instance.RegisterLibraryInfo(module.order, module.name, module.tags);
            }

            for (int n = 0; n < aot_libraries.Count; ++n)
            {
                DynamicLibrary.Instance.RegisterAotLibraryName(aot_libraries[n]);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Variables={");
            for (int n = 0; n < variables.Count; ++n) sb.AppendFormat("{0},", variables[n].ToString());
            sb.Append("},");
            sb.Append("Modules={");
            for (int n = 0; n < modules.Count; ++n) sb.AppendFormat("{0},", modules[n].ToString());
            sb.Append("},");
            sb.Append("AotLibraries={");
            for (int n = 0; n < aot_libraries.Count; ++n) sb.AppendFormat("{0},", aot_libraries[n].ToString());
            sb.Append("}");
            return sb.ToString();
        }
    }
}

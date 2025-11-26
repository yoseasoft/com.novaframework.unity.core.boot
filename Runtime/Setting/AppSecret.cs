/// -------------------------------------------------------------------------------
/// CoreEngine Framework
///
/// Copyright (C) 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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

using GooAsset;

namespace CoreEngine
{
    /// <summary>
    /// 程序加密/解密逻辑接口类，用于对程序内部数据进行加密/解密处理
    /// </summary>
    public static class AppSecret
    {
        const string Uqs3 = "TmKYR6VVLBjsdfXh4fEpRc9yG6Z73sqU";
        const string Dnw4 = "Nzcfvd2Nc0Tx4Wnd";

        /// <summary>
        /// AES encrypt
        /// </summary>
        /// <param name="data">Raw data</param>
        /// <returns>Encrypted byte array</returns>
        public static byte[] Encrypt(byte[] data)
        {
            return Utility.Cryptography.Encrypt(data, Uqs3, Dnw4);
        }

        /// <summary>
        ///  AES decrypt
        /// </summary>
        /// <param name="data">Encrypted data</param>
        /// <returns>Decrypted byte array</returns>
        public static byte[] Decrypt(byte[] data)
        {
            return Utility.Cryptography.Decrypt(data, Uqs3, Dnw4);
        }
    }
}

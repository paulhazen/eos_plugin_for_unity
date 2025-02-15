﻿/*
 * Copyright (c) 2024 PlayEveryWare
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#if !EOS_DISABLE

namespace PlayEveryWare.EpicOnlineServices
{
    // Flags specifically for Android. Note that labels for the baser
    // PlatformConfig need to be specified here.
    [ConfigGroup("Android Config", new[]
    {
     "Android-Specific Options",
     "Deployment",
     "Flags",
     "Tick Budgets",
     "Overlay Options"
     }, false)]
    public class AndroidConfig : PlatformConfig
    {
        [ConfigField("Google Login Client ID",
            ConfigFieldType.Text,
            "Get your project's Google Login Client ID from the " +
            "Google Cloud dashboard",
            0,
            "https://console.cloud.google.com/apis/dashboard")]
        public string GoogleLoginClientID;

        [ConfigField("Google Login Nonce",
            ConfigFieldType.Text,
            "Use a nonce to improve sign-ing security on Android.",
            0,
            "https://developer.android.com/google/play/integrity/classic#nonce")]
        public string GoogleLoginNonce;

        static AndroidConfig()
        {
            RegisterFactory(() => new AndroidConfig());
        }

        protected AndroidConfig() : base(PlatformManager.Platform.Android) { }
    }
}

#endif
/*
* Copyright (c) 2024 PlayEveryWare
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/
namespace PlayEveryWare.EpicOnlineServices.Editor
{
    using Settings;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Contains implementations of IConfigEditor that are common to all ConfigEditors that represent the configuration options for a specific platform.
    /// </summary>
    /// <typeparam name="T">Intended to be a type accepted by the templated class ConfigHandler.</typeparam>
    public abstract class PlatformConfigEditor<T> : ConfigEditor<T> where T : PlatformConfig, new()
    {
        /// <summary>
        /// The platform that this PlatformConfigEditor represents.
        /// </summary>
        protected PlatformManager.Platform Platform;

        protected PlatformConfigEditor(PlatformManager.Platform platform) :
            base(PlatformManager.GetFullName(platform), PlatformManager.GetConfigFilePath(platform))
        {
            this.Platform = platform;
        }

        /// <summary>
        /// Given that most platform configurations allow for override values of a specific subset of the standard
        /// options applied to all platforms, the rendering of these options is shared by all PlatformConfigEditor implementations.
        ///
        /// TODO: Consider the scenario where there are values that need to be overriden for one platform, but not for another. How would this work?
        /// NOTE: Currently, all platforms override the same set of values, so this is not currently an issue.
        /// </summary>
        public virtual void RenderOverrides()
        {
            GUILayout.Label($"{PlatformManager.GetFullName(Platform)} Override Configuration Values",
                EditorStyles.boldLabel);

            GUIEditorHelper.AssigningFlagTextField("Override Platform Flags (Separated by '|')", ref ConfigHandler.Data.overrideValues.platformOptionsFlags, 250);

            GUIEditorHelper.AssigningFloatToStringField("Override initial button delay for overlay", ref ConfigHandler.Data.overrideValues.initialButtonDelayForOverlay, 250);

            GUIEditorHelper.AssigningFloatToStringField("Override repeat button delay for overlay", ref ConfigHandler.Data.overrideValues.repeatButtonDelayForOverlay, 250);

            GUIEditorHelper.AssigningULongToStringField("Thread Affinity: networkWork", ref ConfigHandler.Data.overrideValues.ThreadAffinity_networkWork);
            GUIEditorHelper.AssigningULongToStringField("Thread Affinity: storageIO", ref ConfigHandler.Data.overrideValues.ThreadAffinity_storageIO);
            GUIEditorHelper.AssigningULongToStringField("Thread Affinity: webSocketIO", ref ConfigHandler.Data.overrideValues.ThreadAffinity_webSocketIO);
            GUIEditorHelper.AssigningULongToStringField("Thread Affinity: P2PIO", ref ConfigHandler.Data.overrideValues.ThreadAffinity_P2PIO);
            GUIEditorHelper.AssigningULongToStringField("Thread Affinity: HTTPRequestIO", ref ConfigHandler.Data.overrideValues.ThreadAffinity_HTTPRequestIO);
            GUIEditorHelper.AssigningULongToStringField("Thread Affinity: RTCIO", ref ConfigHandler.Data.overrideValues.ThreadAffinity_RTCIO);

        }

        public override sealed void RenderContents()
        {
            RenderOverrides();
        }
    }
}
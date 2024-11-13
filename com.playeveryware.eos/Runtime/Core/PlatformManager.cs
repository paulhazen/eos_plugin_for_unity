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

namespace PlayEveryWare.EpicOnlineServices
{
    using System.Collections.Generic;
    using System;
    using System.Reflection;

#if UNITY_EDITOR
    using UnityEditor;
#endif

#if !EXTERNAL_TO_UNITY
    using UnityEngine;
#endif
    using Utility;

    public static class PlatformManager
    {
        /// <summary>
        /// Enum that stores the possible platforms
        /// </summary>
        [Flags]
        public enum Platform
        {
            Unknown = 0x0,
            Windows = 0x1,
            Android = 0x2,
            XboxOne = 0x4,
            XboxSeriesX = 0x8,
            iOS = 0x10,
            Linux = 0x20,
            macOS = 0x40,
            PS4 = 0x80,
            PS5 = 0x100,
            Switch = 0x200,
            Steam = 0x400,
            Console = PS4 | PS5 | XboxOne | XboxSeriesX | Switch,
            Any = Unknown | Windows | Android | XboxOne | XboxSeriesX | iOS | Linux | macOS | PS4 | PS5 | Switch | Steam
        }

        private struct PlatformInfo
        {
            public string FullName;
            public string ConfigFileName;
            public Type ConfigType;
            public string DynamicLibraryExtension;
            public string PlatformIconLabel;
            public Func<PlatformConfig> GetConfigFunc;
        }

        /// <summary>
        /// Private collection to store associations between a platform type, it's human readable name, the file in which the configuration is stored, and the type of the config object
        /// </summary>
        private static IDictionary<Platform, PlatformInfo> PlatformInformation =
            new Dictionary<Platform, PlatformInfo>();

        /// <summary>
        /// Backing value for the CurrentPlatform property.
        /// </summary>
        private static Platform s_CurrentPlatform;

        /// <summary>
        /// Returns the current platform. In-order to reduce the number of places in the build pipeline
        /// where build defines are if-branched, this value is designed to be set from outside of the
        /// static class "PlatformManager."
        /// </summary>
        public static Platform CurrentPlatform
        {
            get { return s_CurrentPlatform; }
            set
            {
                // This is to ensure that the platform is only ever determined once
                if (CurrentPlatform == Platform.Unknown)
                {
                    s_CurrentPlatform = value;
                    Debug.Log($"CurrentPlatform has been assigned as {GetFullName(s_CurrentPlatform)}.");
                }
                else
                {
                    Debug.Log($"CurrentPlatform has already been assigned as {GetFullName(s_CurrentPlatform)}.");
                }

            }
        }

        static PlatformManager()
        {
            // This compile conditional is here because if we are external to
            // the Unity Engine, then we only need to use WindowsConfig.
#if !EXTERNAL_TO_UNITY
            AddPlatformInfo(Platform.Android,     "Android",       "eos_android_config.json", typeof(AndroidConfig), "Android");
            AddPlatformInfo(Platform.iOS,         "iOS",           "eos_ios_config.json",     typeof(IOSConfig),     "iPhone");
            AddPlatformInfo(Platform.Linux,       "Linux",         "eos_linux_config.json",   typeof(LinuxConfig),   "Standalone");
            AddPlatformInfo(Platform.macOS,       "macOS",         "eos_macos_config.json",   typeof(MacOSConfig),   "Standalone");
            AddPlatformInfo(Platform.Steam,       "Steam",         "eos_steam_config.json",   null,         "Standalone");
            AddPlatformInfo(Platform.XboxOne,     "Xbox One",      "eos_xb1_config.json",     null, "XboxOne");
            AddPlatformInfo(Platform.XboxSeriesX, "Xbox Series X", "eos_xsx_config.json",     null, "XboxOne");
            AddPlatformInfo(Platform.PS4,         "PS4",           "eos_ps4_config.json",     null, "PS4");
            AddPlatformInfo(Platform.PS5,         "PS5",           "eos_ps5_config.json",     null, "PS5");
            AddPlatformInfo(Platform.Switch,      "Switch",        "eos_switch_config.json",  null, "Switch");
#endif
            AddPlatformInfo(Platform.Windows,     "Windows", "eos_windows_config.json", typeof(WindowsConfig), "Standalone");

            // If external to unity, then we know that the current platform
            // is Windows.
#if EXTERNAL_TO_UNITY
            CurrentPlatform = Platform.Windows;
#else
            // If the Unity Editor is currently running, then the "active"
            // Platform is whatever the current build target is.
#if UNITY_EDITOR
            if (TryGetPlatform(EditorUserBuildSettings.activeBuildTarget, out Platform platform))
#else
            // If the Unity editor is _not_ currently running, then the "active"
            // platform is whatever the runtime application says it is
            if (TryGetPlatform(Application.platform, out Platform platform))
#endif
            {
                CurrentPlatform = platform;
            }
            else
            {
                CurrentPlatform = Platform.Unknown;
                Debug.LogWarning("Platform could not be determined.");
            }
#endif
        }

        public static PlatformConfig GetPlatformConfig()
        {
            PlatformConfig config = null;

            // TODO: Confirm that usage of Reflection here is acceptable.
            MethodInfo methodInfo = typeof(Config).GetMethod("Get");
            MethodInfo genericMethod = methodInfo?.MakeGenericMethod(GetConfigType());

            if (genericMethod != null)
            {
                config = (PlatformConfig)genericMethod.Invoke(null, null);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Could not retrieve platform-specific " +
                    $"configuration values for platform \"{PlatformManager.CurrentPlatform}\".");
            }

            return config;
        }

        public static void SetPlatformDetails(Platform platform, Type configType, string dynamicLibraryExtension)
        {
            PlatformInfo info = PlatformInformation[platform];
            info.ConfigType = configType;
            info.DynamicLibraryExtension = dynamicLibraryExtension;

            PlatformInformation.Remove(platform);
            PlatformInformation.Add(new KeyValuePair<Platform, PlatformInfo>(platform, info));
        }

        public static void AddPlatformInfo(Platform platform, string fullName, string configFileName, Type configType, string iconLabel)
        {
            PlatformInformation.Add(new KeyValuePair<Platform, PlatformInfo>(platform,
                new PlatformInfo()
                {
                    FullName = fullName,
                    ConfigFileName = configFileName,
                    PlatformIconLabel = iconLabel,
                    ConfigType = configType,
                }));
        }

#if UNITY_EDITOR
        /// <summary>
        /// Maps Unity BuildTarget to Platform
        /// </summary>
        private static readonly IDictionary<BuildTarget, Platform> TargetToPlatformsMap =
            new Dictionary<BuildTarget, Platform>()
            {
                { BuildTarget.Android,             Platform.Android     },
                { BuildTarget.GameCoreXboxOne,     Platform.XboxOne     },
                { BuildTarget.GameCoreXboxSeries,  Platform.XboxSeriesX },
                { BuildTarget.iOS,                 Platform.iOS         },
                { BuildTarget.StandaloneLinux64,   Platform.Linux       },
                { BuildTarget.PS4,                 Platform.PS4         },
                { BuildTarget.PS5,                 Platform.PS5         },
                { BuildTarget.Switch,              Platform.Switch      },
                { BuildTarget.StandaloneWindows,   Platform.Windows     },
                { BuildTarget.StandaloneWindows64, Platform.Windows     },
            };

        /// <summary>
        /// Maps Unity RuntimePlatform to Platform
        /// </summary>
        private static readonly IDictionary<RuntimePlatform, Platform> RuntimeToPlatformsMap =
            new Dictionary<RuntimePlatform, Platform>()
            {
                { RuntimePlatform.Android,            Platform.Android},
                { RuntimePlatform.IPhonePlayer,       Platform.iOS},
                { RuntimePlatform.PS4,                Platform.PS4},
                { RuntimePlatform.PS5,                Platform.PS5},
                { RuntimePlatform.GameCoreXboxOne,    Platform.XboxOne},
                { RuntimePlatform.XboxOne,            Platform.XboxOne},
                { RuntimePlatform.Switch,             Platform.Switch},
                { RuntimePlatform.GameCoreXboxSeries, Platform.XboxSeriesX},
                { RuntimePlatform.LinuxPlayer,        Platform.Linux},
                { RuntimePlatform.LinuxEditor,        Platform.Linux},
                { RuntimePlatform.EmbeddedLinuxX64,   Platform.Linux},
                { RuntimePlatform.EmbeddedLinuxX86,   Platform.Linux},
                { RuntimePlatform.LinuxServer,        Platform.Linux},
                { RuntimePlatform.WindowsServer,      Platform.Windows},
                { RuntimePlatform.WindowsPlayer,      Platform.Windows},
                { RuntimePlatform.WindowsEditor,      Platform.Windows},
                { RuntimePlatform.OSXEditor,          Platform.macOS},
                { RuntimePlatform.OSXPlayer,          Platform.macOS},
                { RuntimePlatform.OSXServer,          Platform.macOS},
            };

        /// <summary>
        /// Returns an IEnumerable of the platforms for which the current Unity
        /// Editor is able to build projects for.
        /// </summary>
        /// <returns>
        /// IEnumerable of the platforms for which the current Unity Editor is
        /// able to build projects for.
        /// </returns>
        public static IEnumerable<Platform> GetAvailableBuildTargets()
        {
            foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
            {
                var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);

                if (!BuildPipeline.IsBuildTargetSupported(buildTargetGroup, target))
                    continue;

                if (!TargetToPlatformsMap.TryGetValue(target, out Platform supportedPlatform))
                    continue;

                yield return supportedPlatform;
            }
        }

        /// <summary>
        /// Get the platform that matches the given build target.
        /// </summary>
        /// <param name="target">The build target being built for.</param>
        /// <param name="platform">The platform for that build target.</param>
        /// <returns>True if platform was determined, false otherwise.</returns>
        public static bool TryGetPlatform(BuildTarget target, out Platform platform)
        {
            return TargetToPlatformsMap.TryGetValue(target, out platform);
        }

        /// <summary>
        /// Get the platform that matches the given runtime platform.
        /// </summary>
        /// <param name="runtimePlatform">The active RuntimePlatform</param>
        /// <param name="platform">The platform for that RuntimePlatform.</param>
        /// <returns>True if platform was determined, false otherwise.</returns>
        public static bool TryGetPlatform(RuntimePlatform runtimePlatform, out Platform platform)
        {
            return RuntimeToPlatformsMap.TryGetValue(runtimePlatform, out platform);
        }

        /// <summary>
        /// Get the config type for the current platform.
        /// </summary>
        /// <returns>The config type for the current platform.</returns>
        public static Type GetConfigType()
        {
            return GetConfigType(PlatformManager.CurrentPlatform);
        }

        /// <summary>
        /// Returns the type of the PlatformConfig that holds configuration values for the indicated Platform.
        /// </summary>
        /// <param name="platform">The Platform to get the specific PlatformConfig type of.</param>
        /// <returns>Type of the specific PlatformConfig that represents the indicated Platform.</returns>
        public static Type GetConfigType(Platform platform)
        {
            return PlatformInformation[platform].ConfigType;
        }

        public static bool TryGetConfigType(Platform platform, out Type configType)
        {
            configType = null;

            bool typeFound = PlatformInformation.TryGetValue(platform, out PlatformInfo value);

            if (typeFound)
            {
                configType = value.ConfigType;
            }

            return typeFound;
        }

        /// <summary>
        /// Returns the type of the PlatformConfig that holds configuration values for the indicated BuildTarget
        /// </summary>
        /// <param name="target">The BuildTarget to get the specific PlatformConfig type of.</param>
        /// <returns>Type of the specific PlatformConfig that represents the indicated Platform.</returns>
        public static Type GetConfigType(BuildTarget target)
        {
            Platform platform = TargetToPlatformsMap[target];
            return GetConfigType(platform);
        }

        /// <summary>
        /// Return the fully qualified path to the configuration file for the given build target.
        /// </summary>
        /// <param name="target">The build target to get the configuration file for.</param>
        /// <returns>Fully qualified path.</returns>
        private static string GetConfigFilePath(BuildTarget target)
        {
            var platform = TargetToPlatformsMap[target];
            return GetConfigFilePath(platform);
        }

        /// <summary>
        /// Try to retrieve the config file path for the indicated BuildTarget.
        /// </summary>
        /// <param name="target">The BuildTarget to get the configuration file for.</param>
        /// <param name="configFilePath">The filepath to the configuration file.</param>
        /// <returns>True if there is a config file path for the indicated BuildTarget.</returns>
        public static bool TryGetConfigFilePath(BuildTarget target, out string configFilePath)
        {
            var platform = TargetToPlatformsMap[target];
            return TryGetConfigFilePath(platform, out configFilePath);
        }

        /// <summary>
        /// Return the built-in icon for the indicated platform.
        /// </summary>
        /// <param name="platform">The platform to get the icon for.</param>
        /// <returns>An icon texture representing the platform.</returns>
        public static Texture GetPlatformIcon(Platform platform)
        {
            return EditorGUIUtility.IconContent($"BuildSettings.{PlatformInformation[platform].PlatformIconLabel}.Small").image;
        }
#endif

        /// <summary>
        /// Return a string that represents the file extension used by the indicated platform for dynamic library files.
        /// </summary>
        /// <param name="platform">The platform to get the specific file extension for.</param>
        /// <returns>File extension for the dynamic library used by the indicated platform.</returns>
        public static string GetDynamicLibraryExtension(Platform platform)
        {
            return PlatformInformation[platform].DynamicLibraryExtension;
        }


        /// <summary>
        /// Get the file extension used by the current platform for dynamic library files.
        /// </summary>
        /// <returns>A string containing the file extension used for dynamic library files on the current platform.</returns>
        public static string GetDynamicLibraryExtension()
        {
            return GetDynamicLibraryExtension(CurrentPlatform);
        }

        /// <summary>
        /// Return the fully qualified path to the configuration file for the current platform.
        /// </summary>
        /// <returns>Fully qualified path.</returns>
        public static string GetConfigFilePath()
        {
            return GetConfigFilePath(CurrentPlatform);
        }

        /// <summary>
        /// Return the fully qualified path to the configuration file for the given platform.
        /// </summary>
        /// <param name="platform">The platform to get the configuration file for.</param>
        /// <returns>Fully qualified path.</returns>
        public static string GetConfigFilePath(Platform platform)
        {
            return FileSystemUtility.CombinePaths(
                Application.streamingAssetsPath,
                "EOS",
                GetConfigFileName(platform)
                );
        }

        /// <summary>
        /// Try to retrieve the config file path for the indicated platform.
        /// </summary>
        /// <param name="platform">The platform to get the configuration file for.</param>
        /// <param name="configFilePath">The filepath to the configuration file.</param>
        /// <returns>True if there is a config file path for the indicated platform.</returns>
        public static bool TryGetConfigFilePath(Platform platform, out string configFilePath)
        {
            if (PlatformInformation.ContainsKey(platform))
            {
                configFilePath = GetConfigFilePath(platform);
                return true;
            }

            configFilePath = "";
            return false;
        }

        /// <summary>
        /// Returns the name of the JSON file that contains configuration values for the given platform.
        /// </summary>
        /// <param name="platform">The platform to get the JSON file name of.</param>
        /// <returns>The JSON file that contains configuration values for the given platform.</returns>
        public static string GetConfigFileName(Platform platform)
        {
            return PlatformInformation[platform].ConfigFileName;
        }

        /// <summary>
        /// Get the name for the indicated platform.
        /// </summary>
        /// <param name="platform">The platform to get the full name for.</param>
        /// <returns>Full name of platform.</returns>
        public static string GetFullName(Platform platform)
        {
            return PlatformInformation[platform].FullName;
        }
    }
}
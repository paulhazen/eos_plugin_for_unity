/*
 * Copyright (c) 2021 PlayEveryWare
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

#pragma once
#include <filesystem>
#include <map>
#include <vector>

#include "json.h"
#include <optional>

struct json_value_s;

namespace playeveryware::eos::config
{
    /**
     * \brief
     * Maps string values to values defined by the EOS SDK regarding platform
     * creation.
     */
    static const std::map<std::string, int> PLATFORM_CREATION_FLAGS_STRINGS_TO_ENUM = {
        {"EOS_PF_LOADING_IN_EDITOR",                          EOS_PF_LOADING_IN_EDITOR},
        {"LoadingInEditor",                                   EOS_PF_LOADING_IN_EDITOR},

        {"EOS_PF_DISABLE_OVERLAY",                            EOS_PF_DISABLE_OVERLAY},
        {"DisableOverlay",                                    EOS_PF_DISABLE_OVERLAY},

        {"EOS_PF_DISABLE_SOCIAL_OVERLAY",                     EOS_PF_DISABLE_SOCIAL_OVERLAY},
        {"DisableSocialOverlay",                              EOS_PF_DISABLE_SOCIAL_OVERLAY},

        {"EOS_PF_WINDOWS_ENABLE_OVERLAY_D3D9",                EOS_PF_WINDOWS_ENABLE_OVERLAY_D3D9},
        {"WindowsEnableOverlayD3D9",                          EOS_PF_WINDOWS_ENABLE_OVERLAY_D3D9},

        {"EOS_PF_WINDOWS_ENABLE_OVERLAY_D3D10",               EOS_PF_WINDOWS_ENABLE_OVERLAY_D3D10},
        {"WindowsEnableOverlayD3D10",                         EOS_PF_WINDOWS_ENABLE_OVERLAY_D3D10},

        {"EOS_PF_WINDOWS_ENABLE_OVERLAY_OPENGL",              EOS_PF_WINDOWS_ENABLE_OVERLAY_OPENGL},
        {"WindowsEnableOverlayOpengl",                        EOS_PF_WINDOWS_ENABLE_OVERLAY_OPENGL},

        {"EOS_PF_CONSOLE_ENABLE_OVERLAY_AUTOMATIC_UNLOADING", EOS_PF_CONSOLE_ENABLE_OVERLAY_AUTOMATIC_UNLOADING},
        {"ConsoleEnableOverlayAutomaticUnloading",            EOS_PF_CONSOLE_ENABLE_OVERLAY_AUTOMATIC_UNLOADING},

        {"EOS_PF_RESERVED1",                                  EOS_PF_RESERVED1},
        {"Reserved1",                                         EOS_PF_RESERVED1}
    };

    /**
     * \brief Maps string values to values within the
     * EOS_EIntegratedPlatformManagementFlags enum.
     */
    static const std::map<std::string, EOS_EIntegratedPlatformManagementFlags> INTEGRATED_PLATFORM_MANAGEMENT_FLAGS_STRINGS_TO_ENUM = {
        {"EOS_IPMF_Disabled",                        EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_Disabled },
        {"Disabled",                                 EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_Disabled },

        {"EOS_IPMF_LibraryManagedByApplication",     EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedByApplication },
        {"EOS_IPMF_ManagedByApplication",            EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedByApplication},
        {"ManagedByApplication",                     EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedByApplication},
        {"LibraryManagedByApplication",              EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedByApplication},

        {"ManagedBySDK",                             EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedBySDK },
        {"EOS_IPMF_ManagedBySDK",                    EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedBySDK },
        {"EOS_IPMF_LibraryManagedBySDK",             EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedBySDK },
        {"LibraryManagedBySDK",                      EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedBySDK },

        {"DisableSharedPresence",                    EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_DisablePresenceMirroring },
        {"EOS_IPMF_DisableSharedPresence",           EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_DisablePresenceMirroring },
        {"EOS_IPMF_DisablePresenceMirroring",        EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_DisablePresenceMirroring },
        {"DisablePresenceMirroring",                 EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_DisablePresenceMirroring},

        {"DisableSessions",                          EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_DisableSDKManagedSessions },
        {"EOS_IPMF_DisableSessions",                 EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_DisableSDKManagedSessions },
        {"EOS_IPMF_DisableSDKManagedSessions",       EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_DisableSDKManagedSessions },
        {"DisableSDKManagedSessions",                EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_DisableSDKManagedSessions },

        {"PreferEOS",                                EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_PreferEOSIdentity },
        {"EOS_IPMF_PreferEOS",                       EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_PreferEOSIdentity },
        {"EOS_IPMF_PreferEOSIdentity",               EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_PreferEOSIdentity },
        {"PreferEOSIdentity",                        EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_PreferEOSIdentity},

        {"PreferIntegrated",                         EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_PreferIntegratedIdentity },
        {"EOS_IPMF_PreferIntegrated",                EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_PreferIntegratedIdentity },
        {"EOS_IPMF_PreferIntegratedIdentity",        EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_PreferIntegratedIdentity },
        {"PreferIntegratedIdentity",                 EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_PreferIntegratedIdentity},

        {"EOS_IPMF_ApplicationManagedIdentityLogin", EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_ApplicationManagedIdentityLogin },
        {"ApplicationManagedIdentityLogin",          EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_ApplicationManagedIdentityLogin}
    };

    typedef const char* (*GetConfigAsJSONString_t)();
    static GetConfigAsJSONString_t GetConfigAsJSONString;

    struct SandboxDeploymentOverride
    {
        std::string sandboxID;
        std::string deploymentID;
    };

    struct EOSConfig
    {
        std::string productName;
        std::string productVersion;

        std::string productID;
        std::string sandboxID;
        std::string deploymentID;
        std::vector<SandboxDeploymentOverride> sandboxDeploymentOverrides;

        std::string clientSecret;
        std::string clientID;
        std::string encryptionKey;

        std::string overrideCountryCode;
        std::string overrideLocaleCode;

        // this is called platformOptionsFlags in C#
        uint64_t flags = 0;

        uint32_t tickBudgetInMilliseconds = 0;
        double taskNetworkTimeoutSeconds = 0.0;

        uint64_t ThreadAffinity_networkWork = 0;
        uint64_t ThreadAffinity_storageIO = 0;
        uint64_t ThreadAffinity_webSocketIO = 0;
        uint64_t ThreadAffinity_P2PIO = 0;
        uint64_t ThreadAffinity_HTTPRequestIO = 0;
        uint64_t ThreadAffinity_RTCIO = 0;

        bool isServer = false;

    };

    struct LogLevelConfig
    {
        std::vector<std::string> category;
        std::vector<std::string> level;
    };

    struct EOSSteamConfig
    {
        EOS_EIntegratedPlatformManagementFlags flags;
        uint32_t steamSDKMajorVersion;
        uint32_t steamSDKMinorVersion;
        std::optional<std::string> OverrideLibraryPath;
        std::vector<std::string> steamApiInterfaceVersionsArray;

        EOSSteamConfig()
        {
            flags = static_cast<EOS_EIntegratedPlatformManagementFlags>(0);
        }

        bool isManagedByApplication()
        {
            return std::underlying_type<EOS_EIntegratedPlatformManagementFlags>::type(flags & EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedByApplication);
        }
        bool isManagedBySDK()
        {
            return std::underlying_type<EOS_EIntegratedPlatformManagementFlags>::type(flags & EOS_EIntegratedPlatformManagementFlags::EOS_IPMF_LibraryManagedBySDK);
        }

    };

    /**
     * @brief Parses a JSON configuration object to create a log level configuration.
     *
     * Extracts log category and level pairs from a JSON structure, storing them in a `LogLevelConfig` object.
     *
     * @param config_json The JSON value representing the configuration.
     * @return A `LogLevelConfig` object populated with log categories and their levels.
     */
    LogLevelConfig log_config_from_json_value(json_value_s* config_json);

    /**
     * @brief Retrieves the path to the EOS service configuration file.
     *
     * Attempts to locate the configuration file by navigating up directories.
     * If the path is not found, it defaults to a predefined directory.
     *
     * @param config_filename The name of the configuration file.
     * @return The path to the EOS service configuration file.
     */
    std::filesystem::path get_path_for_eos_service_config(std::string config_filename);

    /**
     * @brief Reads a JSON configuration from a DLL.
     *
     * Attempts to load configuration data from a DLL. The function retrieves the
     * JSON configuration string from the DLL, parses it, and returns the parsed JSON object.
     *
     * @return A pointer to a `json_value_s` representing the configuration data, or `nullptr` if loading fails.
     */
    json_value_s* read_config_json_from_dll();

    /**
     * @brief Parses an EOS configuration object from a JSON structure.
     *
     * Reads EOS-related configuration details such as product name, version, IDs, client credentials,
     * and thread affinities from the given JSON structure, populating an `EOSConfig` object.
     *
     * @param config_json The JSON value representing the configuration.
     * @return An `EOSConfig` object populated with settings from the JSON structure.
     */
    EOSConfig eos_config_from_json_value(json_value_s* config_json);

    /**
     * @brief Collects integrated platform management flags from a JSON element.
     *
     * Parses platform management flags from a JSON element and combines them into a single flag value.
     *
     * @param iter The JSON object element representing the flags.
     * @return The combined `EOS_EIntegratedPlatformManagementFlags` value.
     */
    EOS_EIntegratedPlatformManagementFlags eos_collect_integrated_platform_managment_flags(json_object_element_s* iter);

    /**
     * @brief Parses an EOS Steam configuration from a JSON structure.
     *
     * Reads Steam-specific settings, such as flags, library path, SDK versions, and API versions,
     * from the provided JSON configuration, and stores them in an `EOSSteamConfig` object.
     *
     * @param config_json The JSON value representing the Steam configuration.
     * @return An `EOSSteamConfig` object populated with settings from the JSON structure.
     */
    EOSSteamConfig eos_steam_config_from_json_value(json_value_s* config_json);

    /**
     * @brief Reads an EOS configuration file as a JSON value.
     *
     * Retrieves the specified configuration file, loads its contents, and parses it into a JSON structure.
     *
     * @param config_filename The name of the configuration file.
     * @return A pointer to a `json_value_s` representing the configuration data, or `nullptr` if parsing fails.
     */
    json_value_s* read_eos_config_as_json_value_from_file(std::string config_filename);

}

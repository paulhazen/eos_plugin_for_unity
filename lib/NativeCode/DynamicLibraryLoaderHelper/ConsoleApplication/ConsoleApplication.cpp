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

#define NOMINMAX
#ifdef IServiceProvider
#undef IServiceProvider
#endif
#include <msclr/marshal_cppstd.h>

#include <iostream>

#include "headers/config_legacy.h"
#include "headers/Config/EOSWrapper.h"
#include "ManagedToUnmanagedBridge.hpp"



#using <ManagedPluginCode.dll>
#using <ManagedToUnmanagedBridge.dll>

using namespace Epic::OnlineServices;

#define CHECK_EQUALITY()

bool Equals(const char* c_str, System::String^ str)
{
    std::string managed = msclr::interop::marshal_as<std::string>(str);
    std::string unmanaged = c_str;

    return (managed == unmanaged);
}

bool Equals(const int32_t& bool_value, const bool& managed_bool)
{
    bool translated = bool_value == 1 ? true : false;

    return (translated == managed_bool);
}

bool Equals(const uint64_t& long_value, const uint64_t& long_value_managed)
{
    return (long_value == long_value_managed);
}

void CheckEquality(const char* name, const char* c_str, System::String^ str)
{
    if (!Equals(c_str, str))
    {
        std::string managed_str = msclr::interop::marshal_as<std::string>(str);
        std::cerr << name << " is not equal (Native: " << std::string(c_str) << ", Managed: " << managed_str << ")." << std::endl;
    }
}

void CheckEquality(const char* name, const int32_t& bool_value, const bool& managed_bool)
{
    if (!Equals(bool_value, managed_bool))
    {
        std::cerr << name << " is not equal (Native: " << bool_value << ", Managed: " << managed_bool << ")." << std::endl;
    }
}

void CheckEquality(const char* name, const uint64_t& long_value, Epic::OnlineServices::Platform::PlatformFlags^ flags)
{
    uint64_t managed_long_value = static_cast<uint64_t>(static_cast<Epic::OnlineServices::Platform::PlatformFlags>(*flags));

    if (!Equals(long_value, managed_long_value))
    {
        std::cerr << name << " is not equal (Native: " << long_value << ", Managed: " << managed_long_value << ")." << std::endl;
    }
}

void CheckEquality(const char* name, const double* double_value, System::Nullable<double>^ managed_double_value)
{
    double managed_dbl = 0.0;
    if (managed_double_value->HasValue)
    {
        managed_dbl = managed_double_value->Value;
    }
    if (*double_value != managed_dbl)
    {
        std::cerr << name << " is not equal (Native: " << *double_value << ", Managed: " << managed_dbl << ")." << std::endl;
    }
}

void CheckEquality(const char* name, const uint32_t& unmanaged_uint, const uint32_t& managed_uint)
{
    if (unmanaged_uint != managed_uint)
    {
        std::cerr << name << " is not equal (Native: " << unmanaged_uint << ", Managed: " << managed_uint << ")." << std::endl;
    }
}

bool NativeAndManagedPlatformOptionsAreIdentical()
{
    const auto eos_sdk = new pew::eos::EOSWrapper();
    
    const auto native_platform_options = eos_sdk->PEW_EOS_ExportPlatformOptions();
    const auto managed_platform_options = pew::eos::tests::NativeTestUtilityBridge::get_windows_options();

    CheckEquality("ProductId", native_platform_options.ProductId, managed_platform_options->ProductId->ToString());
    CheckEquality("SandboxId", native_platform_options.SandboxId, managed_platform_options->SandboxId->ToString());
    CheckEquality("ClientId", native_platform_options.ClientCredentials.ClientId, managed_platform_options->ClientCredentials.ClientId->ToString());
    CheckEquality("ClientSecret", native_platform_options.ClientCredentials.ClientSecret, managed_platform_options->ClientCredentials.ClientSecret->ToString());
    CheckEquality("IsServer", native_platform_options.bIsServer, managed_platform_options->IsServer);
    CheckEquality("EncryptionKey", native_platform_options.EncryptionKey, managed_platform_options->EncryptionKey->ToString());
    CheckEquality("DeploymentId", native_platform_options.DeploymentId, managed_platform_options->DeploymentId->ToString());
    CheckEquality("Flags", native_platform_options.Flags, managed_platform_options->Flags);
    CheckEquality("TaskNetworkTimeoutSeconds", native_platform_options.TaskNetworkTimeoutSeconds, managed_platform_options->TaskNetworkTimeoutSeconds);
    CheckEquality("TickBudgetInMilliseconds", native_platform_options.TickBudgetInMilliseconds, managed_platform_options->TickBudgetInMilliseconds);
    
    delete eos_sdk;
    return true;
}

int main()
{
    NativeAndManagedPlatformOptionsAreIdentical();

    pew::eos::config_legacy::EOSConfig eos_config;

    const auto eos_sdk = new pew::eos::EOSWrapper();
    const auto platform_interface = eos_sdk->start_eos();
    

    if(pew::eos::config_legacy::try_get_eos_config(eos_config))
    {
        std::cout << "EOSConfig was read successfully.";
    }
    else
    {
        std::cout << "Could not load EOSConfig.";
    }
}

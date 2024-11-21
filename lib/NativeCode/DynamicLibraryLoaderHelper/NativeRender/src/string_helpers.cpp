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

#include <pch.h>
#include <algorithm>
#include <codecvt>
#include <ctime>
#include <locale>
#include <string_helpers.h>
#include <sstream>
#include <filesystem>

namespace pew::eos::string_helpers
{
    std::string trim(const std::string& str)
    {
        const auto start = std::find_if_not(str.begin(), str.end(), ::isspace);
        const auto end = std::find_if_not(str.rbegin(), str.rend(), ::isspace).base();

        if (start < end)
        {
            return std::basic_string<char>(start, end);
        }
        else
        {
            return "";
        }
    }

    std::vector<std::string> split_and_trim(const std::string& input, char delimiter)
    {
        std::vector<std::string> result;
        std::stringstream ss(input);
        std::string item;

        while (std::getline(ss, item, delimiter))
        {
            std::string trimmedItem = trim(item);
            if (!trimmedItem.empty())
            {
                result.push_back(trimmedItem);
            }
        }

        return result;
    }

    bool create_timestamp_str(char* final_timestamp, size_t final_timestamp_len)
    {
        constexpr size_t buffer_len = 32;
        char buffer[buffer_len];

        if (buffer_len > final_timestamp_len)
        {
            return false;
        }

        time_t raw_time = time(nullptr);
        tm time_info = { 0 };

        timespec time_spec = { 0 };
        timespec_get(&time_spec, TIME_UTC);
        localtime_s(&time_info, &raw_time);

        strftime(buffer, buffer_len, "%Y-%m-%dT%H:%M:%S", &time_info);
        long milliseconds = (long)round(time_spec.tv_nsec / 1.0e6);
        snprintf(final_timestamp, final_timestamp_len, "%s.%03ld", buffer, milliseconds);

        return true;
    }

    size_t utf8_str_bytes_required_for_wide_str(const wchar_t* wide_str, int wide_str_len)
    {
        int bytes_required = WideCharToMultiByte(CP_UTF8, 0, wide_str, wide_str_len, NULL, 0, NULL, NULL);

        if (bytes_required < 0)
        {
            return 0;
        }

        return bytes_required;
    }

    // wide_str must be null terminated if wide_str_len is passed
    bool copy_to_utf8_str_from_wide_str(char* RESTRICT utf8_str, size_t utf8_str_len, const wchar_t* RESTRICT wide_str, int wide_str_len)
    {
        if (utf8_str_len > INT_MAX)
        {
            return false;
        }

        WideCharToMultiByte(CP_UTF8, 0, wide_str, wide_str_len, utf8_str, (int)utf8_str_len, NULL, NULL);

        return true;
    }

    char* create_utf8_str_from_wide_str(const wchar_t* wide_str)
    {
        const int wide_str_len = (int)wcslen(wide_str) + 1;
        int bytes_required = (int)utf8_str_bytes_required_for_wide_str(wide_str, wide_str_len);
        char* to_return = (char*)malloc(bytes_required);

        if (!copy_to_utf8_str_from_wide_str(to_return, bytes_required, wide_str, wide_str_len))
        {
            free(to_return);
            to_return = NULL;
        }

        return to_return;
    }

    wchar_t* create_wide_str_from_utf8_str(const char* utf8_str)
    {
        int chars_required = MultiByteToWideChar(CP_UTF8, 0, utf8_str, -1, NULL, 0);
        wchar_t* to_return = (wchar_t*)malloc(chars_required * sizeof(wchar_t));
        int utf8_str_len = (int)strlen(utf8_str);

        MultiByteToWideChar(CP_UTF8, 0, utf8_str, utf8_str_len, to_return, chars_required);

        return to_return;
    }

    std::string to_utf8_str(const std::wstring& wide_str)
    {
        std::wstring_convert<std::codecvt_utf8<wchar_t>> converter;
        std::string utf8_str = converter.to_bytes(wide_str);

        return utf8_str;
    }

    std::string to_utf8_str(const std::filesystem::path& path)
    {
        return to_utf8_str(path.native());
    }
}

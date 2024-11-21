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

#include <pch.h>
#include <filesystem>
#include <fstream>
#include <sstream>
#include <string>

#include "headers/Config/ConfigBase.h"

#include "logging.h"
#include "headers/Config/Version.hpp"

namespace pew::eos::config
{
    using namespace std::filesystem;

    ConfigBase::ConfigBase(const path& file_name)
    {
        _file_path = file_name;
    }

    void ConfigBase::read() 
    {
        if(!exists(_file_path))
        {
            logging::log_error("Config file \"" + _file_path.string() + "\" does not exist.");
            return;
        }

        const std::ifstream file(_file_path);
        if (!file.is_open())
        {
            logging::log_error("Failed to open existing file: \"" + _file_path.string() + "\"");
            return;
        }

        std::ostringstream buffer;
        buffer << file.rdbuf();
        std::string json_content = buffer.str();

        const nlohmann::json json = nlohmann::json::parse(json_content);

        from_json_internal(json);
    }

    void ConfigBase::from_json_internal(const nlohmann::json& json)
    {
        Version::try_parse(json["schemaVersion"], _schema_version);
        from_json(json);
    }

    ConfigBase::~ConfigBase() = default;
}

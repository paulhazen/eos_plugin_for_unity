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
    using Newtonsoft.Json;
    using System.Text.RegularExpressions;
    using System;

    public struct SandboxId : IEquatable<SandboxId>
    {
        private string _value;
        private const string PreProductionEnvironmentRegex = @"^p\-[a-zA-Z\d]{30}$";

        public string Value
        {
            readonly get
            {
                return _value;
            }
            set
            {
                // allow null value
                if (value == null)
                {
                    _value = null;
                    return;
                }

                if (!Guid.TryParse(value, out _))
                {
                    if (!Regex.IsMatch(value, PreProductionEnvironmentRegex))
                    {
                        throw new ArgumentException(
                            "Value for SandboxId must either be " +
                            "parseable to a Guid, or it must start with a " +
                            "lowercase 'p', followed by a dash and thirty " +
                            "letter characters.");
                    }
                }
                else
                {
                    // If the Guid was correctly parsed, then considering that
                    // the EOS SDK prefers SandboxId to be lowercase without
                    // dashes, do this just to be sure.
                    value = value.Replace("-", "");
                }

                // Whether the value was correctly matched to the regex or it
                // was a Guid, it still should be lowercase.
                _value = value.ToLower();
            }
        }

        /// <summary>
        /// Indicates whether the sandbox id is empty. A sandbox id is empty if
        /// either the underlying value is null or empty, or if the underlying
        /// value is equal to the string value for an empty Guid with the dashes
        /// removed.
        /// TODO: Utilize this property when applying the SandboxId to the EOS
        ///       SDK during initialization.
        /// </summary>
        [JsonIgnore]
        public readonly bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_value) || 
                       Guid.Empty.ToString("N").Equals(_value);
            }
        }

        public readonly bool Equals(SandboxId other)
        {
            return _value == other._value;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is SandboxId sandboxId && Equals(sandboxId);
        }

        public override readonly int GetHashCode()
        {
            return (_value?.GetHashCode()) ?? 0;
        }

        public override readonly string ToString()
        {
            return _value;
        }
    }
}
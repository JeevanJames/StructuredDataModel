// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace NStructuredDataModel
{
    public class FormatOptions
    {
        public Converter<string, string>? PropertyNameConverter { get; set; }

        public string ConvertPropertyName(string propertyName)
        {
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            return PropertyNameConverter?.Invoke(propertyName) ?? propertyName;
        }
    }
}

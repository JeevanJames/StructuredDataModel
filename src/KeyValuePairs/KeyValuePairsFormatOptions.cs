// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace NStructuredDataModel.KeyValuePairs
{
    public sealed class KeyValuePairsFormatOptions : FormatOptions
    {
        private string _propertyNameSeparator = ".";

        public string PropertyNameSeparator
        {
            get => _propertyNameSeparator;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Contains('='))
                    throw new ArgumentException("Property name separator cannot contain the '=' character'.", nameof(value));
                _propertyNameSeparator = value;
            }
        }

        public string NewLine { get; set; } = Environment.NewLine;

        public static readonly KeyValuePairsFormatOptions Default = new();
    }
}

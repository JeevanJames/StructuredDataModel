// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace NStructuredDataModel.KeyValuePairs
{
    public sealed class KeyValuePairsFormatOptions : StructuredDataFormatOptions
    {
        private string _propertyFormat = "{0}={1}";

        public string PropertyNameSeparator { get; set; } = ".";

        public string PropertyFormat
        {
            get => _propertyFormat;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Contains('='))
                    throw new ArgumentException("Property format cannot contain the '=' character'.", nameof(value));
                _propertyFormat = value;
            }
        }

        public string NewLine { get; set; } = Environment.NewLine;

        public static readonly KeyValuePairsFormatOptions Default = new();
    }
}

// Copyright (c) 2021 Jeevan James
// This file is licenses to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace NStructuredDataModel.KeyValuePairs
{
    public sealed class KeyValuePairsFormatOptions : StructuredDataFormatOptions
    {
        public string PropertyNameSeparator { get; set; } = ".";

        public string PropertyFormat { get; set; } = "{0}={1}";

        public string NewLine { get; set; } = Environment.NewLine;

        public static readonly KeyValuePairsFormatOptions Default = new();
    }
}

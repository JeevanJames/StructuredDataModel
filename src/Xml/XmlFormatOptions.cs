// Copyright (c) 2021 Jeevan James
// This file is licenses to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace NStructuredDataModel.Xml
{
    public sealed class XmlFormatOptions : StructuredDataFormatOptions
    {
        public string? RootElementName { get; set; }

        public string? ArrayElementName { get; set; }

        public static readonly XmlFormatOptions Default = new();
    }
}

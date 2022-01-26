// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;

using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace NStructuredDataModel.Yaml;

public sealed class YamlFormat : StructuredDataFormatBase<YamlFormatOptions>
{
    public YamlFormat()
        : base(YamlFormatOptions.Default)
    {
    }

    public YamlFormat(YamlFormatOptions options)
        : base(options)
    {
    }

    protected override void Import(TextReader reader, Node node)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        YamlStream yaml = new();
        yaml.Load(reader);

        foreach (YamlDocument yamlDocument in yaml.Documents)
        {
            YamlImporter importer = new(yamlDocument, node);
            importer.Traverse();
        }
    }
}

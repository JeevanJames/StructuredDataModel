﻿using System;
using System.IO;

using YamlDotNet.RepresentationModel;

namespace NStructuredDataModel.Yaml
{
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

        protected override void Import(TextReader reader, AbstractNode node)
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
}

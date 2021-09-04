// Copyright (c) 2021 Jeevan James
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NStructuredDataModel.Json;
using NStructuredDataModel.KeyValuePairs;
using NStructuredDataModel.Xml;
using NStructuredDataModel.Yaml;

using Shouldly;

using Xunit;
using Xunit.Abstractions;
using Xunit.DataAttributes;

namespace NStructuredDataModel.UnitTests
{
    public sealed class GeneralTests
    {
        private readonly ITestOutputHelper _output;

        public GeneralTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ExportTest()
        {
            StructuredDataModel model = new();
            model.Write("Log.Level", "Information")
                .Write("Log.WriteToFile", true)
                .Write("Log.MaxDepth", 4)
                .Write("Settings.Default", (byte)10);

            JsonFormat jsonFormat = new();
            string json = await jsonFormat.ExportAsync(model);

            json.ShouldNotBeNullOrWhiteSpace();

            _output.WriteLine(json);
        }

        [Theory]
        [EmbeddedResourceContent("NStructuredDataModel.UnitTests.Heroes.yaml")]
        public async Task ImportTest(string yaml)
        {
            YamlFormat yamlFormat = new();
            StructuredDataModel model = await yamlFormat.ImportAsync(yaml);

            model.ShouldNotBeNull();
        }

        [Theory]
        [EmbeddedResourceContent("NStructuredDataModel.UnitTests.Heroes.yaml")]
        public async Task GetNodeEntryValuesTest(string json)
        {
            YamlFormat yamlFormat = new();
            StructuredDataModel model = await yamlFormat.ImportAsync(json);
            List<NodeEntryValue> values = (await model.GetNodeEntryValues(true)).ToList();

            values.Count.ShouldBe(12);
        }

        [Theory]
        [EmbeddedResourceContent("NStructuredDataModel.UnitTests.AppSettings.kvp")]
        public async Task ImportKeyValuePairsTest(string kvp)
        {
            KeyValuePairsFormat kvpFormat = new(new KeyValuePairsFormatOptions { PropertyNameSeparator = ":", });
            StructuredDataModel model = await kvpFormat.ImportAsync(kvp);

            model.ShouldNotBeNull();
        }

        [Theory]
        [EmbeddedResourceContent("NStructuredDataModel.UnitTests.Heroes.yaml")]
        public async Task ConvertFormatTest(string yaml)
        {
            YamlFormat yamlFormat = new();
            StructuredDataModel model = await yamlFormat.ImportAsync(yaml);

            JsonFormat jsonFormat = new(new JsonFormatOptions
            {
                PropertyNameConverter = NameConverters.PascalCase,
            });
            string json = await jsonFormat.ExportAsync(model);
            json.ShouldNotBeNullOrWhiteSpace();

            XmlFormat xmlFormat = new(new XmlFormatOptions
            {
                RootElementName = "Configuration",
                ArrayElementName = "Item",
                PropertyNameConverter = NameConverters.CamelCase,
            });
            string xml = await xmlFormat.ExportAsync(model);
            xml.ShouldNotBeNullOrWhiteSpace();

            KeyValuePairsFormat kvpFormat = new(new KeyValuePairsFormatOptions
            {
                PropertyNameSeparator = "__",
                PropertyNameConverter = NameConverters.PascalCase,
            });
            string kvp = await kvpFormat.ExportAsync(model);
            kvp.ShouldNotBeNullOrWhiteSpace();

            _output.WriteLine(json);
            _output.WriteLine(xml);
            _output.WriteLine(kvp);
        }

        [Theory]
        [EmbeddedResourceContent("NStructuredDataModel.UnitTests.LogSettings.xml")]
        public async Task ImportXmlTest(string xml)
        {
            _output.WriteLine(xml);

            XmlFormat xmlFormat = new(new XmlFormatOptions { PropertyNameConverter = NameConverters.PascalCase });
            StructuredDataModel model = await xmlFormat.ImportAsync(xml);

            model.ShouldNotBeNull();

            JsonFormat jsonFormat = new();
            string json = await jsonFormat.ExportAsync(model);

            _output.WriteLine(json);
        }
    }
}

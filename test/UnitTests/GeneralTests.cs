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

namespace NStructuredDataModel.UnitTests;

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
        model.Write("Log.Level", "Information");
        model.Write("Log.WriteToFile", true);
        model.Write("Log.MaxDepth", 4);
        model.Write("Settings.Default", (byte)10);
        model.WriteNode("Security.Authentication");

        JsonFormat jsonFormat = new();
        string json = await jsonFormat.ExportAsync(model);

        json.ShouldNotBeNullOrWhiteSpace();
        _output.WriteLine(json);

        XmlFormat xmlFormat = new();
        string xml = await xmlFormat.ExportAsync(model);

        xml.ShouldNotBeNullOrWhiteSpace();
        _output.WriteLine(xml);
    }

    [Theory]
    [EmbeddedResourceContent("NStructuredDataModel.UnitTests.Heroes.yaml")]
    public async Task ImportTest(string yaml)
    {
        YamlFormat yamlFormat = new();
        StructuredDataModel model = await yamlFormat.ImportAsync(yaml);

        model.ShouldNotBeNull();
        model.Read("settings.heroes.0.name", string.Empty).ShouldBe("Flash");
    }

    [Theory]
    [EmbeddedResourceContent("NStructuredDataModel.UnitTests.Heroes.yaml")]
    public async Task GetFlattenedNodesTest(string json)
    {
        YamlFormat yamlFormat = new();
        StructuredDataModel model = await yamlFormat.ImportAsync(json);

        var values = (await model.GetFlattenedNodes(true)).ToList();

        values.Count.ShouldBe(12);
    }

    [Theory]
    [EmbeddedResourceContent("NStructuredDataModel.UnitTests.Heroes.yaml")]
    public async Task TryGetAsArrayTest(string json)
    {
        IStructuredDataFormat yamlFormat = new YamlFormat();
        StructuredDataModel model = await yamlFormat.ImportAsync(json);

        Node heroesNode = model.ReadNode("settings", "heroes");
        bool successful = heroesNode.TryGetAsArray(out IList<NodeValue> values);

        successful.ShouldBeTrue();
        values.Count.ShouldBe(4);
        values.ShouldAllBe(nodeValue => nodeValue.IsNode);
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

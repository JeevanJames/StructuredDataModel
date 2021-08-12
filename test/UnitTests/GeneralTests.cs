using System.Threading.Tasks;

using NStructuredDataModel.Json;
using NStructuredDataModel.Yaml;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

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

        [Fact]
        public async Task ImportTest()
        {
            string json = @"
{
    ""Logging"": {
        ""Level"": ""Information""
    },
    ""MaxDiscount"": 10
}";

            JsonFormat jsonFormat = new();
            StructuredDataModel model = await jsonFormat.ImportAsync(json);

            model.ShouldNotBeNull();
        }

        [Fact]
        public async Task ConvertFormatTest()
        {
            YamlFormat yamlFormat = new();
            StructuredDataModel model = await yamlFormat.ImportAsync(Yaml);

            JsonFormat jsonFormat = new();
            string json = await jsonFormat.ExportAsync(model);

            json.ShouldNotBeNullOrWhiteSpace();

            _output.WriteLine(json);
        }

#pragma warning disable SA1203 // Constants should appear before fields
        private const string Yaml = @"
log:
    level: Information
    write_to_file: true
    max_depth: 4
settings:
    default: 10,
    numbers: [2, 4, 6, 8]
"
#pragma warning restore SA1203 // Constants should appear before fields
;
    }
}

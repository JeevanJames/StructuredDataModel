# NStructuredDataModel
NStructuredDataModel is a .NET framework that provides a generic object model to represents a hierarchial key/value structure.

The framework also provides support for importing from and exporting to various compatible structured file formats like JSON, YAML and XML.

## Installation
NStructuredDataModel can be installed from the [NuGet package](https://nuget.org/packages/NStructuredDataModel).

```ps
# Using dotnet CLI
dotnet add package NStructuredDataModel

# Using the package manager console
Install-Package NStructuredDataModel
```

There are separate packages to support importing from and exporting to various file formats:

Format | Supports | Package
-------|----------|--------
JSON | Import/Export | [NStructuredDataModel.Json](https://nuget.org/packages/NStructuredDataModel.Json)
Key/value pairs | Export only | [NStructuredDataModel.KeyValuePairs](https://nuget.org/packages/NStructuredDataModel.KeyValuePairs)
XML | Import/Export | [NStructuredDataModel.Xml](https://nuget.org/packages/NStructuredDataModel.Xml)
YAML | Import/Export | [NStructuredDataModel.Yaml](https://nuget.org/packages/NStructuredDataModel.Yaml)

## Examples

### Loading YAML and saving as JSON
This example demonstrates loading structured data from the YAML format and converting it to JSON.

Required packages:
* `NStructuredDataModel`
* `NStructuredDataModel.Json`
* `NStructuredDataModel.Yaml`

```cs
async Task<string> ConvertToJson(string yaml)
{
    // Import the YAML data
    var yamlFormat = new YamlFormat();
    StructuredDataModel model = await yamlFormat.ImportAsync(yaml);

    // Export to JSON
    var jsonOptions = new JsonFormatOptions
    {
        PropertyNameConverter = PropertyNameConverters.PascalCase
    };
    var jsonFormat = new JsonFormat(jsonOptions);
    string json = await jsonFormat.ExportAsync(model);

    return json;
}
```

### Create in-memory model and save as XML
This example demonstrates creating an in-memory hierarchical key/value structure using the `NStructuredDataModel.StructuredDataModel` class and saving it as XML.

Required packages:
* `NStructuredDataModel`
* `NStructuredDataModel.Xml`

```cs
async Task<string> SaveStructureAsXml()
{
    // Create an in-memory structure
    var model = new StructuredDataModel();
    model.Write("Server.Host", "localhost");
    model.Write("Server.Port", 8080);
    model.Write("Server.UseProxy", true);

    // Export to XML
    var xmlOptions = new XmlFormatOptions
    {
        PropertyNameConverter = NameConverters.CamelCase,
        RootElementName = "Configuration"
    };
    var xmlFormat = new XmlFormat(xmlOptions);
    string xml = await xmlFormat.ExportAsync(model);
    return xml;
}
```

This will return the following XML:
```xml
<configuration>
    <server>
        <host>localhost</host>
        <port>8080</port>
        <useProxy>true</useProxy>
    </server>
</configuration>
```

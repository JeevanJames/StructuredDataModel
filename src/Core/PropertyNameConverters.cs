using System;

using Humanizer;

namespace NStructuredDataModel
{
    public static class PropertyNameConverters
    {
        public static readonly Converter<string, string> PascalCase = s => s.Pascalize();

        public static readonly Converter<string, string> CamelCase = s => s.Camelize();
    }
}

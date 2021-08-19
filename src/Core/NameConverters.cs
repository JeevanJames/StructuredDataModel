// Copyright (c) 2021 Jeevan James
// This file is licenses to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Humanizer;

namespace NStructuredDataModel
{
    public static class NameConverters
    {
        public static readonly Converter<string, string> PascalCase = s => s.Pascalize();

        public static readonly Converter<string, string> CamelCase = s => s.Camelize();

        public static readonly Converter<string, string> KebabCase = s => s.Kebaberize();

        public static readonly Converter<string, string> SnakeCase = s => s.Underscore();

        public static readonly Converter<string, string> AllCaps = s => s.Pascalize().ApplyCase(LetterCasing.AllCaps);

        public static readonly Converter<string, string> AllLower = s => s.Pascalize().ApplyCase(LetterCasing.LowerCase);
    }
}

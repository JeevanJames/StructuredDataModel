namespace NStructuredDataModel.Dictionary
{
    public sealed class DictionaryFormat : StructuredDataFormatBase<DictionaryFormatOptions>
    {
        public DictionaryFormat()
            : base(DictionaryFormatOptions.Default)
        {
        }

        public DictionaryFormat(DictionaryFormatOptions options)
            : base(options)
        {
        }
    }

    public sealed class DictionaryFormatOptions : FormatOptions
    {
        public static readonly DictionaryFormatOptions Default = new();
    }
}

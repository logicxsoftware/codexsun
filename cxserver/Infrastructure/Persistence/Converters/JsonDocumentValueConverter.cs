using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace cxserver.Infrastructure.Persistence.Converters;

internal sealed class JsonDocumentValueConverter : ValueConverter<JsonDocument, string>
{
    public JsonDocumentValueConverter()
        : base(value => value.RootElement.GetRawText(), value => JsonDocument.Parse(value))
    {
    }
}

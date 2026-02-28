using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.WebEngine;

namespace cxserver.Infrastructure.WebEngine;

internal sealed class SectionDataValidator : ISectionDataValidator
{
    public bool IsValid(SectionType sectionType, JsonDocument sectionData)
    {
        if (sectionData.RootElement.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        return sectionType switch
        {
            SectionType.Menu => HasAnyRequiredKey(sectionData, "items"),
            SectionType.Slider => HasAnyRequiredKey(sectionData, "slides"),
            SectionType.Hero => HasAnyRequiredKey(sectionData, "headline", "title"),
            SectionType.About => HasAnyRequiredKey(sectionData, "title", "content"),
            SectionType.Features => HasAnyRequiredKey(sectionData, "items"),
            SectionType.Gallery => HasAnyRequiredKey(sectionData, "images"),
            SectionType.ProductRange => HasAnyRequiredKey(sectionData, "products"),
            SectionType.WhyChooseUs => HasAnyRequiredKey(sectionData, "items"),
            SectionType.Stats => HasAnyRequiredKey(sectionData, "items"),
            SectionType.BrandSlider => HasAnyRequiredKey(sectionData, "brands"),
            SectionType.BlogShow => HasAnyRequiredKey(sectionData, "limit"),
            SectionType.Testimonial => HasAnyRequiredKey(sectionData, "items"),
            SectionType.CallToAction => HasAnyRequiredKey(sectionData, "label", "title"),
            SectionType.Newsletter => HasAnyRequiredKey(sectionData, "title"),
            SectionType.Footer => HasAnyRequiredKey(sectionData, "columns"),
            _ => false
        };
    }

    private static bool HasAnyRequiredKey(JsonDocument payload, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (payload.RootElement.TryGetProperty(key, out _))
            {
                return true;
            }
        }

        return false;
    }
}

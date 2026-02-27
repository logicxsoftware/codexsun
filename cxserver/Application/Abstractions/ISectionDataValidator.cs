using System.Text.Json;
using cxserver.Domain.WebEngine;

namespace cxserver.Application.Abstractions;

public interface ISectionDataValidator
{
    bool IsValid(SectionType sectionType, JsonDocument sectionData);
}

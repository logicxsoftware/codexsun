using System.Globalization;
using cxserver.Application.Abstractions;

namespace cxserver.Infrastructure.Tenancy;

internal sealed class TenantResolver : ITenantResolver
{
    private readonly ITenantDomainLookup _tenantDomainLookup;
    private readonly ITenantRegistry _tenantRegistry;

    public TenantResolver(ITenantDomainLookup tenantDomainLookup, ITenantRegistry tenantRegistry)
    {
        _tenantDomainLookup = tenantDomainLookup;
        _tenantRegistry = tenantRegistry;
    }

    public async Task<TenantRegistryItem?> ResolveAsync(string host, CancellationToken cancellationToken)
    {
        var normalizedDomain = NormalizeDomain(host);
        if (normalizedDomain is null)
        {
            return null;
        }

        var tenant = await _tenantDomainLookup.GetByDomainAsync(normalizedDomain, cancellationToken);

        if (tenant is not null)
        {
            return tenant;
        }

        var identifier = ExtractIdentifier(normalizedDomain);
        if (identifier is null)
        {
            return null;
        }

        return await _tenantRegistry.GetByIdentifierAsync(identifier, cancellationToken);
    }

    private static string? NormalizeDomain(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return null;
        }

        var value = host.Trim().TrimEnd('.');
        if (value.Length == 0 || value.Length > 255 || value.Contains('/') || value.Contains('\\') || value.Contains('@'))
        {
            return null;
        }

        var hostNameType = Uri.CheckHostName(value);
        if (hostNameType is not UriHostNameType.Dns and not UriHostNameType.IPv4 and not UriHostNameType.IPv6)
        {
            return null;
        }

        var idnMapping = new IdnMapping();
        try
        {
            return idnMapping.GetAscii(value).ToLowerInvariant();
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static string? ExtractIdentifier(string normalizedDomain)
    {
        var labels = normalizedDomain.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (labels.Length < 3)
        {
            return null;
        }

        var firstLabel = labels[0];
        if (firstLabel.Length == 0 || firstLabel.Length > 128)
        {
            return null;
        }

        return firstLabel;
    }
}

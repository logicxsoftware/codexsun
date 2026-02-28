using System.Text.Json;
using cxserver.Domain.WebEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Seeding;

internal sealed class TenantWebsitePageSeeder
{
    private readonly Application.Abstractions.IDateTimeProvider _dateTimeProvider;

    public TenantWebsitePageSeeder(Application.Abstractions.IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SeedAsync(TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        var definitions = BuildDefinitions();
        var now = _dateTimeProvider.UtcNow;

        foreach (var definition in definitions)
        {
            var page = await dbContext.WebsitePages
                .Include(x => x.Sections)
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Slug == definition.Slug, cancellationToken);

            if (page is null)
            {
                page = Page.Create(
                    Guid.NewGuid(),
                    definition.Slug,
                    definition.Title,
                    definition.SeoTitle,
                    definition.SeoDescription,
                    now);
                SyncSections(page, definition.Sections, now);
                page.Publish(now);
                await dbContext.WebsitePages.AddAsync(page, cancellationToken);
                continue;
            }

            // Prevent unique key collisions on (page_id, display_order, is_deleted)
            // by clearing historical soft-deleted rows before reseeding order changes.
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM website_page_sections WHERE page_id = {page.Id} AND is_deleted = 1",
                cancellationToken);

            page.Update(definition.Title, definition.SeoTitle, definition.SeoDescription, now);

            if (!page.IsPublished)
            {
                page.Publish(now);
            }

            SyncSections(page, definition.Sections, now);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyList<PageSeedDefinition> BuildDefinitions()
    {
        return
        [
            new PageSeedDefinition(
                "home",
                "Home",
                "Home",
                "Home page",
                [
                    new SectionSeedDefinition(SectionType.Menu, "{\"items\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"},{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}"),
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"Your Trusted Computer Store & Service Partner in Tiruppur\",\"subtitle\":\"Complete IT solutions since 2002 - hardware sales, custom builds, repair, networking, CCTV, cloud services & reliable support.\",\"primaryCtaLabel\":\"Explore Services\",\"primaryCtaHref\":\"/services\"}"),
                    new SectionSeedDefinition(SectionType.About, "{\"title\":\"Serving Tiruppur with Trusted IT Expertise Since 2002\",\"subtitle\":\"Reliable technology solutions tailored for homes and businesses\",\"content\":[\"We specialize in hardware sales, custom-built PCs, laptop and desktop repairs, and enterprise networking solutions.\",\"Our expertise extends to CCTV installation, structured cabling, cloud services, and long-term IT support.\",\"With over two decades of experience, we focus on reliability, performance, and customer satisfaction.\"],\"image\":{\"src\":\"https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=1400&q=80&auto=format&fit=crop\",\"alt\":\"IT specialist workspace with laptops and network equipment\"}}"),
                    new SectionSeedDefinition(SectionType.Stats, "{\"backgroundToken\":\"background\",\"borderToken\":\"border\",\"stats\":[{\"value\":50,\"suffix\":\"+\",\"label\":\"Trusted Brands\",\"order\":1},{\"value\":12000,\"suffix\":\"+\",\"label\":\"Products In Stock\",\"order\":2},{\"value\":8000,\"suffix\":\"+\",\"label\":\"Happy Customers\",\"order\":3},{\"value\":24,\"suffix\":\"\",\"label\":\"Years of Expertise\",\"order\":4}]}"),
                    new SectionSeedDefinition(SectionType.Catalog, "{\"heading\":\"Complete Computer & Accessories Catalog\",\"subheading\":\"High-quality hardware, peripherals, networking gear, CCTV solutions and IT essentials - perfect for retail, bulk orders, offices, schools and businesses.\",\"categories\":[{\"title\":\"Laptops & Notebooks\",\"slug\":\"laptops\",\"description\":\"Latest models from top brands - performance, gaming, business & student series\",\"image\":\"https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=1200&q=80&auto=format&fit=crop\",\"variants\":[\"Gaming Laptops\",\"Ultrabooks\",\"Business Laptops\",\"Student Laptops\"],\"bulkBadge\":\"Ready Stock\",\"featuredBadge\":\"Latest Models\",\"badgeVariant\":\"emerald\",\"featuredBadgeVariant\":\"amber\",\"order\":1},{\"title\":\"Desktops & Workstations\",\"slug\":\"desktops\",\"description\":\"Custom builds, pre-built systems, office PCs, gaming rigs & professional workstations\",\"image\":\"https://images.unsplash.com/photo-1587202372775-e229f172b9d7?w=1200&q=80&auto=format&fit=crop\",\"variants\":[\"Gaming Desktops\",\"Office PCs\",\"All-in-One\",\"Workstations\"],\"bulkBadge\":\"Custom Builds\",\"featuredBadge\":\"\",\"badgeVariant\":\"blue\",\"featuredBadgeVariant\":\"amber\",\"order\":2},{\"title\":\"Monitors & Displays\",\"slug\":\"monitors\",\"description\":\"Full HD, 4K, curved, ultrawide, gaming & professional color-accurate displays\",\"image\":\"https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=1200&q=80&auto=format&fit=crop\",\"variants\":[\"Gaming Monitors\",\"4K Monitors\",\"Office Displays\",\"Portable Monitors\"],\"bulkBadge\":\"\",\"featuredBadge\":\"2026 New Series\",\"badgeVariant\":\"amber\",\"featuredBadgeVariant\":\"purple\",\"order\":3},{\"title\":\"Networking & Connectivity\",\"slug\":\"networking\",\"description\":\"Routers, switches, Wi-Fi extenders, cables, access points and complete office networking solutions\",\"image\":\"https://images.unsplash.com/photo-1544197150-b99a580bb7a8?w=1200&q=80&auto=format&fit=crop\",\"variants\":[\"Routers\",\"Switches\",\"Wi-Fi Extenders\",\"Cables & Adapters\"],\"bulkBadge\":\"Enterprise Bulk\",\"featuredBadge\":\"\",\"badgeVariant\":\"purple\",\"featuredBadgeVariant\":\"amber\",\"order\":4},{\"title\":\"CCTV & Surveillance\",\"slug\":\"cctv\",\"description\":\"HD cameras, NVRs, DVRs, complete security systems with mobile monitoring support\",\"image\":\"https://images.unsplash.com/photo-1585829365295-ab7cd400c167?w=1200&q=80&auto=format&fit=crop\",\"variants\":[\"Bullet Cameras\",\"Dome Cameras\",\"PTZ Cameras\",\"NVR Systems\"],\"bulkBadge\":\"Bulk Offers\",\"featuredBadge\":\"Latest 4K Series\",\"badgeVariant\":\"rose\",\"featuredBadgeVariant\":\"amber\",\"order\":5},{\"title\":\"Peripherals & Accessories\",\"slug\":\"peripherals\",\"description\":\"Keyboards, mice, headsets, webcams, speakers, UPS, laptop bags & much more\",\"image\":\"https://images.unsplash.com/photo-1587825140708-dfaf72ae4b04?w=1200&q=80&auto=format&fit=crop\",\"variants\":[\"Keyboard & Mouse Combo\",\"Headsets\",\"Webcams\",\"UPS & Power Backup\"],\"bulkBadge\":\"Bulk Accessories\",\"featuredBadge\":\"\",\"badgeVariant\":\"cyan\",\"featuredBadgeVariant\":\"amber\",\"order\":6},{\"title\":\"Printers & Scanners\",\"slug\":\"printers\",\"description\":\"Inkjet, laser, all-in-one, multifunction printers, scanners and cartridges\",\"image\":\"https://images.unsplash.com/photo-1580894732444-8ecded7900cd?w=1200&q=80&auto=format&fit=crop\",\"variants\":[\"Laser Printers\",\"Inkjet Printers\",\"All-in-One\",\"Scanners\"],\"bulkBadge\":\"Office Deals\",\"featuredBadge\":\"New Laser Models\",\"badgeVariant\":\"indigo\",\"featuredBadgeVariant\":\"amber\",\"order\":7},{\"title\":\"Storage Devices\",\"slug\":\"storage\",\"description\":\"Internal HDD/SSD, external drives, NAS, memory cards, pen drives & cloud solutions\",\"image\":\"https://images.unsplash.com/photo-1518779578993-ec3579fee39f?w=1200&q=80&auto=format&fit=crop\",\"variants\":[\"External HDD\",\"Portable SSD\",\"Pen Drives\",\"Memory Cards\"],\"bulkBadge\":\"Bulk Pricing\",\"featuredBadge\":\"\",\"badgeVariant\":\"rose\",\"featuredBadgeVariant\":\"amber\",\"order\":8}]}"),
                    new SectionSeedDefinition(SectionType.WhyChooseUs, "{\"heading\":\"Why Choose Tech Media Retail\",\"subheading\":\"Your trusted IT partner in Tiruppur - delivering quality hardware, expert service, and reliable solutions since 2002.\",\"items\":[{\"title\":\"Expert Hardware Selection\",\"description\":\"Carefully curated range of desktops, laptops, monitors, servers, and peripherals from top global brands.\",\"icon\":\"BadgeCheck\",\"order\":1},{\"title\":\"Competitive Pricing\",\"description\":\"Best-in-class prices for retail, bulk, and corporate buyers with transparent no-hidden-cost policy.\",\"icon\":\"IndianRupee\",\"order\":2},{\"title\":\"Custom PC Builds\",\"description\":\"Tailor-made gaming rigs, workstations, office setups and server configurations to match your exact needs.\",\"icon\":\"Factory\",\"order\":3},{\"title\":\"Fast Delivery & Installation\",\"description\":\"Quick delivery across Tiruppur and nearby areas with professional on-site setup and configuration support.\",\"icon\":\"Truck\",\"order\":4},{\"title\":\"Reliable After-Sales Service\",\"description\":\"Dedicated service team for repairs, upgrades, troubleshooting, AMC contracts and warranty support.\",\"icon\":\"ShieldCheck\",\"order\":5},{\"title\":\"Business & Bulk Solutions\",\"description\":\"Special pricing and support for schools, offices, shops, startups and institutional bulk requirements.\",\"icon\":\"Users\",\"order\":6}]}"),
                    new SectionSeedDefinition(SectionType.BrandSlider, "{\"heading\":\"Our Trusted Brand Partners\",\"pauseOnHover\":true,\"animationDuration\":40,\"logos\":[{\"name\":\"HP\",\"logo\":\"https://logo.clearbit.com/hp.com\",\"order\":1},{\"name\":\"Dell\",\"logo\":\"https://logo.clearbit.com/dell.com\",\"order\":2},{\"name\":\"Lenovo\",\"logo\":\"https://logo.clearbit.com/lenovo.com\",\"order\":3},{\"name\":\"Asus\",\"logo\":\"https://logo.clearbit.com/asus.com\",\"order\":4},{\"name\":\"Acer\",\"logo\":\"https://logo.clearbit.com/acer.com\",\"order\":5},{\"name\":\"Intel\",\"logo\":\"https://logo.clearbit.com/intel.com\",\"order\":6},{\"name\":\"AMD\",\"logo\":\"https://logo.clearbit.com/amd.com\",\"order\":7},{\"name\":\"Epson\",\"logo\":\"https://logo.clearbit.com/epson.com\",\"order\":8}]}"),
                    new SectionSeedDefinition(SectionType.Features, "{\"title\":\"Workstations • Corporate Laptops • Gaming PCs & Laptops\",\"description\":\"Expert solutions for professionals, businesses and gamers.\\nCustom-built workstations | Enterprise laptops | High-end gaming rigs\\nBulk orders • Corporate pricing • On-site setup & support\",\"imageSrc\":\"https://images.unsplash.com/photo-1593642532973-d31b6557fa68\",\"imageAlt\":\"Workstations and laptops for enterprise and gaming setups\",\"bullets\":[{\"text\":\"ISV-certified workstations (Dell Precision, HP Z, Lenovo ThinkStation, ASUS ProArt)\",\"order\":1},{\"text\":\"Business-class laptops with vPro, long battery, spill-resistant keyboards\",\"order\":2},{\"text\":\"Custom gaming desktops – optimized airflow, RGB, liquid cooling options\",\"order\":3},{\"text\":\"High-refresh-rate gaming laptops up to RTX 4090 & 240 Hz displays\",\"order\":4},{\"text\":\"Dedicated account managers for bulk & repeat corporate orders\",\"order\":5}]}"),
                    new SectionSeedDefinition(SectionType.CallToAction, "{\"title\":\"Build Your Perfect Computer Setup Today\",\"description\":\"Visit our showroom in Tiruppur or contact us for custom PC builds,\\nbulk hardware orders, workstation configurations, gaming rigs,\\nCCTV & networking solutions, and exclusive pricing.\",\"buttonText\":\"Get Quote or Visit Us\",\"buttonHref\":\"/web-contacts\",\"label\":\"Get Quote or Visit Us\",\"href\":\"/web-contacts\"}"),
                    new SectionSeedDefinition(SectionType.Location, "{\"displayName\":\"Techmedia Retail\",\"title\":\"Our Shop\",\"address\":\"436, Avinashi Road,\\nTiruppur, Tamil Nadu 641602\",\"buttonText\":\"CONTACT US\",\"buttonHref\":\"/web-contacts\",\"imageSrc\":\"https://images.unsplash.com/photo-1517336714731-489689fd1ca8\",\"imageAlt\":\"Tech retail showroom with laptops and desktop displays\",\"imageClassName\":\"object-cover w-full h-[500px] md:h-[600px] rounded-xl\",\"mapEmbedUrl\":\"https://www.google.com/maps?q=11.108153,77.341119&z=15&output=embed\",\"mapTitle\":\"Techmedia Retail Map\",\"placeId\":\"techmedia-retail-tiruppur\",\"latitude\":11.108153,\"longitude\":77.341119,\"timings\":[{\"day\":\"Monday – Friday\",\"hours\":\"9:00 AM – 8:00 PM\",\"order\":1},{\"day\":\"Saturday\",\"hours\":\"9:00 AM – 8:00 PM\",\"order\":2},{\"day\":\"Sunday\",\"hours\":\"10:00 AM – 4:00 PM\",\"order\":3}],\"contact\":{\"phone\":\"+91 98946 44450\",\"email\":\"info@techmedia.in\"}}"),
                    new SectionSeedDefinition(SectionType.Newsletter, "{\"title\":\"Subscribe to Our Newsletter\",\"description\":\"Get the latest updates on new computer arrivals, hardware deals,\\nservice offers, networking solutions, CCTV promotions,\\nand exclusive bulk purchase discounts in Tirupur.\",\"placeholderName\":\"Your Name\",\"placeholderEmail\":\"Your Email\",\"buttonText\":\"Subscribe Now\",\"trustNote\":\"We respect your privacy. No spam. Only useful tech updates & offers.\",\"imageSrc\":\"https://images.unsplash.com/photo-1587202372775-e229f172b9d7\",\"imageAlt\":\"High-performance desktop components and workstation setup\",\"image\":\"https://images.unsplash.com/photo-1587202372775-e229f172b9d7\"}")]
            ),
            new PageSeedDefinition(
                "about",
                "About",
                "About",
                "About page",
                [
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"About Techmedia Retail\",\"subtitle\":\"Delivering Trusted IT Solutions in Tiruppur Since 2002\"}"),
                    new SectionSeedDefinition(SectionType.About, "{\"title\":\"Our Story\",\"subtitle\":\"Two decades of innovation and customer trust\",\"content\":[\"Techmedia Retail began in 2002 with a clear mission: deliver dependable computers and IT services for Tiruppur businesses and households.\",\"Over the years, we expanded from core retail to enterprise procurement, custom workstations, gaming systems, networking infrastructure, and CCTV integration.\",\"Today, we continue to grow through practical technology guidance, transparent pricing, and long-term service relationships.\"],\"image\":{\"src\":\"https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=1400&q=80&auto=format&fit=crop\",\"alt\":\"Team collaborating in a modern IT workspace\"}}"),
                    new SectionSeedDefinition(SectionType.WhyChooseUs, "{\"heading\":\"Why Businesses Trust Techmedia\",\"subheading\":\"Strong technical expertise, practical recommendations, and accountable service delivery.\",\"items\":[{\"title\":\"Enterprise IT Expertise\",\"description\":\"Consultative planning for office infrastructure, procurement, and standardized device rollouts.\",\"icon\":\"BadgeCheck\",\"order\":1},{\"title\":\"High-Performance Gaming Builds\",\"description\":\"Custom desktop and laptop recommendations tuned for sustained thermals and real-world workloads.\",\"icon\":\"Factory\",\"order\":2},{\"title\":\"Reliable Infrastructure Delivery\",\"description\":\"Structured networking, surveillance setup, and integration support for business continuity.\",\"icon\":\"Truck\",\"order\":3},{\"title\":\"Service-First Operations\",\"description\":\"Transparent support, predictable turnaround, and long-term maintenance partnerships.\",\"icon\":\"ShieldCheck\",\"order\":4}]}"),
                    new SectionSeedDefinition(SectionType.Features, "{\"title\":\"Enterprise IT | Gaming Setups | Infrastructure\",\"description\":\"Purpose-built technology services designed for performance, scale, and reliability across retail and corporate environments.\",\"imageSrc\":\"https://images.unsplash.com/photo-1593642532973-d31b6557fa68\",\"imageAlt\":\"Enterprise and gaming hardware setup\",\"bullets\":[{\"text\":\"Enterprise-grade procurement and deployment for organizations\",\"order\":1},{\"text\":\"Custom gaming systems optimized for performance and stability\",\"order\":2},{\"text\":\"Networking and CCTV infrastructure with dependable implementation\",\"order\":3},{\"text\":\"Consistent service reliability with proactive support coverage\",\"order\":4}]}"),
                    new SectionSeedDefinition(SectionType.CallToAction, "{\"title\":\"Plan Your Next IT Upgrade\",\"description\":\"Talk to our experts for workstation planning, gaming rigs, enterprise hardware rollouts, and infrastructure support.\",\"buttonText\":\"Talk to Our Team\",\"buttonHref\":\"/web-contacts\",\"label\":\"Talk to Our Team\",\"href\":\"/web-contacts\"}")]
            ),
            new PageSeedDefinition(
                "services",
                "Services",
                "Services",
                "Services page",
                [
                    new SectionSeedDefinition(SectionType.Menu, "{\"items\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"},{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}"),
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"Our Services\",\"subtitle\":\"Engineering and delivery offerings.\"}"),
                    new SectionSeedDefinition(SectionType.ProductRange, "{\"products\":[{\"name\":\"Platform Engineering\",\"description\":\"Cloud-native backend and DevOps enablement.\"},{\"name\":\"Frontend Engineering\",\"description\":\"Type-safe React architecture and design systems.\"},{\"name\":\"Data Services\",\"description\":\"Schema design, migrations, and performance tuning.\"}]}"),
                    new SectionSeedDefinition(SectionType.Testimonial, "{\"items\":[{\"author\":\"CTO\",\"quote\":\"Delivery quality and speed exceeded expectations.\"},{\"author\":\"Product Lead\",\"quote\":\"The platform is stable and easy to evolve.\"}]}"),
                    new SectionSeedDefinition(SectionType.Footer, "{\"columns\":[{\"title\":\"Services\",\"links\":[{\"label\":\"Platform Engineering\",\"href\":\"/services\"},{\"label\":\"Frontend Engineering\",\"href\":\"/services\"}]},{\"title\":\"Company\",\"links\":[{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}]}")]
            ),
            new PageSeedDefinition(
                "blog",
                "Blog",
                "Blog",
                "Blog page",
                [
                    new SectionSeedDefinition(SectionType.Menu, "{\"items\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"},{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}"),
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"Blog\",\"subtitle\":\"Insights, architecture, and release notes.\"}"),
                    new SectionSeedDefinition(SectionType.BlogShow, "{\"limit\":6,\"title\":\"Latest Posts\"}"),
                    new SectionSeedDefinition(SectionType.Newsletter, "{\"title\":\"Subscribe to Our Newsletter\",\"description\":\"Get the latest updates on new computer arrivals, hardware deals,\\nservice offers, networking solutions, CCTV promotions,\\nand exclusive bulk purchase discounts in Tirupur.\",\"placeholderName\":\"Your Name\",\"placeholderEmail\":\"Your Email\",\"buttonText\":\"Subscribe Now\",\"trustNote\":\"We respect your privacy. No spam. Only useful tech updates & offers.\",\"imageSrc\":\"https://images.unsplash.com/photo-1587202372775-e229f172b9d7\",\"imageAlt\":\"High-performance desktop components and workstation setup\",\"image\":\"https://images.unsplash.com/photo-1587202372775-e229f172b9d7\"}"),
                    new SectionSeedDefinition(SectionType.Footer, "{\"columns\":[{\"title\":\"Read\",\"links\":[{\"label\":\"Latest Posts\",\"href\":\"/blog\"},{\"label\":\"About\",\"href\":\"/about\"}]},{\"title\":\"Reach\",\"links\":[{\"label\":\"Contact\",\"href\":\"/contact\"},{\"label\":\"Services\",\"href\":\"/services\"}]}]}")]
            ),
            new PageSeedDefinition(
                "contact",
                "Contact",
                "Contact",
                "Contact page",
                [
                    new SectionSeedDefinition(SectionType.Menu, "{\"items\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"},{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}"),
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"Contact Techmedia Retail\",\"subtitle\":\"Share your requirements for custom builds, enterprise IT procurement, networking, CCTV, and service support.\"}"),
                    new SectionSeedDefinition(SectionType.Location, "{\"displayName\":\"Techmedia Retail\",\"title\":\"Our Shop\",\"address\":\"436, Avinashi Road,\\nTiruppur, Tamil Nadu 641602\",\"buttonText\":\"CONTACT US\",\"buttonHref\":\"/web-contacts\",\"imageSrc\":\"https://images.unsplash.com/photo-1581091870627-3b5de9e1e6b6\",\"imageAlt\":\"Techmedia retail storefront and service desk\",\"imageClassName\":\"object-cover w-full h-[500px] md:h-[600px] rounded-xl\",\"mapEmbedUrl\":\"https://www.google.com/maps?q=11.108153,77.341119&z=15&output=embed\",\"mapTitle\":\"Techmedia Retail Map\",\"placeId\":\"techmedia-retail-tiruppur\",\"latitude\":11.108153,\"longitude\":77.341119,\"timings\":[{\"day\":\"Monday - Friday\",\"hours\":\"9:00 AM - 8:00 PM\",\"order\":1},{\"day\":\"Saturday\",\"hours\":\"9:00 AM - 8:00 PM\",\"order\":2},{\"day\":\"Sunday\",\"hours\":\"10:00 AM - 4:00 PM\",\"order\":3}],\"contact\":{\"phone\":\"+91 98946 44450\",\"email\":\"info@techmedia.in\"}}")]
            )
        ];
    }

    private static void SyncSections(Page page, IReadOnlyList<SectionSeedDefinition> definitions, DateTimeOffset now)
    {
        var activeSections = page.Sections
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.CreatedAtUtc)
            .ToList();

        var hasDuplicateDisplayOrders = activeSections
            .GroupBy(x => x.DisplayOrder)
            .Any(group => group.Count() > 1);

        var hasShapeMismatch =
            activeSections.Count != definitions.Count ||
            activeSections.Any(x => x.DisplayOrder < 0 || x.DisplayOrder >= definitions.Count) ||
            activeSections.Any(x => definitions[x.DisplayOrder].SectionType != x.SectionType);

        if (hasDuplicateDisplayOrders || hasShapeMismatch)
        {
            foreach (var staleSection in activeSections
                         .OrderByDescending(x => x.DisplayOrder)
                         .ThenByDescending(x => x.CreatedAtUtc))
            {
                page.RemoveSection(staleSection.Id, now);
            }

            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                page.AddSection(
                    Guid.NewGuid(),
                    definition.SectionType,
                    i,
                    JsonDocument.Parse(definition.SectionDataJson),
                    true,
                    now);
            }

            return;
        }

        for (var i = 0; i < definitions.Count; i++)
        {
            var definition = definitions[i];
            var existing = activeSections[i];
            page.UpdateSection(existing.Id, i, JsonDocument.Parse(definition.SectionDataJson), true, now);
        }
    }

    private sealed record PageSeedDefinition(
        string Slug,
        string Title,
        string SeoTitle,
        string SeoDescription,
        IReadOnlyList<SectionSeedDefinition> Sections);

    private sealed record SectionSeedDefinition(
        SectionType SectionType,
        string SectionDataJson);
}

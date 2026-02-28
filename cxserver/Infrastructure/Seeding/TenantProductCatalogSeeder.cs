using cxserver.Domain.ProductCatalog;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Seeding;

internal sealed class TenantProductCatalogSeeder
{
    private readonly Application.Abstractions.IDateTimeProvider _dateTimeProvider;

    public TenantProductCatalogSeeder(Application.Abstractions.IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SeedAsync(TenantDbContext dbContext, Guid tenantId, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;

        var categoryDefinitions = BuildCategoryDefinitions();
        var productDefinitions = BuildProductDefinitions();

        var existingCategories = await dbContext.Categories
            .AsTracking()
            .Where(x => x.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        var categoryMap = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);
        foreach (var definition in categoryDefinitions)
        {
            var category = existingCategories.FirstOrDefault(x => x.Slug == definition.Slug);
            if (category is null)
            {
                category = Category.Create(
                    Guid.NewGuid(),
                    tenantId,
                    definition.Name,
                    definition.Slug,
                    null,
                    definition.Order);
                dbContext.Categories.Add(category);
                existingCategories.Add(category);
            }
            else
            {
                category.Update(definition.Name, definition.Slug, null, definition.Order);
            }

            categoryMap[definition.Slug] = category;
        }

        var existingProducts = await dbContext.Products
            .Include(x => x.Images)
            .Include(x => x.Attributes)
            .AsTracking()
            .Where(x => x.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        foreach (var definition in productDefinitions)
        {
            if (!categoryMap.TryGetValue(definition.CategorySlug, out var category))
            {
                continue;
            }

            var product = existingProducts.FirstOrDefault(x => x.Slug == definition.Slug);
            if (product is null)
            {
                product = Product.Create(
                    Guid.NewGuid(),
                    tenantId,
                    definition.Name,
                    definition.Slug,
                    definition.Description,
                    definition.ShortDescription,
                    definition.Price,
                    definition.ComparePrice,
                    definition.Sku,
                    definition.StockQuantity,
                    definition.IsActive,
                    category.Id,
                    now);

                dbContext.Products.Add(product);
                existingProducts.Add(product);
            }
            else
            {
                product.Update(
                    definition.Name,
                    definition.Slug,
                    definition.Description,
                    definition.ShortDescription,
                    definition.Price,
                    definition.ComparePrice,
                    definition.Sku,
                    definition.StockQuantity,
                    definition.IsActive,
                    category.Id);
            }

            dbContext.ProductImages.RemoveRange(product.Images);
            dbContext.ProductAttributes.RemoveRange(product.Attributes);

            foreach (var image in definition.Images.Select((url, index) => new { Url = url, Index = index }))
            {
                dbContext.ProductImages.Add(ProductImage.Create(
                    Guid.NewGuid(),
                    product.Id,
                    image.Url,
                    image.Index));
            }

            foreach (var attribute in definition.Attributes)
            {
                dbContext.ProductAttributes.Add(ProductAttribute.Create(
                    Guid.NewGuid(),
                    product.Id,
                    attribute.Key,
                    attribute.Value));
            }
        }

        var productSlugSet = productDefinitions.Select(x => x.Slug).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var staleProducts = existingProducts.Where(x => !productSlugSet.Contains(x.Slug)).ToList();
        if (staleProducts.Count > 0)
        {
            dbContext.Products.RemoveRange(staleProducts);
        }

        var categorySlugSet = categoryDefinitions.Select(x => x.Slug).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var staleCategories = existingCategories.Where(x => !categorySlugSet.Contains(x.Slug)).ToList();
        if (staleCategories.Count > 0)
        {
            dbContext.Categories.RemoveRange(staleCategories);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static List<CategorySeed> BuildCategoryDefinitions()
    {
        return
        [
            new CategorySeed("Laptops", "laptops", 1),
            new CategorySeed("Desktops", "desktops", 2),
            new CategorySeed("Computer Spares", "computer-spares", 3),
            new CategorySeed("Monitors", "monitors", 4),
            new CategorySeed("Networking", "networking", 5),
            new CategorySeed("Printers", "printers", 6),
        ];
    }

    private static List<ProductSeed> BuildProductDefinitions()
    {
        return
        [
            new ProductSeed("Dell Latitude 5440", "dell-latitude-5440", "Business laptop with long battery life and security features.", "Business-ready 14-inch laptop", 74999m, 81999m, "LAP-001", 24, true, "laptops", ["https://images.unsplash.com/photo-1496181133206-80ce9b88a853"], [new ProductAttributeSeed("brand", "Dell"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("HP ProBook 440", "hp-probook-440", "Professional productivity laptop for office workflows.", "Durable corporate laptop", 68999m, 73999m, "LAP-002", 18, true, "laptops", ["https://images.unsplash.com/photo-1517336714731-489689fd1ca8"], [new ProductAttributeSeed("brand", "HP"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("Lenovo ThinkPad E14", "lenovo-thinkpad-e14", "Reliable performance with enterprise-grade keyboard and battery.", "Enterprise-class ThinkPad", 71999m, 76999m, "LAP-003", 20, true, "laptops", ["https://images.unsplash.com/photo-1588872657578-7efd1f1555ed"], [new ProductAttributeSeed("brand", "Lenovo"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "1TB SSD")]),
            new ProductSeed("ASUS ROG Strix G16", "asus-rog-strix-g16", "High-refresh gaming laptop with advanced cooling.", "Gaming laptop 240Hz", 149999m, 159999m, "LAP-004", 12, true, "laptops", ["https://images.unsplash.com/photo-1603302576837-37561b2e2302"], [new ProductAttributeSeed("brand", "ASUS"), new ProductAttributeSeed("ram", "32GB"), new ProductAttributeSeed("gpu", "RTX 4070")]),
            new ProductSeed("Acer Nitro 5", "acer-nitro-5", "Balanced gaming performance for mainstream creators and gamers.", "Gaming laptop RTX 4060", 99999m, 109999m, "LAP-005", 15, true, "laptops", ["https://images.unsplash.com/photo-1611078489935-0cb964de46d6"], [new ProductAttributeSeed("brand", "Acer"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("gpu", "RTX 4060")]),
            new ProductSeed("Dell Inspiron 15 3530", "dell-inspiron-15-3530", "Everyday laptop for office and study with reliable Intel performance.", "15-inch productivity laptop", 58999m, 63999m, "LAP-006", 22, true, "laptops", ["https://images.unsplash.com/photo-1515879218367-8466d910aaa4"], [new ProductAttributeSeed("brand", "Dell"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("HP Pavilion 15", "hp-pavilion-15", "Modern laptop for multitasking and entertainment workloads.", "Slim 15-inch laptop", 62999m, 67999m, "LAP-007", 19, true, "laptops", ["https://images.unsplash.com/photo-1517336714731-489689fd1ca8"], [new ProductAttributeSeed("brand", "HP"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("Lenovo LOQ 15", "lenovo-loq-15", "Performance laptop tuned for creators and gamers.", "Gaming-ready laptop", 104999m, 112999m, "LAP-008", 13, true, "laptops", ["https://images.unsplash.com/photo-1496181133206-80ce9b88a853"], [new ProductAttributeSeed("brand", "Lenovo"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("gpu", "RTX 4060")]),
            new ProductSeed("ASUS Vivobook 16", "asus-vivobook-16", "Large-screen portable laptop for daily productivity.", "16-inch thin and light", 66999m, 71999m, "LAP-009", 17, true, "laptops", ["https://images.unsplash.com/photo-1593642532973-d31b6557fa68"], [new ProductAttributeSeed("brand", "ASUS"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "1TB SSD")]),
            new ProductSeed("Acer Aspire 7", "acer-aspire-7", "Balanced laptop for work, coding, and multimedia.", "Performance notebook", 73999m, 78999m, "LAP-010", 16, true, "laptops", ["https://images.unsplash.com/photo-1611078489935-0cb964de46d6"], [new ProductAttributeSeed("brand", "Acer"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("Dell OptiPlex 7010", "dell-optiplex-7010", "Compact desktop for office and business productivity.", "Office desktop", 54999m, 59999m, "DESK-001", 28, true, "desktops", ["https://images.unsplash.com/photo-1591488320449-011701bb6704"], [new ProductAttributeSeed("brand", "Dell"), new ProductAttributeSeed("ram", "8GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("HP EliteDesk 800", "hp-elitedesk-800", "Enterprise desktop with expandability and reliable uptime.", "Business desktop tower", 62999m, 69999m, "DESK-002", 16, true, "desktops", ["https://images.unsplash.com/photo-1587202372616-b43abea06c2a"], [new ProductAttributeSeed("brand", "HP"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "1TB SSD")]),
            new ProductSeed("Lenovo IdeaCentre 5", "lenovo-ideacentre-5", "Everyday desktop for productivity and content workflows.", "Home and office desktop", 57999m, 62999m, "DESK-003", 14, true, "desktops", ["https://images.unsplash.com/photo-1547082299-de196ea013d6"], [new ProductAttributeSeed("brand", "Lenovo"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("ASUS ExpertCenter D7", "asus-expertcenter-d7", "Business desktop designed for reliability and easy service.", "Corporate desktop tower", 71999m, 77999m, "DESK-004", 12, true, "desktops", ["https://images.unsplash.com/photo-1587202372775-e229f172b9d7"], [new ProductAttributeSeed("brand", "ASUS"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "1TB SSD")]),
            new ProductSeed("Acer Veriton M", "acer-veriton-m", "Office-focused desktop optimized for long duty cycles.", "Compact business desktop", 50999m, 55999m, "DESK-005", 15, true, "desktops", ["https://images.unsplash.com/photo-1625842268584-8f3296236761"], [new ProductAttributeSeed("brand", "Acer"), new ProductAttributeSeed("ram", "8GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("Custom Gaming Tower X", "custom-gaming-tower-x", "Custom gaming desktop tuned for airflow and stable FPS.", "Custom build RTX 4080", 189999m, 199999m, "DESK-006", 8, true, "desktops", ["https://images.unsplash.com/photo-1624705002806-5d72df19c3ad"], [new ProductAttributeSeed("brand", "Custom"), new ProductAttributeSeed("ram", "32GB"), new ProductAttributeSeed("gpu", "RTX 4080")]),
            new ProductSeed("Corsair Vengeance RAM 16GB", "corsair-vengeance-16gb-ddr5", "High-speed DDR5 memory kit for desktop upgrades.", "DDR5 16GB memory kit", 6499m, 7499m, "SPR-001", 60, true, "computer-spares", ["https://images.unsplash.com/photo-1562976540-1502c2145186"], [new ProductAttributeSeed("brand", "Corsair"), new ProductAttributeSeed("type", "RAM"), new ProductAttributeSeed("capacity", "16GB")]),
            new ProductSeed("Kingston Fury RAM 32GB", "kingston-fury-32gb-ddr5", "Dual-channel memory upgrade for gaming and creator rigs.", "DDR5 32GB memory kit", 12499m, 13999m, "SPR-002", 42, true, "computer-spares", ["https://images.unsplash.com/photo-1518770660439-4636190af475"], [new ProductAttributeSeed("brand", "Kingston"), new ProductAttributeSeed("type", "RAM"), new ProductAttributeSeed("capacity", "32GB")]),
            new ProductSeed("Samsung 980 Pro 1TB SSD", "samsung-980-pro-1tb-ssd", "NVMe SSD for fast boot and application load times.", "1TB NVMe Gen4 SSD", 9999m, 11499m, "SPR-003", 55, true, "computer-spares", ["https://images.unsplash.com/photo-1591799265444-d66432b91588"], [new ProductAttributeSeed("brand", "Samsung"), new ProductAttributeSeed("type", "Storage"), new ProductAttributeSeed("capacity", "1TB")]),
            new ProductSeed("WD Blue 2TB HDD", "wd-blue-2tb-hdd", "Reliable hard drive for bulk storage and backups.", "2TB SATA hard drive", 5499m, 6299m, "SPR-004", 70, true, "computer-spares", ["https://images.unsplash.com/photo-1531492746076-161ca9bcad58"], [new ProductAttributeSeed("brand", "Western Digital"), new ProductAttributeSeed("type", "Storage"), new ProductAttributeSeed("capacity", "2TB")]),
            new ProductSeed("Seagate Barracuda 1TB HDD", "seagate-barracuda-1tb-hdd", "Entry-level internal drive for desktop systems.", "1TB SATA hard drive", 3999m, 4699m, "SPR-005", 64, true, "computer-spares", ["https://images.unsplash.com/photo-1597852074816-d933c7d2b988"], [new ProductAttributeSeed("brand", "Seagate"), new ProductAttributeSeed("type", "Storage"), new ProductAttributeSeed("capacity", "1TB")]),
            new ProductSeed("NVIDIA RTX 4060 GPU", "nvidia-rtx-4060-gpu", "Next-gen graphics card for high-FPS gaming and creators.", "8GB desktop graphics card", 34999m, 37999m, "SPR-006", 22, true, "computer-spares", ["https://images.unsplash.com/photo-1591799265444-d66432b91588"], [new ProductAttributeSeed("brand", "NVIDIA"), new ProductAttributeSeed("type", "GPU"), new ProductAttributeSeed("memory", "8GB")]),
            new ProductSeed("AMD Radeon RX 7600 GPU", "amd-radeon-rx-7600-gpu", "Efficient graphics card with strong 1080p gaming performance.", "8GB desktop graphics card", 28999m, 31999m, "SPR-007", 19, true, "computer-spares", ["https://images.unsplash.com/photo-1587202372775-e229f172b9d7"], [new ProductAttributeSeed("brand", "AMD"), new ProductAttributeSeed("type", "GPU"), new ProductAttributeSeed("memory", "8GB")]),
            new ProductSeed("Intel Core i7-14700 Processor", "intel-core-i7-14700", "High-core-count processor for productivity and gaming.", "Desktop CPU", 36999m, 39999m, "SPR-008", 26, true, "computer-spares", ["https://images.unsplash.com/photo-1518770660439-4636190af475"], [new ProductAttributeSeed("brand", "Intel"), new ProductAttributeSeed("type", "Processor"), new ProductAttributeSeed("series", "Core i7")]),
            new ProductSeed("AMD Ryzen 7 7800X3D Processor", "amd-ryzen-7-7800x3d", "Gaming-focused processor with high cache and efficiency.", "Desktop CPU", 38999m, 42999m, "SPR-009", 24, true, "computer-spares", ["https://images.unsplash.com/photo-1562976540-1502c2145186"], [new ProductAttributeSeed("brand", "AMD"), new ProductAttributeSeed("type", "Processor"), new ProductAttributeSeed("series", "Ryzen 7")]),
            new ProductSeed("Dell XPS 14", "dell-xps-14", "Premium aluminum laptop with high-resolution display and long battery backup.", "Premium creator ultrabook", 154999m, 164999m, "LAP-011", 9, true, "laptops", ["https://images.unsplash.com/photo-1593642702821-c8da6771f0c6"], [new ProductAttributeSeed("brand", "Dell"), new ProductAttributeSeed("ram", "32GB"), new ProductAttributeSeed("storage", "1TB SSD")]),
            new ProductSeed("HP Envy 14", "hp-envy-14", "Performance ultrabook built for content teams and mobile productivity.", "Creator ultrabook", 132999m, 142999m, "LAP-012", 11, true, "laptops", ["https://images.unsplash.com/photo-1593642532871-8b12e02d091c"], [new ProductAttributeSeed("brand", "HP"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "1TB SSD")]),
            new ProductSeed("Lenovo Yoga 7i", "lenovo-yoga-7i", "Convertible touch laptop for business presentations and travel.", "2-in-1 business laptop", 104999m, 112999m, "LAP-013", 14, true, "laptops", ["https://images.unsplash.com/photo-1498050108023-c5249f4df085"], [new ProductAttributeSeed("brand", "Lenovo"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("ASUS TUF A15", "asus-tuf-a15", "Durable gaming laptop with balanced thermals and high refresh panel.", "Gaming laptop 144Hz", 109999m, 117999m, "LAP-014", 10, true, "laptops", ["https://images.unsplash.com/photo-1517336714731-489689fd1ca8"], [new ProductAttributeSeed("brand", "ASUS"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("gpu", "RTX 4060")]),
            new ProductSeed("Dell Vostro Tower", "dell-vostro-tower", "SMB-focused desktop tower with expansion for office growth.", "SMB tower desktop", 63999m, 68999m, "DESK-007", 13, true, "desktops", ["https://images.unsplash.com/photo-1547082299-de196ea013d6"], [new ProductAttributeSeed("brand", "Dell"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "1TB SSD")]),
            new ProductSeed("HP ProDesk 400", "hp-prodesk-400", "Cost-efficient business desktop for stable daily operations.", "Business desktop", 58999m, 63999m, "DESK-008", 17, true, "desktops", ["https://images.unsplash.com/photo-1591488320449-011701bb6704"], [new ProductAttributeSeed("brand", "HP"), new ProductAttributeSeed("ram", "8GB"), new ProductAttributeSeed("storage", "512GB SSD")]),
            new ProductSeed("Lenovo ThinkCentre M70", "lenovo-thinkcentre-m70", "Reliable desktop platform for enterprise rollouts and support.", "Enterprise desktop", 66999m, 71999m, "DESK-009", 12, true, "desktops", ["https://images.unsplash.com/photo-1625842268584-8f3296236761"], [new ProductAttributeSeed("brand", "Lenovo"), new ProductAttributeSeed("ram", "16GB"), new ProductAttributeSeed("storage", "1TB SSD")]),
            new ProductSeed("MSI Aegis Gaming Desktop", "msi-aegis-gaming-desktop", "Factory-built gaming desktop with advanced cooling and RGB.", "Gaming desktop RTX", 179999m, 189999m, "DESK-010", 7, true, "desktops", ["https://images.unsplash.com/photo-1587202372775-e229f172b9d7"], [new ProductAttributeSeed("brand", "MSI"), new ProductAttributeSeed("ram", "32GB"), new ProductAttributeSeed("gpu", "RTX 4070")]),
            new ProductSeed("MSI B760M Motherboard", "msi-b760m-motherboard", "Micro-ATX motherboard for modern Intel processors.", "Intel motherboard", 14499m, 15999m, "SPR-010", 35, true, "computer-spares", ["https://images.unsplash.com/photo-1518770660439-4636190af475"], [new ProductAttributeSeed("brand", "MSI"), new ProductAttributeSeed("type", "Motherboard"), new ProductAttributeSeed("socket", "LGA1700")]),
            new ProductSeed("ASUS B650M Motherboard", "asus-b650m-motherboard", "AM5 motherboard designed for latest Ryzen desktop builds.", "AMD motherboard", 15999m, 17499m, "SPR-011", 31, true, "computer-spares", ["https://images.unsplash.com/photo-1562976540-1502c2145186"], [new ProductAttributeSeed("brand", "ASUS"), new ProductAttributeSeed("type", "Motherboard"), new ProductAttributeSeed("socket", "AM5")]),
            new ProductSeed("Cooler Master 750W PSU", "cooler-master-750w-psu", "80+ Gold modular PSU for performance desktop systems.", "750W modular PSU", 8999m, 9999m, "SPR-012", 34, true, "computer-spares", ["https://images.unsplash.com/photo-1587202372616-b43abea06c2a"], [new ProductAttributeSeed("brand", "Cooler Master"), new ProductAttributeSeed("type", "PSU"), new ProductAttributeSeed("wattage", "750W")]),
            new ProductSeed("Corsair 850W PSU", "corsair-850w-psu", "High-headroom power supply for GPU intensive systems.", "850W modular PSU", 11499m, 12699m, "SPR-013", 29, true, "computer-spares", ["https://images.unsplash.com/photo-1587202372616-b43abea06c2a"], [new ProductAttributeSeed("brand", "Corsair"), new ProductAttributeSeed("type", "PSU"), new ProductAttributeSeed("wattage", "850W")]),
            new ProductSeed("DeepCool AK620 Air Cooler", "deepcool-ak620-air-cooler", "Dual-tower air cooler for sustained CPU boost clocks.", "High performance air cooler", 6499m, 7499m, "SPR-014", 37, true, "computer-spares", ["https://images.unsplash.com/photo-1587202372616-b43abea06c2a"], [new ProductAttributeSeed("brand", "DeepCool"), new ProductAttributeSeed("type", "Cooling"), new ProductAttributeSeed("size", "Dual Tower")]),
            new ProductSeed("NZXT H5 Flow Cabinet", "nzxt-h5-flow-cabinet", "Airflow-focused cabinet with clean cable management.", "Mid-tower cabinet", 7999m, 8999m, "SPR-015", 23, true, "computer-spares", ["https://images.unsplash.com/photo-1587202372775-e229f172b9d7"], [new ProductAttributeSeed("brand", "NZXT"), new ProductAttributeSeed("type", "Cabinet"), new ProductAttributeSeed("formFactor", "ATX Mid Tower")]),
            new ProductSeed("Logitech Mechanical Keyboard", "logitech-mechanical-keyboard", "Tactile mechanical keyboard for gaming and productivity.", "Mechanical keyboard", 6499m, 7299m, "SPR-016", 44, true, "computer-spares", ["https://images.unsplash.com/photo-1511467687858-23d96c32e4ae"], [new ProductAttributeSeed("brand", "Logitech"), new ProductAttributeSeed("type", "Peripheral"), new ProductAttributeSeed("switch", "Mechanical")]),
            new ProductSeed("Razer Gaming Mouse", "razer-gaming-mouse", "Ergonomic high-DPI mouse with programmable buttons.", "Gaming mouse", 3999m, 4699m, "SPR-017", 52, true, "computer-spares", ["https://images.unsplash.com/photo-1527814050087-3793815479db"], [new ProductAttributeSeed("brand", "Razer"), new ProductAttributeSeed("type", "Peripheral"), new ProductAttributeSeed("dpi", "20000 DPI")]),
            new ProductSeed("TP-Link USB WiFi Adapter", "tp-link-usb-wifi-adapter", "Compact dual-band USB adapter for laptops and desktops.", "USB WiFi adapter", 1799m, 2299m, "SPR-018", 58, true, "computer-spares", ["https://images.unsplash.com/photo-1518770660439-4636190af475"], [new ProductAttributeSeed("brand", "TP-Link"), new ProductAttributeSeed("type", "Networking"), new ProductAttributeSeed("band", "Dual Band")]),
            new ProductSeed("Antec 120mm Case Fan Kit", "antec-120mm-case-fan-kit", "Silent airflow fan kit to improve cabinet thermals.", "Case fan kit", 2299m, 2799m, "SPR-019", 46, true, "computer-spares", ["https://images.unsplash.com/photo-1587202372616-b43abea06c2a"], [new ProductAttributeSeed("brand", "Antec"), new ProductAttributeSeed("type", "Cooling"), new ProductAttributeSeed("size", "120mm")]),
            new ProductSeed("LG UltraGear 27GN800", "lg-ultragear-27gn800", "27-inch IPS gaming monitor with 144Hz smoothness.", "27-inch 144Hz monitor", 23999m, 26999m, "MON-001", 21, true, "monitors", ["https://images.unsplash.com/photo-1527443224154-c4a3942d3acf"], [new ProductAttributeSeed("brand", "LG"), new ProductAttributeSeed("size", "27-inch"), new ProductAttributeSeed("refresh", "144Hz")]),
            new ProductSeed("Samsung Odyssey G5 32", "samsung-odyssey-g5-32", "Curved QHD monitor for immersive gameplay and editing.", "32-inch curved monitor", 28999m, 31999m, "MON-002", 18, true, "monitors", ["https://images.unsplash.com/photo-1546538915-a9e2c8d8c0df"], [new ProductAttributeSeed("brand", "Samsung"), new ProductAttributeSeed("size", "32-inch"), new ProductAttributeSeed("refresh", "165Hz")]),
            new ProductSeed("Dell UltraSharp U2723QE", "dell-ultrasharp-u2723qe", "Color-accurate 4K productivity monitor for creators.", "27-inch 4K monitor", 46999m, 50999m, "MON-003", 9, true, "monitors", ["https://images.unsplash.com/photo-1586210579191-33b45e38fa2c"], [new ProductAttributeSeed("brand", "Dell"), new ProductAttributeSeed("size", "27-inch"), new ProductAttributeSeed("refresh", "60Hz")]),
            new ProductSeed("ASUS ProArt PA279CV", "asus-proart-pa279cv", "Factory-calibrated monitor for design and media workflows.", "Professional creator monitor", 51999m, 55999m, "MON-004", 8, true, "monitors", ["https://images.unsplash.com/photo-1611174743420-3d7df880ce32"], [new ProductAttributeSeed("brand", "ASUS"), new ProductAttributeSeed("size", "27-inch"), new ProductAttributeSeed("refresh", "60Hz")]),
            new ProductSeed("TP-Link Archer AX55 Router", "tp-link-archer-ax55-router", "Wi-Fi 6 router for homes and small offices.", "Dual-band Wi-Fi 6 router", 8999m, 9999m, "NET-001", 27, true, "networking", ["https://images.unsplash.com/photo-1544197150-b99a580bb7a8"], [new ProductAttributeSeed("brand", "TP-Link"), new ProductAttributeSeed("type", "Router"), new ProductAttributeSeed("wifi", "Wi-Fi 6")]),
            new ProductSeed("D-Link 24 Port Gigabit Switch", "d-link-24-port-gigabit-switch", "Managed gigabit switch for office network expansion.", "24-port network switch", 12999m, 14499m, "NET-002", 16, true, "networking", ["https://images.unsplash.com/photo-1544197150-b99a580bb7a8"], [new ProductAttributeSeed("brand", "D-Link"), new ProductAttributeSeed("type", "Switch"), new ProductAttributeSeed("ports", "24")]),
            new ProductSeed("Epson EcoTank L3250", "epson-ecotank-l3250", "All-in-one printer with low-cost refill tank system.", "Ink tank all-in-one printer", 16999m, 18499m, "PRN-001", 14, true, "printers", ["https://images.unsplash.com/photo-1580894732444-8ecded7900cd"], [new ProductAttributeSeed("brand", "Epson"), new ProductAttributeSeed("type", "Printer"), new ProductAttributeSeed("connectivity", "Wi-Fi")]),
        ];
    }
    private sealed record CategorySeed(string Name, string Slug, int Order);

    private sealed record ProductSeed(
        string Name,
        string Slug,
        string Description,
        string ShortDescription,
        decimal Price,
        decimal? ComparePrice,
        string Sku,
        int StockQuantity,
        bool IsActive,
        string CategorySlug,
        IReadOnlyList<string> Images,
        IReadOnlyList<ProductAttributeSeed> Attributes);

    private sealed record ProductAttributeSeed(string Key, string Value);
}

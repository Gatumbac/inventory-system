using Inventory.Core.Abstractions;
using Inventory.Core.Constants;
using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inventory.Infrastructure.Persistence;

public static class DbSeeder
{
    public static void Seed(DbContext context, IConfiguration configuration, IPasswordHasher hasher)
    {
        var adminEmail = GetSetting(configuration, "Seed:AdminEmail");
        var adminPassword = GetSetting(configuration, "Seed:AdminPassword");
        var viewerEmail = GetSetting(configuration, "Seed:ViewerEmail");
        var viewerPassword = GetSetting(configuration, "Seed:ViewerPassword");

        EnsureUser(context, hasher, "Admin", "System", adminEmail, adminPassword, RoleNames.Admin);
        EnsureUser(context, hasher, "Viewer", "System", viewerEmail, viewerPassword, RoleNames.Viewer);

        SeedSuppliers(context);
        SeedProducts(context);
        SeedProductSuppliers(context);
        SeedProductAttributes(context);
    }

    private static void SeedSuppliers(DbContext context)
    {
        EnsureSupplier(context, "Distribuidora Andina", "Electrónica y accesorios");
        EnsureSupplier(context, "Importadora del Pacifico", "Artículos para el hogar y la oficina");
        EnsureSupplier(context, "Suministros Guayaquil", "Ferretería y herramientas");
        EnsureSupplier(context, "Tecnoimport Quito", "Equipos de cómputo y redes");
        EnsureSupplier(context, "Comercial Costa Verde", "Papelería y útiles de oficina");
        EnsureSupplier(context, "Electro Ecuador", "Accesorios electrónicos y energía");
        EnsureSupplier(context, "Megaprint Manta", "Impresoras, tóneres y suministros de impresión");
        EnsureSupplier(context, "Ofimundo Cuenca", "Suministros y mobiliario de oficina");
        EnsureSupplier(context, "Ferretería El Constructor", "Herramientas eléctricas y manuales");
        EnsureSupplier(context, "Muebles Andinos", "Mobiliario para oficina y salas de reunión");
    }

    private static void SeedProducts(DbContext context)
    {
        EnsureProduct(context, "LAP-001", "Laptop 14\"", "Laptop de 14 pulgadas para uso de oficina");
        EnsureProduct(context, "IMP-001", "Impresora multifunción", "Impresora láser con escáner y copiadora");
        EnsureProduct(context, "TEC-001", "Teclado inalámbrico", "Teclado Bluetooth en español");
        EnsureProduct(context, "SIL-001", "Silla ergonómica", "Silla de oficina con soporte lumbar");
        EnsureProduct(context, "TAL-001", "Taladro percutor", "Taladro eléctrico de 800 W con maletín");
        EnsureProduct(context, "LAP-002", "Laptop 15\"", "Laptop de 15.6 pulgadas para diseño y desarrollo");
        EnsureProduct(context, "MON-001", "Monitor 27\" 4K", "Monitor IPS de 27 pulgadas con resolución 4K");
        EnsureProduct(context, "MOU-001", "Mouse inalámbrico", "Mouse ergonómico con receptor USB");
        EnsureProduct(context, "DIS-001", "Disco SSD 1 TB", "Unidad de estado sólido NVMe");
        EnsureProduct(context, "ROU-001", "Router Wi-Fi 6", "Router de doble banda para oficina");
        EnsureProduct(context, "CAM-001", "Cámara web HD", "Cámara 1080p con micrófono integrado");
        EnsureProduct(context, "AUD-001", "Audífonos con micrófono", "Audífonos USB para videollamadas");
        EnsureProduct(context, "UPS-001", "UPS 1000 VA", "Respaldo de energía para estaciones de trabajo");
        EnsureProduct(context, "PRY-001", "Proyector Full HD", "Proyector de 3500 lúmenes para salas de reunión");
        EnsureProduct(context, "TON-001", "Tóner negro", "Cartucho de tóner compatible con impresoras láser");
        EnsureProduct(context, "IMP-002", "Impresora de tinta", "Impresora a color con sistema de tinta continua");
        EnsureProduct(context, "PAP-001", "Resma de papel A4", "Resma de 500 hojas de 75 g");
        EnsureProduct(context, "BOL-001", "Bolígrafos azules (caja)", "Caja de 12 bolígrafos de punta media");
        EnsureProduct(context, "GRA-001", "Grapadora de escritorio", "Grapadora metálica para 25 hojas");
        EnsureProduct(context, "PIZ-001", "Pizarra acrílica", "Pizarra blanca de 120 x 90 cm");
        EnsureProduct(context, "ESC-001", "Escritorio de oficina", "Escritorio de melamina de 120 cm");
        EnsureProduct(context, "ARC-001", "Archivador metálico", "Archivador de 4 gavetas con cerradura");
        EnsureProduct(context, "SIL-002", "Silla de visita", "Silla apilable tapizada");
        EnsureProduct(context, "DES-001", "Juego de destornilladores", "Set de 12 destornilladores de precisión");
        EnsureProduct(context, "ESM-001", "Esmeril angular", "Esmeril de 4 1/2 pulgadas y 850 W");
        EnsureProduct(context, "EXT-001", "Extensión eléctrica 5 m", "Regleta de 6 tomas con protección");
    }

    private static void SeedProductSuppliers(DbContext context)
    {
        EnsureProductSupplier(context, "LAP-001", "Tecnoimport Quito", "L-2026-01", 750.00m, 12);
        EnsureProductSupplier(context, "LAP-001", "Distribuidora Andina", "L-2026-02", 780.00m, 5);
        EnsureProductSupplier(context, "IMP-001", "Distribuidora Andina", "L-2026-01", 210.50m, 20);
        EnsureProductSupplier(context, "SIL-001", "Importadora del Pacifico", "L-2026-03", 120.00m, 8);
        EnsureProductSupplier(context, "TAL-001", "Suministros Guayaquil", "L-2026-01", 89.90m, 15);
        EnsureProductSupplier(context, "TAL-001", "Ferretería El Constructor", "L-2026-01", 92.50m, 6);
        EnsureProductSupplier(context, "IMP-001", "Megaprint Manta", "L-2026-01", 205.00m, 9);
        EnsureProductSupplier(context, "TEC-001", "Distribuidora Andina", "L-2026-04", 35.00m, 18);
        EnsureProductSupplier(context, "TEC-001", "Electro Ecuador", "L-2026-01", 32.50m, 4);
        EnsureProductSupplier(context, "LAP-002", "Tecnoimport Quito", "L-2026-02", 1150.00m, 4);
        EnsureProductSupplier(context, "LAP-002", "Distribuidora Andina", "L-2026-05", 1199.00m, 7);
        EnsureProductSupplier(context, "MON-001", "Tecnoimport Quito", "L-2026-03", 320.00m, 10);
        EnsureProductSupplier(context, "MON-001", "Distribuidora Andina", "L-2026-06", 345.00m, 6);
        EnsureProductSupplier(context, "MON-001", "Electro Ecuador", "L-2026-02", 299.90m, 3);
        EnsureProductSupplier(context, "MOU-001", "Distribuidora Andina", "L-2026-07", 18.50m, 45);
        EnsureProductSupplier(context, "MOU-001", "Tecnoimport Quito", "L-2026-04", 16.90m, 30);
        EnsureProductSupplier(context, "MOU-001", "Electro Ecuador", "L-2026-03", 19.99m, 22);
        EnsureProductSupplier(context, "DIS-001", "Tecnoimport Quito", "L-2026-05", 85.00m, 25);
        EnsureProductSupplier(context, "DIS-001", "Electro Ecuador", "L-2026-04", 79.90m, 18);
        EnsureProductSupplier(context, "ROU-001", "Tecnoimport Quito", "L-2026-06", 95.00m, 9);
        EnsureProductSupplier(context, "ROU-001", "Distribuidora Andina", "L-2026-08", 102.50m, 2);
        EnsureProductSupplier(context, "CAM-001", "Distribuidora Andina", "L-2026-09", 42.00m, 16);
        EnsureProductSupplier(context, "CAM-001", "Electro Ecuador", "L-2026-05", 39.50m, 11);
        EnsureProductSupplier(context, "AUD-001", "Electro Ecuador", "L-2026-06", 28.00m, 1);
        EnsureProductSupplier(context, "AUD-001", "Distribuidora Andina", "L-2026-10", 31.00m, 14);
        EnsureProductSupplier(context, "UPS-001", "Electro Ecuador", "L-2026-07", 115.00m, 0);
        EnsureProductSupplier(context, "UPS-001", "Tecnoimport Quito", "L-2026-07", 120.00m, 6);
        EnsureProductSupplier(context, "TON-001", "Megaprint Manta", "L-2026-02", 58.00m, 30);
        EnsureProductSupplier(context, "TON-001", "Distribuidora Andina", "L-2026-11", 62.00m, 12);
        EnsureProductSupplier(context, "TON-001", "Comercial Costa Verde", "L-2026-01", 60.50m, 8);
        EnsureProductSupplier(context, "IMP-002", "Megaprint Manta", "L-2026-03", 189.00m, 6);
        EnsureProductSupplier(context, "IMP-002", "Importadora del Pacifico", "L-2026-04", 199.90m, 3);
        EnsureProductSupplier(context, "PAP-001", "Comercial Costa Verde", "L-2026-02", 4.75m, 120);
        EnsureProductSupplier(context, "PAP-001", "Ofimundo Cuenca", "L-2026-01", 4.50m, 200);
        EnsureProductSupplier(context, "PAP-001", "Megaprint Manta", "L-2026-04", 4.90m, 80);
        EnsureProductSupplier(context, "BOL-001", "Comercial Costa Verde", "L-2026-03", 3.20m, 60);
        EnsureProductSupplier(context, "BOL-001", "Ofimundo Cuenca", "L-2026-02", 2.95m, 85);
        EnsureProductSupplier(context, "GRA-001", "Ofimundo Cuenca", "L-2026-03", 7.80m, 2);
        EnsureProductSupplier(context, "GRA-001", "Comercial Costa Verde", "L-2026-04", 8.25m, 15);
        EnsureProductSupplier(context, "PIZ-001", "Ofimundo Cuenca", "L-2026-04", 54.00m, 9);
        EnsureProductSupplier(context, "ESC-001", "Muebles Andinos", "L-2026-01", 165.00m, 7);
        EnsureProductSupplier(context, "ESC-001", "Importadora del Pacifico", "L-2026-05", 179.00m, 4);
        EnsureProductSupplier(context, "ARC-001", "Muebles Andinos", "L-2026-02", 210.00m, 3);
        EnsureProductSupplier(context, "ARC-001", "Ofimundo Cuenca", "L-2026-05", 225.00m, 5);
        EnsureProductSupplier(context, "SIL-002", "Muebles Andinos", "L-2026-03", 45.00m, 24);
        EnsureProductSupplier(context, "SIL-002", "Importadora del Pacifico", "L-2026-06", 48.50m, 18);
        EnsureProductSupplier(context, "DES-001", "Ferretería El Constructor", "L-2026-02", 15.90m, 35);
        EnsureProductSupplier(context, "DES-001", "Suministros Guayaquil", "L-2026-02", 14.50m, 20);
        EnsureProductSupplier(context, "ESM-001", "Ferretería El Constructor", "L-2026-03", 62.00m, 8);
        EnsureProductSupplier(context, "ESM-001", "Suministros Guayaquil", "L-2026-03", 58.90m, 5);
        EnsureProductSupplier(context, "EXT-001", "Ferretería El Constructor", "L-2026-04", 12.40m, 40);
        EnsureProductSupplier(context, "EXT-001", "Electro Ecuador", "L-2026-08", 13.10m, 25);
    }

    private static void SeedProductAttributes(DbContext context)
    {
        EnsureProductAttribute(context, "LAP-001", "Memoria RAM", "16 GB");
        EnsureProductAttribute(context, "IMP-001", "Tipo", "Láser");
        EnsureProductAttribute(context, "TEC-001", "Conectividad", "Bluetooth");
        EnsureProductAttribute(context, "SIL-001", "Material", "Malla");
        EnsureProductAttribute(context, "TAL-001", "Potencia", "800 W");
        EnsureProductAttribute(context, "LAP-002", "Memoria RAM", "32 GB");
        EnsureProductAttribute(context, "LAP-002", "Almacenamiento", "1 TB SSD");
        EnsureProductAttribute(context, "MON-001", "Tamaño", "27\"");
        EnsureProductAttribute(context, "MON-001", "Resolución", "3840 x 2160");
        EnsureProductAttribute(context, "MOU-001", "Conectividad", "2.4 GHz");
        EnsureProductAttribute(context, "MOU-001", "DPI", "1600");
        EnsureProductAttribute(context, "DIS-001", "Capacidad", "1 TB");
        EnsureProductAttribute(context, "DIS-001", "Interfaz", "NVMe");
        EnsureProductAttribute(context, "ROU-001", "Estándar", "Wi-Fi 6");
        EnsureProductAttribute(context, "CAM-001", "Resolución", "1080p");
        EnsureProductAttribute(context, "AUD-001", "Conexión", "USB");
        EnsureProductAttribute(context, "UPS-001", "Capacidad", "1000 VA");
        EnsureProductAttribute(context, "PRY-001", "Brillo", "3500 lúmenes");
        EnsureProductAttribute(context, "TON-001", "Rendimiento", "2600 páginas");
        EnsureProductAttribute(context, "IMP-002", "Tipo", "Tinta continua");
        EnsureProductAttribute(context, "PAP-001", "Hojas", "500");
        EnsureProductAttribute(context, "PAP-001", "Gramaje", "75 g");
        EnsureProductAttribute(context, "BOL-001", "Unidades", "12");
        EnsureProductAttribute(context, "GRA-001", "Capacidad", "25 hojas");
        EnsureProductAttribute(context, "PIZ-001", "Medidas", "120 x 90 cm");
        EnsureProductAttribute(context, "ESC-001", "Medidas", "120 x 60 cm");
        EnsureProductAttribute(context, "ESC-001", "Color", "Roble");
        EnsureProductAttribute(context, "ARC-001", "Gavetas", "4");
        EnsureProductAttribute(context, "SIL-002", "Material", "Tela");
        EnsureProductAttribute(context, "DES-001", "Piezas", "12");
        EnsureProductAttribute(context, "ESM-001", "Potencia", "850 W");
        EnsureProductAttribute(context, "EXT-001", "Longitud", "5 m");
        EnsureProductAttribute(context, "EXT-001", "Tomas", "6");
    }

    private static void EnsureProduct(DbContext context, string code, string name, string description)
    {
        var products = context.Set<Product>();
        if (products.Any(p => p.Code == code))
        {
            return;
        }

        var product = new Product()
        {
            Code = code,
            Name = name,
            Description = description
        };

        products.Add(product);
        context.SaveChanges();
    }

    private static void EnsureProductSupplier(
        DbContext context,
        string productCode,
        string supplierName,
        string lotNumber,
        decimal price,
        int stock)
    {
        var productId = context.Set<Product>().Single(p => p.Code == productCode).Id;
        var supplierId = context.Set<Supplier>().Single(s => s.Name == supplierName).Id;

        var productSuppliers = context.Set<ProductSupplier>();
        if (productSuppliers.Any(ps => ps.ProductId == productId && ps.SupplierId == supplierId && ps.LotNumber == lotNumber))
        {
            return;
        }

        var productSupplier = new ProductSupplier()
        {
            ProductId = productId,
            SupplierId = supplierId,
            LotNumber = lotNumber,
            Price = price,
            Stock = stock
        };

        productSuppliers.Add(productSupplier);
        context.SaveChanges();
    }

    private static void EnsureProductAttribute(DbContext context, string productCode, string name, string value)
    {
        var productId = context.Set<Product>().Single(p => p.Code == productCode).Id;

        var attributes = context.Set<ProductAttribute>();
        if (attributes.Any(a => a.ProductId == productId && a.Name == name))
        {
            return;
        }

        var attribute = new ProductAttribute()
        {
            ProductId = productId,
            Name = name,
            Value = value
        };

        attributes.Add(attribute);
        context.SaveChanges();
    }

    private static void EnsureSupplier(DbContext context, string name, string description)
    {
        var suppliers = context.Set<Supplier>();
        if (suppliers.Any(s => s.Name == name))
        {
            return;
        }

        var supplier = new Supplier()
        {
            Name = name,
            Description = description
        };

        suppliers.Add(supplier);
        context.SaveChanges();
    }

    private static string GetSetting(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (value == null)
        {
            throw new InvalidOperationException("Setting '" + key + "' not found.");
        }

        return value;
    }

    private static void EnsureUser(
        DbContext context,
        IPasswordHasher hasher,
        string name,
        string lastName,
        string email,
        string password,
        string roleName)
    {
        var users = context.Set<User>();
        if (users.Any(u => u.Email == email))
        {
            return;
        }

        users.Add(BuildUser(context, hasher, name, lastName, email, password, roleName));
        context.SaveChanges();
    }

    private static User BuildUser(
        DbContext context,
        IPasswordHasher hasher,
        string name,
        string lastName,
        string email,
        string password,
        string roleName)
    {
        var roleId = context.Set<Role>().Single(r => r.Name == roleName).Id;

        var user = new User()
        {
            Name = name,
            LastName = lastName,
            Email = email,
            PasswordHash = hasher.Hash(password),
            RoleId = roleId
        };

        return user;
    }
}

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
    }

    private static void SeedProducts(DbContext context)
    {
        EnsureProduct(context, "LAP-001", "Laptop 14\"", "Laptop de 14 pulgadas para uso de oficina");
        EnsureProduct(context, "IMP-001", "Impresora multifunción", "Impresora láser con escáner y copiadora");
        EnsureProduct(context, "TEC-001", "Teclado inalámbrico", "Teclado Bluetooth en español");
        EnsureProduct(context, "SIL-001", "Silla ergonómica", "Silla de oficina con soporte lumbar");
        EnsureProduct(context, "TAL-001", "Taladro percutor", "Taladro eléctrico de 800 W con maletín");
    }

    private static void SeedProductSuppliers(DbContext context)
    {
        EnsureProductSupplier(context, "LAP-001", "Tecnoimport Quito", "L-2026-01", 750.00m, 12);
        EnsureProductSupplier(context, "LAP-001", "Distribuidora Andina", "L-2026-02", 780.00m, 5);
        EnsureProductSupplier(context, "IMP-001", "Distribuidora Andina", "L-2026-01", 210.50m, 20);
        EnsureProductSupplier(context, "SIL-001", "Importadora del Pacifico", "L-2026-03", 120.00m, 8);
        EnsureProductSupplier(context, "TAL-001", "Suministros Guayaquil", "L-2026-01", 89.90m, 15);
    }

    private static void SeedProductAttributes(DbContext context)
    {
        EnsureProductAttribute(context, "LAP-001", "Memoria RAM", "16 GB");
        EnsureProductAttribute(context, "IMP-001", "Tipo", "Láser");
        EnsureProductAttribute(context, "TEC-001", "Conectividad", "Bluetooth");
        EnsureProductAttribute(context, "SIL-001", "Material", "Malla");
        EnsureProductAttribute(context, "TAL-001", "Potencia", "800 W");
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

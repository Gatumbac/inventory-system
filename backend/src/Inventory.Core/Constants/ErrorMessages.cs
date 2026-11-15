namespace Inventory.Core.Constants;

public static class ErrorMessages
{
    public const string UnexpectedError = "Ocurrió un error inesperado";
    public const string ProductCodeDuplicated = "Ya existe un producto con ese código";
    public const string ProductCodeInvalid = "El código solo puede contener letras, números y guiones";
    public const string ProductHasSuppliers = "No se puede eliminar un producto con proveedores asociados";
    public const string ProductAttributeNamesNotUnique = "Los nombres de los atributos deben ser únicos";
    public const string ProductNotFound = "El producto no existe";
    public const string SupplierNotFound = "El proveedor no existe";
    public const string ProductSupplierNotFound = "El proveedor del producto no existe";
    public const string ProductSupplierLotDuplicated = "Ya existe ese proveedor con ese lote para el producto";
    public const string ProductSupplierPriceInvalid = "El precio debe ser mayor o igual a cero";
    public const string ProductSupplierStockInvalid = "El stock debe ser mayor o igual a cero";
}

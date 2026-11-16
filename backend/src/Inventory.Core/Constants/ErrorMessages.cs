namespace Inventory.Core.Constants;

public static class ErrorMessages
{
    public const string UnexpectedError = "Ocurrió un error inesperado";
    public const string ProductCodeDuplicated = "Ya existe un producto con ese código";
    public const string ProductCodeInvalid = "El código solo puede contener letras, números y guiones";
    public const string ProductHasSuppliers = "No se puede eliminar un producto con proveedores asociados";
    public const string ProductAttributeNamesNotUnique = "Los nombres de los atributos deben ser únicos";
    public const string PageInvalid = "La página debe estar entre 1 y 100000";
    public const string PageSizeInvalid = "El tamaño de página debe estar entre 1 y 50";
    public const string ProductNotFound = "El producto no existe";
    public const string SupplierNotFound = "El proveedor no existe";
    public const string SupplierNameDuplicated = "Ya existe un proveedor con ese nombre";
    public const string SupplierHasProductSuppliers = "No se puede eliminar un proveedor con productos asociados";
    public const string ProductSupplierNotFound = "El proveedor del producto no existe";
    public const string ProductSupplierLotDuplicated = "Ya existe ese proveedor con ese lote para el producto";
    public const string ProductSupplierPriceInvalid = "El precio debe ser mayor o igual a cero";
    public const string ProductSupplierStockInvalid = "El stock debe ser mayor o igual a cero";
    public const string UserNotFound = "El usuario no existe";
    public const string UserEmailDuplicated = "Ya existe un usuario con ese correo";
    public const string UserEmailInvalid = "El correo no es válido";
    public const string UserPasswordInvalid = "La contraseña debe tener entre 8 y 100 caracteres";
    public const string UserRoleInvalid = "El rol debe ser Admin o Viewer";
    public const string UserCannotDeactivateSelf = "No puedes desactivar tu propio usuario";
    public const string UserLastAdmin = "No se puede desactivar al último administrador";
}

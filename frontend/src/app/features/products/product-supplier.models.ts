export interface ProductSupplier {
  id: number;
  productId: number;
  supplierId: number;
  supplierName: string;
  lotNumber: string | null;
  price: number;
  stock: number;
}

export interface ProductSupplierRequest {
  lotNumber: string | null;
  price: number;
  stock: number;
}

export interface CreateProductSupplierRequest extends ProductSupplierRequest {
  supplierId: number;
}

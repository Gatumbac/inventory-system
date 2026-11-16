export interface Supplier {
  id: number;
  name: string;
  description: string | null;
}

export interface SupplierRequest {
  name: string;
  description: string | null;
}

export interface LowStockItem {
  productId: number;
  productName: string;
  supplierName: string;
  lotNumber: string | null;
  stock: number;
}

export interface Dashboard {
  productCount: number;
  supplierCount: number;
  totalStock: number;
  inventoryValue: number;
  lowStock: LowStockItem[];
}

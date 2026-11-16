import { PagedResult } from '../../core/http/paged-result';

export interface ProductAttribute {
  name: string;
  value: string;
}

export interface Product {
  id: number;
  code: string;
  name: string;
  description: string | null;
  attributes: ProductAttribute[];
}

export interface ProductRequest {
  code: string;
  name: string;
  description: string | null;
  attributes: ProductAttribute[];
}

export interface PriceMatrixSupplier {
  id: number;
  name: string;
}

export interface PriceMatrixPrice {
  supplierId: number;
  price: number;
}

export interface PriceMatrixRow {
  productId: number;
  code: string;
  name: string;
  prices: PriceMatrixPrice[];
}

export interface PriceMatrix {
  suppliers: PriceMatrixSupplier[];
  products: PagedResult<PriceMatrixRow>;
}

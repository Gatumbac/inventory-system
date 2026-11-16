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

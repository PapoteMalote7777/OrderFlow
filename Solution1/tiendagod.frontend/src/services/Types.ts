export interface Category {
    id: number;
    name: string;
}

export interface Product {
    id: number;
    name: string;
    price: number;
    description?: string;
    brand?: string;
    imageUrl?: string;
    categoryId?: number;
}

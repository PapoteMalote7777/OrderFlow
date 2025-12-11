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
    stock: number;
    imageUrl?: string;
    categoryId?: number;
}

export interface Product2 {
    id: number;
    name: string;
    price: number;
    description?: string;
    brand?: string;
    stock: number;
    imageUrl?: string;
    categoryName?: string;
}

export type CartItem = {
    productId: number;
    nombre: string;
    precioUnitario: number;
    cantidad: number;
};

export interface PedidoProductoDto {
    productId: number;
    nombreProducto: string;
    precioUnitario: number;
    cantidad: number;
}

export interface PedidoDto {
    id: number;
    userId: string;
    estado: string;
    total: number;
    productos: PedidoProductoDto[];
}
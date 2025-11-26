import { useState, useEffect } from "react";

export interface Product {
    id: number;
    name: string;
    price: number;
    description?: string;
    brand?: string;
    imageUrl?: string;
}

export function useProducts() {
    const [products, setProducts] = useState<Product[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const loadProducts = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const res = await fetch("/api/v1/products");
            if (!res.ok) throw new Error("Error al cargar productos");
            const data: Product[] = await res.json();
            setProducts(data);
        } catch (err) {
            setError("No se pudieron cargar los productos");
            console.error(err);
        } finally {
            setIsLoading(false);
        }
    };

    // Cargar productos automáticamente al montar
    useEffect(() => {
        loadProducts();
    }, []);

    return { products, isLoading, error, loadProducts };
}
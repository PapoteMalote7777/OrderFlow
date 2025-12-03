import { useState, useEffect } from "react";
import type { Product2 } from "../services/Types";

export function useProducts() {
    const [products, setProducts] = useState<Product2[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const loadProducts = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const res = await fetch("/api/v1/products");
            if (!res.ok) throw new Error("Error al cargar productos");
            const data: Product2[] = await res.json();
            setProducts(data);
        } catch (err) {
            setError("No se pudieron cargar los productos");
            console.error(err);
        } finally {
            setIsLoading(false);
        }
    };
    useEffect(() => {
        loadProducts();
    }, []);

    return { products, isLoading, error, loadProducts };
}
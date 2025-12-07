import React from "react";
import { useProducts } from "../../hooks/useProducts";

interface ProductsListProps {
    onAddToCart: (product: { id: number; name: string, price: number }) => void;
}

export default function ProductsList({ onAddToCart }: ProductsListProps) {
    const { products, isLoading, error } = useProducts();

    if (isLoading) return <p>Cargando productos...</p>;
    if (error) return <p className="error">{error}</p>;

    return (
        <div className="amazon-grid">
            {products.filter(p => p.stock > 0).map((p) => (
                <div key={p.id} className="product-card-amz">
                    <img
                        src={p.imageUrl ?? `https://picsum.photos/300?random=${p.id}`}
                        alt={p.name}
                    />
                    <h4>{p.name}</h4>
                    {p.categoryName && <p className="category">{p.categoryName}</p>}
                    <p className="price">{p.price.toFixed(2)} €</p>
                    <button
                        className="buy-btn-amz"
                        onClick={() => onAddToCart({ id: p.id, name: p.name, price: p.price })}
                    >
                        Añadir al carrito
                    </button>
                </div>
            ))}
        </div>
    );
}
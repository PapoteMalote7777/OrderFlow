import React from "react";
import { useProducts } from "../hooks/useProducts";

export default function ProductsList() {
    const { products, isLoading, error } = useProducts();

    if (isLoading) return <p>Cargando productos...</p>;
    if (error) return <p className="error">{error}</p>;

    return (
        <div className="amazon-grid">
            {products.map((p) => (
                <div key={p.id} className="product-card-amz">
                    <img
                        src={p.imageUrl ?? `https://picsum.photos/300?random=${p.id}`}
                        alt={p.name}
                    />
                    <h4>{p.name}</h4>
                    <p className="price">{p.price.toFixed(2)} €</p>
                    <button className="buy-btn-amz">Comprar</button>
                </div>
            ))}
        </div>
    );
}
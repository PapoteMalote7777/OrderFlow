/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from "react";
import { getAllProducts, createProduct, updateProduct, deleteProduct } from "../../services/Products";
import { getAllCategories } from "../../services/Category";
import type { Category, Product } from "../../services/Types";
import "../../App.css";

interface AdminProductsProps {
    onCancel?: () => void;
}

export default function AdminProducts({ onCancel }: AdminProductsProps) {
    const [products, setProducts] = useState<Product[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [editingProduct, setEditingProduct] = useState<Product | null>(null);
    const [newProduct, setNewProduct] = useState<Partial<Product>>({});

    const loadProducts = async () => {
        setLoading(true);
        setError(null);
        try {
            const productsData = await getAllProducts();
            const categoriesData = await getAllCategories();
            setProducts(productsData);
            setCategories(categoriesData);
        } catch (e: any) {
            setError(e?.message || "Error al cargar productos");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadProducts();
    }, []);

    const handleSave = async (product: Product) => {
        try {
            await updateProduct(product);
            setEditingProduct(null);
            await loadProducts();
        } catch (e: any) {
            setError(e?.message || "Error al actualizar producto");
        }
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm("¿Seguro que quieres eliminar este producto?")) return;
        try {
            await deleteProduct(id);
            await loadProducts();
        } catch (e: any) {
            setError(e?.message || "Error al borrar producto");
        }
    };

    const handleCreate = async () => {
        if (!newProduct.name || !newProduct.price) {
            setError("El nombre y precio son obligatorios");
            return;
        }
        try {
            await createProduct(newProduct as Product);
            setNewProduct({});
            await loadProducts();
        } catch (e: any) {
            setError(e?.message || "Error al crear producto");
        }
    };

    if (loading) return <div>Cargando productos...</div>;
    if (error) return <div className="error">{error}</div>;

    return (
        <div className="admin-panel">
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
                <h2>Administración de productos</h2>
                {onCancel && (
                    <button className="sidebar-toggle" onClick={onCancel}>
                        Volver
                    </button>
                )}
            </div>
            <div style={{ marginBottom: 20 }}>
                <h3>Crear producto</h3>
                <input
                    type="text"
                    placeholder="Nombre"
                    value={newProduct.name || ""}
                    onChange={(e) => setNewProduct({ ...newProduct, name: e.target.value })}
                />
                <input
                    type="number"
                    placeholder="Precio"
                    value={newProduct.price || ""}
                    onChange={(e) => setNewProduct({ ...newProduct, price: parseFloat(e.target.value) })}
                />
                <input
                    type="text"
                    placeholder="Descripción"
                    value={newProduct.description || ""}
                    onChange={(e) => setNewProduct({ ...newProduct, description: e.target.value })}
                />
                <input
                    type="text"
                    placeholder="Marca"
                    value={newProduct.brand || ""}
                    onChange={(e) => setNewProduct({ ...newProduct, brand: e.target.value })}
                />
                <select
                    value={newProduct.categoryId || ""}
                    onChange={(e) =>
                        setNewProduct({
                            ...newProduct,
                            categoryId: parseInt(e.target.value),
                        })
                    }
                >
                    <option value="">-- Selecciona categoría --</option>
                    {categories.map((cat) => (
                        <option key={cat.id} value={cat.id}>
                            {cat.name}
                        </option>
                    ))}
                </select>
                <button onClick={handleCreate}>Crear</button>
            </div>
            <table>
                <thead>
                    <tr>
                        <th>Nombre</th>
                        <th>Precio</th>
                        <th>Descripción</th>
                        <th>Marca</th>
                        <th>Acciones</th>
                        <th>Categoría</th>
                    </tr>
                </thead>
                <tbody>
                    {products.map((p) => (
                        <tr key={p.id}>
                            <td>
                                {editingProduct?.id === p.id ? (
                                    <input
                                        value={editingProduct.name}
                                        onChange={(e) => setEditingProduct({ ...editingProduct, name: e.target.value })}
                                    />
                                ) : (
                                    p.name
                                )}
                            </td>
                            <td>
                                {editingProduct?.id === p.id ? (
                                    <input
                                        type="number"
                                        value={editingProduct.price}
                                        onChange={(e) =>
                                            setEditingProduct({ ...editingProduct, price: parseFloat(e.target.value) })
                                        }
                                    />
                                ) : (
                                    p.price.toFixed(2)
                                )}
                            </td>
                            <td>
                                {editingProduct?.id === p.id ? (
                                    <input
                                        value={editingProduct.description || ""}
                                        onChange={(e) =>
                                            setEditingProduct({ ...editingProduct, description: e.target.value })
                                        }
                                    />
                                ) : (
                                    p.description
                                )}
                            </td>
                            <td>
                                {editingProduct?.id === p.id ? (
                                    <input
                                        value={editingProduct.brand || ""}
                                        onChange={(e) =>
                                            setEditingProduct({ ...editingProduct, brand: e.target.value })
                                        }
                                    />
                                ) : (
                                    p.brand
                                )}
                            </td>
                            <td>
                                {editingProduct?.id === p.id ? (
                                    <select
                                        value={editingProduct.categoryId || ""}
                                        onChange={(e) =>
                                            setEditingProduct({
                                                ...editingProduct!,
                                                categoryId: parseInt(e.target.value),
                                            })
                                        }
                                    >
                                        <option value="">-- Selecciona categoría --</option>
                                        {categories.map((cat) => (
                                            <option key={cat.id} value={cat.id}>
                                                {cat.name}
                                            </option>
                                        ))}
                                    </select>
                                ) : (
                                    categories.find(c => c.id === p.categoryId)?.name || ""
                                )}
                            </td>
                            <td>
                                {editingProduct?.id === p.id ? (
                                    <>
                                        <button onClick={() => editingProduct && handleSave(editingProduct)}>Guardar</button>
                                        <button onClick={() => setEditingProduct(null)}>Cancelar</button>
                                    </>
                                ) : (
                                    <>
                                        <button onClick={() => setEditingProduct(p)}>Editar</button>
                                        <button onClick={() => handleDelete(p.id)}>Borrar</button>
                                    </>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
import React, { useState, useEffect } from "react";
import { getAllCategories, createCategory, deleteCategory } from "../../services/Category";
import type { Category } from "../../services/Types";
import "../../App.css";
interface AdminCategoriesProps {
    onCancel?: () => void;
}

export default function AdminCategories({ onCancel }: AdminCategoriesProps) {
    const [categories, setCategories] = useState<Category[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [newCategory, setNewCategory] = useState("");

    const loadCategories = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const data = await getAllCategories();
            setCategories(data);
        } catch (e: any) {
            console.error(e);
            setError(e?.message || "Error al cargar categorías");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadCategories();
    }, []);

    const handleCreate = async () => {
        if (!newCategory.trim()) return;
        try {
            await createCategory({ name: newCategory });
            setNewCategory("");
            await loadCategories();
        } catch (e: any) {
            alert(e?.message);
        }
    };

    const handleDelete = async (id: number) => {
        if (!window.confirm("¿Seguro que quieres eliminar esta categoría?")) return;
        try {
            await deleteCategory(id);
            await loadCategories();
        } catch (e: any) {
            alert(e?.message);
        }
    };

    if (isLoading) return <div>Cargando categorías...</div>;
    if (error) return <div className="error">{error}</div>;

    return (
        <div className="admin-panel">
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
                <h2>Administración de categorías</h2>
                {onCancel && <button className="sidebar-toggle" onClick={onCancel}>Volver</button>}
            </div>

            <div style={{ marginBottom: 20 }}>
                <input
                    type="text"
                    placeholder="Nombre nueva categoría"
                    value={newCategory}
                    onChange={(e) => setNewCategory(e.target.value)}
                />
                <button onClick={handleCreate}>Crear</button>
            </div>

            <table>
                <thead>
                    <tr>
                        <th>Nombre</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    {categories.map((c) => (
                        <tr key={c.id}>
                            <td>{c.name}</td>
                            <td>
                                <button onClick={() => handleDelete(c.id!)}>Borrar</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
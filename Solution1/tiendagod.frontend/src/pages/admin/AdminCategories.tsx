import React, { useState, useEffect } from "react";
import { getAllCategories, createCategory, deleteCategory } from "../../services/Category";
import type { Category } from "../../services/Types";
import "../../App.css";
interface AdminCategoriesProps {
    onCancel?: () => void;
}

export default function AdminCategories({ onCancel }: AdminCategoriesProps) {
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const [newCategory, setNewCategory] = useState("");

    const loadCategories = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await getAllCategories();
            setCategories(data);
        } catch (e: any) {
            console.error(e);
            setError(e?.message || "Error al cargar categorías");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadCategories();
    }, []);

    const showTemporaryMessage = (setMessage: React.Dispatch<React.SetStateAction<string | null>>, msg: string) => {
        setMessage(msg);
        setTimeout(() => setMessage(null), 10000);
    };

    const handleCreate = async () => {
        if (!newCategory.trim()) {
            showTemporaryMessage(setError, "Faltan campos por rellenar.");
            return;
        }
        try {
            await createCategory({ name: newCategory });
            setNewCategory("");
            await loadCategories();
            showTemporaryMessage(setSuccess, "Categoria creada correctamente.");
        } catch (e: any) {
            showTemporaryMessage(setError, e?.message || "Error al crear la categoria.");
        }
    };

    const handleDelete = async (id: number) => {
        try {
            await deleteCategory(id);
            await loadCategories();
            showTemporaryMessage(setSuccess, "Categoria eliminada correctamente.");
        } catch (e: any) {
            showTemporaryMessage(setError, e?.message || "Error al borrar la categoria.");
        }
    };

    return (
        <div className="admin-panel">
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
                <h2>Administracion de categorias</h2>
                {onCancel && <button className="sidebar-toggle" onClick={onCancel}>Volver</button>}
            </div>
            {error && <div className="error" style={{ marginBottom: 20 }}>{error}</div>}
            {success && <div className="success" style={{ marginBottom: 20 }}>{success}</div>}
            <div style={{ marginBottom: 20 }}>
                <input
                    type="text"
                    placeholder="Nombre nueva categoria"
                    value={newCategory}
                    onChange={(e) => setNewCategory(e.target.value)}
                />
                <button onClick={handleCreate}>Crear</button>
            </div>
            {loading ? (
                <div>Cargando categorias...</div>
            ) : (
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
            )}
        </div>
    );
}
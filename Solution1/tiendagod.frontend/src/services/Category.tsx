/* eslint-disable @typescript-eslint/no-explicit-any */
import { getToken } from "./auth";

export const getAllCategories = async () => {
    const res = await fetch("/api/v1/categories");
    if (!res.ok) throw new Error("Error al cargar categorías");
    return res.json();
};

export const createCategory = async (category: any) => {
    const token = getToken();
    const res = await fetch("/api/v1/categories", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: JSON.stringify(category),
    });
    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al crear categoría: ${text}`);
    }
};

export const deleteCategory = async (id: number) => {
    const token = getToken();
    const res = await fetch(`/api/v1/categories/${id}`, {
        method: "DELETE",
        headers: token ? { Authorization: `Bearer ${token}` } : {},
    });
    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al borrar categoría: ${text}`);
    }
};
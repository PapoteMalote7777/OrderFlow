/* eslint-disable @typescript-eslint/no-explicit-any */
import { getToken } from "./auth";

export const getAllProducts = async () => {
    const res = await fetch("/api/v1/products");
    if (!res.ok) throw new Error("Error al cargar productos");
    return res.json();
};

export const createProduct = async (product: any) => {
    const token = getToken();
    console.log("Token actual:", token);
    const res = await fetch("/api/v1/products", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        },
        body: JSON.stringify(product),
    });
    if (!res.ok) {
        let errorMsg = "";
        const contentType = res.headers.get("content-type") || "";
        if (contentType.includes("application/json")) {
            const data = await res.json();
            errorMsg = data?.message || JSON.stringify(data);
        } else {
            errorMsg = await res.text();
        }
        throw new Error(`Error al crear producto: ${errorMsg || res.statusText}`);
    }
};

export const updateProduct = async (product: any) => {
    const token = getToken();
    const res = await fetch(`/api/v1/products/${product.id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        },
        body: JSON.stringify(product),
    });
    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al actualizar producto: ${text}`);
    }
};

export const deleteProduct = async (id: number) => {
    const token = getToken();
    const res = await fetch(`/api/v1/products/${id}`, {
        method: "DELETE",
        headers: token ? { Authorization: `Bearer ${token}` } : {},
    });
    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al borrar producto: ${text}`);
    }
};
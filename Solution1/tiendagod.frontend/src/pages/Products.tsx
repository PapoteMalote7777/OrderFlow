/* eslint-disable @typescript-eslint/no-explicit-any */
export const getAllProducts = async () => {
    const res = await fetch("/api/v1/products");
    if (!res.ok) throw new Error("Error al cargar productos");
    return res.json();
};

export const createProduct = async (product: any) => {
    const res = await fetch("/api/v1/products", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(product),
    });
    if (!res.ok) throw new Error("Error al crear producto");
};

export const updateProduct = async (product: any) => {
    const res = await fetch(`/api/v1/products/${product.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(product),
    });
    if (!res.ok) throw new Error("Error al actualizar producto");
};

export const deleteProduct = async (id: number) => {
    const res = await fetch(`/api/v1/products/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error("Error al borrar producto");
};
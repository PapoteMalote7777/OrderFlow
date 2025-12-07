import { getToken } from "./auth";

export const createOrder = async (pedido: {
    userId: number;
    productos: { productId: number; cantidad: number }[];
}) => {
    const token = getToken();

    const res = await fetch("/api/Pedidos", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        },
        body: JSON.stringify(pedido)
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

        throw new Error(`Error al crear pedido: ${errorMsg || res.statusText}`);
    }

    return res.json();
};

export const getMyOrders = async () => {
    const token = getToken();

    const res = await fetch("/api/Pedidos", {
        headers: token ? { Authorization: `Bearer ${token}` } : {}
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al cargar pedidos: ${text}`);
    }

    return res.json();
};

export const getOrderById = async (id: number) => {
    const token = getToken();

    const res = await fetch(`/api/Pedidos/${id}`, {
        headers: token ? { Authorization: `Bearer ${token}` } : {}
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al cargar pedido: ${text}`);
    }

    return res.json();
};

export const deleteOrder = async (id: number) => {
    const token = getToken();

    const res = await fetch(`/api/Pedidos/${id}`, {
        method: "DELETE",
        headers: token ? { Authorization: `Bearer ${token}` } : {}
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al eliminar pedido: ${text}`);
    }
};
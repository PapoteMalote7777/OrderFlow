import { getToken, parseJwtPayload } from "./auth";

export const createOrder = async (productos: {
    productId: number;
    cantidad: number;
}[]) => {
    const token = getToken();

    if (!token) throw new Error("No hay token. Inicia sesión.");

    const payload = parseJwtPayload(token);
    const userId =
        payload?.sub ||
        payload?.nameid ||
        payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

    if (!userId) {
        throw new Error("No se pudo obtener el usuario. Vuelve a iniciar sesión.");
    }

    const pedido = {
        userId: userId,
        productos: productos
    };

    const res = await fetch("/api/Pedidos/create", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
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

export const getOrders = async () => {
    const token = getToken();

    const res = await fetch("/api/Pedidos/list", {
        headers: token ? { Authorization: `Bearer ${token}` } : {}
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al cargar pedidos: ${text}`);
    }

    return res.json();
};

export const deleteOrder = async (id: number) => {
    const token = getToken();

    const res = await fetch(`/api/admin/pedidos/${id}`, {
        method: "DELETE",
        headers: token ? { Authorization: `Bearer ${token}` } : {}
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al eliminar pedido: ${text}`);
    }
};

export const getAllOrders = async () => {
    const token = getToken();

    const res = await fetch("/api/admin/pedidos/list", {
        headers: token ? { Authorization: `Bearer ${token}` } : {}
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error al cargar pedidos: ${text}`);
    }

    return res.json();
};

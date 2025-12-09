import React, { useEffect, useState } from "react";
import { getAllOrders, deleteOrder } from "../../services/Pedidos";
import type { PedidoDto, PedidoProductoDto } from "../../services/Types";
import "../../App.css";

interface AdminOrdersProps {
    onCancel?: () => void;
}

export default function AdminOrders({ onCancel }: AdminOrdersProps) {
    const [orders, setOrders] = useState<PedidoDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);

    const loadOrders = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await getAllOrders();
            setOrders(data);
        } catch (e: any) {
            setError(e?.message || "Error al cargar pedidos");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadOrders();
    }, []);

    const showTemporaryMessage = (setMessage: React.Dispatch<React.SetStateAction<string | null>>, msg: string) => {
        setMessage(msg);
        setTimeout(() => setMessage(null), 10000);
    };

    const handleDelete = async (id: number) => {
        try {
            await deleteOrder(id);
            await loadOrders();
            showTemporaryMessage(setSuccess, "Pedido eliminado correctamente.");
        } catch (e: any) {
            showTemporaryMessage(setError, e?.message || "Error al eliminar pedido");
        }
    }

    return (
        <div className="admin-panel">
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
                <h2>Administracion de pedidos</h2>
                {onCancel && <button className="sidebar-toggle" onClick={onCancel}>Volver</button>}
            </div>

            {error && <div className="error">{error}</div>}
            {success && <div className="success">{success}</div>}

            {loading ? (
                <div>Cargando pedidos...</div>
            ) : (
                <table className="orders-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Usuario</th>
                            <th>Productos</th>
                            <th>Cantidad</th>
                            <th>Precio Unitario</th>
                            <th>Total</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orders.map(pedido => (
                            <tr key={pedido.id}>
                                <td>{pedido.id}</td>
                                <td>{pedido.userId}</td>
                                <td>
                                    <ul className="no-bullets">
                                        {pedido.productos.map(prod => (
                                            <li key={prod.productId}>{prod.nombreProducto}</li>
                                        ))}
                                    </ul>
                                </td>
                                <td>
                                    <ul className="no-bullets">
                                        {pedido.productos.map(prod => (
                                            <li key={prod.productId}>{prod.cantidad}</li>
                                        ))}
                                    </ul>
                                </td>
                                <td>
                                    <ul className="no-bullets">
                                        {pedido.productos.map(prod => (
                                            <li key={prod.productId}>{prod.precioUnitario.toFixed(2)}</li>
                                        ))}
                                    </ul>
                                </td>
                                <td><b>{pedido.total.toFixed(2)}</b></td>
                                <td>
                                    <button onClick={() => handleDelete(pedido.id)}>Eliminar</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}
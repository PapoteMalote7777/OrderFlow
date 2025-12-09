/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import { getOrders } from "../../services/Pedidos";
import type { PedidoDto, PedidoProductoDto } from "../../services/Types";

interface OrdersProps {
    onBack?: () => void;
}

export default function Orders({ onBack }: OrdersProps) {
    const [orders, setOrders] = useState<PedidoDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string>("");

    useEffect(() => {
        const loadOrders = async () => {
            try {
                const data = await getOrders();
                setOrders(data);
            } catch (err: any) {
                setError(err.message || "Error al cargar pedidos");
            } finally {
                setLoading(false);
            }
        };

        loadOrders();
    }, []);

    if (loading) return <p>Cargando pedidos...</p>;
    if (error) return <p className="error">{error}</p>;

    return (
        <div className="admin-panel">
            <h2>Mis pedidos</h2>
            {onBack && (
                <button className="btn-back" onClick={onBack}>
                    Volver
                </button>
            )}
            {orders.length === 0 ? (
                <p>No tienes pedidos realizados.</p>
            ) : (
                <table className="orders-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Productos</th>
                            <th>Cantidad</th>
                            <th>Precio Unitario</th>
                            <th>Precio Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orders.map(pedido => (
                            <tr key={pedido.id}>
                                <td>{pedido.id}</td>
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
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}

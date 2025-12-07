import { useEffect, useState } from "react";
import { getMyOrders } from "../../services/Pedidos";

interface PedidoProductoDto {
    productId: number;
    nombreProducto: string;
    precioUnitario: number;
    cantidad: number;
}

interface PedidoDto {
    id: number;
    userId: number;
    total: number;
    productos: PedidoProductoDto[];
}

export default function Orders() {
    const [orders, setOrders] = useState<PedidoDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string>("");

    useEffect(() => {
        const loadOrders = async () => {
            try {
                const data = await getMyOrders();
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

            {orders.length === 0 ? (
                <p>No tienes pedidos realizados.</p>
            ) : (
                <table className="orders-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Productos</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orders.map(pedido => (
                            <tr key={pedido.id}>
                                <td>{pedido.id}</td>
                                <td>
                                    <ul>
                                        {pedido.productos.map(prod => (
                                            <li key={prod.productId}>
                                                {prod.nombreProducto} — {prod.cantidad} x {prod.precioUnitario.toFixed(2)} €
                                            </li>
                                        ))}
                                    </ul>
                                </td>
                                <td><b>{pedido.total.toFixed(2)} €</b></td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}

import { useState } from "react";
import { getUserIdFromToken, isAdmin } from "../../services/auth";
import ProductsList from "../products/ProductsList";
import AdminUsers from "../admin/AdminUsers";
import AdminProducts from "../admin/AdminProducts";
import AdminCategories from "../admin/AdminCategories";
import AdminOrders from "../admin/AdminOrders";
import { createOrder } from "../../services/Pedidos";
import Orders from "../orders/orders";

interface HomeProps {
    onLogout: () => void;
    username: string;
    userId: number;
    goToLogin: () => void;
    goToProfile: () => void;
    goToAdmin: () => void;
    goToAdminProducts: () => void;
    goToAdminCategories: () => void;
    goToOrders: () => void;
}

export default function Home({ onLogout, username, goToLogin, goToProfile}: HomeProps) {
    const [menuOpen, setMenuOpen] = useState(false);
    const [currentView, setCurrentView] = useState<"products" | "adminUsers" | "adminProducts" | "adminCategories" | "orders" | "adminOrders">("products");
    const goToAdminUsers = () => setCurrentView("adminUsers");
    const goToAdminProducts = () => setCurrentView("adminProducts");
    const goToAdminCategories = () => setCurrentView("adminCategories");
    const goToAdminOrders = () => setCurrentView("adminOrders");
    const [cart, setCart] = useState<{ id: number; name: string; price: number; quantity: number }[]>([]);
    const [cartOpen, setCartOpen] = useState(false);
    const goToOrders = () => setCurrentView("orders");
    const [cartError, setCartError] = useState<string | null>(null);
    const [cartSuccess, setCartSuccess] = useState<string | null>(null);

    const handleLogout = () => {
        setMenuOpen(false);
        onLogout();
    };

    const showTemporaryMessage = (setMessage: React.Dispatch<React.SetStateAction<string | null>>, msg: string) => {
        setMessage(msg);
        setTimeout(() => setMessage(null), 5000);
    };

    const addToCart = (product: { id: number; name: string; price: number }) => {
        setCart(prev => {
            const existing = prev.find(item => item.id === product.id);
            if (existing) {
                return prev.map(item =>
                    item.id === product.id ? { ...item, quantity: item.quantity + 1 } : item
                );
            }
            return [...prev, { ...product, quantity: 1 }];
        });
        setCartOpen(true);
    };

    const removeFromCart = (productId: number) => {
        setCart(prev =>
            prev
                .map(item =>
                    item.id === productId ? { ...item, quantity: item.quantity - 1 } : item
                )
                .filter(item => item.quantity > 0)
        );
    };

    const clearCart = () => setCart([]);
    return (
        <div className="amazon-container">
            {currentView === "products" && (
                <header className="amazon-header">
                    <div className="logo">Tienda GOD</div>
                    <input className="search-bar" placeholder="Buscar productos..." />
                    <div className="header-right">
                        {username ? (
                            <>
                                <span className="welcome-user">Hola, {username}</span>
                                <button
                                    className="sidebar-toggle"
                                    onClick={() => setMenuOpen(!menuOpen)}
                                >
                                    Menú {menuOpen ? "▲" : "▼"}
                                </button>
                                <div style={{ position: "relative" }}>
                                    <div className="cart-icon" onClick={() => setCartOpen(!cartOpen)}>
                                        🛒
                                        <span className="cart-badge">{cart.length}</span>
                                    </div>

                                    {cartOpen && (
                                        <div className="cart-dropdown">
                                            {cart.length === 0 ? (
                                                <p>Tu carrito está vacío</p>
                                            ) : (
                                                <>
                                                    {cart.map(item => (
                                                        <div key={item.id} className="cart-item">
                                                            <span className="cart-item-name">{item.name}</span>
                                                            <div className="cart-item-controls">
                                                                <button onClick={() => removeFromCart(item.id)}>-</button>
                                                                <span>{item.quantity}</span>
                                                                <button onClick={() => addToCart(item)}>+</button>
                                                            </div>
                                                            <span>{(item.price * item.quantity).toFixed(2)} €</span>
                                                        </div>
                                                    ))}

                                                    <div className="cart-total">
                                                        <span>Total:</span>
                                                        <span>
                                                            {cart.reduce((sum, item) => sum + item.price * item.quantity, 0).toFixed(2)} €
                                                        </span>
                                                    </div>
                                                    <div style={{ display: "flex", gap: "8px", marginTop: "10px" }}>
                                                            <button
                                                                className="cart-btn-comprar"
                                                                onClick={async () => {
                                                                    try {
                                                                        const productos = cart.map(item => ({
                                                                            productId: item.id,
                                                                            cantidad: item.quantity
                                                                        }));

                                                                        await createOrder(productos);

                                                                        showTemporaryMessage(setCartSuccess, "Pedido realizado correctamente. Revise sus pedidos");
                                                                        clearCart();
                                                                        setCartOpen(false);

                                                                    } catch (err: any) {
                                                                        showTemporaryMessage(setCartError, err.message || "Error al realizar el pedido. Pruebe mas tarde.");
                                                                    }
                                                                }}
                                                            >
                                                                Hacer pedido
                                                            </button>
                                                        <button className="cart-btn-vaciar" onClick={clearCart}>
                                                            Vaciar carrito
                                                        </button>
                                                    </div>
                                                </>
                                            )}
                                        </div>
                                    )}
                                </div>
                            </>
                        ) : (
                            <div className="header-center">
                                <span
                                    className="welcome-user switch-link"
                                    onClick={goToLogin}
                                >
                                    Inicia sesión
                                </span>
                            </div>
                        )}
                    </div>
                </header>
            )}
            {(cartError || cartSuccess) && (
                <div style={{ padding: "10px 20px" }}>
                    {cartError && <div className="error">{cartError}</div>}
                    {cartSuccess && <div className="success">{cartSuccess}</div>}
                </div>
            )}
            {menuOpen && username && (
                <ul className="sidebar-menu">
                    <li onClick={() => { setMenuOpen(false); goToProfile(); }}>Mi cuenta</li>
                    <li onClick={() => { setMenuOpen(false); goToOrders(); }}> Mis pedidos</li>
                    {isAdmin() && (
                        <>
                            <li onClick={() => { setMenuOpen(false); goToAdminUsers(); }}>Administrar usuarios</li>
                            <li onClick={() => { setMenuOpen(false); goToAdminProducts(); }}>Administrar productos</li>
                            <li onClick={() => { setMenuOpen(false); goToAdminCategories(); }}>Administrar categorías</li>
                            <li onClick={() => { setMenuOpen(false); goToAdminOrders(); }}>Administrar pedidos</li>
                        </>
                    )}
                    <li onClick={handleLogout}>Cerrar sesión</li>
                </ul>
            )}
            <div className="amazon-body">
                <main className="amazon-main">
                    {currentView === "products" && <ProductsList onAddToCart={addToCart} />}
                    {currentView === "orders" && <Orders onBack={() => setCurrentView("products")} />}
                    {currentView === "adminUsers" && <AdminUsers onCancel={() => setCurrentView("products")} />}
                    {currentView === "adminProducts" && <AdminProducts onCancel={() => setCurrentView("products")} />}
                    {currentView === "adminCategories" && <AdminCategories onCancel={() => setCurrentView("products")} />}
                    {currentView === "adminOrders" && <AdminOrders onCancel={() => setCurrentView("products")} />}
                </main>
            </div>
        </div>
    );
}
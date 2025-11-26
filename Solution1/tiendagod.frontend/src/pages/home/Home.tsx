import { useState } from "react";
import { isAdmin } from "../../services/auth";
import ProductsList from "../products/ProductsList";
import AdminUsers from "../admin/AdminUsers";
import AdminProducts from "../admin/AdminProducts";

interface HomeProps {
    onLogout: () => void;
    username: string;
    goToLogin: () => void;
    goToProfile: () => void;
    goToAdmin: () => void;
    goToAdminProducts: () => void;
}

export default function Home({ onLogout, username, goToLogin, goToProfile, goToAdmin}: HomeProps) {
    const [menuOpen, setMenuOpen] = useState(false);
    const [currentView, setCurrentView] = useState<"products" | "adminUsers" | "adminProducts">("products");
    const goToAdminProducts = () => setCurrentView("adminProducts");
    const handleLogout = () => {
        setMenuOpen(false);
        onLogout();
    };

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

            {menuOpen && username && (
                <ul className="sidebar-menu">
                    <li onClick={() => { setMenuOpen(false); goToProfile(); }}>Mi cuenta</li>
                    <li>Pedidos</li>
                    {isAdmin() && (
                        <>
                            <li onClick={() => { setMenuOpen(false); goToAdmin(); }}>Administrar usuarios</li>
                            <li onClick={() => { setMenuOpen(false); goToAdminProducts(); }}>Administrar productos</li>
                        </>
                    )}
                    <li>Recomendados</li>
                    <li>Favoritos</li>
                    <li onClick={handleLogout}>Cerrar sesión</li>
                </ul>
            )}
            <div className="amazon-body">
                <main className="amazon-main">
                    {currentView === "products" && <ProductsList />}
                    {currentView === "adminUsers" && <AdminUsers onCancel={() => setCurrentView("products")} />}
                    {currentView === "adminProducts" && <AdminProducts onCancel={() => setCurrentView("products")} />}
                </main>
            </div>
        </div>
    );
}
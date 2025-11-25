import { useState } from "react";
import { isAdmin } from "../services/auth";
import ProductsList from "../pages/ProductsList";

interface HomeProps {
    onLogout: () => void;
    username: string;
    goToLogin: () => void;
    goToProfile: () => void;
    goToAdmin: () => void;
}

export default function Home({ onLogout, username, goToLogin, goToProfile, goToAdmin}: HomeProps) {
    const [menuOpen, setMenuOpen] = useState(false);

    const handleLogout = () => {
        setMenuOpen(false);
        onLogout();
    };

    return (
        <div className="amazon-container">

            {/* HEADER */}
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

            {menuOpen && username && (
                <ul className="sidebar-menu">
                    <li onClick={() => { setMenuOpen(false); goToProfile(); }}>Mi cuenta</li>
                    <li>Pedidos</li>
                    {isAdmin() && <li onClick={() => { setMenuOpen(false); goToAdmin(); }}>Administrador</li>}
                    <li>Recomendados</li>
                    <li>Favoritos</li>
                    <li onClick={handleLogout}>Cerrar sesión</li>
                </ul>
            )}
            {/* BODY */}
            <div className="amazon-body">
                {/* CONTENIDO PRINCIPAL */}
                <main className="amazon-main">
                    <h1 className="title">Productos disponibles</h1>
                    <ProductsList />
                </main>
            </div>
        </div>
    );
}
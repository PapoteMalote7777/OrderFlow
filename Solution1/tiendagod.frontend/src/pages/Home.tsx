import React from "react";
import { useNavigate } from "react-router-dom";

export default function Home() {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem("token");
        navigate("/auth/login");
    };

    return (
        <div className="form-card">
            <h1>Bienvenido a Home</h1>
            <p>Has iniciado sesión correctamente ✅</p>
            <button onClick={handleLogout}>Cerrar sesión</button>
        </div>
    );
}
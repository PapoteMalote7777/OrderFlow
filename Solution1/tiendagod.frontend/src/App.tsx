import React, { useState } from "react";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Home from "./pages/Home";
import "./App.css";

const App: React.FC = () => {
    const [page, setPage] = useState<"login" | "register" | "home">("login");

    // Verificar token en localStorage al iniciar
    React.useEffect(() => {
        const token = localStorage.getItem("token");
        if (token) setPage("home");
    }, []);

    const handleLoginSuccess = () => {
        setPage("home");
    };

    const handleLogout = () => {
        localStorage.removeItem("token");
        setPage("login");
    };

    return (
        <div className="app-container">
            <div className="form-card">
                {page === "login" && (
                    <Login
                        onSwitchToRegister={() => setPage("register")}
                        onLoginSuccess={handleLoginSuccess}
                    />
                )}
                {page === "register" && (
                    <Register
                        onSwitchToLogin={() => setPage("login")}
                        onRegisterSuccess={() => setPage("login")}
                    />
                )}
                {page === "home" && (
                    <Home onLogout={handleLogout} />
                )}
            </div>
        </div>
    );
};

export default App;

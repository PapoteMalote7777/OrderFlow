import React, { useState } from "react";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Home from "./pages/Home";
import Profile from "./pages/Profile";
import "./App.css";

const App: React.FC = () => {
    const [page, setPage] = useState<"login" | "register" | "home" | "profile">("home");
    const [username, setUsername] = useState<string>("");

    // Verificar token en localStorage al iniciar
    React.useEffect(() => {
        const token = localStorage.getItem("token");
        const storedUsername = localStorage.getItem("username")
        if (token && storedUsername) {
            setUsername(storedUsername);
            setPage("home");
        }
    }, []);

    const handleLoginSuccess = (name: string) => {
        setUsername(name);
        localStorage.setItem("username", name);
        setPage("home");
    };

    const handleLogout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("username");
        setUsername("");
        setPage("home");
    };

    const handleDeleteAccount = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("username");
        setUsername("");
        setPage("home");
    };

    return (
        <div className="app-container">
            {page === "login" && (
                <div className="form-card">
                    <Login
                        onSwitchToRegister={() => setPage("register")}
                        onLoginSuccess={handleLoginSuccess}
                    />
                </div>
            )}

            {page === "register" && (
                <div className="form-card">
                    <Register
                        onSwitchToLogin={() => setPage("login")}
                        onRegisterSuccess={() => setPage("login")}
                    />
                </div>
            )}

            {page === "home" && (
                <Home
                    onLogout={handleLogout}
                    username={username}
                    goToLogin={() => setPage("login")}
                    goToProfile={() => setPage("profile")}
                />
            )}

            {page === "profile" && (
                <Profile
                    username={username}
                    setUsername={setUsername}
                    onCancel={() => setPage("home")}
                    onDeleteAccount={handleDeleteAccount}
                />
            )}
        </div>
    );
};

export default App;

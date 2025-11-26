import React, { useState } from "react";
import Login from "./pages/users/Login";
import Register from "./pages/users/Register";
import Home from "./pages/home/Home";
import Profile from "./pages/users/Profile";
import AdminUsers from "./pages/admin/AdminUsers";
import "./App.css";

const App: React.FC = () => {
    const [page, setPage] = useState<"login" | "register" | "home" | "profile" | "admin">("home");
    const [username, setUsername] = useState<string>("");

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
                    goToAdmin={() => setPage("admin")}
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

            {page === "admin" && (
                <AdminUsers onCancel={() => setPage("home")} />
            )}
        </div>
    );
};

export default App;

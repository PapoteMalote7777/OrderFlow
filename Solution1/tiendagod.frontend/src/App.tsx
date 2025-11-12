import React, { useState } from "react";
import Login from "./pages/Login";
import Register from "./pages/Register";
import "./App.css";

const App: React.FC = () => {
    const [showLogin, setShowLogin] = useState(true);

    return (
        <div className="app-container">
            <div className="form-card">
                {showLogin ? (
                    <Login
                        // pasamos una función para cambiar a registro
                        onSwitchToRegister={() => setShowLogin(false)}
                    />
                ) : (
                    <Register
                        // pasamos una función para volver a login
                        onSwitchToLogin={() => setShowLogin(true)}
                    />
                )}
            </div>
        </div>
    );
};

export default App;

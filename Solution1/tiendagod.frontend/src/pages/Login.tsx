/* eslint-disable @typescript-eslint/no-explicit-any */
import { useState } from "react";
import { login } from "../services/auth";

interface LoginProps {
    onSwitchToRegister?: () => void;
}

export default function Login({ onSwitchToRegister }: LoginProps) {
    const [name, setName] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!name || !password) {
            setMessage("Por favor completa todos los campos");
            return;
        }

        try {
            const token = await login(name, password);
            localStorage.setItem("token", token);
            setMessage("Inicio de sesión exitoso ✅");
        } catch (error: any) {
            setMessage(error.message);
        }
    };

    return (
        <>
            <h1>Iniciar sesión</h1>
            <form onSubmit={handleSubmit} className="login-form">
                <input
                    type="text"
                    placeholder="Nombre"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                />
                <input
                    type="password"
                    placeholder="Contraseña"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                <button type="submit">Entrar</button>
            </form>
            {message && <p className="message">{message}</p>}
            <p className="text-sm text-gray-700 mt-2">
                ¿No tienes cuenta?{" "}
                <span
                    onClick={onSwitchToRegister}
                    className="switch-link"
                >
                    Crear cuenta
                </span>
            </p>
        </>
    );
}
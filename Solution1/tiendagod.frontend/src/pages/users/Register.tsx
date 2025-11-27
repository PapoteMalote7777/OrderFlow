/* eslint-disable @typescript-eslint/no-explicit-any */
import { useState } from "react";
import { register } from "../../services/auth";

interface RegisterProps {
    onSwitchToLogin?: () => void;
    onRegisterSuccess?: () => void;
}
export default function Register({ onSwitchToLogin, onRegisterSuccess }: RegisterProps) {
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");

    const validatePassword = (password: string) => {
        const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
        return regex.test(password);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (password !== confirmPassword) {
            setMessage("Las contraseñas no coinciden");
            return;
        }

        if (!validatePassword(password)) {
            setMessage(
                "La contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula y un número"
            );
            return;
        }

        try {
            const result = await register(name, email, password);
            setMessage(result);
            if (onRegisterSuccess) onRegisterSuccess();

        } catch (error: any) {
            setMessage(error.message);
        }
    };

    return (
        <>
            <h1>Registro</h1>
            <form onSubmit={handleSubmit} className="register-form">
                <input
                    type="text"
                    placeholder="Nombre"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                />
                <input
                    type="email"
                    placeholder="Correo electrónico"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />
                <input
                    type="password"
                    placeholder="Contraseña"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                <input
                    type="password"
                    placeholder="Confirmar contraseña"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                />
                <button type="submit">Registrarse</button>
            </form>
            {message && <p className="message">{message}</p>}
            <p className="text-sm text-gray-700 mt-2">
                ¿Ya tienes cuenta?{" "}
                <span
                    onClick={onSwitchToLogin}
                    className="switch-link"
                >
                    Iniciar sesión
                </span>
            </p>
        </>
    );
}
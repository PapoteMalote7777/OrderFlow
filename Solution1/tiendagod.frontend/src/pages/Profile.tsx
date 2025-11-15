import React, { useState, useEffect } from "react";

interface ProfileProps {
    username: string;
    setUsername: (name: string) => void;
    onCancel: () => void;
    onDeleteAccount: () => void;
}

export default function Profile({ username, setUsername, onCancel, onDeleteAccount }: ProfileProps) {
    const [newName, setNewName] = useState(username);
    const [message, setMessage] = useState<string | null>(null);
    const [isError, setIsError] = useState(false);
    const [loading, setLoading] = useState(false);

    const token = localStorage.getItem("token");
    if (!token) console.error("Token no encontrado");

    useEffect(() => {
        if (message) {
            const timer = setTimeout(() => setMessage(null), 10000);
            return () => clearTimeout(timer);
        }
    }, [message]);

    const showMessage = (msg: string, error = false) => {
        setMessage(msg);
        setIsError(error);
    };

    const handleSave = async () => {
        if (!newName.trim()) {
            showMessage("El nombre no puede estar vacío", true);
            return;
        }

        setLoading(true);
        try {
            const res = await fetch("/api/auth/update-username", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({ NewName: newName })
            });
            const data = await res.json();
            if (!res.ok) showMessage(data.message || "Error al actualizar", true);
            else {
                setUsername(newName);
                localStorage.setItem("username", newName);
                showMessage(data.message);
            }
        } catch (err) {
            showMessage("No se pudo conectar con el servidor", true);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async () => {
        if (!window.confirm("¿Estás seguro de borrar la cuenta?")) return;

        setLoading(true);
        try {
            const res = await fetch("/api/auth/delete-account", {
                method: "DELETE",
                headers: { "Authorization": `Bearer ${token}` }
            });
            const data = await res.json();
            if (!res.ok) showMessage(data.message || "Error al eliminar", true);
            else {
                localStorage.removeItem("token");
                localStorage.removeItem("username");
                showMessage(data.message);
                setTimeout(() => onDeleteAccount(), 1000);
            }
        } catch {
            showMessage("No se pudo conectar con el servidor", true);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="app-container">
            <div className="form-card">
                <h1>Mi Perfil</h1>
                {message && (
                    <p style={{ color: isError ? "#f44336" : "#4f46e5", fontWeight: "bold" }}>{message}</p>
                )}

                <form className="login-form" onSubmit={(e) => { e.preventDefault(); handleSave(); }}>
                    <label>
                        Nombre de usuario:
                        <input type="text" value={newName} onChange={(e) => setNewName(e.target.value)} required />
                    </label>

                    <button type="submit" disabled={loading}>{loading ? "Guardando..." : "Guardar cambios"}</button>
                    <button type="button" style={{ background: "#f44336", color: "white" }} onClick={handleDelete} disabled={loading}>
                        {loading ? "Eliminando..." : "Eliminar cuenta"}
                    </button>
                    <button type="button" style={{ background: "#ccc" }} onClick={onCancel} disabled={loading}>Volver</button>
                </form>
            </div>
        </div>
    );
}
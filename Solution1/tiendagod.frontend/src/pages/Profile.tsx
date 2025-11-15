import React, { useState } from "react";

interface ProfileProps {
    username: string;
    setUsername: (name: string) => void;
    onCancel: () => void;
    onDeleteAccount: () => void;
}

export default function Profile({ username, setUsername, onCancel, onDeleteAccount }: ProfileProps) {
    const [newName, setNewName] = useState(username);

    const handleSave = () => {
        if (!newName) return alert("El nombre no puede estar vacío");
        setUsername(newName);
        localStorage.setItem("username", newName);
        alert("Nombre actualizado ✅");
    };

    const handleDelete = () => {
        const confirm = window.confirm("¿Estás seguro de borrar la cuenta?");
        if (confirm) {
            onDeleteAccount();
        }
    };

    return (
        <div className="amazon-container">
            <header className="amazon-header">
                <div className="logo">Tienda GOD</div>
            </header>

            <div className="amazon-main">
                <h1 className="title">Mi Perfil</h1>

                <div style={{ maxWidth: "400px", margin: "0 auto", display: "flex", flexDirection: "column", gap: "1rem" }}>
                    <label>
                        Nombre de usuario:
                        <input
                            type="text"
                            value={newName}
                            onChange={(e) => setNewName(e.target.value)}
                        />
                    </label>

                    <button onClick={handleSave}>Guardar cambios</button>
                    <button style={{ background: "#f44336", color: "white" }} onClick={handleDelete}>
                        Eliminar cuenta
                    </button>
                    <button onClick={onCancel} style={{ background: "#ccc" }}>
                        Volver
                    </button>
                </div>
            </div>
        </div>
    );
}
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-require-imports */
import React, { useEffect, useState } from "react";
import { getAllUsers, assignRole, removeRole, isAdmin, adminUpdateUsername, adminDeleteUser } from "../../services/auth";
import "../../App.css";

interface UserWithRoles {
    userName: string;
    email: string;
    roles: string[];
}

interface AdminUsersProps {
    onCancel?: () => void;
}

export default function AdminUsers({ onCancel }: AdminUsersProps) {
    const [users, setUsers] = useState<UserWithRoles[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const [isAdminState, setIsAdminState] = useState<boolean | null>(null);
    const [editingUser, setEditingUser] = useState<string | null>(null);
    const [editingName, setEditingName] = useState<string>("");

    const currentUsername = localStorage.getItem("username") ?? "";

    const load = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await getAllUsers();
            setUsers(data);
        } catch (e: any) {
            setError(e?.message || "Error al cargar usuarios");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        (async () => {
            setLoading(true);
            setError(null);
            try {
                const admin = isAdmin();
                setIsAdminState(admin);
                if (admin) await load();
                else setLoading(false);
            } catch (e: any) {
                setError(e?.message || "Error verificando permisos");
                setIsAdminState(false);
                setLoading(false);
            }
        })();
    }, []);

    const showTemporaryMessage = (setMessage: React.Dispatch<React.SetStateAction<string | null>>, msg: string) => {
        setMessage(msg);
        setTimeout(() => setMessage(null), 10000);
    };

    const toggleAdmin = async (user: UserWithRoles, checked: boolean) => {
        try {
            if (user.userName === currentUsername && !checked) {
                setError("No puedes quitarte el rol Admin a ti mismo.");
                return;
            }

            if (checked) await assignRole(user.userName, "Admin");
            else await removeRole(user.userName, "Admin");
            await load();
            showTemporaryMessage(setSuccess, `Rol ${checked ? "asignado" : "removido"} correctamente a ${user.userName}`);
        } catch (e: any) {
            setError(e?.message || "Error al cambiar rol");
        }
    };

    const startEdit = (u: UserWithRoles) => {
        setEditingUser(u.userName);
        setEditingName(u.userName);
        setError(null);
        setSuccess(null);
    };

    const cancelEdit = () => {
        setEditingUser(null);
        setEditingName("");
    };

    const saveEdit = async () => {
        if (!editingUser) return;
        const trimmed = editingName.trim();
        if (!trimmed) {
            setError("El nombre no puede estar vacío.");
            return;
        }
        try {
            await adminUpdateUsername(editingUser, trimmed);
            await load();
            showTemporaryMessage(setSuccess, "Nombre de usuario actualizado correctamente");
            setEditingUser(null);
            setEditingName("");
        } catch (e: any) {
            setError(e?.message || "Error al actualizar nombre");
        }
    };

    const handleDelete = async (userName: string) => {
        if (userName === currentUsername) {
            setError("No puedes borrar tu propia cuenta desde aquí.");
            return;
        }
        try {
            await adminDeleteUser(userName);
            await load();
            showTemporaryMessage(setSuccess, `Usuario '${userName}' eliminado correctamente`);
        } catch (e: any) {
            setError(e?.message || "Error al eliminar cuenta");
        }
    };

    if (isAdminState === false) return null;

    return (
        <div className="admin-panel">
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                <h2>Administración de usuarios</h2>
                {onCancel && (
                    <button className="sidebar-toggle" onClick={onCancel}>
                        Volver
                    </button>
                )}
            </div>
            {error && <div className="error" style={{ marginBottom: 20 }}>{error}</div>}
            {success && <div className="success" style={{ marginBottom: 20 }}>{success}</div>}
            {loading ? (
                <div>Cargando usuarios...</div>
            ) : (
                <table>
                    <thead>
                        <tr>
                            <th>Usuario</th>
                            <th>Email</th>
                            <th>Admin</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map(u => (
                            <tr key={u.userName}>
                                <td style={{ minWidth: 220 }}>
                                    {editingUser === u.userName ? (
                                        <input
                                            value={editingName}
                                            onChange={(e) => setEditingName(e.target.value)}
                                        />
                                    ) : (
                                        <strong>{u.userName}</strong>
                                    )}
                                </td>
                                <td>{u.email}</td>
                                <td>
                                    <input
                                        type="checkbox"
                                        checked={u.roles.includes("Admin")}
                                        disabled={u.userName === currentUsername}
                                        onChange={(e) => toggleAdmin(u, e.target.checked)}
                                    />
                                </td>
                                <td>
                                    {editingUser === u.userName ? (
                                        <>
                                            <button onClick={saveEdit} style={{ marginRight: 8 }}>Guardar</button>
                                            <button onClick={cancelEdit}>Cancelar</button>
                                        </>
                                    ) : (
                                        <>
                                            <button onClick={() => startEdit(u)} style={{ marginRight: 8 }}>Editar</button>
                                            <button onClick={() => handleDelete(u.userName)} disabled={u.userName === currentUsername}>Borrar</button>
                                        </>
                                    )}
                                    <div style={{ marginTop: 6, fontSize: 12, color: "#666" }}>
                                        {u.roles.join(", ")}
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}
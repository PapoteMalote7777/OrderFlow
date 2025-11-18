/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from "react";
import { getAllUsers, assignRole, removeRole } from "../services/auth";

interface UserWithRoles {
    userName: string;
    email: string;
    roles: string[];
}

export default function AdminUsers() {
    const [users, setUsers] = useState<UserWithRoles[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const load = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await getAllUsers();
            setUsers(data);
        } catch (e: any) {
            setError(e.message || "Error al cargar usuarios");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        load();
    }, []);

    const toggleAdmin = async (user: UserWithRoles, checked: boolean) => {
        try {
            if (checked) {
                await assignRole(user.userName, "Admin");
            } else {
                await removeRole(user.userName, "Admin");
            }
            await load();
        } catch (e: any) {
            setError(e.message || "Error al cambiar rol");
        }
    };

    if (loading) return <div>Cargando usuarios...</div>;
    if (error) return <div className="error">Error: {error}</div>;

    return (
        <div>
            <h2>Administración de usuarios</h2>
            <table>
                <thead>
                    <tr>
                        <th>Usuario</th>
                        <th>Email</th>
                        <th>Admin</th>
                        <th>Roles</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(u => (
                        <tr key={u.userName}>
                            <td>{u.userName}</td>
                            <td>{u.email}</td>
                            <td>
                                <input
                                    type="checkbox"
                                    checked={u.roles.includes("Admin")}
                                    onChange={(e) => toggleAdmin(u, e.target.checked)}
                                />
                            </td>
                            <td>{u.roles.join(", ")}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
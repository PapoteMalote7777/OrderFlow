/* eslint-disable @typescript-eslint/no-explicit-any */
export const API_URL = "/api";

export function getToken(): string | null {
    return localStorage.getItem("token");
}

export function setToken(token: string | null) {
    if (token) localStorage.setItem("token", token);
    else localStorage.removeItem("token");
}

export async function register(name: string, email: string, password: string) {
    const res = await fetch(`/api/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, email, password })
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error en registro: ${text}`);
    }

    return res.text();
}

export async function login(name: string, password: string) {
    const res = await fetch(`/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, password })
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(`Error en login: ${text}`);
    }

    const data = await res.json();
    return data.token;
}

async function parseJsonSafe(res: Response) {
    const text = await res.text();
    try { return JSON.parse(text); } catch { return text; }
}

export async function updateUsername(newName: string) {
    const token = getToken();
    const res = await fetch(`/api/user/update`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        },
        body: JSON.stringify({ NewName: newName })
    });

    const body = await parseJsonSafe(res);
    if (!res.ok) throw new Error(body?.message || body || res.statusText);
    return body;
}

export async function deleteAccount() {
    const token = getToken();
    const res = await fetch(`/api/user/delete`, {
        method: "DELETE",
        headers: {
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        }
    });

    const body = await parseJsonSafe(res);
    if (!res.ok) throw new Error(body?.message || body || res.statusText);
    return body;
}

function base64UrlDecode(input: string): string {
    input = input.replace(/-/g, "+").replace(/_/g, "/");
    const pad = input.length % 4;
    if (pad === 2) input += "==";
    else if (pad === 3) input += "=";
    else if (pad !== 0) return "";
    try {
        return atob(input);
    } catch {
        return "";
    }
}

export function parseJwtPayload(token: string | null): Record<string, any> | null {
    if (!token) return null;
    const parts = token.split(".");
    if (parts.length < 2) return null;
    const payload = base64UrlDecode(parts[1]);
    if (!payload) return null;
    try {
        return JSON.parse(payload);
    } catch {
        return null;
    }
}

export function getRolesFromToken(): string[] {
    const token = getToken();
    const payload = parseJwtPayload(token);
    if (!payload) return [];

    const roles: string[] = [];
    const roleKeys = [
        "role",
        "roles",
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        "roles"
    ];
    for (const key of roleKeys) {
        const v = payload[key];
        if (!v) continue;
        if (Array.isArray(v)) roles.push(...v.map(String));
        else roles.push(String(v));
    }
    return Array.from(new Set(roles.map(r => r.trim()).filter(Boolean)));
}

export function isAdmin(): boolean {
    return getRolesFromToken().some(r => r.toLowerCase() === "admin");
}
export async function getAllUsers() {
    const token = getToken();
    const res = await fetch(`/api/roles/list`, {
        headers: {
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            "Content-Type": "application/json"
        }
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(text || res.statusText);
    }

    return res.json();
}

export async function assignRole(userName: string, role: string) {
    const token = getToken();
    const res = await fetch(`/api/roles/assign`, {
        method: "POST",
        headers: {
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ userName, role })
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(text || res.statusText);
    }

    return res.json();
}

export async function removeRole(userName: string, role: string) {
    const token = getToken();
    const res = await fetch(`/api/roles/remove`, {
        method: "POST",
        headers: {
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ userName, role })
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error(text || res.statusText);
    }

    return res.json();
}

export async function adminUpdateUsername(userName: string, newName: string) {
    const token = getToken();
    const res = await fetch(`/api/userAdmin/update-user/${encodeURIComponent(userName)}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        },
        body: JSON.stringify({ NewName: newName })
    });

    const text = await res.text();
    try {
        const body = JSON.parse(text);
        if (!res.ok) throw new Error(body?.message || body || res.statusText);
        return body;
    } catch {
        if (!res.ok) throw new Error(text || res.statusText);
        return text;
    }
}

export async function adminDeleteUser(userName: string) {
    const token = getToken();
    const res = await fetch(`/api/userAdmin/delete-user/${encodeURIComponent(userName)}`, {
        method: "DELETE",
        headers: {
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        }
    });

    const text = await res.text();
    try {
        const body = JSON.parse(text);
        if (!res.ok) throw new Error(body?.message || body || res.statusText);
        return body;
    } catch {
        if (!res.ok) throw new Error(text || res.statusText);
        return text;
    }
}
export function getUserIdFromToken(): string | null {
    const token = getToken();
    const payload = parseJwtPayload(token);

    if (!payload) return null;

    return (
        payload.sub ||
        payload.nameid ||
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
        null
    );
}
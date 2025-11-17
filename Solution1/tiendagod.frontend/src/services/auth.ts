export const API_URL = "https://localhost:7134";

export function getToken(): string | null {
    return localStorage.getItem("token");
}

export function setToken(token: string | null) {
    if (token) localStorage.setItem("token", token);
    else localStorage.removeItem("token");
}

export async function register(name: string, email: string, password: string) {
    const res = await fetch(`${API_URL}/api/auth/register`, {
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
    const res = await fetch(`${API_URL}/api/auth/login`, {
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
    const res = await fetch(`${API_URL}/api/user/update`, {
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
    const res = await fetch(`${API_URL}/api/user/delete`, {
        method: "DELETE",
        headers: {
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        }
    });

    const body = await parseJsonSafe(res);
    if (!res.ok) throw new Error(body?.message || body || res.statusText);
    return body;
}
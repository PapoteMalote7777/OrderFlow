export async function register(name: string, email: string, password: string) {
    const res = await fetch("https://localhost:7134/api/auth/register", {
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
    const res = await fetch("https://localhost:7134/api/auth/login", {
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
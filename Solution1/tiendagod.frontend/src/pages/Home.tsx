interface HomeProps {
    onLogout: () => void;
}
export default function Home({ onLogout }: HomeProps) {
    return (
        <div className="form-card">
            <h1>Bienvenido a Home</h1>
            <p>Has iniciado sesión correctamente ✅</p>
            <button onClick={onLogout}>Cerrar sesión</button>
        </div>
    );
}
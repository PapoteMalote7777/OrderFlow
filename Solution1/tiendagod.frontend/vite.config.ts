import { defineConfig } from 'vite';
import react from "@vitejs/plugin-react";

export default defineConfig({
    plugins: [react()],
    server: {
        port: parseInt(process.env.PORT || "59210"),
        strictPort: true,
        host: true,
        /*proxy: {
            "/api": {
                target: "http://localhost:5248",  // ⭐ BACKEND REAL
                changeOrigin: true,
                secure: false,
            }
        }*/
    }
});

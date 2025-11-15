import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    server: {
        port: parseInt(process.env.PORT || '59210'),
        strictPort: true,
        host: true,
    }
})

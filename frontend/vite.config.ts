import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': { target: 'http://localhost:5141', changeOrigin: true },
    },
  },
  build: {
    // npm run build → derlenmiş site doğrudan backend'in wwwroot klasörüne
    outDir: '../backend/src/StokPanel.Api/wwwroot',
    emptyOutDir: true,
  },
})
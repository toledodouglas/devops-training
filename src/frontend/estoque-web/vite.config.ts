import react from '@vitejs/plugin-react'
import { defineConfig } from 'vitest/config'

const apiTarget =
  process.env.VITE_API_TARGET?.trim().length ?? 0
    ? process.env.VITE_API_TARGET!
    : 'http://localhost:5277'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: apiTarget,
        changeOrigin: true,
      },
    },
  },
})

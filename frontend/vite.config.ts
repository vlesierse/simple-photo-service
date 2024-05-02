import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import svgr from "@svgr/rollup";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), svgr()],
  define: {
    global: {},
  },
  server: {
    host: true,
    port: parseInt(process.env.PORT ?? "5173"),
  },
});

/**
 * Local dev proxy — bypass CORS when FE calls a local origin that forwards to the real API.
 * Loads `.env.local` (same as Next.js) for API_PROXY_PORT / API_PROXY_TARGET.
 *
 * FE should set NEXT_PUBLIC_API_URL=http://localhost:{API_PROXY_PORT}
 * Proxy forwards /api/* → API_PROXY_TARGET
 */
const fs = require("fs");
const http = require("http");
const https = require("https");
const path = require("path");
const { URL } = require("url");

function loadEnvLocal() {
  const envPath = path.join(__dirname, ".env.local");
  if (!fs.existsSync(envPath)) return;

  for (const line of fs.readFileSync(envPath, "utf8").split(/\r?\n/)) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith("#")) continue;
    const eq = trimmed.indexOf("=");
    if (eq === -1) continue;
    const key = trimmed.slice(0, eq).trim();
    const value = trimmed.slice(eq + 1).trim().replace(/^["']|["']$/g, "");
    if (key && process.env[key] === undefined) {
      process.env[key] = value;
    }
  }
}

loadEnvLocal();

const PROXY_PORT = Number(process.env.API_PROXY_PORT) || 4000;
const BACKEND_URL =
  process.env.API_PROXY_TARGET || "https://api.ezsim.vn";
  
const server = http.createServer((req, res) => {
  res.setHeader("Access-Control-Allow-Origin", "*");
  res.setHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, PATCH, OPTIONS");
  res.setHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");

  if (req.method === "OPTIONS") {
    res.writeHead(204);
    res.end();
    return;
  }

  const targetUrl = new URL(req.url, BACKEND_URL);

  const options = {
    hostname: targetUrl.hostname,
    port: targetUrl.protocol === "https:" ? 443 : targetUrl.port || 80,
    path: targetUrl.pathname + targetUrl.search,
    method: req.method,
    headers: {
      ...req.headers,
      host: targetUrl.hostname,
    },
  };

  delete options.headers.origin;
  delete options.headers.referer;

  const transport = targetUrl.protocol === "https:" ? https : http;

  const proxyReq = transport.request(options, (proxyRes) => {
    const responseHeaders = { ...proxyRes.headers };
    responseHeaders["access-control-allow-origin"] = "*";
    delete responseHeaders["transfer-encoding"];

    res.writeHead(proxyRes.statusCode, responseHeaders);
    proxyRes.pipe(res, { end: true });
  });

  proxyReq.on("error", (err) => {
    console.error("Proxy error:", err.message);
    res.writeHead(502, { "Content-Type": "application/json" });
    res.end(JSON.stringify({ message: "Proxy error: " + err.message }));
  });

  req.pipe(proxyReq, { end: true });
});

server.listen(PROXY_PORT, () => {
  console.log(`✓ Dev proxy: http://localhost:${PROXY_PORT} → ${BACKEND_URL}`);
});

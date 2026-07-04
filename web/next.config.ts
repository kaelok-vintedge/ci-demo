import type { NextConfig } from "next";

// Same pattern as the real frontend: the browser calls /api/v1/* on the
// Next.js origin and the rewrite forwards it to the .NET API.
const API_BASE_URL = process.env.API_BASE_URL ?? "http://localhost:5252";

const nextConfig: NextConfig = {
  async rewrites() {
    return [
      {
        source: "/api/v1/:path*",
        destination: `${API_BASE_URL}/api/v1/:path*`,
      },
    ];
  },
};

export default nextConfig;

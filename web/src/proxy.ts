import { NextRequest, NextResponse } from "next/server";

// Miniature of the real proxy.ts: code that runs before every matched
// request. Here the "auth" is a plain cookie so the concept stays visible
// without a login system — /admin is blocked unless the cookie is set.
const ADMIN_COOKIE = "demo_admin";

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (pathname.startsWith("/admin")) {
    if (request.cookies.get(ADMIN_COOKIE)?.value !== "1") {
      return NextResponse.redirect(new URL("/", request.url));
    }
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/admin/:path*"],
};

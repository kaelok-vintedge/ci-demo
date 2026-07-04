import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "ci-demo",
  description: "Miniature class-booking app demonstrating a gated CI pipeline",
};

export default function RootLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}

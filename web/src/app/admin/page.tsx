export default function AdminPage() {
  return (
    <main>
      <h1>Admin area</h1>
      <p>
        You can only see this because the <code>demo_admin=1</code> cookie is
        set — <code>src/proxy.ts</code> redirected you otherwise. This is the
        same mechanism the real platform uses for its member/studio/admin
        portals, with JWTs instead of a demo cookie.
      </p>
    </main>
  );
}

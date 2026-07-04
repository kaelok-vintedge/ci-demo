"use client";

import { useEffect, useState } from "react";
import { Session, sessionListSchema } from "@/lib/credits";

// Demo page: list sessions from the .NET API and book one.
// (The real app uses TanStack Query for this; a plain effect keeps the
// demo dependency-free.)
export default function Home() {
  const [sessions, setSessions] = useState<Session[]>([]);
  const [status, setStatus] = useState<string>("Loading sessions…");

  async function loadSessions() {
    try {
      const res = await fetch("/api/v1/sessions");
      const parsed = sessionListSchema.safeParse(await res.json());
      if (!parsed.success) {
        setStatus("The API returned an unexpected payload.");
        return;
      }
      setSessions(parsed.data);
      setStatus("");
    } catch {
      setStatus("API not reachable — start it with: dotnet run --project api/Api");
    }
  }

  useEffect(() => {
    // Deferring to a microtask keeps setState out of the synchronous effect
    // body (react-hooks/set-state-in-effect). The real app avoids this
    // entirely by fetching with TanStack Query.
    void Promise.resolve().then(loadSessions);
  }, []);

  async function book(classSessionId: number) {
    const memberName = window.prompt("Your name?")?.trim();
    if (!memberName) return;

    const res = await fetch("/api/v1/bookings", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ classSessionId, memberName }),
    });

    if (res.ok) {
      setStatus(`Booked! ${memberName} is in.`);
      await loadSessions();
    } else {
      const body = await res.json().catch(() => null);
      setStatus(body?.error ?? "Booking failed — check the input.");
    }
  }

  return (
    <main>
      <h1>ci-demo — class booking</h1>
      <p>
        A miniature of the real platform, here to demonstrate the CI
        pipeline. Try booking the same class twice, or the one that already
        started.
      </p>
      {status && <p role="status">{status}</p>}
      <ul>
        {sessions.map((s) => (
          <li key={s.id}>
            <strong>{s.name}</strong> — {s.spotsLeft}/{s.maxCapacity} spots,{" "}
            {s.costCredits} credits{s.isPeak ? " (peak)" : ""}{" "}
            <button onClick={() => book(s.id)}>Book</button>
          </li>
        ))}
      </ul>
      <p>
        <a href="/admin">/admin</a> is protected by <code>src/proxy.ts</code> —
        it redirects here unless the <code>demo_admin=1</code> cookie is set.
      </p>
    </main>
  );
}

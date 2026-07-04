import { describe, expect, it } from "vitest";
import { costForSession, sessionListSchema } from "./credits";

describe("costForSession", () => {
  it("charges peak credits for peak sessions", () => {
    expect(costForSession(true, 8, 5)).toBe(8);
  });

  it("charges off-peak credits for off-peak sessions", () => {
    expect(costForSession(false, 8, 5)).toBe(5);
  });
});

describe("sessionListSchema", () => {
  it("rejects a malformed API payload", () => {
    const malformed = [{ id: "not-a-number", name: 42 }];
    expect(sessionListSchema.safeParse(malformed).success).toBe(false);
  });
});

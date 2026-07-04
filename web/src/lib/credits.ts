import { z } from "zod";

// Validation at the boundary: the API response is untrusted input to the
// frontend until it passes this schema.
export const sessionSchema = z.object({
  id: z.number().int().positive(),
  name: z.string(),
  startsAtUtc: z.string(),
  maxCapacity: z.number().int().nonnegative(),
  spotsLeft: z.number().int(),
  isPeak: z.boolean(),
  costCredits: z.number().int().nonnegative(),
});

export const sessionListSchema = z.array(sessionSchema);

export type Session = z.infer<typeof sessionSchema>;

export function costForSession(
  isPeak: boolean,
  creditsPeak: number,
  creditsOffPeak: number,
): number {
  return isPeak ? creditsPeak : creditsOffPeak;
}

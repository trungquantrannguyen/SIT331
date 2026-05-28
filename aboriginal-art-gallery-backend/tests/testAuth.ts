import jwt from "jsonwebtoken";
import { UserRole } from "../src/models/User";

export const createTestToken = (
  role: UserRole = "admin",
  id = "665f1e111111111111111111",
  email = "test-admin@gallery.com",
): string => {
  const jwtSecret = process.env.JWT_SECRET;

  if (!jwtSecret) {
    throw new Error("JWT_SECRET is missing from environment variables.");
  }

  return jwt.sign(
    {
      id,
      email,
      role,
    },
    jwtSecret,
    {
      expiresIn: "1h",
    },
  );
};

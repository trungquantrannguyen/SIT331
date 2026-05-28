import request from "supertest";
import app from "../src/app";

describe("Protected route authorization", () => {
  it("should reject profile access without JWT token", async () => {
    const response = await request(app).get("/api/auth/profile");

    expect(response.status).toBe(401);
    expect(response.body.status).toBe(401);
    expect(response.body.message).toBe("Access denied. No token provided.");
  });

  it("should reject admin-only access without JWT token", async () => {
    const response = await request(app).get("/api/auth/admin-only");

    expect(response.status).toBe(401);
    expect(response.body.status).toBe(401);
    expect(response.body.message).toBe("Access denied. No token provided.");
  });

  it("should reject invalid JWT token", async () => {
    const response = await request(app)
      .get("/api/auth/profile")
      .set("Authorization", "Bearer invalid-token-value");

    expect(response.status).toBe(401);
    expect(response.body.status).toBe(401);
    expect(response.body.message).toBe("Invalid or expired token.");
  });
});

import request from "supertest";
import app from "../src/app";

describe("Authentication route validation", () => {
  it("should reject registration with weak password", async () => {
    const response = await request(app).post("/api/auth/register").send({
      name: "Weak Password User",
      email: "weak@example.com",
      password: "weak",
    });

    expect(response.status).toBe(400);
    expect(response.body.status).toBe(400);
    expect(response.body.message).toBe("Validation failed");
  });

  it("should reject registration with invalid email", async () => {
    const response = await request(app).post("/api/auth/register").send({
      name: "Invalid Email User",
      email: "not-an-email",
      password: "Password123",
    });

    expect(response.status).toBe(400);
    expect(response.body.status).toBe(400);
    expect(response.body.message).toBe("Validation failed");
  });

  it("should reject login with invalid email format", async () => {
    const response = await request(app).post("/api/auth/login").send({
      email: "wrong-email-format",
      password: "Password123",
    });

    expect(response.status).toBe(400);
    expect(response.body.status).toBe(400);
    expect(response.body.message).toBe("Validation failed");
  });
});

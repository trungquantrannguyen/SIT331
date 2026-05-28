import request from "supertest";
import app from "../src/app";

describe("Health and base API routes", () => {
  it("should return base API information", async () => {
    const response = await request(app).get("/");

    expect(response.status).toBe(200);
    expect(response.body.status).toBe(200);
    expect(response.body.message).toBe(
      "Aboriginal Art Gallery Backend API is running.",
    );
    expect(response.body.data.project).toBe("Aboriginal Art Gallery Backend");
    expect(response.body.data.unit).toBe("SIT331");
  });

  it("should return health check information", async () => {
    const response = await request(app).get("/health");

    expect(response.status).toBe(200);
    expect(response.body.status).toBe(200);
    expect(response.body.message).toBe("Server health check successful.");
    expect(response.body.data).toHaveProperty("uptime");
    expect(response.body.data).toHaveProperty("timestamp");
  });
});

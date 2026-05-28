import request from "supertest";
import app from "../src/app";
import { connectTestDb, clearTestDb, closeTestDb } from "./testDb";
import { createTestToken } from "./testAuth";

describe("Artists integration tests", () => {
  const adminToken = createTestToken("admin");
  const curatorToken = createTestToken("curator");
  const memberToken = createTestToken("member");

  beforeAll(async () => {
    await connectTestDb();
  });

  beforeEach(async () => {
    await clearTestDb();
  });

  afterAll(async () => {
    await closeTestDb();
  });

  it("should create an artist with admin token", async () => {
    const response = await request(app)
      .post("/api/artists")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Integration Test Artist",
        nationOrCommunity: "Anmatyerre",
        languageGroup: "Anmatyerre",
        biography:
          "This is a test biography for an Aboriginal artist used in database-connected integration testing.",
        birthYear: 1950,
        region: "Central Desert",
        artStyles: ["Dot Painting", "Contemporary Aboriginal Art"],
      });

    expect(response.status).toBe(201);
    expect(response.body.status).toBe(201);
    expect(response.body.message).toBe("Artist created successfully.");
    expect(response.body.data.name).toBe("Integration Test Artist");
    expect(response.body.data.nationOrCommunity).toBe("Anmatyerre");
  });

  it("should reject artist creation without token", async () => {
    const response = await request(app).post("/api/artists").send({
      name: "No Token Artist",
      nationOrCommunity: "Test Community",
      biography:
        "This request should fail because no authentication token is provided.",
    });

    expect(response.status).toBe(401);
    expect(response.body.message).toBe("Access denied. No token provided.");
  });

  it("should reject artist creation with member token", async () => {
    const response = await request(app)
      .post("/api/artists")
      .set("Authorization", `Bearer ${memberToken}`)
      .send({
        name: "Member Token Artist",
        nationOrCommunity: "Test Community",
        biography:
          "This request should fail because a member role is not allowed to create artists.",
      });

    expect(response.status).toBe(403);
    expect(response.body.message).toBe(
      "Forbidden. You do not have permission to access this resource.",
    );
  });

  it("should get all active artists", async () => {
    await request(app)
      .post("/api/artists")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Readable Artist",
        nationOrCommunity: "Western Desert",
        biography:
          "This artist is created so the get all artists endpoint can be tested with real database data.",
        region: "Western Desert",
        artStyles: ["Storytelling"],
      });

    const response = await request(app)
      .get("/api/artists")
      .set("Authorization", `Bearer ${adminToken}`);

    expect(response.status).toBe(200);
    expect(response.body.status).toBe(200);
    expect(response.body.data).toHaveLength(1);
    expect(response.body.data[0].name).toBe("Readable Artist");
  });

  it("should update an artist with curator token", async () => {
    const createResponse = await request(app)
      .post("/api/artists")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Artist Before Update",
        nationOrCommunity: "Central Desert",
        biography:
          "This artist will be updated during the integration test to verify update functionality.",
        region: "Central Desert",
        artStyles: ["Dot Painting"],
      });

    const artistId = createResponse.body.data._id;

    const updateResponse = await request(app)
      .put(`/api/artists/${artistId}`)
      .set("Authorization", `Bearer ${curatorToken}`)
      .send({
        region: "Updated Region",
        artStyles: ["Dot Painting", "Updated Style"],
      });

    expect(updateResponse.status).toBe(200);
    expect(updateResponse.body.message).toBe("Artist updated successfully.");
    expect(updateResponse.body.data.region).toBe("Updated Region");
    expect(updateResponse.body.data.artStyles).toContain("Updated Style");
  });

  it("should soft delete an artist with admin token", async () => {
    const createResponse = await request(app)
      .post("/api/artists")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Artist To Delete",
        nationOrCommunity: "Test Community",
        biography:
          "This artist will be soft deleted during the integration test to verify deletion behavior.",
      });

    const artistId = createResponse.body.data._id;

    const deleteResponse = await request(app)
      .delete(`/api/artists/${artistId}`)
      .set("Authorization", `Bearer ${adminToken}`);

    expect(deleteResponse.status).toBe(200);
    expect(deleteResponse.body.message).toBe("Artist deleted successfully.");
    expect(deleteResponse.body.data.isActive).toBe(false);

    const getResponse = await request(app)
      .get(`/api/artists/${artistId}`)
      .set("Authorization", `Bearer ${adminToken}`);

    expect(getResponse.status).toBe(404);
    expect(getResponse.body.message).toBe("Artist not found.");
  });
});

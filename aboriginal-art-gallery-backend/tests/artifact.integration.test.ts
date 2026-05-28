import request from "supertest";
import app from "../src/app";
import { connectTestDb, clearTestDb, closeTestDb } from "./testDb";
import { createTestToken } from "./testAuth";
import { Artist } from "../src/models/Artist";

describe("Artifacts integration tests", () => {
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

  const createTestArtist = async () => {
    return Artist.create({
      name: "Artifact Test Artist",
      nationOrCommunity: "Anmatyerre",
      languageGroup: "Anmatyerre",
      biography:
        "This artist is created as a dependency for artifact integration testing.",
      region: "Central Desert",
      artStyles: ["Dot Painting"],
    });
  };

  it("should create an artifact with admin token and valid artist", async () => {
    const artist = await createTestArtist();

    const response = await request(app)
      .post("/api/artifacts")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Integration Test Artifact",
        artist: artist._id.toString(),
        description:
          "This artifact is created during database-connected integration testing.",
        artType: "Painting",
        materials: ["Acrylic on canvas"],
        yearCreated: 2022,
        culturalRegion: "Central Desert",
        dimensions: {
          heightCm: 80,
          widthCm: 120,
          depthCm: 3,
        },
        priceAud: 2500,
        status: "on_display",
        tags: ["integration", "artifact", "testing"],
      });

    expect(response.status).toBe(201);
    expect(response.body.status).toBe(201);
    expect(response.body.message).toBe("Artifact created successfully.");
    expect(response.body.data.title).toBe("Integration Test Artifact");
    expect(response.body.data.artist.name).toBe("Artifact Test Artist");
  });

  it("should reject artifact creation without token", async () => {
    const artist = await createTestArtist();

    const response = await request(app).post("/api/artifacts").send({
      title: "No Token Artifact",
      artist: artist._id.toString(),
      description:
        "This request should fail because no authentication token is provided.",
      artType: "Painting",
    });

    expect(response.status).toBe(401);
    expect(response.body.message).toBe("Access denied. No token provided.");
  });

  it("should reject artifact creation with member token", async () => {
    const artist = await createTestArtist();

    const response = await request(app)
      .post("/api/artifacts")
      .set("Authorization", `Bearer ${memberToken}`)
      .send({
        title: "Member Token Artifact",
        artist: artist._id.toString(),
        description:
          "This request should fail because a member role cannot create artifacts.",
        artType: "Painting",
      });

    expect(response.status).toBe(403);
    expect(response.body.message).toBe(
      "Forbidden. You do not have permission to access this resource.",
    );
  });

  it("should reject artifact creation with invalid artist ID", async () => {
    const response = await request(app)
      .post("/api/artifacts")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Invalid Artist Artifact",
        artist: "64f111111111111111111111",
        description:
          "This request should fail because the artist does not exist in the database.",
        artType: "Painting",
        materials: ["Acrylic on canvas"],
        tags: ["invalid"],
      });

    expect(response.status).toBe(400);
    expect(response.body.message).toBe(
      "Cannot create artifact because the artist does not exist.",
    );
  });

  it("should get all active artifacts with authenticated user", async () => {
    const artist = await createTestArtist();

    await request(app)
      .post("/api/artifacts")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Readable Artifact",
        artist: artist._id.toString(),
        description:
          "This artifact is created so the get all artifacts endpoint can be tested.",
        artType: "Painting",
        materials: ["Acrylic on canvas"],
        culturalRegion: "Central Desert",
        status: "available",
        tags: ["readable", "testing"],
      });

    const response = await request(app)
      .get("/api/artifacts")
      .set("Authorization", `Bearer ${memberToken}`);

    expect(response.status).toBe(200);
    expect(response.body.status).toBe(200);
    expect(response.body.data).toHaveLength(1);
    expect(response.body.data[0].title).toBe("Readable Artifact");
    expect(response.body.data[0].artist.name).toBe("Artifact Test Artist");
  });

  it("should search artifacts by tag", async () => {
    const artist = await createTestArtist();

    await request(app)
      .post("/api/artifacts")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Searchable Artifact",
        artist: artist._id.toString(),
        description:
          "This artifact is created so the artifact tag search endpoint can be tested.",
        artType: "Painting",
        materials: ["Acrylic on canvas"],
        culturalRegion: "Central Desert",
        status: "available",
        tags: ["unique-artifact-tag"],
      });

    const matchResponse = await request(app)
      .get("/api/artifacts?tag=unique-artifact-tag")
      .set("Authorization", `Bearer ${memberToken}`);

    expect(matchResponse.status).toBe(200);
    expect(matchResponse.body.data).toHaveLength(1);
    expect(matchResponse.body.data[0].title).toBe("Searchable Artifact");

    const noMatchResponse = await request(app)
      .get("/api/artifacts?tag=does-not-exist")
      .set("Authorization", `Bearer ${memberToken}`);

    expect(noMatchResponse.status).toBe(200);
    expect(noMatchResponse.body.data).toHaveLength(0);
  });

  it("should update an artifact with curator token", async () => {
    const artist = await createTestArtist();

    const createResponse = await request(app)
      .post("/api/artifacts")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Artifact Before Update",
        artist: artist._id.toString(),
        description:
          "This artifact will be updated during the integration test process.",
        artType: "Painting",
        materials: ["Acrylic on canvas"],
        status: "available",
        tags: ["before-update"],
      });

    const artifactId = createResponse.body.data._id;

    const updateResponse = await request(app)
      .put(`/api/artifacts/${artifactId}`)
      .set("Authorization", `Bearer ${curatorToken}`)
      .send({
        status: "reserved",
        priceAud: 3000,
        tags: ["after-update", "curator-updated"],
      });

    expect(updateResponse.status).toBe(200);
    expect(updateResponse.body.message).toBe("Artifact updated successfully.");
    expect(updateResponse.body.data.status).toBe("reserved");
    expect(updateResponse.body.data.tags).toContain("curator-updated");
  });

  it("should soft delete an artifact with admin token", async () => {
    const artist = await createTestArtist();

    const createResponse = await request(app)
      .post("/api/artifacts")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Artifact To Delete",
        artist: artist._id.toString(),
        description:
          "This artifact will be soft deleted during integration testing.",
        artType: "Painting",
        materials: ["Acrylic on canvas"],
        status: "available",
        tags: ["delete-test"],
      });

    const artifactId = createResponse.body.data._id;

    const deleteResponse = await request(app)
      .delete(`/api/artifacts/${artifactId}`)
      .set("Authorization", `Bearer ${adminToken}`);

    expect(deleteResponse.status).toBe(200);
    expect(deleteResponse.body.message).toBe("Artifact deleted successfully.");
    expect(deleteResponse.body.data.isActive).toBe(false);
    expect(deleteResponse.body.data.status).toBe("archived");

    const getResponse = await request(app)
      .get(`/api/artifacts/${artifactId}`)
      .set("Authorization", `Bearer ${memberToken}`);

    expect(getResponse.status).toBe(404);
    expect(getResponse.body.message).toBe("Artifact not found.");
  });

  it("should reject get all artifacts without token", async () => {
    const response = await request(app).get("/api/artifacts");

    expect(response.status).toBe(401);
    expect(response.body.message).toBe("Access denied. No token provided.");
  });
});

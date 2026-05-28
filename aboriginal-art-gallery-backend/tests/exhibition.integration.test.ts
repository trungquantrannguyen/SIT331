import request from "supertest";
import app from "../src/app";
import { connectTestDb, clearTestDb, closeTestDb } from "./testDb";
import { createTestToken } from "./testAuth";
import { Artist } from "../src/models/Artist";
import { Artifact } from "../src/models/Artifact";

describe("Exhibitions integration tests", () => {
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

  const createFeaturedArtifact = async () => {
    const artist = await Artist.create({
      name: "Exhibition Test Artist",
      nationOrCommunity: "Anmatyerre",
      languageGroup: "Anmatyerre",
      biography:
        "This artist is created as a dependency for exhibition integration tests.",
      region: "Central Desert",
      artStyles: ["Dot Painting"],
    });

    const artifact = await Artifact.create({
      title: "Exhibition Test Artifact",
      artist: artist._id,
      description:
        "This artifact is created as a dependency for exhibition integration tests.",
      artType: "Painting",
      materials: ["Acrylic on canvas"],
      yearCreated: 2021,
      culturalRegion: "Central Desert",
      status: "on_display",
      tags: ["exhibition", "testing"],
    });

    return artifact;
  };

  it("should create an exhibition with admin token and valid featured artifact", async () => {
    const artifact = await createFeaturedArtifact();

    const response = await request(app)
      .post("/api/exhibitions")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Integration Test Exhibition",
        description:
          "This exhibition is created during database-connected integration testing.",
        location: "Main Gallery Hall",
        startDate: "2026-06-01T00:00:00.000Z",
        endDate: "2026-08-01T00:00:00.000Z",
        status: "planned",
        featuredArtifacts: [artifact._id.toString()],
        curatorNotes:
          "This exhibition is used only for automated integration testing.",
        tags: ["integration", "exhibition", "testing"],
      });

    expect(response.status).toBe(201);
    expect(response.body.status).toBe(201);
    expect(response.body.message).toBe("Exhibition created successfully.");
    expect(response.body.data.title).toBe("Integration Test Exhibition");
    expect(response.body.data.featuredArtifacts).toHaveLength(1);
    expect(response.body.data.featuredArtifacts[0].title).toBe(
      "Exhibition Test Artifact",
    );
  });

  it("should reject exhibition creation without token", async () => {
    const artifact = await createFeaturedArtifact();

    const response = await request(app)
      .post("/api/exhibitions")
      .send({
        title: "No Token Exhibition",
        description:
          "This request should fail because no authentication token is provided.",
        location: "Test Gallery",
        startDate: "2026-06-01T00:00:00.000Z",
        endDate: "2026-08-01T00:00:00.000Z",
        featuredArtifacts: [artifact._id.toString()],
      });

    expect(response.status).toBe(401);
    expect(response.body.message).toBe("Access denied. No token provided.");
  });

  it("should reject exhibition creation with member token", async () => {
    const artifact = await createFeaturedArtifact();

    const response = await request(app)
      .post("/api/exhibitions")
      .set("Authorization", `Bearer ${memberToken}`)
      .send({
        title: "Member Token Exhibition",
        description:
          "This request should fail because a member role cannot create exhibitions.",
        location: "Test Gallery",
        startDate: "2026-06-01T00:00:00.000Z",
        endDate: "2026-08-01T00:00:00.000Z",
        featuredArtifacts: [artifact._id.toString()],
      });

    expect(response.status).toBe(403);
    expect(response.body.message).toBe(
      "Forbidden. You do not have permission to access this resource.",
    );
  });

  it("should reject exhibition creation with invalid featured artifact ID", async () => {
    const response = await request(app)
      .post("/api/exhibitions")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Invalid Artifact Exhibition",
        description:
          "This request should fail because the featured artifact does not exist in the database.",
        location: "Test Gallery",
        startDate: "2026-06-01T00:00:00.000Z",
        endDate: "2026-08-01T00:00:00.000Z",
        featuredArtifacts: ["64f111111111111111111111"],
        tags: ["invalid"],
      });

    expect(response.status).toBe(400);
    expect(response.body.message).toBe(
      "One or more featured artifact IDs are invalid.",
    );
  });

  it("should reject exhibition creation when end date is before start date", async () => {
    const response = await request(app)
      .post("/api/exhibitions")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Invalid Date Exhibition",
        description:
          "This request should fail because the end date is before the start date.",
        location: "Test Gallery",
        startDate: "2026-08-01T00:00:00.000Z",
        endDate: "2026-07-01T00:00:00.000Z",
        featuredArtifacts: [],
        tags: ["invalid-date"],
      });

    expect(response.status).toBe(400);
    expect(response.body.message).toBe("Validation failed");
  });

  it("should get all active exhibitions with authenticated user", async () => {
    const artifact = await createFeaturedArtifact();

    await request(app)
      .post("/api/exhibitions")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Readable Exhibition",
        description:
          "This exhibition is created so the get all exhibitions endpoint can be tested.",
        location: "Main Gallery Hall",
        startDate: "2026-06-01T00:00:00.000Z",
        endDate: "2026-08-01T00:00:00.000Z",
        featuredArtifacts: [artifact._id.toString()],
        tags: ["readable", "testing"],
      });

    const response = await request(app)
      .get("/api/exhibitions")
      .set("Authorization", `Bearer ${memberToken}`);

    expect(response.status).toBe(200);
    expect(response.body.status).toBe(200);
    expect(response.body.data).toHaveLength(1);
    expect(response.body.data[0].title).toBe("Readable Exhibition");
    expect(response.body.data[0].featuredArtifacts[0].title).toBe(
      "Exhibition Test Artifact",
    );
  });

  it("should search exhibitions by tag", async () => {
    const artifact = await createFeaturedArtifact();

    await request(app)
      .post("/api/exhibitions")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Searchable Exhibition",
        description:
          "This exhibition is created so the exhibition tag search endpoint can be tested.",
        location: "Search Gallery",
        startDate: "2026-06-01T00:00:00.000Z",
        endDate: "2026-08-01T00:00:00.000Z",
        featuredArtifacts: [artifact._id.toString()],
        tags: ["unique-exhibition-tag"],
      });

    const matchResponse = await request(app)
      .get("/api/exhibitions?tag=unique-exhibition-tag")
      .set("Authorization", `Bearer ${memberToken}`);

    expect(matchResponse.status).toBe(200);
    expect(matchResponse.body.data).toHaveLength(1);
    expect(matchResponse.body.data[0].title).toBe("Searchable Exhibition");

    const noMatchResponse = await request(app)
      .get("/api/exhibitions?tag=does-not-exist")
      .set("Authorization", `Bearer ${memberToken}`);

    expect(noMatchResponse.status).toBe(200);
    expect(noMatchResponse.body.data).toHaveLength(0);
  });

  it("should update an exhibition with curator token", async () => {
    const artifact = await createFeaturedArtifact();

    const createResponse = await request(app)
      .post("/api/exhibitions")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Exhibition Before Update",
        description:
          "This exhibition will be updated during the integration test process.",
        location: "Before Update Gallery",
        startDate: "2026-06-01T00:00:00.000Z",
        endDate: "2026-08-01T00:00:00.000Z",
        featuredArtifacts: [artifact._id.toString()],
        tags: ["before-update"],
      });

    const exhibitionId = createResponse.body.data._id;

    const updateResponse = await request(app)
      .put(`/api/exhibitions/${exhibitionId}`)
      .set("Authorization", `Bearer ${curatorToken}`)
      .send({
        status: "open",
        curatorNotes: "Updated curator note for integration testing.",
        tags: ["after-update", "curator-updated"],
      });

    expect(updateResponse.status).toBe(200);
    expect(updateResponse.body.message).toBe(
      "Exhibition updated successfully.",
    );
    expect(updateResponse.body.data.status).toBe("open");
    expect(updateResponse.body.data.tags).toContain("curator-updated");
  });

  it("should soft delete an exhibition with admin token", async () => {
    const artifact = await createFeaturedArtifact();

    const createResponse = await request(app)
      .post("/api/exhibitions")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        title: "Exhibition To Delete",
        description:
          "This exhibition will be soft deleted during integration testing.",
        location: "Delete Test Gallery",
        startDate: "2026-06-01T00:00:00.000Z",
        endDate: "2026-08-01T00:00:00.000Z",
        featuredArtifacts: [artifact._id.toString()],
        tags: ["delete-test"],
      });

    const exhibitionId = createResponse.body.data._id;

    const deleteResponse = await request(app)
      .delete(`/api/exhibitions/${exhibitionId}`)
      .set("Authorization", `Bearer ${adminToken}`);

    expect(deleteResponse.status).toBe(200);
    expect(deleteResponse.body.message).toBe(
      "Exhibition deleted successfully.",
    );
    expect(deleteResponse.body.data.isActive).toBe(false);
    expect(deleteResponse.body.data.status).toBe("cancelled");

    const getResponse = await request(app)
      .get(`/api/exhibitions/${exhibitionId}`)
      .set("Authorization", `Bearer ${memberToken}`);

    expect(getResponse.status).toBe(404);
    expect(getResponse.body.message).toBe("Exhibition not found.");
  });

  it("should reject get all exhibitions without token", async () => {
    const response = await request(app).get("/api/exhibitions");

    expect(response.status).toBe(401);
    expect(response.body.message).toBe("Access denied. No token provided.");
  });
});

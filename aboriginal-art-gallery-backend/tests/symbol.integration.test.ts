import request from "supertest";
import app from "../src/app";
import { connectTestDb, clearTestDb, closeTestDb } from "./testDb";
import { createTestToken } from "./testAuth";
import { Artist } from "../src/models/Artist";
import { Artifact } from "../src/models/Artifact";

describe("Symbols integration tests", () => {
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

  const createRelatedArtifact = async () => {
    const artist = await Artist.create({
      name: "Symbol Test Artist",
      nationOrCommunity: "Anmatyerre",
      languageGroup: "Anmatyerre",
      biography:
        "This artist is created as a dependency for symbol integration tests.",
      region: "Central Desert",
      artStyles: ["Dot Painting"],
    });

    const artifact = await Artifact.create({
      title: "Symbol Test Artifact",
      artist: artist._id,
      description:
        "This artifact is created as a related artifact for symbol integration tests.",
      artType: "Painting",
      materials: ["Acrylic on canvas"],
      yearCreated: 2021,
      culturalRegion: "Central Desert",
      status: "on_display",
      tags: ["test", "symbol"],
    });

    return artifact;
  };

  it("should create a symbol with admin token and valid related artifact", async () => {
    const artifact = await createRelatedArtifact();

    const response = await request(app)
      .post("/api/symbols")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Integration Meeting Place",
        meaning:
          "This symbol represents a place where people gather for ceremony, teaching, and storytelling.",
        culturalNote:
          "This symbol is used in integration testing and includes a respectful note about cultural variation.",
        commonVisualForm: "Concentric circles or circular visual forms.",
        associatedRegions: ["Central Desert", "Northern Territory"],
        relatedArtifacts: [artifact._id.toString()],
        tags: ["community", "ceremony", "testing"],
      });

    expect(response.status).toBe(201);
    expect(response.body.status).toBe(201);
    expect(response.body.message).toBe("Symbol created successfully.");
    expect(response.body.data.name).toBe("Integration Meeting Place");
    expect(response.body.data.relatedArtifacts).toHaveLength(1);
    expect(response.body.data.relatedArtifacts[0].title).toBe(
      "Symbol Test Artifact",
    );
  });

  it("should reject symbol creation without token", async () => {
    const response = await request(app)
      .post("/api/symbols")
      .send({
        name: "No Token Symbol",
        meaning:
          "This request should fail because no authentication token is provided.",
        culturalNote:
          "This cultural note is long enough for validation but should not be saved.",
        commonVisualForm: "Circular form",
        tags: ["test"],
      });

    expect(response.status).toBe(401);
    expect(response.body.message).toBe("Access denied. No token provided.");
  });

  it("should reject symbol creation with member token", async () => {
    const response = await request(app)
      .post("/api/symbols")
      .set("Authorization", `Bearer ${memberToken}`)
      .send({
        name: "Member Symbol",
        meaning:
          "This request should fail because a member role is not allowed to create symbols.",
        culturalNote:
          "This cultural note is long enough for validation but should be rejected by authorization.",
        commonVisualForm: "Circular form",
        tags: ["test"],
      });

    expect(response.status).toBe(403);
    expect(response.body.message).toBe(
      "Forbidden. You do not have permission to access this resource.",
    );
  });

  it("should reject symbol creation with invalid related artifact ID", async () => {
    const response = await request(app)
      .post("/api/symbols")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Invalid Artifact Symbol",
        meaning:
          "This request should fail because the related artifact does not exist in the database.",
        culturalNote:
          "This test proves that symbol relationship validation works before saving data.",
        commonVisualForm: "Test visual form",
        associatedRegions: ["Test Region"],
        relatedArtifacts: ["64f111111111111111111111"],
        tags: ["test"],
      });

    expect(response.status).toBe(400);
    expect(response.body.message).toBe(
      "One or more related artifact IDs are invalid.",
    );
  });

  it("should get all active symbols", async () => {
    const artifact = await createRelatedArtifact();

    await request(app)
      .post("/api/symbols")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Readable Symbol",
        meaning:
          "This symbol is created so the get all symbols endpoint can be tested.",
        culturalNote:
          "This cultural note confirms that the symbol is only used for test data.",
        commonVisualForm: "Circular form",
        associatedRegions: ["Central Desert"],
        relatedArtifacts: [artifact._id.toString()],
        tags: ["readable", "testing"],
      });

    const response = await request(app)
      .get("/api/symbols")
      .set("Authorization", `Bearer ${adminToken}`);

    expect(response.status).toBe(200);
    expect(response.body.status).toBe(200);
    expect(response.body.data).toHaveLength(1);
    expect(response.body.data[0].name).toBe("Readable Symbol");
  });

  it("should search symbols by query", async () => {
    const artifact = await createRelatedArtifact();

    await request(app)
      .post("/api/symbols")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Searchable Symbol",
        meaning:
          "This symbol is created so the search by tag endpoint can be tested.",
        culturalNote:
          "This test checks that the tag query returns only matching symbol records.",
        commonVisualForm: "Circular form",
        associatedRegions: ["Central Desert"],
        relatedArtifacts: [artifact._id.toString()],
        tags: ["unique-test-tag"],
      });

    const matchResponse = await request(app)
      .get("/api/symbols?search=Symbol")
      .set("Authorization", `Bearer ${adminToken}`);

    expect(matchResponse.status).toBe(200);
    expect(matchResponse.body.data).toHaveLength(1);
    expect(matchResponse.body.data[0].name).toBe("Searchable Symbol");

    const noMatchResponse = await request(app)
      .get("/api/symbols?search=does-not-exist")
      .set("Authorization", `Bearer ${adminToken}`);

    expect(noMatchResponse.status).toBe(200);
    expect(noMatchResponse.body.data).toHaveLength(0);
  });

  it("should update a symbol with curator token", async () => {
    const artifact = await createRelatedArtifact();

    const createResponse = await request(app)
      .post("/api/symbols")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Symbol Before Update",
        meaning:
          "This symbol will be updated during the integration test process.",
        culturalNote:
          "This cultural note is used before the update operation is performed.",
        commonVisualForm: "Original circular form",
        relatedArtifacts: [artifact._id.toString()],
        tags: ["before-update"],
      });

    const symbolId = createResponse.body.data._id;

    const updateResponse = await request(app)
      .put(`/api/symbols/${symbolId}`)
      .set("Authorization", `Bearer ${curatorToken}`)
      .send({
        commonVisualForm: "Updated concentric circular form",
        tags: ["after-update", "curator-updated"],
      });

    expect(updateResponse.status).toBe(200);
    expect(updateResponse.body.message).toBe("Symbol updated successfully.");
    expect(updateResponse.body.data.commonVisualForm).toBe(
      "Updated concentric circular form",
    );
    expect(updateResponse.body.data.tags).toContain("curator-updated");
  });

  it("should soft delete a symbol with admin token", async () => {
    const artifact = await createRelatedArtifact();

    const createResponse = await request(app)
      .post("/api/symbols")
      .set("Authorization", `Bearer ${adminToken}`)
      .send({
        name: "Symbol To Delete",
        meaning: "This symbol will be soft deleted during integration testing.",
        culturalNote:
          "This cultural note confirms that the symbol is used only for delete testing.",
        commonVisualForm: "Circular form",
        relatedArtifacts: [artifact._id.toString()],
        tags: ["delete-test"],
      });

    const symbolId = createResponse.body.data._id;

    const deleteResponse = await request(app)
      .delete(`/api/symbols/${symbolId}`)
      .set("Authorization", `Bearer ${adminToken}`);

    expect(deleteResponse.status).toBe(200);
    expect(deleteResponse.body.message).toBe("Symbol deleted successfully.");
    expect(deleteResponse.body.data.isActive).toBe(false);

    const getResponse = await request(app)
      .get(`/api/symbols/${symbolId}`)
      .set("Authorization", `Bearer ${adminToken}`);

    expect(getResponse.status).toBe(404);
    expect(getResponse.body.message).toBe("Symbol not found.");
  });
});

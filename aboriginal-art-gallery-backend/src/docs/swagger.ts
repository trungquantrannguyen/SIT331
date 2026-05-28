import swaggerJsdoc from "swagger-jsdoc";
import { version } from "../../package.json";

export const swaggerSpec = swaggerJsdoc({
  definition: {
    openapi: "3.0.0",
    info: {
      title: "Aboriginal Art Gallery Backend API",
      version,
      description:
        "SIT331 5.2HD backend service for an Aboriginal art gallery using Express.js, TypeScript, MongoDB, Mongoose, JWT authentication, role-based authorization, and bounded context design.",
    },
    servers: [
      {
        url: "http://localhost:5050",
        description: "Local development server",
      },
    ],
    tags: [
      {
        name: "Health",
        description: "API health check endpoints",
      },
      {
        name: "Authentication",
        description: "User registration, login, profile, and role security",
      },
      {
        name: "Artists",
        description: "Artist bounded context",
      },
      {
        name: "Artifacts",
        description: "Artwork/artifact bounded context",
      },
      {
        name: "Symbols",
        description: "Aboriginal symbols and iconography bounded context",
      },
      {
        name: "Exhibitions",
        description: "Exhibition bounded context",
      },
    ],
    components: {
      securitySchemes: {
        bearerAuth: {
          type: "http",
          scheme: "bearer",
          bearerFormat: "JWT",
        },
      },
      schemas: {
        ApiResponse: {
          type: "object",
          properties: {
            status: {
              type: "number",
              example: 200,
            },
            message: {
              type: "string",
              example: "Request completed successfully.",
            },
            data: {
              type: "object",
              nullable: true,
            },
          },
        },
        RegisterRequest: {
          type: "object",
          required: ["name", "email", "password"],
          properties: {
            name: {
              type: "string",
              example: "Gallery Admin",
            },
            email: {
              type: "string",
              example: "admin@gallery.com",
            },
            password: {
              type: "string",
              example: "Admin12345",
            },
            role: {
              type: "string",
              enum: ["admin", "curator", "member"],
              example: "admin",
            },
          },
        },
        LoginRequest: {
          type: "object",
          required: ["email", "password"],
          properties: {
            email: {
              type: "string",
              example: "admin@gallery.com",
            },
            password: {
              type: "string",
              example: "Admin12345",
            },
          },
        },
        ArtistRequest: {
          type: "object",
          required: ["name", "nationOrCommunity", "biography"],
          properties: {
            name: {
              type: "string",
              example: "Emily Kame Kngwarreye",
            },
            nationOrCommunity: {
              type: "string",
              example: "Anmatyerre",
            },
            languageGroup: {
              type: "string",
              example: "Anmatyerre",
            },
            biography: {
              type: "string",
              example:
                "Emily Kame Kngwarreye was a highly respected Aboriginal artist known for powerful contemporary works connected to Country, ceremony, and cultural knowledge.",
            },
            birthYear: {
              type: "number",
              example: 1910,
            },
            region: {
              type: "string",
              example: "Utopia, Northern Territory",
            },
            artStyles: {
              type: "array",
              items: {
                type: "string",
              },
              example: ["Contemporary Aboriginal Art", "Dot Painting"],
            },
          },
        },
        ArtifactRequest: {
          type: "object",
          required: ["title", "artist", "description", "artType"],
          properties: {
            title: {
              type: "string",
              example: "Bush Medicine Leaves",
            },
            artist: {
              type: "string",
              example: "665f1e111111111111111111",
            },
            description: {
              type: "string",
              example:
                "This artwork represents the movement and importance of bush medicine leaves within Aboriginal cultural storytelling and connection to Country.",
            },
            artType: {
              type: "string",
              example: "Painting",
            },
            materials: {
              type: "array",
              items: {
                type: "string",
              },
              example: ["Acrylic on canvas"],
            },
            yearCreated: {
              type: "number",
              example: 2021,
            },
            culturalRegion: {
              type: "string",
              example: "Utopia, Northern Territory",
            },
            priceAud: {
              type: "number",
              example: 2500,
            },
            status: {
              type: "string",
              enum: ["available", "on_display", "reserved", "archived"],
              example: "on_display",
            },
            tags: {
              type: "array",
              items: {
                type: "string",
              },
              example: ["bush medicine", "leaves", "utopia"],
            },
          },
        },
        SymbolRequest: {
          type: "object",
          required: ["name", "meaning", "culturalNote", "commonVisualForm"],
          properties: {
            name: {
              type: "string",
              example: "Meeting Place",
            },
            meaning: {
              type: "string",
              example:
                "A meeting place symbol often represents a location where people gather for ceremony, community, teaching, or storytelling.",
            },
            culturalNote: {
              type: "string",
              example:
                "This symbol should be documented respectfully because specific meanings may vary between communities, artists, regions, and cultural permissions.",
            },
            commonVisualForm: {
              type: "string",
              example: "Concentric circles or circular forms.",
            },
            associatedRegions: {
              type: "array",
              items: {
                type: "string",
              },
              example: ["Central Desert", "Northern Territory"],
            },
            relatedArtifacts: {
              type: "array",
              items: {
                type: "string",
              },
              example: ["665f1e222222222222222222"],
            },
            tags: {
              type: "array",
              items: {
                type: "string",
              },
              example: ["community", "ceremony", "storytelling"],
            },
          },
        },
        ExhibitionRequest: {
          type: "object",
          required: [
            "title",
            "description",
            "location",
            "startDate",
            "endDate",
          ],
          properties: {
            title: {
              type: "string",
              example: "Stories of Country",
            },
            description: {
              type: "string",
              example:
                "This exhibition presents Aboriginal artworks that explore cultural connection to Country, movement, ceremony, and visual storytelling across different regions.",
            },
            location: {
              type: "string",
              example: "Main Gallery Hall",
            },
            startDate: {
              type: "string",
              format: "date-time",
              example: "2026-06-01T00:00:00.000Z",
            },
            endDate: {
              type: "string",
              format: "date-time",
              example: "2026-08-31T00:00:00.000Z",
            },
            status: {
              type: "string",
              enum: ["planned", "open", "closed", "cancelled"],
              example: "planned",
            },
            featuredArtifacts: {
              type: "array",
              items: {
                type: "string",
              },
              example: ["665f1e333333333333333333"],
            },
            curatorNotes: {
              type: "string",
              example:
                "The exhibition should be introduced with cultural sensitivity.",
            },
            tags: {
              type: "array",
              items: {
                type: "string",
              },
              example: ["country", "storytelling", "ceremony"],
            },
          },
        },
      },
    },
  },
  apis: ["./src/routes/*.ts", "./src/app.ts"],
});

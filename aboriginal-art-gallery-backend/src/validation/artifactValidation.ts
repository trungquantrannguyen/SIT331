import { z } from "zod";

const objectIdRegex = /^[0-9a-fA-F]{24}$/;

export const createArtifactSchema = z.object({
  body: z.object({
    title: z
      .string()
      .min(2, "Title must be at least 2 characters")
      .max(150, "Title cannot exceed 150 characters"),

    artist: z.string().regex(objectIdRegex, "Invalid artist ID format"),

    description: z
      .string()
      .min(20, "Description must be at least 20 characters")
      .max(2500, "Description cannot exceed 2500 characters"),

    artType: z
      .string()
      .min(2, "Art type must be at least 2 characters")
      .max(100, "Art type cannot exceed 100 characters"),

    materials: z.array(z.string().min(1)).optional(),

    yearCreated: z
      .number()
      .int()
      .min(1800)
      .max(new Date().getFullYear())
      .optional(),

    culturalRegion: z.string().max(120).optional(),

    dimensions: z
      .object({
        heightCm: z.number().min(0).optional(),
        widthCm: z.number().min(0).optional(),
        depthCm: z.number().min(0).optional(),
      })
      .optional(),

    priceAud: z.number().min(0).optional(),

    status: z
      .enum(["available", "on_display", "reserved", "archived"])
      .optional(),

    tags: z.array(z.string().min(1)).optional(),

    imageUrl: z.string().url("Image URL must be a valid URL").optional(),
  }),
});

export const updateArtifactSchema = z.object({
  body: z.object({
    title: z.string().min(2).max(150).optional(),
    artist: z
      .string()
      .regex(objectIdRegex, "Invalid artist ID format")
      .optional(),
    description: z.string().min(20).max(2500).optional(),
    artType: z.string().min(2).max(100).optional(),
    materials: z.array(z.string().min(1)).optional(),
    yearCreated: z
      .number()
      .int()
      .min(1800)
      .max(new Date().getFullYear())
      .optional(),
    culturalRegion: z.string().max(120).optional(),
    dimensions: z
      .object({
        heightCm: z.number().min(0).optional(),
        widthCm: z.number().min(0).optional(),
        depthCm: z.number().min(0).optional(),
      })
      .optional(),
    priceAud: z.number().min(0).optional(),
    status: z
      .enum(["available", "on_display", "reserved", "archived"])
      .optional(),
    tags: z.array(z.string().min(1)).optional(),
    imageUrl: z.string().url("Image URL must be a valid URL").optional(),
    isActive: z.boolean().optional(),
  }),
});

export const artifactIdParamSchema = z.object({
  params: z.object({
    id: z.string().regex(objectIdRegex, "Invalid artifact ID format"),
  }),
});

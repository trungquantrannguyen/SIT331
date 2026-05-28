import { z } from "zod";

const objectIdRegex = /^[0-9a-fA-F]{24}$/;

export const createSymbolSchema = z.object({
  body: z.object({
    name: z
      .string()
      .min(2, "Symbol name must be at least 2 characters")
      .max(100, "Symbol name cannot exceed 100 characters"),

    meaning: z
      .string()
      .min(10, "Meaning must be at least 10 characters")
      .max(1000, "Meaning cannot exceed 1000 characters"),

    culturalNote: z
      .string()
      .min(20, "Cultural note must be at least 20 characters")
      .max(1500, "Cultural note cannot exceed 1500 characters"),

    commonVisualForm: z
      .string()
      .min(2, "Common visual form is required")
      .max(500, "Common visual form cannot exceed 500 characters"),

    associatedRegions: z.array(z.string().min(1)).optional(),

    relatedArtifacts: z
      .array(z.string().regex(objectIdRegex, "Invalid artifact ID format"))
      .optional(),

    tags: z.array(z.string().min(1)).optional(),
  }),
});

export const updateSymbolSchema = z.object({
  body: z.object({
    name: z.string().min(2).max(100).optional(),
    meaning: z.string().min(10).max(1000).optional(),
    culturalNote: z.string().min(20).max(1500).optional(),
    commonVisualForm: z.string().min(2).max(500).optional(),
    associatedRegions: z.array(z.string().min(1)).optional(),
    relatedArtifacts: z
      .array(z.string().regex(objectIdRegex, "Invalid artifact ID format"))
      .optional(),
    tags: z.array(z.string().min(1)).optional(),
    isActive: z.boolean().optional(),
  }),
});

export const symbolIdParamSchema = z.object({
  params: z.object({
    id: z.string().regex(objectIdRegex, "Invalid symbol ID format"),
  }),
});

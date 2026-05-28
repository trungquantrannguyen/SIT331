import { z } from "zod";

const objectIdRegex = /^[0-9a-fA-F]{24}$/;

export const createArtistSchema = z.object({
  body: z.object({
    name: z
      .string()
      .min(2, "Artist name must be at least 2 characters")
      .max(120, "Artist name cannot exceed 120 characters"),

    nationOrCommunity: z
      .string()
      .min(2, "Nation or community must be at least 2 characters")
      .max(120, "Nation or community cannot exceed 120 characters"),

    languageGroup: z
      .string()
      .max(120, "Language group cannot exceed 120 characters")
      .optional(),

    biography: z
      .string()
      .min(20, "Biography must be at least 20 characters")
      .max(2000, "Biography cannot exceed 2000 characters"),

    birthYear: z
      .number()
      .int()
      .min(1800, "Birth year cannot be before 1800")
      .max(new Date().getFullYear(), "Birth year cannot be in the future")
      .optional(),

    region: z
      .string()
      .max(120, "Region cannot exceed 120 characters")
      .optional(),

    artStyles: z.array(z.string().min(1)).optional(),
  }),
});

export const updateArtistSchema = z.object({
  body: z.object({
    name: z.string().min(2).max(120).optional(),
    nationOrCommunity: z.string().min(2).max(120).optional(),
    languageGroup: z.string().max(120).optional(),
    biography: z.string().min(20).max(2000).optional(),
    birthYear: z
      .number()
      .int()
      .min(1800)
      .max(new Date().getFullYear())
      .optional(),
    region: z.string().max(120).optional(),
    artStyles: z.array(z.string().min(1)).optional(),
    isActive: z.boolean().optional(),
  }),
});

export const artistIdParamSchema = z.object({
  params: z.object({
    id: z.string().regex(objectIdRegex, "Invalid artist ID format"),
  }),
});

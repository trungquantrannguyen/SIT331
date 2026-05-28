import { z } from "zod";

const objectIdRegex = /^[0-9a-fA-F]{24}$/;

const dateStringSchema = z
  .string()
  .refine((value) => !Number.isNaN(Date.parse(value)), {
    message: "Date must be a valid ISO date string",
  });

export const createExhibitionSchema = z.object({
  body: z
    .object({
      title: z
        .string()
        .min(2, "Title must be at least 2 characters")
        .max(150, "Title cannot exceed 150 characters"),

      description: z
        .string()
        .min(20, "Description must be at least 20 characters")
        .max(2500, "Description cannot exceed 2500 characters"),

      location: z
        .string()
        .min(2, "Location must be at least 2 characters")
        .max(150, "Location cannot exceed 150 characters"),

      startDate: dateStringSchema,

      endDate: dateStringSchema,

      status: z.enum(["planned", "open", "closed", "cancelled"]).optional(),

      featuredArtifacts: z
        .array(z.string().regex(objectIdRegex, "Invalid artifact ID format"))
        .optional(),

      curatorNotes: z
        .string()
        .max(1500, "Curator notes cannot exceed 1500 characters")
        .optional(),

      tags: z.array(z.string().min(1)).optional(),
    })
    .refine((data) => new Date(data.endDate) >= new Date(data.startDate), {
      message: "End date must be after or equal to start date",
      path: ["endDate"],
    }),
});

export const updateExhibitionSchema = z.object({
  body: z
    .object({
      title: z.string().min(2).max(150).optional(),
      description: z.string().min(20).max(2500).optional(),
      location: z.string().min(2).max(150).optional(),
      startDate: dateStringSchema.optional(),
      endDate: dateStringSchema.optional(),
      status: z.enum(["planned", "open", "closed", "cancelled"]).optional(),
      featuredArtifacts: z
        .array(z.string().regex(objectIdRegex, "Invalid artifact ID format"))
        .optional(),
      curatorNotes: z.string().max(1500).optional(),
      tags: z.array(z.string().min(1)).optional(),
      isActive: z.boolean().optional(),
    })
    .refine(
      (data) => {
        if (!data.startDate || !data.endDate) {
          return true;
        }

        return new Date(data.endDate) >= new Date(data.startDate);
      },
      {
        message: "End date must be after or equal to start date",
        path: ["endDate"],
      },
    ),
});

export const exhibitionIdParamSchema = z.object({
  params: z.object({
    id: z.string().regex(objectIdRegex, "Invalid exhibition ID format"),
  }),
});

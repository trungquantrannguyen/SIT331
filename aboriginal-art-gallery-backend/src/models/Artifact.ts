import mongoose, { Document, Schema, Types } from "mongoose";

export type ArtifactStatus =
  | "available"
  | "on_display"
  | "reserved"
  | "archived";

export interface IArtifact extends Document {
  title: string;
  artist: Types.ObjectId;
  description: string;
  artType: string;
  materials: string[];
  yearCreated?: number;
  culturalRegion?: string;
  dimensions?: {
    heightCm?: number;
    widthCm?: number;
    depthCm?: number;
  };
  priceAud?: number;
  status: ArtifactStatus;
  tags: string[];
  imageUrl?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

const artifactSchema = new Schema<IArtifact>(
  {
    title: {
      type: String,
      required: [true, "Artifact title is required"],
      trim: true,
      minlength: [2, "Title must be at least 2 characters"],
      maxlength: [150, "Title cannot exceed 150 characters"],
    },
    artist: {
      type: Schema.Types.ObjectId,
      ref: "Artist",
      required: [true, "Artist reference is required"],
    },
    description: {
      type: String,
      required: [true, "Artifact description is required"],
      trim: true,
      minlength: [20, "Description must be at least 20 characters"],
      maxlength: [2500, "Description cannot exceed 2500 characters"],
    },
    artType: {
      type: String,
      required: [true, "Art type is required"],
      trim: true,
      maxlength: [100, "Art type cannot exceed 100 characters"],
    },
    materials: {
      type: [String],
      default: [],
    },
    yearCreated: {
      type: Number,
      min: [1800, "Year created cannot be before 1800"],
      max: [new Date().getFullYear(), "Year created cannot be in the future"],
    },
    culturalRegion: {
      type: String,
      trim: true,
      maxlength: [120, "Cultural region cannot exceed 120 characters"],
    },
    dimensions: {
      heightCm: {
        type: Number,
        min: [0, "Height cannot be negative"],
      },
      widthCm: {
        type: Number,
        min: [0, "Width cannot be negative"],
      },
      depthCm: {
        type: Number,
        min: [0, "Depth cannot be negative"],
      },
    },
    priceAud: {
      type: Number,
      min: [0, "Price cannot be negative"],
    },
    status: {
      type: String,
      enum: ["available", "on_display", "reserved", "archived"],
      default: "available",
    },
    tags: {
      type: [String],
      default: [],
    },
    imageUrl: {
      type: String,
      trim: true,
    },
    isActive: {
      type: Boolean,
      default: true,
    },
  },
  {
    timestamps: true,
  },
);

artifactSchema.index({ title: 1 });
artifactSchema.index({ artist: 1 });
artifactSchema.index({ artType: 1 });
artifactSchema.index({ culturalRegion: 1 });
artifactSchema.index({ status: 1 });
artifactSchema.index({ tags: 1 });

export const Artifact = mongoose.model<IArtifact>("Artifact", artifactSchema);

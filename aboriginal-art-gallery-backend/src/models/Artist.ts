import mongoose, { Document, Schema } from "mongoose";

export interface IArtist extends Document {
  name: string;
  nationOrCommunity: string;
  languageGroup?: string;
  biography: string;
  birthYear?: number;
  region?: string;
  artStyles: string[];
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

const artistSchema = new Schema<IArtist>(
  {
    name: {
      type: String,
      required: [true, "Artist name is required"],
      trim: true,
      minlength: [2, "Artist name must be at least 2 characters"],
      maxlength: [120, "Artist name cannot exceed 120 characters"],
    },
    nationOrCommunity: {
      type: String,
      required: [true, "Nation or community is required"],
      trim: true,
      maxlength: [120, "Nation or community cannot exceed 120 characters"],
    },
    languageGroup: {
      type: String,
      trim: true,
      maxlength: [120, "Language group cannot exceed 120 characters"],
    },
    biography: {
      type: String,
      required: [true, "Biography is required"],
      trim: true,
      minlength: [20, "Biography must be at least 20 characters"],
      maxlength: [2000, "Biography cannot exceed 2000 characters"],
    },
    birthYear: {
      type: Number,
      min: [1800, "Birth year cannot be before 1800"],
      max: [new Date().getFullYear(), "Birth year cannot be in the future"],
    },
    region: {
      type: String,
      trim: true,
      maxlength: [120, "Region cannot exceed 120 characters"],
    },
    artStyles: {
      type: [String],
      default: [],
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

artistSchema.index({ name: 1 });
artistSchema.index({ nationOrCommunity: 1 });
artistSchema.index({ region: 1 });

export const Artist = mongoose.model<IArtist>("Artist", artistSchema);

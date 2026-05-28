import mongoose, { Document, Schema, Types } from "mongoose";

export interface ISymbol extends Document {
  name: string;
  meaning: string;
  culturalNote: string;
  commonVisualForm: string;
  associatedRegions: string[];
  relatedArtifacts: Types.ObjectId[];
  tags: string[];
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

const symbolSchema = new Schema<ISymbol>(
  {
    name: {
      type: String,
      required: [true, "Symbol name is required"],
      trim: true,
      minlength: [2, "Symbol name must be at least 2 characters"],
      maxlength: [100, "Symbol name cannot exceed 100 characters"],
    },
    meaning: {
      type: String,
      required: [true, "Symbol meaning is required"],
      trim: true,
      minlength: [10, "Meaning must be at least 10 characters"],
      maxlength: [1000, "Meaning cannot exceed 1000 characters"],
    },
    culturalNote: {
      type: String,
      required: [true, "Cultural note is required"],
      trim: true,
      minlength: [20, "Cultural note must be at least 20 characters"],
      maxlength: [1500, "Cultural note cannot exceed 1500 characters"],
    },
    commonVisualForm: {
      type: String,
      required: [true, "Common visual form is required"],
      trim: true,
      maxlength: [500, "Common visual form cannot exceed 500 characters"],
    },
    associatedRegions: {
      type: [String],
      default: [],
    },
    relatedArtifacts: [
      {
        type: Schema.Types.ObjectId,
        ref: "Artifact",
      },
    ],
    tags: {
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

symbolSchema.index({ name: 1 });
symbolSchema.index({ associatedRegions: 1 });
symbolSchema.index({ tags: 1 });
symbolSchema.index({ relatedArtifacts: 1 });

export const SymbolModel = mongoose.model<ISymbol>("Symbol", symbolSchema);

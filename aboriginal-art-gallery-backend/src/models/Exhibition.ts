import mongoose, { Document, Schema, Types } from "mongoose";

export type ExhibitionStatus = "planned" | "open" | "closed" | "cancelled";

export interface IExhibition extends Document {
  title: string;
  description: string;
  location: string;
  startDate: Date;
  endDate: Date;
  status: ExhibitionStatus;
  featuredArtifacts: Types.ObjectId[];
  curatorNotes?: string;
  tags: string[];
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

const exhibitionSchema = new Schema<IExhibition>(
  {
    title: {
      type: String,
      required: [true, "Exhibition title is required"],
      trim: true,
      minlength: [2, "Title must be at least 2 characters"],
      maxlength: [150, "Title cannot exceed 150 characters"],
    },
    description: {
      type: String,
      required: [true, "Exhibition description is required"],
      trim: true,
      minlength: [20, "Description must be at least 20 characters"],
      maxlength: [2500, "Description cannot exceed 2500 characters"],
    },
    location: {
      type: String,
      required: [true, "Exhibition location is required"],
      trim: true,
      maxlength: [150, "Location cannot exceed 150 characters"],
    },
    startDate: {
      type: Date,
      required: [true, "Start date is required"],
    },
    endDate: {
      type: Date,
      required: [true, "End date is required"],
    },
    status: {
      type: String,
      enum: ["planned", "open", "closed", "cancelled"],
      default: "planned",
    },
    featuredArtifacts: [
      {
        type: Schema.Types.ObjectId,
        ref: "Artifact",
      },
    ],
    curatorNotes: {
      type: String,
      trim: true,
      maxlength: [1500, "Curator notes cannot exceed 1500 characters"],
    },
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

exhibitionSchema.index({ title: 1 });
exhibitionSchema.index({ status: 1 });
exhibitionSchema.index({ startDate: 1 });
exhibitionSchema.index({ endDate: 1 });
exhibitionSchema.index({ featuredArtifacts: 1 });
exhibitionSchema.index({ tags: 1 });

export const Exhibition = mongoose.model<IExhibition>(
  "Exhibition",
  exhibitionSchema,
);

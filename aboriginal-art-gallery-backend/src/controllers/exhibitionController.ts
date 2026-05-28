import { Request, Response } from "express";
import { Exhibition } from "../models/Exhibition";
import { Artifact } from "../models/Artifact";

const validateFeaturedArtifacts = async (
  artifactIds: string[] | undefined,
): Promise<boolean> => {
  if (!artifactIds || artifactIds.length === 0) {
    return true;
  }

  const count = await Artifact.countDocuments({
    _id: { $in: artifactIds },
    isActive: true,
  });

  return count === artifactIds.length;
};

export const getExhibitions = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const search =
      typeof req.query.search === "string" ? req.query.search.trim() : "";
    const status =
      typeof req.query.status === "string" ? req.query.status.trim() : "";
    const location =
      typeof req.query.location === "string" ? req.query.location.trim() : "";
    const tag = typeof req.query.tag === "string" ? req.query.tag.trim() : "";

    const filter: Record<string, unknown> = {
      isActive: true,
    };

    const searchValue = search;

    if (searchValue.length > 0) {
      filter.title = {
        $regex: searchValue,
        $options: "i",
      };
    }

    if (status.length > 0) {
      filter.status = status;
    }

    if (location.length > 0) {
      filter.location = {
        $regex: location,
        $options: "i",
      };
    }

    if (tag.length > 0) {
      filter.tags = {
        $elemMatch: {
          $regex: tag,
          $options: "i",
        },
      };
    }

    console.log("Exhibition query filter:", filter);

    const exhibitions = await Exhibition.find(filter)
      .populate("featuredArtifacts", "title artType culturalRegion status")
      .sort({ startDate: 1 });

    res.status(200).json({
      status: 200,
      message: "Exhibitions retrieved successfully.",
      data: exhibitions,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to retrieve exhibitions.",
      data: null,
    });
  }
};

export const getExhibitionById = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const exhibition = await Exhibition.findOne({
      _id: req.params.id,
      isActive: true,
    }).populate("featuredArtifacts", "title artType culturalRegion status");

    if (!exhibition) {
      res.status(404).json({
        status: 404,
        message: "Exhibition not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Exhibition retrieved successfully.",
      data: exhibition,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to retrieve exhibition.",
      data: null,
    });
  }
};

export const createExhibition = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artifactsAreValid = await validateFeaturedArtifacts(
      req.body.featuredArtifacts,
    );

    if (!artifactsAreValid) {
      res.status(400).json({
        status: 400,
        message: "One or more featured artifact IDs are invalid.",
        data: null,
      });
      return;
    }

    const exhibition = await Exhibition.create(req.body);

    const populatedExhibition = await Exhibition.findById(
      exhibition._id,
    ).populate("featuredArtifacts", "title artType culturalRegion status");

    res.status(201).json({
      status: 201,
      message: "Exhibition created successfully.",
      data: populatedExhibition,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to create exhibition.",
      data: null,
    });
  }
};

export const updateExhibition = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artifactsAreValid = await validateFeaturedArtifacts(
      req.body.featuredArtifacts,
    );

    if (!artifactsAreValid) {
      res.status(400).json({
        status: 400,
        message: "One or more featured artifact IDs are invalid.",
        data: null,
      });
      return;
    }

    const exhibition = await Exhibition.findByIdAndUpdate(
      req.params.id,
      req.body,
      {
        new: true,
        runValidators: true,
      },
    ).populate("featuredArtifacts", "title artType culturalRegion status");

    if (!exhibition) {
      res.status(404).json({
        status: 404,
        message: "Exhibition not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Exhibition updated successfully.",
      data: exhibition,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to update exhibition.",
      data: null,
    });
  }
};

export const deleteExhibition = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const exhibition = await Exhibition.findByIdAndUpdate(
      req.params.id,
      {
        isActive: false,
        status: "cancelled",
      },
      {
        new: true,
        runValidators: true,
      },
    ).populate("featuredArtifacts", "title artType culturalRegion status");

    if (!exhibition) {
      res.status(404).json({
        status: 404,
        message: "Exhibition not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Exhibition deleted successfully.",
      data: exhibition,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to delete exhibition.",
      data: null,
    });
  }
};

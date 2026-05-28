import { Request, Response } from "express";
import { Artifact } from "../models/Artifact";
import { Artist } from "../models/Artist";

export const getArtifacts = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const { search, artType, culturalRegion, status, tag } = req.query;

    const filter: Record<string, unknown> = {
      isActive: true,
    };

    if (search) {
      filter.title = {
        $regex: String(search),
        $options: "i",
      };
    }

    if (artType) {
      filter.artType = {
        $regex: String(artType),
        $options: "i",
      };
    }

    if (culturalRegion) {
      filter.culturalRegion = {
        $regex: String(culturalRegion),
        $options: "i",
      };
    }

    if (status) {
      filter.status = status;
    }

    if (tag) {
      filter.tags = {
        $regex: String(tag),
        $options: "i",
      };
    }

    const artifacts = await Artifact.find(filter)
      .populate("artist", "name nationOrCommunity region")
      .sort({ createdAt: -1 });

    res.status(200).json({
      status: 200,
      message: "Artifacts retrieved successfully.",
      data: artifacts,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to retrieve artifacts.",
      data: null,
    });
  }
};

export const getArtifactById = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artifact = await Artifact.findOne({
      _id: req.params.id,
      isActive: true,
    }).populate(
      "artist",
      "name nationOrCommunity languageGroup region artStyles",
    );

    if (!artifact) {
      res.status(404).json({
        status: 404,
        message: "Artifact not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Artifact retrieved successfully.",
      data: artifact,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to retrieve artifact.",
      data: null,
    });
  }
};

export const createArtifact = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artistExists = await Artist.findOne({
      _id: req.body.artist,
      isActive: true,
    });

    if (!artistExists) {
      res.status(400).json({
        status: 400,
        message: "Cannot create artifact because the artist does not exist.",
        data: null,
      });
      return;
    }

    const artifact = await Artifact.create(req.body);

    const populatedArtifact = await Artifact.findById(artifact._id).populate(
      "artist",
      "name nationOrCommunity region",
    );

    res.status(201).json({
      status: 201,
      message: "Artifact created successfully.",
      data: populatedArtifact,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to create artifact.",
      data: null,
    });
  }
};

export const updateArtifact = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    if (req.body.artist) {
      const artistExists = await Artist.findOne({
        _id: req.body.artist,
        isActive: true,
      });

      if (!artistExists) {
        res.status(400).json({
          status: 400,
          message: "Cannot update artifact because the artist does not exist.",
          data: null,
        });
        return;
      }
    }

    const artifact = await Artifact.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    }).populate("artist", "name nationOrCommunity region");

    if (!artifact) {
      res.status(404).json({
        status: 404,
        message: "Artifact not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Artifact updated successfully.",
      data: artifact,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to update artifact.",
      data: null,
    });
  }
};

export const deleteArtifact = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artifact = await Artifact.findByIdAndUpdate(
      req.params.id,
      { isActive: false, status: "archived" },
      {
        new: true,
        runValidators: true,
      },
    ).populate("artist", "name nationOrCommunity region");

    if (!artifact) {
      res.status(404).json({
        status: 404,
        message: "Artifact not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Artifact deleted successfully.",
      data: artifact,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to delete artifact.",
      data: null,
    });
  }
};

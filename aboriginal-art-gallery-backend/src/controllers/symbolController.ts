import { Request, Response } from "express";
import { SymbolModel } from "../models/Symbol";
import { Artifact } from "../models/Artifact";

const validateRelatedArtifacts = async (
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

export const getSymbols = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const { search, region, tag } = req.query;

    const filter: Record<string, unknown> = {
      isActive: true,
    };

    if (search) {
      filter.name = {
        $regex: String(search),
        $options: "i",
      };
    }

    if (region) {
      filter.associatedRegions = {
        $regex: String(region),
        $options: "i",
      };
    }

    if (tag) {
      filter.tags = {
        $regex: String(tag),
        $options: "i",
      };
    }

    const symbols = await SymbolModel.find(filter)
      .populate("relatedArtifacts", "title artType culturalRegion status")
      .sort({ name: 1 });

    res.status(200).json({
      status: 200,
      message: "Symbols retrieved successfully.",
      data: symbols,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to retrieve symbols.",
      data: null,
    });
  }
};

export const getSymbolById = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const symbol = await SymbolModel.findOne({
      _id: req.params.id,
      isActive: true,
    }).populate("relatedArtifacts", "title artType culturalRegion status");

    if (!symbol) {
      res.status(404).json({
        status: 404,
        message: "Symbol not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Symbol retrieved successfully.",
      data: symbol,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to retrieve symbol.",
      data: null,
    });
  }
};

export const createSymbol = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artifactsAreValid = await validateRelatedArtifacts(
      req.body.relatedArtifacts,
    );

    if (!artifactsAreValid) {
      res.status(400).json({
        status: 400,
        message: "One or more related artifact IDs are invalid.",
        data: null,
      });
      return;
    }

    const symbol = await SymbolModel.create(req.body);

    const populatedSymbol = await SymbolModel.findById(symbol._id).populate(
      "relatedArtifacts",
      "title artType culturalRegion status",
    );

    res.status(201).json({
      status: 201,
      message: "Symbol created successfully.",
      data: populatedSymbol,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to create symbol.",
      data: null,
    });
  }
};

export const updateSymbol = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artifactsAreValid = await validateRelatedArtifacts(
      req.body.relatedArtifacts,
    );

    if (!artifactsAreValid) {
      res.status(400).json({
        status: 400,
        message: "One or more related artifact IDs are invalid.",
        data: null,
      });
      return;
    }

    const symbol = await SymbolModel.findByIdAndUpdate(
      req.params.id,
      req.body,
      {
        new: true,
        runValidators: true,
      },
    ).populate("relatedArtifacts", "title artType culturalRegion status");

    if (!symbol) {
      res.status(404).json({
        status: 404,
        message: "Symbol not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Symbol updated successfully.",
      data: symbol,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to update symbol.",
      data: null,
    });
  }
};

export const deleteSymbol = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const symbol = await SymbolModel.findByIdAndUpdate(
      req.params.id,
      { isActive: false },
      {
        new: true,
        runValidators: true,
      },
    );

    if (!symbol) {
      res.status(404).json({
        status: 404,
        message: "Symbol not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Symbol deleted successfully.",
      data: symbol,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to delete symbol.",
      data: null,
    });
  }
};

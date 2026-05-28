import { Request, Response } from "express";
import { Artist } from "../models/Artist";

export const getArtists = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const { search, region, nationOrCommunity } = req.query;

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
      filter.region = {
        $regex: String(region),
        $options: "i",
      };
    }

    if (nationOrCommunity) {
      filter.nationOrCommunity = {
        $regex: String(nationOrCommunity),
        $options: "i",
      };
    }

    const artists = await Artist.find(filter).sort({ name: 1 });

    res.status(200).json({
      status: 200,
      message: "Artists retrieved successfully.",
      data: artists,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to retrieve artists.",
      data: null,
    });
  }
};

export const getArtistById = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artist = await Artist.findOne({
      _id: req.params.id,
      isActive: true,
    });

    if (!artist) {
      res.status(404).json({
        status: 404,
        message: "Artist not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Artist retrieved successfully.",
      data: artist,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to retrieve artist.",
      data: null,
    });
  }
};

export const createArtist = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artist = await Artist.create(req.body);

    res.status(201).json({
      status: 201,
      message: "Artist created successfully.",
      data: artist,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to create artist.",
      data: null,
    });
  }
};

export const updateArtist = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artist = await Artist.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });

    if (!artist) {
      res.status(404).json({
        status: 404,
        message: "Artist not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Artist updated successfully.",
      data: artist,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to update artist.",
      data: null,
    });
  }
};

export const deleteArtist = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const artist = await Artist.findByIdAndUpdate(
      req.params.id,
      { isActive: false },
      {
        new: true,
        runValidators: true,
      },
    );

    if (!artist) {
      res.status(404).json({
        status: 404,
        message: "Artist not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Artist deleted successfully.",
      data: artist,
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to delete artist.",
      data: null,
    });
  }
};

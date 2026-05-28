import { Request, Response } from "express";
import bcrypt from "bcrypt";
import jwt, { SignOptions } from "jsonwebtoken";
import { User, UserRole } from "../models/User";

const generateToken = (user: {
  id: string;
  email: string;
  role: UserRole;
}): string => {
  const jwtSecret = process.env.JWT_SECRET;

  if (!jwtSecret) {
    throw new Error("JWT_SECRET is missing from environment variables.");
  }

  const expiresIn: any = process.env.JWT_EXPIRES_IN || "1h";

  const options: SignOptions = {
    expiresIn: expiresIn,
  };

  return jwt.sign(
    {
      id: user.id,
      email: user.email,
      role: user.role,
    },
    jwtSecret,
    options,
  );
};

export const registerUser = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const { name, email, password, role } = req.body;

    const existingUser = await User.findOne({ email });

    if (existingUser) {
      res.status(409).json({
        status: 409,
        message: "Email is already registered.",
        data: null,
      });
      return;
    }

    const saltRounds = Number(process.env.BCRYPT_SALT_ROUNDS) || 12;
    const hashedPassword = await bcrypt.hash(password, saltRounds);

    const user = await User.create({
      name,
      email,
      password: hashedPassword,
      role: role || "member",
    });

    res.status(201).json({
      status: 201,
      message: "User registered successfully.",
      data: {
        id: user._id,
        name: user.name,
        email: user.email,
        role: user.role,
        isActive: user.isActive,
        createdAt: user.createdAt,
      },
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to register user.",
      data: null,
    });
  }
};

export const loginUser = async (req: Request, res: Response): Promise<void> => {
  try {
    const { email, password } = req.body;

    const user = await User.findOne({ email }).select("+password");

    if (!user || !user.isActive) {
      res.status(401).json({
        status: 401,
        message: "Invalid email or password.",
        data: null,
      });
      return;
    }

    const isPasswordValid = await bcrypt.compare(password, user.password);

    if (!isPasswordValid) {
      res.status(401).json({
        status: 401,
        message: "Invalid email or password.",
        data: null,
      });
      return;
    }

    const token = generateToken({
      id: user._id.toString(),
      email: user.email,
      role: user.role,
    });

    res.status(200).json({
      status: 200,
      message: "Login successful.",
      data: {
        token,
        user: {
          id: user._id,
          name: user.name,
          email: user.email,
          role: user.role,
        },
      },
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to login.",
      data: null,
    });
  }
};

export const getProfile = async (
  req: Request,
  res: Response,
): Promise<void> => {
  try {
    const user = await User.findById(req.user?.id);

    if (!user) {
      res.status(404).json({
        status: 404,
        message: "User not found.",
        data: null,
      });
      return;
    }

    res.status(200).json({
      status: 200,
      message: "Profile retrieved successfully.",
      data: {
        id: user._id,
        name: user.name,
        email: user.email,
        role: user.role,
        isActive: user.isActive,
        createdAt: user.createdAt,
        updatedAt: user.updatedAt,
      },
    });
  } catch (error) {
    res.status(500).json({
      status: 500,
      message: "Failed to retrieve profile.",
      data: null,
    });
  }
};

export const adminOnly = async (req: Request, res: Response): Promise<void> => {
  res.status(200).json({
    status: 200,
    message: "Admin-only endpoint accessed successfully.",
    data: {
      user: req.user,
    },
  });
};

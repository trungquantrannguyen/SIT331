import { NextFunction, Request, Response } from "express";
import jwt from "jsonwebtoken";
import { UserRole } from "../models/User";

interface JwtPayload {
  id: string;
  email: string;
  role: UserRole;
}

export const authenticate = (
  req: Request,
  res: Response,
  next: NextFunction,
): void => {
  const authHeader = req.headers.authorization;

  if (!authHeader || !authHeader.startsWith("Bearer ")) {
    res.status(401).json({
      status: 401,
      message: "Access denied. No token provided.",
      data: null,
    });
    return;
  }

  const token = authHeader.split(" ")[1];
  const jwtSecret = process.env.JWT_SECRET;

  if (!jwtSecret) {
    res.status(500).json({
      status: 500,
      message: "JWT secret is not configured.",
      data: null,
    });
    return;
  }

  try {
    const decoded = jwt.verify(token, jwtSecret) as JwtPayload;

    req.user = {
      id: decoded.id,
      email: decoded.email,
      role: decoded.role,
    };

    next();
  } catch {
    res.status(401).json({
      status: 401,
      message: "Invalid or expired token.",
      data: null,
    });
  }
};

export const authorizeRoles = (...roles: UserRole[]) => {
  return (req: Request, res: Response, next: NextFunction): void => {
    if (!req.user) {
      res.status(401).json({
        status: 401,
        message: "Authentication required.",
        data: null,
      });
      return;
    }

    if (!roles.includes(req.user.role)) {
      res.status(403).json({
        status: 403,
        message:
          "Forbidden. You do not have permission to access this resource.",
        data: null,
      });
      return;
    }

    next();
  };
};

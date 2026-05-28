import express, { Application, Request, Response } from "express";
import cors from "cors";
import helmet from "helmet";
import morgan from "morgan";
import dotenv from "dotenv";
import { errorHandler, notFoundHandler } from "./middleware/errorMiddleware";
import authRoutes from "./routes/authRoutes";
import artistRoutes from "./routes/artistRoutes";
import artifactRoutes from "./routes/artifactRoutes";
import symbolRoutes from "./routes/symbolRoutes";
import exhibitionRoutes from "./routes/exhibitionRoutes";
import swaggerUI from "swagger-ui-express";
import { swaggerSpec } from "./docs/swagger";

dotenv.config();

const app: Application = express();
app.use(helmet());
app.use(cors());
app.use(express.json());
app.use(morgan("dev"));

app.use("/api/docs", swaggerUI.serve, swaggerUI.setup(swaggerSpec));

/**
 * @swagger
 * /:
 *   get:
 *     summary: Check base API status
 *     tags: [Health]
 *     description: Returns a basic confirmation that the Aboriginal Art Gallery Backend API is running.
 *     responses:
 *       200:
 *         description: API base route is running successfully.
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 status:
 *                   type: number
 *                   example: 200
 *                 message:
 *                   type: string
 *                   example: Aboriginal Art Gallery Backend API is running.
 *                 data:
 *                   type: object
 *                   properties:
 *                     project:
 *                       type: string
 *                       example: Aboriginal Art Gallery Backend
 *                     unit:
 *                       type: string
 *                       example: SIT331
 *                     student:
 *                       type: string
 *                       example: Trung Quan Tran Nguyen
 *                     studentId:
 *                       type: string
 *                       example: "225054634"
 */
app.get("/", (_req: Request, res: Response) => {
  res.status(200).json({
    status: 200,
    message: "Aboriginal Art Gallery Backend API is running.",
    data: {
      project: "Aboriginal Art Gallery Backend",
      unit: "SIT331",
      student: "Trung Quan Tran Nguyen",
      studentId: "225054634",
    },
  });
});

/**
 * @swagger
 * /health:
 *   get:
 *     summary: Run API health check
 *     tags: [Health]
 *     description: Returns server uptime and timestamp to confirm that the backend service is reachable and running.
 *     responses:
 *       200:
 *         description: Server health check successful.
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 status:
 *                   type: number
 *                   example: 200
 *                 message:
 *                   type: string
 *                   example: Server health check successful.
 *                 data:
 *                   type: object
 *                   properties:
 *                     uptime:
 *                       type: number
 *                       example: 120.56
 *                     timestamp:
 *                       type: string
 *                       format: date-time
 *                       example: "2026-05-28T04:30:00.000Z"
 */
app.get("/health", (_req: Request, res: Response) => {
  res.status(200).json({
    status: 200,
    message: "Server health check successful.",
    data: {
      uptime: process.uptime(),
      timestamp: new Date().toISOString(),
    },
  });
});

app.use("/api/auth", authRoutes);
app.use("/api/artists", artistRoutes);
app.use("/api/artifacts", artifactRoutes);
app.use("/api/symbols", symbolRoutes);
app.use("/api/exhibitions", exhibitionRoutes);

app.use(notFoundHandler);
app.use(errorHandler);

export default app;

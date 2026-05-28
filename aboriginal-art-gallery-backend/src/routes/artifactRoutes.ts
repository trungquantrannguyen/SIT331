import { Router } from "express";
import {
  createArtifact,
  deleteArtifact,
  getArtifactById,
  getArtifacts,
  updateArtifact,
} from "../controllers/artifactController";
import { authenticate, authorizeRoles } from "../middleware/authMiddleware";
import { validateRequest } from "../middleware/validateRequest";
import {
  artifactIdParamSchema,
  createArtifactSchema,
  updateArtifactSchema,
} from "../validation/artifactValidation";

/**
 * @swagger
 * /api/artifacts:
 *   get:
 *     summary: Get all active artifacts
 *     tags: [Artifacts]
 *     security:
 *      - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: title
 *         schema:
 *           type: string
 *       - in: query
 *         name: artType
 *         schema:
 *           type: string
 *       - in: query
 *         name: culturalRegion
 *         schema:
 *           type: string
 *       - in: query
 *         name: tag
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Artifacts retrieved successfully
 *   post:
 *     summary: Create a new artifact
 *     tags: [Artifacts]
 *     security:
 *       - bearerAuth: []
 *     description: Requires admin or curator role. The artist ID must reference an active artist.
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/ArtifactRequest'
 *     responses:
 *       201:
 *         description: Artifact created successfully
 *       400:
 *         description: Invalid artist relationship
 *
 * /api/artifacts/{id}:
 *   get:
 *     summary: Get artifact by ID
 *     tags: [Artifacts]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Artifact retrieved successfully
 *   put:
 *     summary: Update artifact
 *     tags: [Artifacts]
 *     security:
 *       - bearerAuth: []
 *     description: Requires admin or curator role.
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Artifact updated successfully
 *   delete:
 *     summary: Soft delete artifact
 *     tags: [Artifacts]
 *     security:
 *       - bearerAuth: []
 *     description: Requires admin role.
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Artifact deleted successfully
 */

const router = Router();

/**
 * @route GET /api/artifacts
 * @desc Get all active artifacts
 * @access Public
 */
router.get("/", authenticate, getArtifacts);

/**
 * @route GET /api/artifacts/:id
 * @desc Get artifact by ID
 * @access Public
 */
router.get(
  "/:id",
  authenticate,
  validateRequest(artifactIdParamSchema),
  getArtifactById,
);

/**
 * @route POST /api/artifacts
 * @desc Create a new artifact
 * @access Admin, Curator
 */
router.post(
  "/",
  authenticate,
  authorizeRoles("admin", "curator"),
  validateRequest(createArtifactSchema),
  createArtifact,
);

/**
 * @route PUT /api/artifacts/:id
 * @desc Update an artifact
 * @access Admin, Curator
 */
router.put(
  "/:id",
  authenticate,
  authorizeRoles("admin", "curator"),
  validateRequest(artifactIdParamSchema),
  validateRequest(updateArtifactSchema),
  updateArtifact,
);

/**
 * @route DELETE /api/artifacts/:id
 * @desc Soft delete an artifact
 * @access Admin only
 */
router.delete(
  "/:id",
  authenticate,
  authorizeRoles("admin"),
  validateRequest(artifactIdParamSchema),
  deleteArtifact,
);

export default router;

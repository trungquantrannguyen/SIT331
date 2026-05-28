import { Router } from "express";
import {
  createExhibition,
  deleteExhibition,
  getExhibitionById,
  getExhibitions,
  updateExhibition,
} from "../controllers/exhibitionController";
import { authenticate, authorizeRoles } from "../middleware/authMiddleware";
import { validateRequest } from "../middleware/validateRequest";
import {
  createExhibitionSchema,
  exhibitionIdParamSchema,
  updateExhibitionSchema,
} from "../validation/exhibitionValidation";

/**
 * @swagger
 * /api/exhibitions:
 *   get:
 *     summary: Get all active exhibitions
 *     tags: [Exhibitions]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: title
 *         schema:
 *           type: string
 *       - in: query
 *         name: status
 *         schema:
 *           type: string
 *       - in: query
 *         name: location
 *         schema:
 *           type: string
 *       - in: query
 *         name: tag
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Exhibitions retrieved successfully
 *   post:
 *     summary: Create a new exhibition
 *     tags: [Exhibitions]
 *     security:
 *       - bearerAuth: []
 *     description: Requires admin or curator role. Featured artifacts must exist. End date must be after start date.
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/ExhibitionRequest'
 *     responses:
 *       201:
 *         description: Exhibition created successfully
 *
 * /api/exhibitions/{id}:
 *   get:
 *     summary: Get exhibition by ID
 *     tags: [Exhibitions]
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
 *         description: Exhibition retrieved successfully
 *   put:
 *     summary: Update exhibition
 *     tags: [Exhibitions]
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
 *         description: Exhibition updated successfully
 *   delete:
 *     summary: Soft delete exhibition
 *     tags: [Exhibitions]
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
 *         description: Exhibition deleted successfully
 */

const router = Router();

/**
 * @route GET /api/exhibitions
 * @desc Get all active exhibitions
 * @access Public
 */
router.get("/", authenticate, getExhibitions);

/**
 * @route GET /api/exhibitions/:id
 * @desc Get exhibition by ID
 * @access Public
 */
router.get(
  "/:id",
  authenticate,
  validateRequest(exhibitionIdParamSchema),
  getExhibitionById,
);

/**
 * @route POST /api/exhibitions
 * @desc Create a new exhibition
 * @access Admin, Curator
 */
router.post(
  "/",
  authenticate,
  authorizeRoles("admin", "curator"),
  validateRequest(createExhibitionSchema),
  createExhibition,
);

/**
 * @route PUT /api/exhibitions/:id
 * @desc Update an exhibition
 * @access Admin, Curator
 */
router.put(
  "/:id",
  authenticate,
  authorizeRoles("admin", "curator"),
  validateRequest(exhibitionIdParamSchema),
  validateRequest(updateExhibitionSchema),
  updateExhibition,
);

/**
 * @route DELETE /api/exhibitions/:id
 * @desc Soft delete an exhibition
 * @access Admin only
 */
router.delete(
  "/:id",
  authenticate,
  authorizeRoles("admin"),
  validateRequest(exhibitionIdParamSchema),
  deleteExhibition,
);

export default router;

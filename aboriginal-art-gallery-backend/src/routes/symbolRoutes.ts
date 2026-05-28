import { Router } from "express";
import {
  createSymbol,
  deleteSymbol,
  getSymbolById,
  getSymbols,
  updateSymbol,
} from "../controllers/symbolController";
import { authenticate, authorizeRoles } from "../middleware/authMiddleware";
import { validateRequest } from "../middleware/validateRequest";
import {
  createSymbolSchema,
  symbolIdParamSchema,
  updateSymbolSchema,
} from "../validation/symbolValidation";

/**
 * @swagger
 * /api/symbols:
 *   get:
 *     summary: Get all active Aboriginal symbols
 *     tags: [Symbols]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: search
 *         schema:
 *           type: string
 *       - in: query
 *         name: region
 *         schema:
 *           type: string
 *       - in: query
 *         name: tag
 *         schema:
 *           type: string
 *     responses:
 *       200:
 *         description: Symbols retrieved successfully
 *   post:
 *     summary: Create a new Aboriginal symbol
 *     tags: [Symbols]
 *     security:
 *       - bearerAuth: []
 *     description: Requires admin or curator role. Related artifacts must exist.
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/SymbolRequest'
 *     responses:
 *       201:
 *         description: Symbol created successfully
 *
 * /api/symbols/{id}:
 *   get:
 *     summary: Get symbol by ID
 *     tags: [Symbols]
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
 *         description: Symbol retrieved successfully
 *   put:
 *     summary: Update symbol
 *     tags: [Symbols]
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
 *         description: Symbol updated successfully
 *   delete:
 *     summary: Soft delete symbol
 *     tags: [Symbols]
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
 *         description: Symbol deleted successfully
 */

const router = Router();

/**
 * @route GET /api/symbols
 * @desc Get all active Aboriginal symbols
 * @access Public
 */
router.get("/", authenticate, getSymbols);

/**
 * @route GET /api/symbols/:id
 * @desc Get Aboriginal symbol by ID
 * @access Public
 */
router.get(
  "/:id",
  authenticate,
  validateRequest(symbolIdParamSchema),
  getSymbolById,
);

/**
 * @route POST /api/symbols
 * @desc Create a new Aboriginal symbol
 * @access Admin, Curator
 */
router.post(
  "/",
  authenticate,
  authorizeRoles("admin", "curator"),
  validateRequest(createSymbolSchema),
  createSymbol,
);

/**
 * @route PUT /api/symbols/:id
 * @desc Update Aboriginal symbol
 * @access Admin, Curator
 */
router.put(
  "/:id",
  authenticate,
  authorizeRoles("admin", "curator"),
  validateRequest(symbolIdParamSchema),
  validateRequest(updateSymbolSchema),
  updateSymbol,
);

/**
 * @route DELETE /api/symbols/:id
 * @desc Soft delete Aboriginal symbol
 * @access Admin only
 */
router.delete(
  "/:id",
  authenticate,
  authorizeRoles("admin"),
  validateRequest(symbolIdParamSchema),
  deleteSymbol,
);

export default router;

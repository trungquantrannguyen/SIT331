import { Router } from "express";
import {
  createArtist,
  deleteArtist,
  getArtistById,
  getArtists,
  updateArtist,
} from "../controllers/artistController";
import { authenticate, authorizeRoles } from "../middleware/authMiddleware";
import { validateRequest } from "../middleware/validateRequest";
import {
  artistIdParamSchema,
  createArtistSchema,
  updateArtistSchema,
} from "../validation/artistValidation";

/**
 * @swagger
 * /api/artists:
 *   get:
 *     summary: Get all active artists
 *     tags: [Artists]
 *     security:
 *       - bearerAuth: []
 *     parameters:
 *       - in: query
 *         name: search
 *         schema:
 *           type: string
 *         description: Search artists by name
 *       - in: query
 *         name: region
 *         schema:
 *           type: string
 *         description: Filter artists by region
 *       - in: query
 *         name: nationOrCommunity
 *         schema:
 *           type: string
 *         description: Filter artists by nation or community
 *     responses:
 *       200:
 *         description: Artists retrieved successfully
 *   post:
 *     summary: Create a new artist
 *     tags: [Artists]
 *     security:
 *       - bearerAuth: []
 *     description: Requires admin or curator role.
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/ArtistRequest'
 *     responses:
 *       201:
 *         description: Artist created successfully
 *       401:
 *         description: Missing token
 *       403:
 *         description: Insufficient role permission
 *
 * /api/artists/{id}:
 *   get:
 *     summary: Get artist by ID
 *     tags: [Artists]
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
 *         description: Artist retrieved successfully
 *       404:
 *         description: Artist not found
 *   put:
 *     summary: Update artist by ID
 *     tags: [Artists]
 *     security:
 *       - bearerAuth: []
 *     description: Requires admin or curator role.
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/ArtistRequest'
 *     responses:
 *       200:
 *         description: Artist updated successfully
 *   delete:
 *     summary: Soft delete artist by ID
 *     tags: [Artists]
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
 *         description: Artist deleted successfully
 */

const router = Router();

/**
 * @route GET /api/artists
 * @desc Get all active artists
 * @access Public
 */
router.get("/", authenticate, getArtists);

/**
 * @route GET /api/artists/:id
 * @desc Get artist by ID
 * @access Public
 */
router.get(
  "/:id",
  authenticate,
  validateRequest(artistIdParamSchema),
  getArtistById,
);

/**
 * @route POST /api/artists
 * @desc Create a new artist
 * @access Admin, Curator
 */
router.post(
  "/",
  authenticate,
  authorizeRoles("admin", "curator"),
  validateRequest(createArtistSchema),
  createArtist,
);

/**
 * @route PUT /api/artists/:id
 * @desc Update an artist
 * @access Admin, Curator
 */
router.put(
  "/:id",
  authenticate,
  authorizeRoles("admin", "curator"),
  validateRequest(artistIdParamSchema),
  validateRequest(updateArtistSchema),
  updateArtist,
);

/**
 * @route DELETE /api/artists/:id
 * @desc Soft delete an artist
 * @access Admin only
 */
router.delete(
  "/:id",
  authenticate,
  authorizeRoles("admin"),
  validateRequest(artistIdParamSchema),
  deleteArtist,
);

export default router;

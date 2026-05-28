import { Router } from "express";
import {
  adminOnly,
  getProfile,
  loginUser,
  registerUser,
} from "../controllers/authController";
import { authenticate, authorizeRoles } from "../middleware/authMiddleware";
import { validateRequest } from "../middleware/validateRequest";
import { loginSchema, registerSchema } from "../validation/authValidation";

/**
 * @swagger
 * /api/auth/register:
 *   post:
 *     summary: Register a new user
 *     tags: [Authentication]
 *     description: Creates a new gallery user. Passwords are hashed using bcrypt before storage.
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/RegisterRequest'
 *     responses:
 *       201:
 *         description: User registered successfully
 *       400:
 *         description: Validation failed
 *       409:
 *         description: Email already registered
 *
 * /api/auth/login:
 *   post:
 *     summary: Login user
 *     tags: [Authentication]
 *     description: Authenticates a user and returns a JWT access token.
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             $ref: '#/components/schemas/LoginRequest'
 *     responses:
 *       200:
 *         description: Login successful
 *       401:
 *         description: Invalid email or password
 *
 * /api/auth/profile:
 *   get:
 *     summary: Get current user profile
 *     tags: [Authentication]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: Profile retrieved successfully
 *       401:
 *         description: Missing or invalid token
 *
 * /api/auth/admin-only:
 *   get:
 *     summary: Admin-only authorization test
 *     tags: [Authentication]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: Admin-only endpoint accessed successfully
 *       403:
 *         description: User does not have admin role
 */

const router = Router();

/**
 * @route POST /api/auth/register
 * @desc Register a new user
 * @access Public
 */
router.post("/register", validateRequest(registerSchema), registerUser);

/**
 * @route POST /api/auth/login
 * @desc Login user and return JWT
 * @access Public
 */
router.post("/login", validateRequest(loginSchema), loginUser);

/**
 * @route GET /api/auth/profile
 * @desc Get current logged-in user profile
 * @access Private
 */
router.get("/profile", authenticate, getProfile);

/**
 * @route GET /api/auth/admin-only
 * @desc Test admin-only authorization
 * @access Admin
 */
router.get("/admin-only", authenticate, authorizeRoles("admin"), adminOnly);

export default router;

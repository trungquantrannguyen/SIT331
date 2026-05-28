# Business Requirements Document

## Aboriginal Art Gallery Backend API

**Student:** Trung Quan Tran Nguyen  
**Student ID:** 225054634  
**Unit:** SIT331 Full Stack Development: Secure Backend Services  
**Task:** 5.2HD Build Your Own Backend  
**Technology Stack:** Express.js, TypeScript, MongoDB Atlas, Mongoose, JWT, Jest, Supertest, Swagger/OpenAPI

---

## 1. Project Overview

The Aboriginal Art Gallery Backend API is a secure backend service for managing an Aboriginal art gallery domain. The system supports authenticated users, role-based access control, artist records, artwork/artifact records, Aboriginal symbols/iconography, and gallery exhibitions.

The purpose of this backend is to demonstrate secure backend development using a different Web API technology stack from the standard ASP.NET Core stack. The project uses Express.js with TypeScript, MongoDB Atlas as a cloud database engine, Mongoose as the object data modelling library, JWT authentication, Swagger/OpenAPI documentation, and Jest/Supertest automated testing.

---

## 2. Business Goals

The main business goals are:

1. Provide a secure API for managing Aboriginal art gallery information.
2. Allow authenticated users to browse gallery data.
3. Allow curators and administrators to manage artists, artifacts, symbols, and exhibitions.
4. Protect write operations using role-based authorization.
5. Maintain clean relationships between artists, artworks, symbols, and exhibitions.
6. Document the API clearly for future developers and testers.
7. Verify backend behaviour using automated integration tests.

---

## 3. Scope

### In Scope

The backend includes the following bounded contexts:

1. Users / Membership / Roles
2. Artists
3. Artifacts
4. Aboriginal Symbols / Iconography
5. Exhibitions

The API supports:

- User registration
- User login
- JWT authentication
- Role-based authorization
- CRUD operations for gallery contexts
- Soft delete behaviour
- MongoDB Atlas data persistence
- Swagger/OpenAPI documentation
- Jest and Supertest automated tests

### Out of Scope

The following features are not included in this version:

- Frontend application
- Online payment processing
- Real image upload storage
- Full content management workflow
- OAuth/OIDC external provider integration
- Geospatial map features
- Public comments or reviews
- Production deployment

---

## 4. User Roles

| Role    | Description                | Main Permissions                                                          |
| ------- | -------------------------- | ------------------------------------------------------------------------- |
| Member  | Authenticated gallery user | Can read gallery data                                                     |
| Curator | Gallery content manager    | Can read, create, and update artists, artifacts, symbols, and exhibitions |
| Admin   | System administrator       | Can read, create, update, and soft delete records                         |

---

## 5. Functional Requirements

### FR1 — User Registration

The system shall allow a new user to register with name, email, password, and role.

Acceptance criteria:

- The email must be valid.
- The password must meet validation rules.
- The password must be hashed before storage.
- Duplicate email registration must be rejected.

---

### FR2 — User Login

The system shall allow users to log in with email and password.

Acceptance criteria:

- The system checks the submitted password against the stored password hash.
- The system returns a JWT token after successful login.
- Invalid credentials return an error response.

---

### FR3 — Authentication for API Access

The system shall require a valid JWT Bearer token for gallery data endpoints.

Acceptance criteria:

- Requests without a token return 401.
- Requests with invalid or expired tokens return 401.
- Valid tokens allow the request to continue.

---

### FR4 — Role-Based Authorization

The system shall restrict write and delete operations by role.

Acceptance criteria:

- Members can read data only.
- Curators can create and update gallery records.
- Admins can create, update, and soft delete records.
- Unauthorized roles receive 403.

---

### FR5 — Artist Management

The system shall allow authorized users to manage artist records.

Acceptance criteria:

- Artist records include name, nation/community, biography, region, language group, birth year, and art styles.
- Admins and curators can create and update artists.
- Admins can soft delete artists.
- Deleted artists are hidden from normal API results.

---

### FR6 — Artifact Management

The system shall allow authorized users to manage artwork/artifact records.

Acceptance criteria:

- Each artifact must reference an active artist.
- Invalid artist references are rejected.
- Artifact records include title, description, art type, materials, cultural region, price, status, tags, and dimensions.
- Admins and curators can create and update artifacts.
- Admins can soft delete artifacts.
- Deleted artifacts are archived and hidden from normal API results.

---

### FR7 — Aboriginal Symbol / Iconography Management

The system shall allow authorized users to manage Aboriginal symbol records.

Acceptance criteria:

- Symbol records include name, meaning, cultural note, common visual form, associated regions, related artifacts, and tags.
- Related artifact IDs must exist if provided.
- Admins and curators can create and update symbols.
- Admins can soft delete symbols.
- Symbol meanings must include cultural notes to avoid oversimplified interpretation.

---

### FR8 — Exhibition Management

The system shall allow authorized users to manage exhibition records.

Acceptance criteria:

- Exhibition records include title, description, location, dates, status, featured artifacts, curator notes, and tags.
- Featured artifact IDs must exist if provided.
- End date must be after or equal to start date.
- Admins and curators can create and update exhibitions.
- Admins can soft delete exhibitions.
- Deleted exhibitions are marked as cancelled.

---

### FR9 — Search and Filtering

The system shall support search and filtering for gallery data.

Acceptance criteria:

- Artists can be filtered by name, region, and nation/community.
- Artifacts can be filtered by title, art type, cultural region, status, and tag.
- Symbols can be filtered by name/search, region, and tag.
- Exhibitions can be filtered by title, status, location, and tag.
- Non-matching filters return an empty array rather than unrelated results.

---

### FR10 — API Documentation

The system shall provide Swagger/OpenAPI documentation.

Acceptance criteria:

- Swagger UI is available at `/api/docs`.
- API route groups are documented.
- JWT Bearer authentication is documented.
- Health, authentication, artists, artifacts, symbols, and exhibitions routes are visible.

---

### FR11 — Automated Testing

The system shall include automated backend tests.

Acceptance criteria:

- Health routes are tested.
- Authentication validation is tested.
- Authorization failures are tested.
- Artists, Artifacts, Symbols, and Exhibitions include database-connected integration tests.
- Tests use a separate MongoDB test database.

---

## 6. Non-Functional Requirements

| Requirement     | Description                                                                                  |
| --------------- | -------------------------------------------------------------------------------------------- |
| Security        | Passwords must be hashed, tokens must be verified, and role permissions must be enforced     |
| Maintainability | Code should be separated into models, controllers, routes, middleware, validation, and tests |
| Data integrity  | Related IDs must be validated before records are created or updated                          |
| Reliability     | Automated tests should verify common success and failure cases                               |
| Documentation   | Swagger, diagrams, and BRD should explain the backend clearly                                |
| Scalability     | MongoDB indexes should support searching and filtering                                       |
| Auditability    | Soft delete keeps records in the database while hiding them from normal API results          |

---

## 7. Security Requirements

1. Passwords must never be stored as plain text.
2. JWT secret values must be stored in environment variables.
3. Protected routes must reject missing or invalid tokens.
4. Role-based authorization must return 403 for insufficient permissions.
5. Request bodies must be validated before reaching database logic.
6. Soft delete must be used for important gallery records to avoid accidental data loss.
7. Test data must use a separate test database.

---

## 8. Implemented Bounded Contexts

| Bounded Context                  | Main Model  | Main API Path      | Responsibility                                           |
| -------------------------------- | ----------- | ------------------ | -------------------------------------------------------- |
| Users / Membership / Roles       | User        | `/api/auth`        | Authentication and role identity                         |
| Artists                          | Artist      | `/api/artists`     | Artist profile management                                |
| Artifacts                        | Artifact    | `/api/artifacts`   | Artwork management and artist relationship               |
| Aboriginal Symbols / Iconography | SymbolModel | `/api/symbols`     | Cultural symbol documentation                            |
| Exhibitions                      | Exhibition  | `/api/exhibitions` | Exhibition management and featured artifact relationship |

---

## 9. Business Rules

| Rule ID | Business Rule                                                               |
| ------- | --------------------------------------------------------------------------- |
| BR1     | A user must authenticate before accessing gallery data                      |
| BR2     | A member can read gallery data but cannot create, update, or delete records |
| BR3     | A curator can create and update gallery records                             |
| BR4     | Only an admin can soft delete gallery records                               |
| BR5     | An artifact must reference an existing active artist                        |
| BR6     | A symbol can only reference active artifacts                                |
| BR7     | An exhibition can only feature active artifacts                             |
| BR8     | Exhibition end date must not be earlier than start date                     |
| BR9     | Deleted artifacts are marked as archived                                    |
| BR10    | Deleted exhibitions are marked as cancelled                                 |
| BR11    | Search queries must only return matching records                            |
| BR12    | Invalid request bodies must return validation errors                        |

---

## 10. API Summary

| Context     | Method | Endpoint               | Access         |
| ----------- | ------ | ---------------------- | -------------- |
| Health      | GET    | `/`                    | Public         |
| Health      | GET    | `/health`              | Public         |
| Auth        | POST   | `/api/auth/register`   | Public         |
| Auth        | POST   | `/api/auth/login`      | Public         |
| Auth        | GET    | `/api/auth/profile`    | Authenticated  |
| Artists     | GET    | `/api/artists`         | Authenticated  |
| Artists     | GET    | `/api/artists/:id`     | Authenticated  |
| Artists     | POST   | `/api/artists`         | Admin, Curator |
| Artists     | PUT    | `/api/artists/:id`     | Admin, Curator |
| Artists     | DELETE | `/api/artists/:id`     | Admin          |
| Artifacts   | GET    | `/api/artifacts`       | Authenticated  |
| Artifacts   | GET    | `/api/artifacts/:id`   | Authenticated  |
| Artifacts   | POST   | `/api/artifacts`       | Admin, Curator |
| Artifacts   | PUT    | `/api/artifacts/:id`   | Admin, Curator |
| Artifacts   | DELETE | `/api/artifacts/:id`   | Admin          |
| Symbols     | GET    | `/api/symbols`         | Authenticated  |
| Symbols     | GET    | `/api/symbols/:id`     | Authenticated  |
| Symbols     | POST   | `/api/symbols`         | Admin, Curator |
| Symbols     | PUT    | `/api/symbols/:id`     | Admin, Curator |
| Symbols     | DELETE | `/api/symbols/:id`     | Admin          |
| Exhibitions | GET    | `/api/exhibitions`     | Authenticated  |
| Exhibitions | GET    | `/api/exhibitions/:id` | Authenticated  |
| Exhibitions | POST   | `/api/exhibitions`     | Admin, Curator |
| Exhibitions | PUT    | `/api/exhibitions/:id` | Admin, Curator |
| Exhibitions | DELETE | `/api/exhibitions/:id` | Admin          |

---

## 11. Testing Requirements

The backend must be tested using Jest and Supertest.

Required evidence:

- Health endpoint tests pass.
- Authentication validation tests pass.
- Authorization tests pass.
- Artist integration tests pass.
- Artifact integration tests pass.
- Symbol integration tests pass.
- Exhibition integration tests pass.
- Test database is separated from main database.

# Aggregate Design Canvases

## Canvas 1 — User / Membership / Roles Aggregate

| Section                  | Description                                                                             |
| ------------------------ | --------------------------------------------------------------------------------------- |
| Aggregate Name           | User / Membership / Roles                                                               |
| Purpose                  | Manages user identity, login credentials, and role-based access control                 |
| Root Entity              | User                                                                                    |
| Entities / Value Objects | User, Role                                                                              |
| Key Fields               | name, email, password, role, isActive                                                   |
| Commands                 | Register user, login user, get profile                                                  |
| Queries                  | Get current authenticated user profile                                                  |
| Business Rules           | Email must be unique; password must be hashed; role determines permissions              |
| Invariants               | Password is never stored in plain text; JWT must be signed; invalid tokens are rejected |
| Security                 | bcrypt password hashing, JWT Bearer authentication, role-based authorization            |
| API Endpoints            | `/api/auth/register`, `/api/auth/login`, `/api/auth/profile`, `/api/auth/admin-only`    |
| Failure Cases            | Duplicate email, invalid credentials, missing token, invalid token, insufficient role   |
| Test Evidence            | Auth validation tests and authorization tests                                           |

---

## Canvas 2 — Artist Aggregate

| Section                  | Description                                                                               |
| ------------------------ | ----------------------------------------------------------------------------------------- |
| Aggregate Name           | Artist                                                                                    |
| Purpose                  | Stores artist profile and cultural background information                                 |
| Root Entity              | Artist                                                                                    |
| Entities / Value Objects | Artist, ArtStyle                                                                          |
| Key Fields               | name, nationOrCommunity, languageGroup, biography, birthYear, region, artStyles, isActive |
| Commands                 | Create artist, update artist, soft delete artist                                          |
| Queries                  | Get artists, get artist by ID, search by name/region/community                            |
| Business Rules           | Artist name and biography are required; deleted artists are hidden from normal results    |
| Invariants               | Active artist records must contain enough descriptive information for gallery use         |
| Security                 | Authenticated read access; admin/curator write access; admin-only delete                  |
| API Endpoints            | `/api/artists`, `/api/artists/:id`                                                        |
| Failure Cases            | Missing required fields, invalid ID, unauthorized role, missing token                     |
| Test Evidence            | Artist integration tests                                                                  |

---

## Canvas 3 — Artifact Aggregate

| Section                  | Description                                                                                                                         |
| ------------------------ | ----------------------------------------------------------------------------------------------------------------------------------- |
| Aggregate Name           | Artifact                                                                                                                            |
| Purpose                  | Manages artworks or gallery pieces and connects them to artists                                                                     |
| Root Entity              | Artifact                                                                                                                            |
| Entities / Value Objects | Artifact, Dimensions, ArtifactStatus, Tags                                                                                          |
| Key Fields               | title, artist, description, artType, materials, yearCreated, culturalRegion, dimensions, priceAud, status, tags, imageUrl, isActive |
| Commands                 | Create artifact, update artifact, soft delete artifact                                                                              |
| Queries                  | Get artifacts, get artifact by ID, filter by title/art type/region/status/tag                                                       |
| Business Rules           | Artifact must reference an existing active artist; deleted artifacts become archived                                                |
| Invariants               | Artifact cannot exist with a broken artist reference; price and dimensions cannot be negative                                       |
| Security                 | Authenticated read access; admin/curator write access; admin-only delete                                                            |
| API Endpoints            | `/api/artifacts`, `/api/artifacts/:id`                                                                                              |
| Failure Cases            | Invalid artist ID, missing token, insufficient role, invalid body, invalid artifact ID                                              |
| Test Evidence            | Artifact integration tests                                                                                                          |

---

## Canvas 4 — Aboriginal Symbols / Iconography Aggregate

| Section                  | Description                                                                                        |
| ------------------------ | -------------------------------------------------------------------------------------------------- |
| Aggregate Name           | Aboriginal Symbols / Iconography                                                                   |
| Purpose                  | Stores symbol meanings, cultural notes, visual form descriptions, and related artworks             |
| Root Entity              | SymbolModel                                                                                        |
| Entities / Value Objects | Symbol, CulturalNote, RelatedArtifacts, Tags                                                       |
| Key Fields               | name, meaning, culturalNote, commonVisualForm, associatedRegions, relatedArtifacts, tags, isActive |
| Commands                 | Create symbol, update symbol, soft delete symbol                                                   |
| Queries                  | Get symbols, get symbol by ID, search by name/region/tag                                           |
| Business Rules           | Related artifact IDs must exist; symbol records must include cultural notes                        |
| Invariants               | Symbol information should not be saved without contextual cultural note                            |
| Security                 | Authenticated read access; admin/curator write access; admin-only delete                           |
| API Endpoints            | `/api/symbols`, `/api/symbols/:id`                                                                 |
| Failure Cases            | Invalid related artifact, missing token, insufficient role, invalid ID, invalid body               |
| Test Evidence            | Symbol integration tests                                                                           |

---

## Canvas 5 — Exhibition Aggregate

| Section                  | Description                                                                                                        |
| ------------------------ | ------------------------------------------------------------------------------------------------------------------ |
| Aggregate Name           | Exhibition                                                                                                         |
| Purpose                  | Manages gallery exhibitions and connects exhibitions to featured artifacts                                         |
| Root Entity              | Exhibition                                                                                                         |
| Entities / Value Objects | Exhibition, ExhibitionStatus, FeaturedArtifacts, DateRange, CuratorNotes                                           |
| Key Fields               | title, description, location, startDate, endDate, status, featuredArtifacts, curatorNotes, tags, isActive          |
| Commands                 | Create exhibition, update exhibition, soft delete exhibition                                                       |
| Queries                  | Get exhibitions, get exhibition by ID, filter by title/status/location/tag                                         |
| Business Rules           | Featured artifacts must exist; end date must be after or equal to start date; deleted exhibitions become cancelled |
| Invariants               | Exhibition date range must be valid; exhibition cannot feature broken artifact references                          |
| Security                 | Authenticated read access; admin/curator write access; admin-only delete                                           |
| API Endpoints            | `/api/exhibitions`, `/api/exhibitions/:id`                                                                         |
| Failure Cases            | Invalid artifact ID, invalid date range, missing token, insufficient role, invalid ID                              |
| Test Evidence            | Exhibition integration tests                                                                                       |

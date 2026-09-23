# ResumeBuilder.API -User, Registration, Login, and JWT-based authentication, Refresh Token.
## .NET Web API — Complete Architecture & Project Structure

### Major Key Points

- Built a layered **ASP.NET Core Web API** using Controller → Service → Repository architecture.
- Implemented **User CRUD, Registration, Login, and JWT-based authentication**.
- Used **DTOs** to separate API contracts from database entities.
- Implemented secure **password hashing**; plaintext passwords are never stored.
- Used short-lived **JWT Access Tokens** for authenticated API requests.
- Implemented a secure **Refresh Token architecture** for maintaining user sessions.
- Refresh tokens are stored as **hashed values** in the database rather than plaintext.
- Implemented **Refresh Token Rotation** after every successful refresh operation.
- Added **Token Family tracking** to manage individual user sessions/devices.
- Implemented **Refresh Token Replay Detection** to detect reuse of revoked tokens.
- Added **Current Device Logout** and **Logout All Devices** functionality.
- Used **database transactions and concurrency protection** during token rotation.
- Implemented token **expiration, revocation, and replacement tracking** for session security.
- Designed the authentication flow to support **multiple active devices securely**.
- Followed security principles such as **HTTPS, secure secrets, token validation, least exposure, and separation of responsibilities**.

### Project Overview
This project is a .NET Web API application for managing users and their
profile/resume-related information.

The application currently includes:

- User CRUD
- User Registration
- User Login
- Password hashing
- JWT Access Token
- Refresh Token
- Refresh Token Rotation
- Refresh Token Repository
- Token Family
- Refresh Token Replay Detection
- Logout Current Device
- Logout All Devices
- Concurrency-aware Refresh Token Rotation
- Transactional Refresh Token Rotation
- Repository Pattern
- Entity Framework Core
- SQL Server
- DTO-based API contracts
- Authentication and Authorization foundation

---

# 1. High-Level Architecture

The application follows a layered architecture.

```text
                    CLIENT
                      |
                      | HTTP / HTTPS
                      v
              +------------------+
              |   API Layer      |
              |   Controllers    |
              +------------------+
                      |
                      v
              +------------------+
              | Service Layer    |
              | Business Logic   |
              +------------------+
                      |
                      v
              +------------------+
              | Repository Layer |
              | Data Access      |
              +------------------+
                      |
                      v
              +------------------+
              | Entity Framework |
              | Core / DbContext  |
              +------------------+
                      |
                      v
              +------------------+
              |   SQL Server     |
              +------------------+
````

---

# 2. Layer Responsibilities

## 2.1 API / Controller Layer

Responsible for:

* Receiving HTTP requests
* Validating request models
* Calling application services
* Returning HTTP responses
* Handling authentication-related HTTP responses
* Extracting request information such as:

  * IP address
  * User-Agent
* Mapping service results to HTTP status codes

Controllers should NOT contain:

* Database queries
* Password hashing logic
* JWT generation logic
* Refresh-token business rules
* Repository implementation

---

# 2.2 Service Layer

Responsible for business logic.

Main responsibilities include:

* User registration
* Login
* Password verification
* JWT generation coordination
* Refresh-token generation
* Refresh-token rotation
* Replay detection
* Token-family revocation
* Logout
* Logout-all-devices
* Account-status validation
* Authentication decisions

The service layer communicates with repositories rather than directly
embedding database queries throughout business logic.

---

# 2.3 Repository Layer

Responsible for data access.

Repositories provide an abstraction over Entity Framework Core.

Responsibilities include:

* User retrieval
* User creation
* User update
* User deletion
* Refresh-token lookup
* Refresh-token insertion
* Refresh-token revocation
* Token-family revocation
* Token consumption
* Persistence operations

The service layer should not need to know how SQL queries are implemented.

---

# 2.4 Data Access Layer

This layer contains:

* DbContext
* Entity configurations
* Database mappings
* EF Core configuration
* SQL Server integration

Entity Framework Core is responsible for communication with SQL Server.

---

# 2.5 Domain / Entity Layer

Contains database-oriented entities such as:

* User
* Profile
* Achievement
* Education
* Employment
* Feedback
* Objective
* Project
* Skill
* Social
* RefreshToken

These represent persistent application data.

---

# 3. Recommended Project Structure

The structure developed so far can be organized as follows:

```text
MyApplication/
│
├── Controllers/
│   │
│   ├── AuthController
│   └── UsersController
│
├── Services/
│   │
│   ├── Interfaces/
│   │   ├── IAuthService
│   │   └── IUserService
│   │
│   ├── AuthService
│   └── UserService
│
├── Repositories/
│   │
│   ├── Interfaces/
│   │   ├── IAuthRepository
│   │   ├── IRefreshTokenRepository
│   │   └── IUserRepository
│   │
│   ├── AuthRepository
│   ├── RefreshTokenRepository
│   └── UserRepository
│
├── Models/
│   │
│   ├── User
│   ├── Profile
│   ├── Achievement
│   ├── Education
│   ├── Employment
│   ├── Feedback
│   ├── Objective
│   ├── Project
│   ├── Skill
│   ├── Social
│   └── RefreshToken
│
├── DTOs/
│   │
│   ├── Auth/
│   │   ├── RegisterRequestDto
│   │   ├── LoginRequestDto
│   │   ├── LoginResponseDto
│   │   ├── RefreshTokenRequestDto
│   │   └── RefreshTokenResultDto
│   │
│   └── Users/
│       ├── UserCreateDto
│       ├── UserUpdateDto
│       └── UserResponseDto
│
├── Enums/
│   │
│   └── RefreshTokenResult
│
├── Data/
│   │
│   └── ApplicationDbContext
│
├── Security/
│   │
│   ├── JWT configuration
│   ├── Password hashing
│   └── Authentication configuration
│
├── Configuration/
│   │
│   └── JWT settings
│
├── Migrations/
│
├── Program
│
├── appsettings.json
│
└── appsettings.Development.json
```

> Exact filenames can vary depending on the implementation choices made
> during the individual parts, but the architectural responsibilities
> remain the same.

---

# 4. Database Architecture

The SQL Server database is centered around the `Users` table.

```text
                         Users
                           |
          +----------------+----------------+
          |                |                |
          v                v                v
       Profile       Achievements       Education
        (1:1)          (1:N)              (1:N)
          |
          |
          +---------- Employments
          |             (1:N)
          |
          +---------- Feedback
          |             (1:N)
          |
          +---------- Objectives
          |             (1:1)
          |
          +---------- Projects
          |             (1:N)
          |
          +---------- Skills
          |             (1:Many)
          |
          +---------- Social
                        (1:1)

                         Users
                           |
                           |
                           v
                    RefreshTokens
```

---

# 5. Users Table

The `Users` table is the primary user/account table.

```text
Users
------------------------------------------------
UserId
Username
UserType
UserEmail
UserHashedPass
AccountStatus
CreatedAt
UpdatedAt
```

Important constraints:

```text
UserId
    |
    +-- Primary Key

UserEmail
    |
    +-- Unique
```

The password is stored as a hash rather than plain text.

---

# 6. Profile Table

Relationship:

```text
Users
  |
  | 1 : 1
  |
Profile
```

Main fields:

```text
UserId
FirstName
LastName
Sex
FathersName
Dob
Email
Phone
Address
Nationality
Relationship
Languages
Interests
Hobbies
Image
Status
CreatedAt
UpdatedAt
```

`UserId` acts as both:

* Primary Key
* Foreign Key to Users

Delete behavior:

```text
Delete User
     |
     v
Delete Profile
```

---

# 7. Achievements Table

Relationship:

```text
Users
  |
  | 1 : Many
  |
Achievements
```

Main fields:

```text
Aid
UserId
Awards
Subtitle
Place
Year
CreatedAt
UpdatedAt
```

A single user can have multiple achievements.

---

# 8. Education Table

Relationship:

```text
Users
  |
  | 1 : Many
  |
Education
```

Main fields:

```text
ExamId
UserId
Exam
Board
Year
College
Percentage
CreatedAt
UpdatedAt
```

A user can have multiple education records.

---

# 9. Employments Table

Relationship:

```text
Users
  |
  | 1 : Many
  |
Employments
```

Main fields:

```text
Eid
UserId
FromYear
ToYear
Company
Designation
Detail
CreatedAt
UpdatedAt
```

---

# 10. Feedback Table

Relationship:

```text
Users
  |
  | 1 : Many
  |
Feedback
```

Main fields:

```text
FeedbackId
UserId
FeedbackMsg
Rating
AdminId
Response
CreatedAt
UpdatedAt
```

---

# 11. Objectives Table

Relationship:

```text
Users
  |
  | 1 : 1
  |
Objectives
```

Main fields:

```text
UserId
ResumeType
Obj
CreatedAt
UpdatedAt
```

---

# 12. Projects Table

Relationship:

```text
Users
  |
  | 1 : Many
  |
Projects
```

Main fields:

```text
Pid
UserId
Title
Subtitle
Type
Detail
CreatedAt
UpdatedAt
```

---

# 13. Skills Table

The current SQL design has a `Skills` table with:

```text
SId
UserId
SkillName
SpecializationType
CreatedAt
UpdatedAt
```

The database has a foreign key from:

```text
Skills.UserId
        |
        v
Users.UserId
```

Although the original design description refers to Skills as 1:1,
the presence of an identity `SId` means the schema technically allows
multiple skill records for the same user unless an additional unique
constraint is added to `UserId`.

---

# 14. Social Table

Relationship:

```text
Users
  |
  | 1 : 1
  |
Social
```

Fields:

```text
UserId
Website
Linkedln
GitHub
Google
Facebook
Instagram
CreatedAt
UpdatedAt
```

---

# 15. Refresh Token Architecture

Refresh tokens are maintained separately from the Users table.

```text
Users
  |
  | 1 : Many
  |
RefreshTokens
```

A user can have multiple refresh-token records because the user can
have multiple sessions/devices.

---

# 16. RefreshToken Data Model

The refresh-token model developed through RT-1 to RT-9 contains the
following concepts:

```text
RefreshToken
------------------------------------------------
RefreshTokenId
UserId
TokenFamilyId
TokenHash
CreatedAt
ExpiresAt
RevokedAt
ReplacedByTokenHash
CreatedByIp
RevokedByIp
UserAgent
RowVersion
```

---

# 17. RefreshTokenId

Unique identifier for a refresh-token database record.

```text
RefreshTokenId
       |
       +-- Primary Key
```

---

# 18. TokenHash

The actual refresh token is not stored directly.

Conceptually:

```text
Raw Refresh Token
        |
        v
     Hashing
        |
        v
    TokenHash
        |
        v
      SQL DB
```

When a refresh request arrives:

```text
Client Refresh Token
        |
        v
      Hash
        |
        v
Search TokenHash
```

This prevents the database from directly containing usable raw
refresh-token values.

---

# 19. TokenFamilyId

`TokenFamilyId` represents a logical login/session chain.

Example:

```text
Family F1

Token A
   |
   v
Token B
   |
   v
Token C
   |
   v
Token D
```

All tokens belong to:

```text
F1
```

When Token A is replaced by Token B:

```text
Token A → F1
Token B → F1
```

The family remains unchanged during normal rotation.

---

# 20. Token Rotation

The refresh-token system uses rotation.

Conceptually:

```text
Refresh Token A
       |
       | Refresh
       v
Refresh Token B
       |
       | Refresh
       v
Refresh Token C
       |
       | Refresh
       v
Refresh Token D
```

Previous tokens become revoked.

---

# 21. ReplacedByTokenHash

This maintains the token replacement relationship.

Example:

```text
Token A
   |
   +---- ReplacedBy ---> Token B

Token B
   |
   +---- ReplacedBy ---> Token C
```

This provides a logical token chain for auditing and security analysis.

---

# 22. Token Expiration

Each refresh token contains:

```text
CreatedAt
ExpiresAt
```

The refresh operation verifies whether the token has expired before
issuing a replacement.

---

# 23. Token Revocation

A refresh token is considered active when:

```text
RevokedAt = NULL
```

A revoked token has:

```text
RevokedAt = timestamp
```

Revocation is therefore represented as a state transition:

```text
Active
  |
  v
Revoked
```

---

# 24. Token Family Revocation

When replay is detected, the token family can be revoked.

Example:

```text
Family F1

Token A → Revoked
Token B → Active
Token C → Active
```

Replay of Token A:

```text
Token A
   |
   v
Replay Detected
   |
   v
Revoke Family F1
```

Result:

```text
Token A → Revoked
Token B → Revoked
Token C → Revoked
```

---

# 25. Multiple Device Sessions

Each login/session can have its own token family.

Example:

```text
User
 |
 +-- Laptop
 |     |
 |     +-- Family F1
 |
 +-- Mobile
 |     |
 |     +-- Family F2
 |
 +-- Tablet
       |
       +-- Family F3
```

If Family F1 is compromised:

```text
F1 → Revoked
F2 → Active
F3 → Active
```

This provides session-level isolation.

---

# 26. RowVersion

RT-9 introduced concurrency protection.

The RefreshToken table contains:

```text
RowVersion
```

which is managed by SQL Server.

Conceptually:

```text
Token A
   |
   +-- RowVersion = X
```

After an update:

```text
Token A
   |
   +-- RowVersion = Y
```

EF Core uses this value as a concurrency token.

---

# 27. Why RowVersion Exists

It protects against concurrent requests attempting to modify the same
refresh-token record.

Example:

```text
Request A ── Token A ──┐
                       |
Request B ── Token A ──┘
```

Both initially see:

```text
Token A = Active
```

Only one should successfully consume the token.

The other request should encounter a concurrency conflict.

---

# 28. Transactional Refresh Rotation

RT-9 also introduced the concept of a database transaction.

The refresh operation conceptually performs:

```text
BEGIN TRANSACTION
       |
       v
Validate old token
       |
       v
Consume old token
       |
       v
Create replacement token
       |
       v
COMMIT
```

If an error occurs:

```text
ROLLBACK
```

This prevents partial refresh-token state.

---

# 29. Refresh Token Lifecycle

The overall lifecycle is:

```text
                LOGIN
                  |
                  v
           Generate Tokens
                  |
                  v
          Refresh Token A
                  |
                  v
             Active
                  |
                  | Refresh
                  v
             Revoked
                  |
                  v
          Refresh Token B
                  |
                  v
             Active
                  |
                  | Refresh
                  v
          Refresh Token C
```

---

# 30. Replay Detection

Replay detection was introduced in RT-8.

Scenario:

```text
Token A
   |
   v
Successfully used
   |
   v
Token A becomes revoked
```

Someone later sends Token A again:

```text
Client
  |
  v
Token A
  |
  v
Database
  |
  v
Token already revoked
  |
  v
Replay Detected
```

This is different from an unknown token.

---

# 31. Invalid Token vs Replay

The system distinguishes between:

```text
Unknown Token
```

and:

```text
Previously Valid but Already Revoked Token
```

Conceptually:

```text
Unknown token
     |
     v
InvalidToken
```

while:

```text
Existing + Revoked
     |
     v
ReplayDetected
```

---

# 32. RefreshTokenResult

The refresh-token workflow uses a result model to represent different
authentication outcomes.

Conceptually:

```text
RefreshTokenResult
│
├── Success
├── InvalidToken
├── Expired
├── ReplayDetected
├── UserNotFound
└── AccountInactive
```

This allows the controller to translate business results into
appropriate HTTP responses.

---

# 33. Authentication Architecture

The authentication architecture is:

```text
                 Authentication
                       |
          +------------+-------------+
          |                          |
          v                          v
       Register                    Login
          |                          |
          v                          v
    Password Hash              Verify Password
                                     |
                                     v
                              Generate Tokens
                                     |
                       +-------------+-------------+
                       |                           |
                       v                           v
                 Access Token              Refresh Token
```

---

# 34. Registration Flow

The registration flow is conceptually:

```text
Client
  |
  v
Registration Request
  |
  v
Auth Controller
  |
  v
Auth Service
  |
  +-- Validate input
  |
  +-- Check existing email
  |
  +-- Hash password
  |
  +-- Create User
  |
  v
Repository
  |
  v
SQL Server
```

---

# 35. Login Flow

```text
Client
  |
  v
Login Request
  |
  v
Auth Controller
  |
  v
Auth Service
  |
  +-- Find user
  |
  +-- Check account status
  |
  +-- Verify password
  |
  +-- Generate Access Token
  |
  +-- Generate Refresh Token
  |
  +-- Hash Refresh Token
  |
  +-- Create Token Family
  |
  +-- Store Refresh Token
  |
  v
Login Response
```

---

# 36. Login Token Structure

A successful login produces:

```text
Login
 |
 +-- Access Token
 |
 +-- Refresh Token
 |
 +-- Access Token Expiration
 |
 +-- Refresh Token Expiration
 |
 +-- Token Type
```

The access token is intended for API authorization.

The refresh token is intended to obtain a new access token.

---

# 37. Access Token Architecture

The access token is:

```text
Short-lived
     |
     v
JWT
     |
     +-- User Identity
     +-- Claims
     +-- Expiration
     +-- Signature
```

The API validates the JWT before allowing protected operations.

---

# 38. Refresh Token Architecture

The refresh token is:

```text
Longer-lived
      |
      v
Opaque Random Token
      |
      v
Hashed before storage
      |
      v
RefreshTokens table
```

The raw refresh token is returned to the client but is not stored
directly in the database.

---

# 39. Refresh Operation

The refresh flow developed so far is:

```text
Client
  |
  | Refresh Token
  v
AuthController
  |
  v
AuthService
  |
  +-- Hash supplied token
  |
  +-- Find token
  |
  +-- Validate token
  |
  +-- Check expiration
  |
  +-- Check account
  |
  +-- Detect replay
  |
  +-- Generate new access token
  |
  +-- Generate new refresh token
  |
  +-- Revoke old refresh token
  |
  +-- Store replacement
  |
  +-- Commit transaction
  |
  v
New Token Pair
```

---

# 40. Refresh Token Repository

The repository is responsible for operations such as:

```text
Find token
      |
      v
Find active token
      |
      v
Add token
      |
      v
Revoke token
      |
      v
Revoke token family
      |
      v
Revoke all user sessions
      |
      v
Persist changes
```

---

# 41. User Repository

The user repository abstracts database access for users.

Responsibilities include:

```text
Get User
Get User By ID
Get User By Email
Create User
Update User
Delete User
Check Email Exists
```

The service layer uses these operations rather than embedding SQL
queries in controllers.

---

# 42. Repository Pattern

The overall repository architecture is:

```text
                 Service Layer
                       |
          +------------+-------------+
          |                          |
          v                          v
     IUserRepository      IRefreshTokenRepository
          |                          |
          v                          v
     UserRepository       RefreshTokenRepository
          |                          |
          +-------------+------------+
                        |
                        v
                 ApplicationDbContext
                        |
                        v
                    SQL Server
```

---

# 43. DTO Architecture

DTOs separate API contracts from database entities.

```text
Client
  |
  v
Request DTO
  |
  v
Controller
  |
  v
Service
  |
  v
Entity
  |
  v
Repository
  |
  v
Database
```

For responses:

```text
Database
  |
  v
Entity
  |
  v
Service
  |
  v
Response DTO
  |
  v
Controller
  |
  v
Client
```

---

# 44. Why DTOs Are Used

DTOs prevent the API from directly exposing database entities.

Benefits:

```text
Entity
   |
   +-- Database structure

DTO
   |
   +-- API contract
```

This allows the database model and API model to evolve independently.

---

# 45. CRUD Architecture

The User CRUD flow is:

```text
                User CRUD
                   |
       +-----------+-----------+
       |           |           |
       v           v           v
     Create       Read       Update
       |           |           |
       +-----------+-----------+
                   |
                   v
                 Delete
```

---

# 46. Create User

```text
POST
  |
  v
UsersController
  |
  v
UserService
  |
  v
IUserRepository
  |
  v
Users table
```

---

# 47. Get User

```text
GET
  |
  v
UsersController
  |
  v
UserService
  |
  v
IUserRepository
  |
  v
Users table
```

---

# 48. Update User

```text
PUT
  |
  v
UsersController
  |
  v
UserService
  |
  v
IUserRepository
  |
  v
Users table
```

---

# 49. Delete User

```text
DELETE
  |
  v
UsersController
  |
  v
UserService
  |
  v
IUserRepository
  |
  v
Users table
```

Because child tables use cascading foreign keys, deleting a user can
also delete related child records.

Conceptually:

```text
Delete User
    |
    +-- Profile
    +-- Achievements
    +-- Education
    +-- Employments
    +-- Feedback
    +-- Objectives
    +-- Projects
    +-- Skills
    +-- Social
```

---

# 50. Logout Current Device

The logout-current-session flow is:

```text
Client
  |
  v
Logout Request
  |
  v
Auth Controller
  |
  v
Auth Service
  |
  v
Identify Current Refresh Token
  |
  v
Revoke Current Token
  |
  v
Database
```

The current session/token is revoked without intentionally revoking
other token families belonging to the same user.

---

# 51. Logout All Devices

The logout-all-devices flow is:

```text
Client
  |
  v
Logout All Request
  |
  v
Auth Controller
  |
  v
Auth Service
  |
  v
UserId
  |
  v
Revoke All Refresh Tokens
  |
  v
Database
```

Conceptually:

```text
User
 |
 +-- Family F1 → Revoked
 |
 +-- Family F2 → Revoked
 |
 +-- Family F3 → Revoked
```

The user must authenticate again on all devices.

---

# 52. Current Authentication Security Model

The authentication system now follows this model:

```text
                 USER
                  |
                  v
                LOGIN
                  |
        +---------+---------+
        |                   |
        v                   v
  Access Token       Refresh Token
                           |
                           v
                     Token Family
                           |
                           v
                      Rotation
                           |
                           v
                     Old Token
                       Revoked
                           |
                           v
                    New Token
                           |
                           v
                     Active
```

---

# 53. Replay Security Model

```text
                    Refresh Request
                           |
                           v
                    Find Token Hash
                           |
                +----------+----------+
                |                     |
             Not Found              Found
                |                     |
                v                     v
          Invalid Token        Check Revocation
                                      |
                         +------------+------------+
                         |                         |
                       Active                   Revoked
                         |                         |
                         v                         v
                    Normal Refresh          Replay Detected
                                                   |
                                                   v
                                          Revoke Token Family
```

---

# 54. Concurrency Security Model

RT-9 introduced concurrency protection.

```text
                 Token A
                    |
          +---------+---------+
          |                   |
          v                   v
      Request A           Request B
          |                   |
          +---------+---------+
                    |
              Concurrent Use
                    |
                    v
             Database Control
                    |
          +---------+---------+
          |                   |
          v                   v
        Winner              Loser
          |                   |
          v                   v
      Token B          Concurrency/
                       Replay Detection
```

Only one request should successfully consume the same refresh token.

---

# 55. Transaction Architecture

The refresh rotation operation is treated as one database unit:

```text
BEGIN TRANSACTION
       |
       +-- Validate
       |
       +-- Consume old token
       |
       +-- Create replacement token
       |
       +-- Save
       |
       v
COMMIT
```

If an unexpected failure occurs:

```text
ROLLBACK
```

---

# 56. Dependency Injection Architecture

The application uses dependency injection to connect layers.

Conceptually:

```text
Controller
    |
    v
Service Interface
    |
    v
Service Implementation
    |
    v
Repository Interface
    |
    v
Repository Implementation
    |
    v
DbContext
```

For example:

```text
AuthController
      |
      v
IAuthService
      |
      v
AuthService
      |
      +--------------------+
      |                    |
      v                    v
IUserRepository   IRefreshTokenRepository
      |                    |
      v                    v
UserRepository    RefreshTokenRepository
      |                    |
      +----------+---------+
                 |
                 v
        ApplicationDbContext
```

---

# 57. Authentication Components

The authentication subsystem consists of:

```text
Authentication
│
├── AuthController
│
├── IAuthService
│
├── AuthService
│
├── IUserRepository
│
├── IRefreshTokenRepository
│
├── UserRepository
│
├── RefreshTokenRepository
│
├── JWT configuration
│
├── Password hashing
│
├── Refresh Token
│
├── Token Family
│
├── Replay Detection
│
└── Concurrency Protection
```

---

# 58. User Management Components

```text
User Management
│
├── UsersController
│
├── IUserService
│
├── UserService
│
├── IUserRepository
│
├── UserRepository
│
├── User Entity
│
├── User Create DTO
│
├── User Update DTO
│
└── User Response DTO
```

---

# 59. API Request Flow

General API flow:

```text
HTTP Request
     |
     v
Controller
     |
     v
DTO Validation
     |
     v
Service
     |
     v
Business Rules
     |
     v
Repository
     |
     v
EF Core
     |
     v
SQL Server
     |
     v
Database Result
     |
     v
Repository
     |
     v
Service
     |
     v
Response DTO
     |
     v
Controller
     |
     v
HTTP Response
```

---

# 60. Authentication Request Flow

```text
HTTP Request
     |
     v
AuthController
     |
     v
IAuthService
     |
     v
AuthService
     |
     +---- IUserRepository
     |
     +---- IRefreshTokenRepository
     |
     +---- JWT / Security Services
     |
     v
SQL Server
     |
     v
Authentication Result
     |
     v
HTTP Response
```

---

# 61. Error Handling Concept

The API distinguishes different types of failures.

```text
Validation Error
      |
      v
400 Bad Request

Authentication Failure
      |
      v
401 Unauthorized

Authorization Failure
      |
      v
403 Forbidden

Resource Not Found
      |
      v
404 Not Found

Conflict
      |
      v
409 Conflict

Unexpected Server Error
      |
      v
500 Internal Server Error
```

Authentication-specific service results are mapped by the controller
to the appropriate HTTP response.

---

# 62. Security Principles Implemented So Far

The current architecture follows these principles:

```text
1. Never store plain-text passwords
2. Store password hashes
3. Never store raw refresh tokens
4. Store refresh-token hashes
5. Rotate refresh tokens
6. Revoke old refresh tokens
7. Detect refresh-token replay
8. Revoke compromised token families
9. Separate device/session families
10. Protect concurrent token consumption
11. Use database transactions
12. Avoid logging raw secrets
13. Keep authentication logic out of controllers
14. Keep database logic out of controllers
15. Use DTOs for API contracts
```

---

# 63. Complete Authentication Architecture

```text
                         CLIENT
                           |
                           v
                   +---------------+
                   | AuthController |
                   +---------------+
                           |
                           v
                    +------------+
                    | AuthService|
                    +------------+
                           |
             +-------------+-------------+
             |             |             |
             v             v             v
        User Repo    Refresh Repo    JWT/Security
             |             |             |
             +-------------+-------------+
                           |
                           v
                  ApplicationDbContext
                           |
                           v
                       SQL Server
                           |
             +-------------+-------------+
             |                           |
             v                           v
          Users                    RefreshTokens
             |                           |
             |                           |
             |                     TokenFamilyId
             |                           |
             |                     TokenHash
             |                           |
             |                     RevokedAt
             |                           |
             |                     RowVersion
             |                           |
             +-------------+-------------+
                           |
                           v
                   Authentication
```

---

# 64. Complete Database Relationship Overview

```text
                                  Users
                                    |
        +-------------+-------------+-------------+-------------+
        |             |             |             |             |
        v             v             v             v             v
     Profile    Achievements    Education    Employments    Feedback
      1:1           1:N           1:N           1:N            1:N
        |
        |
        +-------------------+
        |                   |
        v                   v
   Objectives            Projects
      1:1                  1:N

        |
        +-------------------+
        |                   |
        v                   v
      Skills              Social
       1:N                  1:1


                                  Users
                                    |
                                    |
                                    v
                             RefreshTokens
                                    |
                     +--------------+--------------+
                     |              |              |
                     v              v              v
                TokenHash      TokenFamilyId   RowVersion
                     |
                     v
              Token Rotation
                     |
                     v
              Replay Detection
                     |
                     v
             Family Revocation
```

---


# 65. Current Architecture Snapshot


```text
.NET Web API
│
├── API Layer
│   └── Controllers
│
├── Service Layer
│   ├── User Service
│   └── Authentication Service
│
├── Repository Layer
│   ├── User Repository
│   └── Refresh Token Repository
│
├── DTO Layer
│   ├── User DTOs
│   └── Authentication DTOs
│
├── Entity / Model Layer
│   ├── User
│   ├── Profile
│   ├── Achievement
│   ├── Education
│   ├── Employment
│   ├── Feedback
│   ├── Objective
│   ├── Project
│   ├── Skill
│   ├── Social
│   └── RefreshToken
│
├── Data Layer
│   └── ApplicationDbContext
│
├── Security
│   ├── Password Hashing
│   ├── JWT
│   ├── Refresh Token
│   ├── Token Rotation
│   ├── Token Family
│   ├── Replay Detection
│   ├── Session Revocation
│   └── Concurrency Protection
│
└── SQL Server
    │
    ├── Users
    ├── Profile
    ├── Achievements
    ├── Education
    ├── Employments
    ├── Feedback
    ├── Objectives
    ├── Projects
    ├── Skills
    ├── Social
    └── RefreshTokens
```

---

# 66. Final Architecture Principle

The most important separation in the application is:

```text
Controller
    |
    | "What request came in?"
    v
Service
    |
    | "What should the application do?"
    v
Repository
    |
    | "How do I access the data?"
    v
Database
    |
    | "Where is the data stored?"
    v
SQL Server
```

And for authentication:

```text
Controller
    |
    v
Authentication Service
    |
    +-- User Repository
    |
    +-- Refresh Token Repository
    |
    +-- Token / Security Services
    |
    v
Database
```

This keeps the application maintainable, testable, and easier to extend.

---

```

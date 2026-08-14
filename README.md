# ResumeBuilder.API
Authentication Architecture

                  AuthController
                       │
                       ▼
                  IAuthService
                       │
                       ▼
                  AuthService
                  /    |    \
                 /     |     \
                ▼      ▼      ▼
             User    JWT    Refresh
           Repository Service Repository
                \      |      /
                 \     |     /
                    SQL Server

  REGISTER
   │
   ▼
  Users

LOGIN
   │
   ├── Access Token
   │
   └── Refresh Token
           │
           ▼
      RefreshTokens


REFRESH
   │
   ├── Revoke Old Token
   │
   └── Create New Token


LOGOUT
   │
   ▼
Revoke Refresh Token

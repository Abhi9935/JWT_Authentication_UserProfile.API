# ResumeBuilder.API
## .NET Web API — Complete Architecture & Project Structure

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

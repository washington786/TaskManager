# 🚀 TaskManager API

A simple, secure, and clean ASP.NET Web API for managing tasks with **JWT authentication**, **role-based authorization**, **ASP.NET Identity**, **Entity Framework Core**, and **Swagger UI**.

Perfect for learning or as a starting point for your own projects.

---

## Features

- **User Authentication**
  - Register, Login, Password Reset
  - JWT Token Generation
- **Role-Based Authorization**
  - `Admin`: Full access
  - `TaskManager`: Manage all tasks
  - `User`: Manage own tasks only
- **Task CRUD Operations**
  - Create, Read, Update, Delete tasks
- **Security**
  - Secure password hashing via ASP.NET Identity
  - JWT with issuer, audience, and signing key validation
- **Developer Friendly**
  - Swagger UI with JWT support
  - Clean folder structure
  - Entity Framework Migrations
  - CORS configured

---

## Technologies Used

| Technology | Purpose |
|----------|-------|
| **ASP.NET Core Web API** | REST backend |
| **Entity Framework Core** | ORM & Database |
| **ASP.NET Identity** | User & Role Management |
| **JWT (HMAC-SHA256)** | Stateless Authentication |
| **Swagger (Swashbuckle)** | API Documentation |
| **SQL Server (LocalDB)** | Database |
| **.NET 8** | Runtime & SDK |

---

## Folder Structure

TaskManager/
├── Controllers/
│ ├── AuthController.cs
│ └── TasksController.cs
├── Data/
│ └── ApplicationDbContext.cs
├── Models/
│ ├── AppUser.cs
│ └── TaskItem.cs
├── DTOs/
│ ├── auth/
│ └── task/
├── Services/
│ └── TokenService.cs
├── Utils/
│ └── Helper.cs
├── appsettings.json
├── Program.cs
└── TaskManager.csproj

---

## Getting Started

### 1. Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or SQL Express)
- IDE (VS Code, Visual Studio, or Rider)

### 2. Clone the Repository

```bash
git clone https://github.com/washington786/TaskManager.git
cd TaskManager

##  Configure appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagerDb;Trusted_Connection=true;"
  },
  "Jwt": {
    "Key": "task-api-secret-api-key-has-all-secrecies-of-sort",
    "Issuer": "task-api",
    "Audience": "task-api"
  }
}


### Run Database Migrations
dotnet ef database update


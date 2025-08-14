# Avyyan Knitfab Backend API

A comprehensive ASP.NET Core 8.0 Web API for managing a textile/knitfab business with PostgreSQL database support.

## 🚀 Features

- **Entity Framework Core** with PostgreSQL
- **Repository Pattern** with Unit of Work
- **AutoMapper** for object mapping
- **JWT Authentication** ready
- **Swagger/OpenAPI** documentation
- **FluentValidation** for input validation
- **CORS** configuration
- **Serilog** structured logging with file and console sinks
- **Exception handling middleware**
- **Request/Response logging**

## 📁 Project Structure

```
AvyyanBackend/
├── Controllers/           # API Controllers
├── Models/               # Entity Models
├── DTOs/                 # Data Transfer Objects
├── Data/                 # Database Context
├── Services/             # Business Logic Services
├── Repositories/         # Data Access Layer
├── Interfaces/           # Service Contracts
├── Extensions/           # Service Extensions & Mapping Profiles
├── Middleware/           # Custom Middleware (future)
└── Properties/           # Launch Settings
```

## 🗄️ Database Models

### Core Entities
- **Product** - Main product catalog
- **Category** - Product categorization (hierarchical)
- **Customer** - Customer information
- **Address** - Customer addresses
- **Order** - Customer orders
- **OrderItem** - Order line items
- **Supplier** - Supplier information
- **PurchaseOrder** - Purchase orders from suppliers
- **PurchaseOrderItem** - Purchase order line items
- **InventoryTransaction** - Stock movement tracking
- **ProductImage** - Product images

## 🔧 Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 12+ 
- Visual Studio 2022 or VS Code

### Database Setup
1. Install PostgreSQL and create a database:
   ```sql
   CREATE DATABASE AvyyanKnitfab;
   CREATE DATABASE AvyyanKnitfab_Dev; -- for development
   ```

2. Update connection strings in `appsettings.json` and `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=AvyyanKnitfab;Username=postgres;Password=your_password_here;Port=5432"
     }
   }
   ```

### Running the Application
1. Restore packages:
   ```bash
   dotnet restore
   ```

2. Create and run database migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Access Swagger UI at: `https://localhost:7009` or `http://localhost:5133`

## 📦 Installed Packages

- **Microsoft.EntityFrameworkCore** (9.0.8) - ORM framework
- **Npgsql.EntityFrameworkCore.PostgreSQL** (9.0.4) - PostgreSQL provider
- **Microsoft.EntityFrameworkCore.Tools** (9.0.8) - EF Core CLI tools
- **Microsoft.AspNetCore.Authentication.JwtBearer** (8.0.8) - JWT authentication
- **AutoMapper.Extensions.Microsoft.DependencyInjection** (12.0.1) - Object mapping
- **FluentValidation.AspNetCore** (11.3.1) - Input validation
- **Serilog.AspNetCore** (9.0.0) - Structured logging framework
- **Serilog.Sinks.File** (7.0.0) - File logging sink
- **Serilog.Sinks.Console** (6.0.0) - Console logging sink
- **Serilog.Enrichers.Environment** (3.0.1) - Environment enrichers
- **Serilog.Enrichers.Process** (3.0.0) - Process enrichers
- **Serilog.Enrichers.Thread** (4.0.0) - Thread enrichers
- **Swashbuckle.AspNetCore** (6.6.2) - Swagger/OpenAPI

## 📊 Logging Configuration

### Serilog Features
- **Structured Logging** - JSON formatted logs for production
- **Multiple Sinks** - Console and file outputs
- **Log Rotation** - Daily rotation with size limits
- **Request Logging** - Automatic HTTP request/response logging
- **Exception Logging** - Comprehensive error tracking
- **Environment Enrichment** - Machine, process, and thread information

### Log Files
- **General Logs**: `logs/avyyan-knitfab-YYYYMMDD.log` (JSON format)
- **Error Logs**: `logs/avyyan-knitfab-errors-YYYYMMDD.log` (Text format)
- **Development Logs**: `logs/dev/avyyan-knitfab-dev-YYYYMMDD.log`

### Log Levels
- **Production**: Information and above
- **Development**: Debug and above
- **Microsoft**: Warning and above (filtered)
- **Entity Framework**: Information for database commands

## 🔐 Security Configuration

### JWT Settings
Update the JWT settings in `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-here-make-it-at-least-32-characters-long",
    "Issuer": "AvyyanKnitfab",
    "Audience": "AvyyanKnitfab-Users",
    "ExpirationInMinutes": 60
  }
}
```

## 🌐 API Endpoints

### Products API
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/sku/{sku}` - Get product by SKU
- `GET /api/products/category/{categoryId}` - Get products by category
- `GET /api/products/search?searchTerm={term}` - Search products
- `GET /api/products/featured` - Get featured products
- `GET /api/products/low-stock` - Get low stock products
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product (soft delete)
- `PATCH /api/products/{id}/stock` - Update product stock

## 🏗️ Architecture Patterns

### Repository Pattern
- Generic repository for common CRUD operations
- Specific repositories for complex queries
- Unit of Work for transaction management

### Service Layer
- Business logic separation
- DTO mapping
- Validation handling

### Dependency Injection
- All services registered in `ServiceExtensions.cs`
- Scoped lifetime for database-related services

## 🔄 Next Steps

1. **Authentication & Authorization**
   - Implement user registration/login
   - Add role-based authorization
   - Create user management endpoints

2. **Additional Controllers**
   - Categories controller
   - Customers controller
   - Orders controller
   - Suppliers controller

3. **Advanced Features**
   - File upload for product images
   - Email notifications
   - Reporting endpoints
   - Inventory management
   - Payment integration

4. **Testing**
   - Unit tests for services
   - Integration tests for controllers
   - Database testing with in-memory provider

## 🐛 Development Notes

- The application uses soft deletes (IsActive flag)
- All entities inherit from BaseEntity for common properties
- Database relationships are properly configured with appropriate delete behaviors
- CORS is configured for frontend integration
- Swagger UI is available at the root URL in development mode

## 📝 Environment Variables

For production, consider using environment variables for sensitive data:
- `ConnectionStrings__DefaultConnection`
- `JwtSettings__SecretKey`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`

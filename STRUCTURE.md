# Project Structure & Layer Description
# Cấu trúc dự án
## 1. Domain Layer
**Vị trí:** `Domain/`

- **Chức năng:**  
  Chứa các entity, model, và các logic nghiệp vụ thuần túy (business logic, validation, rule).
- **Thành phần chính:**
  - `Models/`
    - `Product.cs`, `User.cs`: Định nghĩa các entity chính của hệ thống.
  - `Base/BaseEntity.cs`:  
    - Lớp cơ sở cho các entity, thường có các trường như Id, CreatedDate, v.v.
  - `Abstractions/`:  
    - (Có thể chứa các interface nghiệp vụ hoặc value object nếu cần.)

---

## 2. Application Layer
**Vị trí:** `Application/`

- **Chức năng:**  
  Chứa các interface cho repository, unit of work, service, và các service xử lý logic ứng dụng (application logic).
- **Thành phần chính:**
  - `Interfaces/Interfaces/`
    - `IRepositoryBase<T>`:  
      - Interface trừu tượng cho các thao tác CRUD (Add, Update, Delete, Get, ...).
    - `IUnitOfWork`:  
      - Interface quản lý transaction, repository động, SaveChanges, Begin/Commit/Rollback transaction.
    - `IRepositoryStrategy<T>`:  
      - Interface cho pattern chọn repository động theo provider (EF, Dapper, Mongo).
  - `Services/`
    - `ProductService.cs`:  
      - Service xử lý logic liên quan đến Product, sử dụng UnitOfWork hoặc Strategy để thao tác với repository.
    - `Base/ServiceBase.cs`:  
      - (Nếu còn dùng) Lớp base cho các service, cung cấp các tiện ích chung như SaveChanges, lấy repository, v.v.

---

## 3. Infrastructure Layer
**Vị trí:** `Infrastructure/`

- **Chức năng:**  
  Chứa các class triển khai (implementation) cho các interface ở Application Layer, kết nối với các hệ quản trị CSDL (EF, Dapper, Mongo), đăng ký DI, migration, v.v.
- **Thành phần chính:**
  - `Persistence/Repositories/`
    - `EFRepository<T>`:  
      - Triển khai repository cho Entity Framework.
    - `DapperRepository<T>`:  
      - Triển khai repository cho Dapper/Npgsql.
    - `MongoRepository<T>`:  
      - Triển khai repository cho MongoDB.
    - `UnitOfWork.cs`:  
      - Triển khai UnitOfWork cho EF, quản lý transaction, repository động.
    - `RepositoryStrategy.cs`:  
      - Triển khai Strategy Pattern để chọn repository động theo provider.
    - `RepositoryBase.cs`:  
      - (Nếu còn dùng) Base repository cho EF.
  - `ApplicationDbContext.cs`:  
    - DbContext cho Entity Framework, quản lý DbSet các entity.
  - `ServiceRegistration.cs`:  
    - Đăng ký các service, repository, UnitOfWork, Strategy vào Dependency Injection.
  - `Migrations/`:  
    - Chứa các file migration cho EF Core.

---

## 4. API Layer (Presentation)
**Vị trí:** `API/FirstWorkWithPattern/`

- **Chức năng:**  
  Chứa các controller, cấu hình web, view, static file, cấu hình appsettings, v.v.
- **Thành phần chính:**
  - `Controllers/`
    - `ProductController.cs`:  
      - Controller cho các API thao tác với Product.
  - `Pages/`, `wwwroot/`:  
    - View, static file (nếu dùng Razor Page hoặc MVC).
  - `appsettings.json`:  
    - Cấu hình connection string, provider, logging, v.v.
  - `Program.cs`, `Startup.cs` (nếu có):  
    - Khởi tạo ứng dụng, cấu hình DI, middleware, routing, v.v.

---

# Sơ đồ luồng phụ thuộc
```
API/Controller
    ↓
Application/Service
    ↓
Application/Interfaces (IRepositoryBase, IUnitOfWork, IRepositoryStrategy)
    ↓
Infrastructure/Repository, UnitOfWork, Strategy (triển khai)
    ↓
Database (SQL, Postgre, Mongo)
```

---

# Tóm tắt vai trò các interface/class chính

- **IRepositoryBase<T>**: Chuẩn hóa thao tác CRUD cho mọi entity/provider.
- **IUnitOfWork**: Quản lý transaction, repository động, SaveChanges, Begin/Commit/Rollback.
- **IRepositoryStrategy<T>**: Chọn repository động theo provider (EF, Dapper, Mongo).
- **EFRepository<T>, DapperRepository<T>, MongoRepository<T>**: Triển khai repository cho từng provider.
- **UnitOfWork**: Quản lý transaction, repository động cho EF.
- **RepositoryStrategy**: Chọn repository động theo cấu hình provider.
- **ProductService**: Xử lý logic nghiệp vụ cho Product, thao tác qua UnitOfWork hoặc Strategy.
- **ProductController**: API endpoint cho Product. 
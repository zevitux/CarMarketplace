# 🚗 CarMarketplace API  

**CarMarketplace** is a secure vehicle trading platform API built with **.NET 9** and **ASP.NET Core Web API**. It supports user authentication (JWT), role-based access control, and full CRUD operations for car listings and reviews.  

---

## ✨ Features  
- 🔐 **Secure Authentication**: JWT tokens with refresh token rotation  
- 🚗 **Car Management**: Create, read, update, and delete vehicle listings  
- ⭐ **Review System**: Rate and comment on cars  
- 👥 **Role-Based Access**:  
  - **Admin**: Full system control  
  - **Seller**: Manage own car listings  
  - **Buyer**: Browse and review vehicles  
- ♻️ **Token Refresh**: Secure token renewal mechanism  
- 📊 **Data Integrity**: SQL Server with Entity Framework Core  

---

## 🛠️ Technologies Used  
- 🎯 .NET 9  
- 🌐 ASP.NET Core Web API  
- 🔄 Entity Framework Core  
- 🗃️ SQL Server 
- 🔑 JWT Authentication  
- 📜 OpenAPI (Swagger/Scalar)  

---

## 🚀 Getting Started  

### 📋 Prerequisites  
- [.NET 9 SDK](https://dotnet.microsoft.com/download)  
- SQL Server (or Docker for containerized DB)  
- IDE: JetBrains Rider / VS Code / Visual Studio   

---

## ⚙️ Setup  

1. **Clone the repository**  
   ```bash  
   git clone https://github.com/yourusername/CarMarketplace.git  
   cd CarMarketplace
2. **Configure databse**
   ```json
   "ConnectionStrings": {  
    "DefaultConnection": "Server=YOUR_SERVER;Database=CarMarketplaceDB;Trusted_Connection=True;TrustServerCertificate=True"  
    }
3. **Apply migrations**
   ```bash
   dotnet ef database update  
4. **Set JWT Secret key**
   ```json
   "AppSettings": {  
    "Token": "your-32-character-secure-key",  
    "Issuer": "CarMarketplace",  
    "Audience": "CarMarketplaceUsers"  
    }  
5. **Run the API 🚀**
   ```bash
   dotnet run

---

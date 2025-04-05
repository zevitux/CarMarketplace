🚗 CarMarketplace API
CarMarketplace is a secure vehicle trading platform API built with .NET 9 and ASP.NET Core Web API. It supports user authentication (JWT), role-based access control, and full CRUD operations for car listings and reviews.

✨ Features
🔐 Secure Authentication: JWT tokens with refresh token rotation

🚗 Car Management: Create, read, update, and delete vehicle listings

⭐ Review System: Rate and comment on cars

👥 Role-Based Access:

Admin: Full system control

Seller: Manage own car listings

Buyer: Browse and review vehicles

♻️ Token Refresh: Secure token renewal mechanism

📊 Data Integrity: SQL Server with Entity Framework Core

🛠️ Technologies Used
🎯 .NET 9

🌐 ASP.NET Core Web API

🔄 Entity Framework Core

🗃️ SQL Server (with 🐳 Docker support)

🔑 JWT Authentication

📜 OpenAPI (Swagger/Scalar)

🚀 Getting Started
📋 Prerequisites
.NET 9 SDK

SQL Server (or Docker for containerized DB)

IDE: JetBrains Rider / VS Code / Visual Studio

Postman (API testing)

⚙️ Setup
Clone the repository

bash
Copy
git clone https://github.com/yourusername/CarMarketplace.git  
cd CarMarketplace  
Configure database
Update appsettings.json:

json
Copy
"ConnectionStrings": {  
  "DefaultConnection": "Server=YOUR_SERVER;Database=CarMarketplaceDB;Trusted_Connection=True;TrustServerCertificate=True"  
}  
Apply migrations

bash
Copy
dotnet ef database update  
Set JWT secret key
Add to appsettings.json:

json
Copy
"AppSettings": {  
  "Token": "your-32-character-secure-key",  
  "Issuer": "CarMarketplace",  
  "Audience": "CarMarketplaceUsers"  
}  
Run the API 🚀

bash
Copy
dotnet run  
Access docs at: https://localhost:7275/scalar

📡 API Endpoints
🔑 Authentication
📤 POST /api/auth/register - Register new user

📤 POST /api/auth/login - Get JWT tokens

📤 POST /api/auth/refresh-token - Renew tokens

🚗 Car Endpoints
📥 GET /api/cars - List all vehicles

📤 POST /api/cars - Add new car (Seller/Admin only)

🔄 PUT /api/cars/{id} - Update listing (Owner/Admin)

⭐ Review Endpoints
📤 POST /api/reviews - Add review to car

🗑️ DELETE /api/reviews/{id} - Remove review (Owner/Admin)

🔒 Protected Example
📥 GET /api/auth/admin-dashboard - Admin-only analytics

💡 First-Time Use
Register an admin:

json
Copy
{  
  "name": "Admin",  
  "email": "admin@example.com",  
  "password": "SecurePass123!",  
  "role": "Admin"  
}  
Use the 🔑 login endpoint to get tokens

Add cars 🚗 and start trading!

🤝 Contributing
Fork the repository

Create a feature branch:

bash
Copy
git checkout -b feature/your-feature  
Submit a pull request


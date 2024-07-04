## Previous requirements

- **Lenguaje/Framework:** .NET Core 7.0
- **Base de Datos:** SQL Server 2016 o superior
- **Otros:** Node.js (si se necesita ejecutar un servidor local para pruebas de frontend)
- **Paquetes NuGet:**
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer
  - MediatR
  - FluentValidation
  - Swashbuckle.AspNetCore

## Settings
modify the database connection string for the 2 databases, and keep the name of the databases :
example :

  "ConnectionStrings": {
    "DefaultConnection": "Data Source=yourServer;Persist Security Info=False;Initial Catalog=ApplicationDb;User ID=yourId;Password=yourPassword;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
    "ProductConnection": "Data Source=yourServer;Persist Security Info=False;Initial Catalog=ProductDb_{tenant};User ID=yourId;Password=yourPassword;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
  },

  
- By default the ApplicationDB database will be created with the following values ​​configured with EF :

ApplicationDbContext.cs
   modelBuilder.Entity<Organization>().HasData(new Organization { Id = 1, Name = "Organization1", SlugTenant = "default_tenant" });

- Finally it will be necessary to execute the migrations :


- dotnet ef database update --context ApplicationDbContext -p src/MyMultitenantApp.Persistence -s src/MyMultitenantApp.Api
- dotnet ef database update --context ProductDbContext -p src/MyMultitenantApp.Persistence -s src/MyMultitenantApp.Api

- The steps to use the API, first is to create a user and log in with that user and with the token we can authorize from Swagger, later you can create organizations, and then you can create products in the tenant that you created.

follow swagger documentation.

# Huong dan cai dat moi truong, Database First va deploy .NET API len Render

Tai lieu nay duoc viet theo dung flow cua du an hien tai: ASP.NET Core Web API, Entity Framework Core Database First, PostgreSQL va deploy len Render bang Dockerfile.

## 1. Muc tieu

Sau khi lam xong, ban se biet cach:

- Cai moi truong de chay du an .NET Web API
- Tao project API moi
- Ket noi PostgreSQL theo Database First
- Sinh model va `DbContext` tu database co san
- Chay thu local
- Deploy len Render
- Push source len GitHub
- Tim domain public va mo Swagger

## 2. Cong nghe dung trong mau nay

- .NET 8 SDK
- ASP.NET Core Web API
- Entity Framework Core 8
- Npgsql cho PostgreSQL
- Render Web Service
- Dockerfile de chay .NET tren Render

## 3. Cai dat moi truong

### 3.1. Cai .NET SDK 8

Tai ve va cai .NET 8 SDK:

- Trang chu: `https://dotnet.microsoft.com/en-us/download/dotnet/8.0`

Kiem tra sau khi cai:

```powershell
dotnet --version
```

Neu hien phien ban `8.x.x` la dat.

### 3.2. Cai Visual Studio Code hoac Visual Studio

Neu dung VS Code, nen cai them:

- C# Dev Kit
- C#

### 3.3. Cai PostgreSQL

Co the dung:

- PostgreSQL cai tren may
- Neon
- Supabase
- Render PostgreSQL

Ban can co:

- `Host`
- `Port`
- `Database`
- `Username`
- `Password`

### 3.4. Tao tai khoan Render

Ban can co:

- tai khoan Render
- tai khoan GitHub
- mot repo GitHub de Render lay source deploy

### 3.5 Thiet lap chatbox

```powershell
dotnet add package Mscc.GenerativeAI
```

## 4. Tao project ASP.NET Core Web API moi

```powershell
dotnet new webapi -n MyApiService
cd MyApiService
```

## 5. Cai package cho Database First

Voi PostgreSQL:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
dotnet add package Mscc.GenerativeAI
```

## 6. Cau hinh connection string

Them vao `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_HOST;Port=5432;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Swagger": {
    "Enabled": true
  }
}
```

Luu y:

- Khong nen de password that trong source khi dua len production
- Tot nhat la dua sang bien moi truong sau khi deploy

## 7. Database First voi Entity Framework Core

Neu database da co san bang va du lieu, dung lenh scaffold:

```powershell
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=PersonalBlogDB;Username=postgres;Password=Quoc@123" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c AppDbContext --force
```

Y nghia:

- `-o Models`: sinh model vao thu muc `Models`
- `-c AppDbContext`: dat ten `DbContext`
- `--force`: ghi de file cu

Neu chi muon scaffold mot so bang:

```powershell
dotnet ef dbcontext scaffold "YOUR_CONNECTION_STRING" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c AppDbContext --table users --table roles --force
```

## 8. Dang ky DbContext trong Program.cs

Vi du:

```csharp
using Microsoft.EntityFrameworkCore;
using MyApiService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var enableSwagger = app.Environment.IsDevelopment() ||
    app.Configuration.GetValue<bool>("Swagger:Enabled");

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
```

## 9. Tao controller de test

Vi du controller doc du lieu users:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiService.Models;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }
}
```
    // "DefaultConnection": "Host=ep-bold-forest-azj67ls7-pooler.c-3.ap-southeast-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_VZSns3NxLJ5Y;SSL Mode=Require;Channel Binding=Require"

## 10. Chay local

```powershell
dotnet run
```

Neu project bat Swagger, mo:

```text
http://localhost:xxxx/swagger
```

Hoac neu co HTTPS:

```text
https://localhost:xxxx/swagger
```

## 11. Luu y khi dung Database First

- Moi khi bang trong database thay doi, can scaffold lai model
- Nen scaffold vao mot folder rieng nhu `Models`
- Neu da co code custom trong model, can backup truoc khi scaffold lai voi `--force`
- Thuong nen tach:
  - model scaffold tu DB
  - DTO de tra API
  - service/business logic

## 12. Chuan bi source truoc khi deploy len Render

### 12.1. Tao `.gitignore`

Vi source nay co file local va build artifact, nen tao `.gitignore` truoc khi push:

```gitignore
appsettings.json
appsettings.Development.json
bin/
obj/
publish/
.vs/
.vscode/
*.user
*.suo
*.cache
*.log
.agents/
.codexbuild/
codex-write-test.txt
CheckInProApiService.http
```

### 12.2. Tao `Dockerfile`

Tinh den ngay `July 20, 2026`, Render khong co runtime native `.NET` trong dropdown `Language`, vi vay can deploy bang `Docker`.

Tao file `Dockerfile` trong root project:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CheckInProApiService.csproj ./
RUN dotnet restore CheckInProApiService.csproj

COPY . ./
RUN dotnet publish CheckInProApiService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production

CMD ["sh", "-c", "dotnet CheckInProApiService.dll --urls http://0.0.0.0:${PORT:-10000}"]
```

### 12.3. Tao `.dockerignore`

```gitignore
bin/
obj/
publish/
.git/
.vs/
.vscode/
.agents/
.codexbuild/
codex-write-test.txt
CheckInProApiService.csproj.user
```

## 13. Push source len GitHub

### 13.1. Tao repo GitHub

1. Vao GitHub
2. Chon `New repository`
3. Dat ten repo
4. Chon `Public` hoac `Private`
5. Khong can tick `README`, `.gitignore`, `license`
6. Bam `Create repository`

### 13.2. Push source tu may len GitHub

Trong thu muc project:

```powershell
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/<username>/<repo>.git
git branch -M main
git push -u origin main
```

Neu ban dung branch khac `main`, co the giu branch do va deploy branch do tren Render.

## 14. Deploy bang Render

### 14.1. Ket noi GitHub voi Render

1. Dang nhap Render
2. Chon `New +`
3. Chon `Web Service`
4. Chon `Git Provider`
5. Ket noi GitHub
6. Neu co nhieu account, vao `Configure in GitHub`
7. Cap quyen cho dung account GitHub chua repo
8. Chon dung repo vua push

### 14.2. Chon source repo

Neu Render hien repo cua account khac, khong nen chon nham.

Can dam bao:

- repo thuoc dung account GitHub cua ban
- Render da duoc cap quyen truy cap repo do

### 14.3. Cau hinh service tren Render

Trong man hinh tao service:

- `Name`: `CheckInProApiService` hoac ten ban muon
- `Language`: `Docker`
- `Branch`: branch ban vua push, vi du `main` hoac `production`
- `Region`: chon khu vuc phu hop
- `Root Directory`: de trong
- `Dockerfile Path`: `Dockerfile`

Khong can `Build Command` va `Start Command` khi da dung `Dockerfile`.

## 15. Bien moi truong va connection string tren Render

Tot hon cho production.

Vao Render:

- `Dashboard`
- `Service`
- `Environment`

Them bien:

```text
ConnectionStrings__DefaultConnection=Host=...;Port=5432;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true
Jwt__Key=YOUR_JWT_SECRET_KEY
Jwt__Issuer=MyAuthServer
Jwt__Audience=CheckinProApp
Gemini__ApiKey=YOUR_GEMINI_API_KEY
Swagger__Enabled=true
```

Luu y:

- Trong .NET, dau `:` trong config duoc doi thanh `__`
- `ConnectionStrings__DefaultConnection` se override gia tri trong `appsettings.json`
- `Jwt__Key` lay tu `Jwt:Key`
- `Jwt__Issuer` lay tu `Jwt:Issuer`
- `Jwt__Audience` lay tu `Jwt:Audience`
- `Gemini__ApiKey` lay tu `Gemini:ApiKey`
- `Swagger__Enabled=true` de mo Swagger tren domain production

Khuyen nghi:

- local de trong file local
- production dung `Environment Variables` cua Render
- khong commit secret that len git
- neu secret da tung len git, nen doi lai secret do

## 16. Bam deploy lan dau

Sau khi dien xong:

1. Kiem tra lai `Dockerfile Path`
2. Kiem tra lai branch
3. Kiem tra lai cac `Environment Variables`
4. Bam `Deploy Web Service`

Render se:

- clone repo tu GitHub
- build Docker image tu `Dockerfile`
- chay container va cap domain public

## 17. Tim link public cua API sau khi deploy

Sau khi deploy thanh cong:

1. Vao Render project
2. Chon service
3. Mo domain duoc Render cap

Ban se nhan duoc domain kieu:

```text
https://your-service-name.onrender.com
```

## 18. Mo Swagger tren Render

Neu da bat Swagger, mo:

```text
https://your-service-name.onrender.com/swagger
```

Neu can:

```text
https://your-service-name.onrender.com/swagger/index.html
```

## 19. Moi lan cap nhat source thi deploy lai nhu the nao

Sau lan dau, nhung lan deploy sau rat don gian:

1. Sua code tren may
2. Chay:

```powershell
git add .
git commit -m "mo ta thay doi"
git push
```

3. Render se tu dong detect commit moi
4. Render tu build lai image va deploy lai

Neu muon deploy lai ma khong can commit moi:

1. Vao service tren Render
2. Chon `Manual Deploy`
3. Chon `Deploy latest commit`

Neu chi thay doi secret:

1. Vao `Environment`
2. Sua bien moi truong
3. Save
4. Deploy lai hoac restart service neu can

## 20. Cau hinh Program.cs cho Render

Khi deploy sau reverse proxy nhu Render, nen them `ForwardedHeaders`:

```csharp
using Microsoft.AspNetCore.HttpOverrides;

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
```

Neu dang deploy cloud, han che `UseHttpsRedirection()` trong production de tranh loop redirect:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
```

## 21. Cac loi thuong gap va cach xu ly

### Loi 1. Render khong co `.NET` trong dropdown `Language`

Nguyen nhan:

- Render hien tai khong ho tro `.NET` native trong giao dien tao Web Service

Cach xu ly:

- chon `Docker`
- tao `Dockerfile`
- deploy bang Docker image build tu repo

### Loi 2. Build fail tren Render

Nguyen nhan:

- chua push `Dockerfile`
- `Dockerfile Path` sai
- project file name sai

Cach xu ly:

- dam bao repo co `Dockerfile`
- dam bao `Dockerfile Path` la `Dockerfile`
- dam bao trong `Dockerfile` dung ten `CheckInProApiService.csproj`

### Loi 3. App chay nhung khong vao duoc database

Nguyen nhan:

- sai connection string
- database khong mo ket noi public
- SSL cua PostgreSQL chua dung

Cach xu ly:

- kiem tra lai `ConnectionStrings__DefaultConnection`
- kiem tra host, port, user, password
- thu them `SSL Mode=Require;Trust Server Certificate=true`

### Loi 4. Khong vao duoc Swagger

Kiem tra:

- da them `Swagger__Enabled=true` tren Render chua
- route co phai `/swagger` khong
- app da deploy thanh cong chua

### Loi 5. App chay nhung frontend goi bi CORS

Kiem tra:

- domain frontend da duoc them vao `WithOrigins(...)` chua
- browser console bao loi CORS hay auth

## 22. Quy trinh mau de ap dung cho du an khac

Moi lan lam du an moi, ban co the di theo checklist sau:

1. Cai .NET SDK
2. Tao project Web API
3. Cai package EF Core + provider DB
4. Them connection string
5. Scaffold Database First
6. Dang ky `DbContext`
7. Tao controller test
8. Chay local va test Swagger
9. Tao `.gitignore`
10. Tao `Dockerfile`
11. Tao `.dockerignore`
12. Push source len GitHub
13. Tao Web Service tren Render
14. Chon `Language = Docker`
15. Them environment variables
16. Deploy
17. Test `/swagger`

## 23. Lenh mau tong hop

### Tao project

```powershell
dotnet new webapi -n MyApiService
cd MyApiService
```

### Cai package

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
```

### Scaffold DB First

```powershell
dotnet ef dbcontext scaffold "Host=YOUR_HOST;Port=5432;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c AppDbContext --force
```
```powershell
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=checkinprodb;Username=postgres;Password=Quoc@123" Npgsql.EntityFrameworkCore.PostgreSQL -o Models
```
Scaffold again when add new column or table on databse first
```powershell
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=checkinprodb;Username=postgres;Password=Quoc@123" Npgsql.EntityFrameworkCore.PostgreSQL -o Models --force --no-onconfiguring
```

### Chay local

```powershell
dotnet run

dotnet run --launch-profile <ten-profile>
```

### Deploy Render

```powershell
git add .
git commit -m "update for render deploy"
git push
```

## 24. Khuyen nghi thuc te

- Khong commit password that len source khi lam du an that
- Dung DTO thay vi tra truc tiep entity scaffold
- Tach business logic ra service
- Production nen dung Render Environment Variables cho connection string, JWT va Gemini key
- Nen bo `bin/obj` khoi source deploy
- Nen co `Dockerfile` ro rang khi deploy .NET len Render
- Neu deploy cloud, can than voi `UseHttpsRedirection()`

## 25. Mau file nen co trong project

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_HOST;Port=5432;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Swagger": {
    "Enabled": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Gemini": {
    "ApiKey": "Key",
    "Model": "gemini-2.5-flash"
  },
  "AllowedHosts": "*"
}
```

### `.gitignore`

```gitignore
appsettings.json
appsettings.Development.json
bin
obj
publish
.vs
.vscode
```

### `Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CheckInProApiService.csproj ./
RUN dotnet restore CheckInProApiService.csproj

COPY . ./
RUN dotnet publish CheckInProApiService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production

CMD ["sh", "-c", "dotnet CheckInProApiService.dll --urls http://0.0.0.0:${PORT:-10000}"]
```

## 26. Ket luan

Neu ban nho 4 diem nay, ban se deploy nhanh hon rat nhieu:

1. Database First thi dung `dotnet ef dbcontext scaffold`
2. Chay local on roi moi deploy
3. Render hien tai can `Dockerfile` de chay `.NET`
4. Nen dua secret sang Environment Variables thay vi de trong source

Ban co the dung tai lieu nay lam mau cho cac du an .NET API khac voi PostgreSQL trong tuong lai.

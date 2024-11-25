# 1. Sử dụng hình ảnh chính thức của .NET để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# 2. Copy file project và khôi phục các dependency
COPY *.csproj ./
RUN dotnet restore

# 3. Copy toàn bộ mã nguồn và build ứng dụng
COPY . ./
RUN dotnet publish -c Release -o /out

# 4. Sử dụng hình ảnh runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /out .

# 5. Cấu hình lệnh để chạy ứng dụng
ENTRYPOINT ["dotnet", "EventManagement.dll"]

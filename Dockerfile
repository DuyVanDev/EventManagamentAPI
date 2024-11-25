# 1. Sử dụng hình ảnh chính thức của .NET runtime cho ứng dụng cuối
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. Sử dụng hình ảnh SDK để build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Sao chép file .csproj và khôi phục các dependencies
COPY ["EventManagament/EventManagament.csproj", "EventManagament/"]
RUN dotnet restore "EventManagament/EventManagament.csproj"

# Sao chép toàn bộ mã nguồn và build
COPY . .
WORKDIR "/src/EventManagament"
RUN dotnet build "EventManagament.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Publish ứng dụng
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EventManagament.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# 4. Tạo container cuối cùng dựa trên runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventManagament.dll"]

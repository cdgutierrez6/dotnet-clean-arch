FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/CleanArch.WebApi/CleanArch.WebApi.csproj", "src/CleanArch.WebApi/"]
COPY ["src/CleanArch.Application/CleanArch.Application.csproj", "src/CleanArch.Application/"]
COPY ["src/CleanArch.Infrastructure/CleanArch.Infrastructure.csproj", "src/CleanArch.Infrastructure/"]
COPY ["src/CleanArch.Domain/CleanArch.Domain.csproj", "src/CleanArch.Domain/"]

RUN dotnet restore "src/CleanArch.WebApi/CleanArch.WebApi.csproj"

COPY . .
WORKDIR "/src/src/CleanArch.WebApi"

RUN dotnet publish "CleanArch.WebApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanArch.WebApi.dll"]

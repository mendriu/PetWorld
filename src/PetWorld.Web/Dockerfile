FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy project files
COPY ["PetWorld.Domain/PetWorld.Domain.csproj", "PetWorld.Domain/"]
COPY ["PetWorld.Application/PetWorld.Application.csproj", "PetWorld.Application/"]
COPY ["PetWorld.Infrastructure/PetWorld.Infrastructure.csproj", "PetWorld.Infrastructure/"]
COPY ["PetWorld.AI/PetWorld.AI.csproj", "PetWorld.AI/"]
COPY ["PetWorld.Web/PetWorld.Web.csproj", "PetWorld.Web/"]

# Restore
RUN dotnet restore "PetWorld.Web/PetWorld.Web.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/PetWorld.Web"
RUN dotnet build "PetWorld.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PetWorld.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PetWorld.Web.dll"]

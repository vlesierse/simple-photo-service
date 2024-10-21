#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NuGet.config", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/SimplePhotoService.Api/SimplePhotoService.Api.csproj", "src/SimplePhotoService.Api/"]
RUN dotnet restore "src/SimplePhotoService.Api/SimplePhotoService.Api.csproj"
COPY . .
WORKDIR "/src/src/SimplePhotoService.Api"
RUN dotnet build "SimplePhotoService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN echo "Build"
RUN dotnet publish "SimplePhotoService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimplePhotoService.Api.dll"]
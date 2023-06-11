FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/WebApi/WebApi.csproj", "src/WebApi/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["tests/Application.UnitTests/Application.UnitTests.csproj", "tests/Application.UnitTests/"]
RUN dotnet restore "src/WebApi/WebApi.csproj"
RUN dotnet restore "tests/Application.UnitTests/Application.UnitTests.csproj"
COPY . .
WORKDIR "/src/src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR /src
RUN dotnet test

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "WebApi.dll"]

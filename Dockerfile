# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source

# Configure a custom global package folder for NuGet
ENV NUGET_PACKAGES=/source/.nuget/packages

# Copy solution and restore dependencies
COPY InternalSourceTransform.sln ./
COPY RestAPI/ RestAPI/
COPY BusinessTransformer/ BusinessTransformer/
COPY DocumentParser/ DocumentParser/
COPY CommonInterfaces/ CommonInterfaces/
COPY RestAPITests/ RestAPITests/
COPY BusinessTransformerTests/ BusinessTransformerTests/
COPY DocumentParserTests/ DocumentParserTests/

RUN dotnet restore --packages $NUGET_PACKAGES

# Build the app
RUN dotnet build -c Release --no-restore

# Publish the app
WORKDIR /source/RestAPI
RUN dotnet publish -c Release -o /app/out --no-build

# Test the app
WORKDIR /source
RUN dotnet test -c Release --no-build --logger:trx

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "RestAPI.dll"]

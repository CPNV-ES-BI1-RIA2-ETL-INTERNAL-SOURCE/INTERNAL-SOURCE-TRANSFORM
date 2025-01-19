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

RUN dotnet restore RestAPI/RestAPI.csproj --packages $NUGET_PACKAGES

# Build and publish the app
WORKDIR /source/RestAPI
RUN dotnet publish -c Release -o /app/out --packages $NUGET_PACKAGES

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/out .

# Expose the port your app runs on (default is 5067)
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "RestAPI.dll"]

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build
WORKDIR /source

# Configure a custom global package folder for NuGet
ENV NUGET_PACKAGES=/source/.nuget/packages

# Copy solution and restore dependencies
COPY InternalSourceTransform.sln ./
COPY RestAPI/RestAPI.csproj RestAPI/
COPY BusinessTransformer/BusinessTransformer.csproj BusinessTransformer/
COPY DocumentParser/DocumentParser.csproj DocumentParser/
COPY CommonInterfaces/CommonInterfaces.csproj CommonInterfaces/
COPY RestAPITests/RestAPITests.csproj RestAPITests/
COPY BusinessTransformerTests/BusinessTransformerTests.csproj BusinessTransformerTests/
COPY DocumentParserTests/DocumentParserTests.csproj DocumentParserTests/

RUN dotnet restore RestAPI/RestAPI.csproj --packages $NUGET_PACKAGES

# Copy the rest of the source code
COPY . ./

# Build and publish the app
WORKDIR /source/RestAPI
RUN dotnet publish -c Release -o /app/out --packages $NUGET_PACKAGES

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/out .

# Expose the port your app runs on (default is 5067)
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "RestAPI.dll"]

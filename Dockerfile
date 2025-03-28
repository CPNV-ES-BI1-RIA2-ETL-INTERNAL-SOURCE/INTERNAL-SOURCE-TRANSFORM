# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source

# Configure a custom global package folder for NuGet
ENV NUGET_PACKAGES=/source/.nuget/packages

# Copy solution and project files
COPY InternalSourceTransform.sln ./
COPY RestAPI/ RestAPI/
COPY BusinessTransformer/ BusinessTransformer/
COPY DocumentParser/ DocumentParser/
COPY RestAPITests/ RestAPITests/
COPY BusinessTransformerTests/ BusinessTransformerTests/
COPY DocumentParserTests/ DocumentParserTests/

# Restore dependencies
RUN dotnet restore --packages $NUGET_PACKAGES

# Build the app
RUN dotnet build -c Release --no-restore

# Stage 2: Test
FROM build AS test
WORKDIR /source
RUN dotnet test -c Release --logger:trx --no-build

# Stage 3: Publish
FROM build AS publish
WORKDIR /source/RestAPI
RUN dotnet publish -c Release -o /app/out --no-build

# Stage 4: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=publish /app/out .

# Disable the invariant mode (set in base image)
RUN apk add --no-cache icu-libs icu-data-full

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
 
ENTRYPOINT ["dotnet", "RestAPI.dll"]
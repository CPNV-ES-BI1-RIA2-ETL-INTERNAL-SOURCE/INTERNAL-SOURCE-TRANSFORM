# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS restore
WORKDIR /source

# Configure a custom global package folder for NuGet
ENV NUGET_PACKAGES=/source/.nuget/packages

# Copy solution and project files
COPY InternalSourceTransform.sln ./
COPY RestAPI/ RestAPI/
COPY BusinessTransformer/ BusinessTransformer/
COPY DocumentParser/ DocumentParser/
COPY CommonInterfaces/ CommonInterfaces/
COPY RestAPITests/ RestAPITests/
COPY BusinessTransformerTests/ BusinessTransformerTests/
COPY DocumentParserTests/ DocumentParserTests/

# Restore dependencies
RUN dotnet restore --packages $NUGET_PACKAGES

# Stage 2: Test
FROM restore AS test
WORKDIR /source
RUN dotnet test -c Release --logger:trx

# Stage 3: Publish
FROM restore AS publish
WORKDIR /source/RestAPI
RUN dotnet publish -c Release -o /app/out 

# Stage 4: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "RestAPI.dll"]

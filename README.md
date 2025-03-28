# INTERNAL-SOURCE-TRANSFORM
## Description
This is a .NET 8 solution, designed to be a microservice in an ETL software.

This is the **transformation** part of a SBB CFF FFS app. There are four distinct parts to this solution :

- The **Restfull API** served with ASP.net core (contains only controllers)
- **Common interfaces** : Contains only interfaces for communication between the two main parts
- **Raw format conversion** (from text extracted from a PDF) and associated test project :
  - Core maintainer : Cyprien Jaquier
- **Business transformation / computation** of what was returned by the text parser and associated test project :
  - Core maintainer : Eliott Jaquier

## Getting Started

### Prerequisites
#### Source control
* Git version 2.39+ [Download](https://git-scm.com/)
* Git LFS 3.5+ [Download](https://git-lfs.github.com/)
#### Development
* .NET SDK 8.0+ [Download](https://dotnet.microsoft.com/download)
* IDE: JetBrains Rider 2024.2+ [Download](https://www.jetbrains.com/rider/download/)
#### Documentation
* Astah UML 10+ [Download](https://astah.net/products/astah-uml/)
#### Containerization
* Docker 26+ [Download](https://www.docker.com/products/docker-desktop)

### Getting started
#### Build the project:
##### Restore dependencies
```shell
dotnet restore
# Expected: Restores all project dependencies with output showing restored packages
```

##### Debug Mode (Development)
```shell
dotnet build
# Expected: Builds with debug symbols and optimizations disabled
# Successful build ends with "Build succeeded." and below "0 Error(s)"
```

#### Release Mode (Production)
```shell
dotnet build --configuration Release
# Expected: Builds with optimizations enabled and debug symbols excluded
# Successful build ends with "Build succeeded." and below "0 Error(s)"
```


_With Docker_
```shell
docker build --target build -t build .
# Expected: Builds Docker image with output showing each build step
# Successful build contains "exporting to image" and "What's Next?" messages
docker images
# REPOSITORY   TAG     IMAGE ID       CREATED         SIZE
# build        latest  XXXXXXXXXXXX   X seconds ago   XXXMB
```

#### Run the api locally
##### Debug Mode (Development)
```shell
cd RestAPI
dotnet run
# [11:54:26 INF] Starting web application
# [11:54:27 INF] Now listening on: http://localhost:5067
# [11:54:27 INF] Application started. Press Ctrl+C to shut down.
# [11:54:27 INF] Hosting environment: Development
```

You can go to [http://localhost:5067/swagger/](http://localhost:5067/swagger/index.html) to see API endpoints.

##### Release Mode (Production)
```shell
cd RestAPI
dotnet run --configuration Release --environment Production
# [XX:XX:XX INF] Starting web application
# [XX:XX:XX INF] Now listening on: http://localhost:5067
# [XX:XX:XX INF] Application started. Press Ctrl+C to shut down.
# [XX:XX:XX INF] Hosting environment: Production
```

**Note : in production versions, /swagger endpoint isn't served**


_With Docker_

*Note : Keep in mind that the logs are not stored in a volume*
```shell
docker build --target runtime -t runtime .
# Expected: Builds Docker image with output showing each build step
# Successful build contains "exporting to image" and "What's Next?" messages
docker run -d -p 8080:8080 --name runtime runtime
# Expected: Starts the container in detached mode with the name "runtime"
# Successful run contains the container ID
docker ps
# CONTAINER ID   IMAGE     COMMAND                CREATED          STATUS          PORTS                    NAMES
# XXXXXXXXXXXX   runtime   "dotnet RestAPI.dll"   X seconds ago    Up X seconds    0.0.0.0:8080->8080/tcp   runtime
```

#### Test projects:
```shell
dotnet test
# Success! - Failed: 0, Passed: X, Skipped: 0, Total: X, Duration: X ms - DocumentParserTests.dll (net8.0)
# Success! - Failed: 0, Passed: X, Skipped: 0, Total: X, Duration: X ms - BusinessTransformerTests.dll (net8.0)
# Success! - Failed: 0, Passed: X, Skipped: 0, Total: X, Duration: X ms - RestAPITests.dll (net8.0)
```

_With Docker_
```shell
docker build --target test -t test .
# Expected: Builds Docker image with output showing each build step
# Successful build contains "exporting to image" and "What's Next?" messages

docker images
# REPOSITORY   TAG     IMAGE ID       CREATED         SIZE
# test         latest  XXXXXXXXXXXX   X seconds ago   XXXMB
```

##### For a specific test project:
```shell
dotnet test DocumentParserTests
# Success! - Failed: 0, Passed: X, Skipped: 0, Total: X, Duration: X ms - DocumentParserTests.dll (net8.0)
```

##### For a specific test:
```shell
dotnet test DocumentParserTests --filter FullyQualifiedName~DocumentParserTests.Parse_SingleString
# Success! - Failed: 0, Passed: 1, Skipped: 0, Total: 1, Duration: X ms - DocumentParserTests.dll (net8.0)
```

## How to use / configure for Docker Compose
For a quick start, there is an example docker-compose file in the root of the project (docker-compose-example.yml).
You can use it as inspiration when creating your own. It exposes changes to environment variables frequently used in production (such as changing the location of logging and creating volumes).

You can run the example with the following command:
```shell
docker-compose -f docker-compose-example.yml up -d 

#[+] Running 1/1
# ✔ Container internal-etl-transform  Started

docker-compose -f docker-compose-example.yml ps    
# NAME                     IMAGE                COMMAND                SERVICE   CREATED         STATUS         PORTS
# internal-etl-transform   your-restapi-image   "dotnet RestAPI.dll"   restapi   X seconds ago   Up X seconds   0.0.0.0:5000->8080/tcp

docker-compose -f docker-compose-example.yml logs
# internal-etl-transform  | [XX:XX:XX INF] Starting web application
```

## Download latest docker image (built for production)
You don't want to clone the repo or install the depedencies? No worry, we automatically generate the docker images for you. Follow these few setps to run the app on you machine:
1. Go to the [action](https://github.com/CPNV-ES-BI1-RIA2-ETL-INTERNAL-SOURCE/INTERNAL-SOURCE-TRANSFORM/actions) tab of the repo.
2. Click on the latest one and scroll to the 'Artifacts' section.
3. Download the internal-source-transform-tar.
4. Unzip the previously downloaded artifact.
5. Load the .tar image in your docker with `docker load -i <path/to/the/image.tar>`
6. Then run the image `docker run -d -p 8080:8080 --name internal-source-transform internal-source-transform` 
   - Note: To log in a volume, you can add `-v logs:/var/logs` and `-e Serilog__WriteTo__1__Args__path=/var/logs/app.log` when running the image. (You will need to `docker volume create logs` the first time)

## Collaborate

### Directory Structure
```shell
├───.idea                      
├── .gitignore                 
├───Doc                        
├───BusinessTransformer        // Core logic for business transformation and computation
├───BusinessTransformerTests   
├───DocumentParser             // Handles raw format conversion from PDF-extracted text
├───DocumentParserTests        
├───RestAPI                    // RESTful API using ASP.NET Core, contains only controllers
├───RestAPITests               // Contains integration + *e2e* tests of RESTful API using ASP.NET Core
├── LICENSE.txt                // MIT License
├── README.md                  
```

### General Architecture
The project is divided into three parts: BusinessTransformer, DocumentParser, and RestAPI.
Here is the dependency graph (exported image of astah uml) of the project:

![Dependency graph](Doc/UMLExports/PackageDependencies.png)

We have 3 layers in the project: 
 - The common interfaces for all project parts
 - Standalone models layers (like the document parser or the business transformer)
 - The RestAPI, with controllers, using all models and interfaces.

For model layers, we have tests projects that test the implementation of the interfaces (unit and integration tests).

For the RestAPI, we have end-to-end tests that test the API endpoints.

It is impossible to have a circular dependency between the projects (this generally implies a bad design).

For more information, see the [UML diagrams](Doc/UML.asta), the main [Technologies documentation](../../wiki/Technologies.md) used in the project and the [Definition Of Done](../../wiki/DefinitionOfDone.md).

### Class syntax
Classes and code structure follow the [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

### Workflow
#### Commits
Follow the [Conventionnal Commits](https://www.conventionalcommits.org/en/v1.0.0/), inspired by [Angular Conventional Config](https://github.com/conventional-changelog/commitlint/tree/master/%40commitlint/config-conventional) and [Angular Contributing Guidelines](https://github.com/angular/angular/blob/22b96b9/CONTRIBUTING.md#-commit-message-guidelines)
Scheme: `<type>(<scope>): <description>`

Types can be:
- **feat:** A new feature for the user of the software (a *MINOR* change in [semantic versioning](https://semver.org/#summary))
- **fix:** A bug fix for the user of the software (a *PATCH* change in [semantic versioning](https://semver.org/#summary))
- **refactor:** A code change that neither fixes a bug nor adds a feature
- **perf:** A code change that improves performance
- **test:** Adding missing tests, correcting existing tests, adding test utils or refactoring tests
- **style:** Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
- **docs:** Documentation only changes
- **chore:** Changes to the build process, auxiliary tools and libraries such as documentation generation
- **ci:** Changes to our CI configuration files and scripts

To indicate a *MAJOR* change in [semantic versioning](https://semver.org/#summary), you can use the `BREAKING CHANGE: ` keyword in the commit body and draw attention to this change with a `!` after the type (e.g. `feat!: ...`).

The list of supported scopes:
- **business-transformer**
- **document-parser**
- **restapi**

#### Branches
* Pull requests are open to merge in the develop branch.
* Feature branches are created from the develop branch and merged back into it. 
  * We use the [Gitflow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow).
  * The name of a feature branch should be `feature/feature_branch` in snake case.

## License
Distributed under the MIT License. See [LICENSE.txt](LICENSE.txt) for more information.

## Contact
* If needed you can create an issue on GitHub we will try to respond as quickly as possible.

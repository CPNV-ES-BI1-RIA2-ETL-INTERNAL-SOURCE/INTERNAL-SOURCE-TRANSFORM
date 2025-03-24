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
```shell
dotnet restore
dotnet build
```

_With Docker_
```shell
docker build --target build -t build .
```

#### Run the api locally
```shell
cd RestAPI
dotnet run
```

You can go to [http://localhost:5067/swagger/](http://localhost:5067/swagger/index.html) to see API endpoints.

_With Docker (prod only so the /swagger isn't served)_
```shell
docker build --target runtime -t runtime .
docker run -d -p 8080:8080 --name runtime runtime
```

#### Test projects:
```shell
dotnet test
```

_With Docker_
```shell
docker build --target test -t test .
```

## How to use / configure for Docker Compose
For a quick start, there is an example docker-compose file in the root of the project (docker-compose-example.yml).
You can use it as inspiration when creating your own. It exposes changes to environment variables frequently used in production (such as changing the location of logging and creating volumes).

You can run the example with the following command:
```shell
docker-compose -f docker-compose-example.yml up -d 
```

## Download latest docker image (built for production)
You don't want to clone the repo or install the depedencies? No worry, we automatically generate the docker images for you. Follow these few setps to run the app on you machine:
1. Go to the [action](https://github.com/CPNV-ES-BI1-RIA2-ETL-INTERNAL-SOURCE/INTERNAL-SOURCE-TRANSFORM/actions) tab of the repo.
2. Click on the latest one and scroll to the 'Artifacts' section.
3. Download the internal-source-transform-tar.
4. Unzip the previously downloaded artifact.
5. Load the .tar image in your docker with `docker load -i <path/to/the/image.tar>`
6. Then run the image `docker run -d -p 8080:8080 --name internal-source-transform internal-source-transform` 


## Collaborate

### Directory Structure
```shell
├───.idea                      
├── .gitignore                 
├───Doc                        // Project documentation in markdown
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
* [How to commit](https://www.conventionalcommits.org/en/v1.0.0/)
* Pull requests are open to merge in the develop branch.
* Feature branches are created from the develop branch and merged back into it. 
  * We use the [Gitflow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow).
  * The name of a feature branch should be `feature/feature_branch` in snake case.

## License
Distributed under the MIT License. See [LICENSE.txt](LICENSE.txt) for more information.

## Contact
* If needed you can create an issue on GitHub we will try to respond as quickly as possible.

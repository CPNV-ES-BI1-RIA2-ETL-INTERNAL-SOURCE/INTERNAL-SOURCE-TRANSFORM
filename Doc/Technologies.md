# Technologies used for this project
## C# with .NET 8
The project is written in C# using .NET 8.0.

We needed a strongly typed, widely recognized language with excellent community support that could cope with making apis rest.
In addition, the language had to meet these constraints:
- A good working environment / development tools which supports out of the box, many features (like events, interfaces, etc.)
- Cross-platform support
- An object-oriented web server
- Language must be compatible with SDKs (like AWS S3)
- Easily containerize the application
- Support dependency injection with ease
- Reliable toolchain (like type safety), making it easier to reduce runtime errors

We could also have used these languages (which meet the requirements):
- Java with Spring Boot (but it is forbidden by the client)
- TypeScript with Node.js (but it is not as strongly typed as C#)

Besides, it's a language that has already been seen and mastered by the developers of this service.

### .NET 8
We chose .NET 8 because it is the latest stable version of .NET, and it is supported by Microsoft. 
It unified the .NET platform, which was previously fragmented into .NET Framework, .NET Core, and Xamarin/Mono.
We can now use the same libraries and tools for all .NET applications, which makes it easier to maintain and develop applications (and is cross-platform).

### NUnit3
We chose NUnit3 as the testing framework because it is a widely used testing framework for .NET applications with a lot of examples and documentation available online.
It is also compatible with the .NET 8 framework.

We could have used the new MSTest, but NUnit3 is more widely used and has more features.

Note: An exception for the end-to-end test project, which uses XUnit, as recommended by Microsoft (there is no support for NUnit 3).
**For future releases, we'll be running all tests on XUnit for greater consistency**.

### ASP.NET Core
We chose ASP.NET Core as the web server because it is a widely used web server for .NET applications with a lot of examples and documentation available online.

Plus, it already contains a lot of features out of the box, like **dependency injection**, middleware, auto documentation (like with Swagger pre-configured out of the box), routing and the architecture makes it very flexible to adapt to design patterns like MVC, MVVM, etc.

Even better, there are a lot of examples to create microservices with it.

**Warning**: *Although we could have used the new .NET 8 minimal APIs, we chose to use ASP.NET Core because it is more mature and has more features out of the box. This makes it easier to develop BUT also heavier in terms of resources, making it less efficient than lighter alternatives for small, microservices applications.

## Astah UML 8.4
We chose Astah UML for all our UML diagrams because it is a tool that can produce high-quality UML diagrams in a really short time and in a uniform way (things described in a class diagram can be reused in a sequence diagram, etc.).

We can also freely choose the placement of elements (to demonstrate, for example, the application's layers).

But, unlike document as code like PlantUml or mermaid, this requires tracking changes on Git in binary form (which is not practical). To resolve this, each feature has it's own astah file, and we can simpliy move the information to the master UML.astah file after the feature is merged. If features need to change a dependency, the developer will have to update the master UML file in develop branch and merge it back to the feature branch.
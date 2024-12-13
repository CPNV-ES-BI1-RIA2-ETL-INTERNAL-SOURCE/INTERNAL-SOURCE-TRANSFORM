# Technologies used for this project
## C# with .NET 8
The project is written in C# using .NET 8.0.

We needed a strongly typed, widely recognized language with excellent community support that could cope with making apis rest.
In addition, the language had to meet these constraints:
- Support dependency injection with ease
- A good working environment / development tools which supports out of the box, many features (like events, interfaces, etc.)
- Cross-platform support
- An object-oriented web server
- Language must be compatible with SDKs (like AWS S3)
- Easily containerize the application

We could also have used these languages (which meet the requirements):
- Java with Spring Boot (but it is forbidden by the client)
- TypeScript with Node.js (but it is not as strongly typed as C#)

Besides, it's a language that has already been seen and mastered by the developers of this service.

## .NET 8
We chose .NET 8 because it is the latest stable version of .NET, and it is supported by Microsoft. 
It unified the .NET platform, which was previously fragmented into .NET Framework, .NET Core, and Xamarin/Mono.
We can now use the same libraries and tools for all .NET applications, which makes it easier to maintain and develop applications (and is cross-platform).

## NUnit3
We chose NUnit3 as the testing framework because it is a widely used testing framework for .NET applications with a lot of examples and documentation available online.
It is also compatible with the .NET 8 framework.

We could have used the new MSTest, but NUnit3 is more widely used and has more features.

Note: An exception for the end-to-end test project, which uses XUnit, as recommended by Microsoft (there is no support for NUnit 3).
**For future releases, we'll be running all tests on XUnit for greater consistency**.

## ASP.NET Core
We chose ASP.NET Core as the web server because it is a widely used web server for .NET applications with a lot of examples and documentation available online.
Plus, it already contains a lot of features out of the box, like **dependency injection**, middleware, auto documentation (like with Swagger pre-configured out of the box), and routing.
Even better, there are a lot of examples to create microservices with it.
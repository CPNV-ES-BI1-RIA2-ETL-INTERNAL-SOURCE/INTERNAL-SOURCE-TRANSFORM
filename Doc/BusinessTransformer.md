# Business Transformer
The Business Transformer project is designed to handle the transformation of structured document data into domain-specific business objects.
The main focus of this implementation is to process departure documents and transform them into TrainStation objects, including station names, departures, and train details.

## Architecture
For this project, we have two parts :
 - The **Sting manipulation** part, which provides a reusable set of functions to manipulate strings (non-structured data / non-business data). 
   - This comme with a standard implementation using the standard library.
 - The **Business transformation** part, which provides a function to transform structured data (coming from a document related structure) into business objects.
   - A interface using generics is provided to allow the implementation of the transformation function.
   - A implementation, transforming a DeparturesDocument into a TrainStation object, is provided. This specific part is strongly following the KISS + YAGNI principles as it's a simple transformation relying on strongly typed data. If, in future, we need a more modular transformation, we will refactor this part to use dynamic input schemes (but is was not needed at the time of writing).

## Tests
- A lot of times has been put into the tests of this project. They are written in BDD style and should validate (and be the source of truth about) the requirements of the project. These should equal the requirements of the project / acceptance criteria.
- All implementations classes of this project rely on interfaces.
- The tests are written using NUnit3 and use the interface to test the implementation. 
- It's expected that any implementation of the interfaces will pass the tests.
- We expect all maintainers to maintain 100% of code coverage for the tests + test for principal exceptions + test for principal edge cases.

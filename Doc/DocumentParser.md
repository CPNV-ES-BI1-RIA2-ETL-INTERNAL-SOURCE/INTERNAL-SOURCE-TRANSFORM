# Document Parser  
The Document Parser project is responsible for "standardizing" the structure of incoming documents so that the Business Transformer can actually transform the data it contains.  
The main focus of this implementation is to instantiate objects compatible with the "Business Transformer" while minimizing the use of business knowledge to reduce the impact of business changes.  

## Architecture  
For this project, we have two parts:  
- The **Document Parser** part, which is used to generate JSON format to organize the data hierarchically without real business knowledge except a "contract" with the "Export" of the ETL. [In the future, the output should be an instance of an enumerable object instead of a string JSON]  
  - An interface using generics is provided to allow the implementation of the Parse function.  
  - The contract:  
    - The input MUST be a list of strings.  
    - Tables MUST have at least 3 white spaces between columns.  
    - If there is one, the table MUST be the last entry of the input.  
    - An input MUST NOT have more than one table.  
    - A closure MUST be signified by a line feed. (⚠️ Deprecated but left as dead code)

## Tests  
- Significant time has been invested in testing this project. Tests are written in BDD style and should validate (and be the source of truth about) the project's requirements and acceptance criteria.  
- The tests are written using NUnit3 and use the interface to test the implementation.  
- It's expected that any implementation of the interfaces will pass the tests.  
- Tests MUST be improved in the future to ensure 100% code coverage.

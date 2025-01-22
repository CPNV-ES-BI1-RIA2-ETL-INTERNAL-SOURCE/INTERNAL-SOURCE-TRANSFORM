# Business Transformer
The **Business Transformer** project is designed to handle the transformation of semi-structured document data into domain-specific business objects using a schema mapping. Its primary goal is to decouple data manipulation logic from domain logic, allowing flexible and reusable data transformations.

## Architecture
For this project, we have two parts :

### 1. **String Manipulation**
- This part provides a reusable set of functions to manipulate strings (non-structured data or non-business data).
- It comes with a standard implementation using the .NET standard library. Examples include splitting, prefix removal, and regex processing.
- The operations in this module are designed to ensure robustness, including handling edge cases such as empty inputs or invalid formats.

### 2. **Mapping**
- This part provides functionality to transform semi-structured document data into structured JSON documents or other business-specific objects.
- It uses a schema mapping defined in terms of `FieldMapping` objects that describe the transformation logic for each field.
- Key features include support for nested mappings, parent-child relationships in data, and the ability to dynamically process arrays.
- **Main Components:**
   - `FieldMapping<TFrom>`: Represents a mapping from an input field index (`TFrom`) to a transformed output field, including optional methods to apply transformations.
   - `JsonMappingTransformer`: Implements the logic to process a mapping and produce structured output.
   - **Methods**: Supports a wide range of reusable transformations such as regex application, localization-aware date parsing, and custom transformations via user-defined parameters.

---

## Key Features and Implementation Details

### Transformations
- **Dynamic Mappings:** The transformer supports dynamic mappings provided at runtime, making it highly flexible for various input schemas.
- **Bag Concept:** Temporary storage (referred to as "bag") allows sharing intermediate results across different transformation steps.
- **Method Chaining:** Multiple methods can be applied to a field sequentially to achieve complex transformations.

### Error Handling
- Comprehensive error handling includes checks for out-of-range indices, invalid formats, and missing keys in dynamic objects.
- Custom exceptions, such as `InvalidInputFormatException`, are thrown when the input data violates the expected schema.
- Note that, to catch all project related exception, we use interfaces instead of concrete classes so we can reserve inheritance for custom typed system exception like `ArgumentException`.
- 
---

## Tests
- A lot of times has been put into the tests of this project. They are written in BDD style and should validate (and be the source of truth about) the requirements of the project. These should equal the requirements of the project / acceptance criteria.
- All implementations classes of this project rely on interfaces.
- The tests are written using NUnit3 and use the interface to test the implementation.
- It's expected that any implementation of the interfaces will pass the tests.
- We expect all maintainers to maintain 100% of code coverage for the tests + test for principal exceptions + test for principal edge cases.

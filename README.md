# Arcana Programming Language Specification

## Introduction

Arcana is an object-oriented, high-level programming language crafted with a focus on clarity, conciseness, and practicality. Acknowledging that these attributes often conflict, Arcana strives to strike a balanced harmony among them. It aims to minimize errors by catching them during compilation, leveraging a modern syntax influenced by languages like C#, Java, Swift, Rust, and Kotlin.

Designed for general-purpose use, Arcana is adaptable to a broad array of applications. It upholds the principle of least surprise, prioritizing straightforwardness and consistency to mitigate complexity and reduce the likelihood of programming errors. This focus does not make Arcana a simplistic language; rather, it aims to make its behavior predictable and its usage intuitive, successfully managing the challenges that arise from its goals of clarity, conciseness, and practical utility.

## Compiler
For now the front end will be written with C#, with the backend using LLVM IR. Eventully it would be nice to have the langauge to bootstrap itself.

## Syntax and Conventions

### File Extensions
The file extension for Arcana source files is `.arc`.

### Comments

- Comments are used to document code and are ignored by the compiler.
- Single-line: `// comment`
- Multi-line: `/* comment */`

### Blocks and Statements

- Uses `{}` for blocks, avoiding reliance on whitespace for formatting.
- New lines end statements instead of semicolons.

### Variable Declaration

- Variables are declared with the type followed by the name, e.g., `int count`.
- Variables can be initialized at the time of declaration, e.g., `int count = 10`.
- Variables must be definitely assigned before use, meaning they can be declared without an initial value but must be initialized in all code paths before being accessed.
- By default, variables are non-nullable and cannot be assigned a null value.

### Types

- **Primitives**: `int`, `long` (as `int64`), `float`, `double`, `bool`, `char`, `string`, and a syntactical sugar `byte` for `char`.
- **Unsigned Versions**: Supported with a 'u' prefix, e.g., `uint64`.
- **Type Passing**: By reference for objects and by value for primitives, with syntax to specify otherwise.

### Strings

- Strings are immutable.
- Internally a table of strings is maintained to avoid duplicate strings.
- Unlike other primitives, strings are reference types.

### Lack of Type Inference

- To make code more readable and clear, Arcana does not support type inference. All variables must be declared with a type.

### Lack of void

- Arcana does not have a `void` type. Instead, functions that do not return a value simply omit the return type.

### Arrays

- Arrays are reference types.
- Arrays are declared with the type followed by `[]`, e.g., `int[]`.
- Arrays are initialized with the `new` keyword, e.g., `int[] numbers = new int[10]`.
- Arrays can be initialized with values, e.g., `int[] numbers = new int[] { 1, 2, 3 }`.
- Arrays can be initialized with values without the `new` keyword, e.g., `int[] numbers = { 1, 2, 3 }`.
- Arrays can be multi-dimensional, e.g., `int[][] matrix = new int[10][10]`.
- Arrays can be jagged, e.g., `int[][] matrix = new int[10][]`.
- Arrays support the `Length` property, e.g., `int length = numbers.Length`.

### Tuples

- Tuples group multiple values into a single unit, useful for multiple return values and temporary data grouping.
- Tuples can contain elements of different types, enhancing their versatility for grouping diverse data.
- Tuples are lightweight and have a minimal memory footprint, making them efficient for temporary data storage.
- Tuples allow assignment between each other when their type signatures match, ensuring type consistency. Unlike structs, where identical structures are considered distinct types and are not directly assignable.
- Tuples are declared with the `()` syntax, e.g., `(int, string)`.
- They can be unnamed, or named for clarity.
- Elements in named tuples are accessed by name, and unnamed tuples are accessed by index using .0, .1, etc.
- Tuples support destructuring for assignment.
- Tuples can be compared directly, and type signatures can be defined for reuse. 
- Tuples are value types.
- Tuples are immutable after initialization. The lifetime of a tuple should be short-lived. For longer-lived data structures, use classes or structs.
- Tuples can be nested.
- Best practices recommend using named tuples for clarity and defining tuple type signatures centrally for reused structures. 
- Use tuples for simple groupings; complex data structures should use classes or structs.

### Memory Management

- Utilizes reference counting with considerations for future garbage collection integration.

### Casting

#### Implicit Casting

- **Numeric Widening**: Automatic conversions among numeric types following precision and size.
- **String Concatenation**: Implicit casting of primitives to strings.
- **Boolean Context**: Numeric types and `char` interpreted in boolean expressions.
- **Numeric Operations**: Follows the highest precision rule in mixed type operations.

#### Explicit Casting

- Required for narrowing conversions, sign changes, or potentially unsafe operations.
- Syntax: `<value>::<target_type>`.

### Casting Objects

- Objects can be checked for type compatibility with the `is` keyword which returns a boolean value.
- Object can be case using ::<type> syntax but only if the object is wrapped in a compatibility check with the `is` keyword.
- Alternatively, the `::?` keyword can be used to safe cast an object to a type, returning null if the cast fails.

### Operators

- **Arithmetic**: `+`, `-`, `*`, `/`, `%`
- **Bitwise**: `&`, `|`, `^`, `~`, `<<`, `>>`
- **Logical**: `&&`, `||`, `!`
- **Comparison**: `==`, `!=`, `>`, `<`, `>=`, `<=`
- **Null Coalescing**: `??`
- **Null Safe**: `?.`

### Access Modifiers

- `public`, `private`, `protected`, `internal`
- By default, access modifiers can be omitted, and the default is `private` except for the following exceptions:
	- Constructors require a access modifier, there is no default.
	- Functions within interfaces are always public and do not require an access modifier, though for clarity one can be added

### Constants

- Constants are declared with the `const` keyword.
- Constants are checked at compile time and must be initialized with a value.
- Constants are immutable and cannot be changed after initialization.
- Constants are implicitly static and can be accessed without an instance of the class.

### Read-Only Variables

- Read-only variables are declared with the `readonly` keyword.
- Read-only variables can be initialized at the time of declaration or in the constructor.
- Read-only variables are checked at runtime and can be changed only in the constructor.
- Read-only variables are not implicitly static and require an instance of the class to access.
- Read-only variables are immutable after initialization.
- If a read-only variable is a reference type, the object it refers to can still be modified, it just cannot be reassigned.
- If a read-only variable is a value type, the value itself cannot be modified. For example, a read-only integer cannot be changed to a 
  different integer value. If a struct is read-only, the fields of the struct cannot be modified, they are immutable.

### Functions

- Functions with support for overloading.
- Functions can return multiple values.
- Functions can be passed as arguments to other functions.
- Functions can be nested inside other functions (closures).
- Functions can have default parameters.
- Functions can have global scope by using the `static` keyword.
- Functions can be overridden in subclasses, no keyword needed, just match the method signature.

### Function Invocation

- Functions are invoked with the function name followed by the arguments in parentheses, e.g., `MyFunction("Hello")`.
- Functions can be invoked with named parameters, e.g., `MyFunction(message: "Hello")`.
- Functions can be invoked with named parameters in any order, e.g., `MyFunction(message: "Hello", count: 5)`.
- If a function has default parameters, they can be omitted when invoking the function

### Pass by Reference and Pass by Value

- By default, objects are passed by reference and primitives are passed by value.
- Value types can be forced to be passed by reference using the `ref` keyword.
- Reference types can be forced to be passed by value using the `val` keyword.

### Classes and Inheritance
- Classes are the primary building blocks of Arcana.
- Classes can have mebers such as fields, properties, functions, and events.
- Variables and functions in Arcana must reside within classes.
- Supports classes, inheritance, and interfaces.
- Properties with getters and setters.
- Constructor chaining with `super`.

### Constructors

- Constructors are defined with the `func` keyword followed by the class name.
- Does not have a return type.
- Can be overloaded.
- Can reference other constructors using `this` for itself or `super` for the base class.
- Constructors are public by default but can be made private.
- Can be omitted if the class does not require initialization.

### Object Initializer Syntax
- Objects can be initialized with the `{}` syntax.
- The syntax is `{ <property_name>: <value>, ... }`.

### this Keyword

- `this` refers to the current instance of the class.
- `this` is used to access class members and methods.
- `this` is used to disambiguate between class members and local variables.

### super Keyword

- `super` refers to the base class.
- `super` is used to access base class members and methods.

### Properties

- Properties can have getters and setters.
- Properties have a backing field that can be accessed using the `field` keyword.
- Properties can be declared with an access modifiers with the default being `private`.
- Properties are accessed like fields, e.g., `myObject.myProperty`.
- Properties are used to encapsulate fields and provide access to them while maintaining control over how they are accessed and modified.
- Properties can be used to validate input and provide a consistent interface for accessing fields.

### Abstract Classes

- Classes can be declared as `abstract` to prevent them from being instantiated.
- Abstract classes can contain abstract methods that must be implemented by subclasses.
- Abstract classes can contain concrete methods that can be used by subclasses.
- Abstract classes can contain fields and properties.

### Sealed Classes

- Classes can be declared as `sealed` to prevent them from being inherited from.
- Sealed classes cannot be used as a base class, but can inherit from other classes.
- Sealed classes cannot be abstract.
- Sealed classes are ideal for creating closed hierarchies where the set of derived classes is known and controlled by 
  the application. For example, defining a set of payment methods where each method has a distinct processing logic but 
  should not be further subclassed.

### Static Classes

- Classes can be declared as `static` to prevent them from being instantiated.
- Static classes are implicitly sealed.
- Static classes cannot be inherited from.
- Static classes cannot be instantiated.
- Static classes are useful for organizing helper functions, constants, or utility methods that do not require object 
  instantiation. For example, a math helper class might contain static methods for common mathematical operations.

### Structs

Structs in Arcana are designed to offer lightweight, value-type semantics for efficient data storage and manipulation. Unlike classes,
which are reference types, structs provide an ideal solution for creating small data structures that do not require inheritance or complex behaviors.
Structs do not allow methods or events, but structs support delegates, enabling them to reference methods indirectly. This feature is crucial for 
interoperability with other programming languages, such as C, where function pointers within structs are a common pattern.

- Structs are value types.
- Structs are declared with the `struct` keyword.
- Structs are data structures that can contain fields, properties, but does not support methods or events.
- Structs cannot be inherited from.
- Structs cannot be used as a base class.
- Structs cannot be abstract.
- Structs do support delegates as for interoperability with other languages.
- Structs can be compared using the `==` and `!=` operators, comparing each field for equality.

### Namespaces

- Namespaces for organizing code.
- `import` statements for importing namespaces.

#### Closure Support

- Closures with support for capturing variables from the enclosing scope.
 -Closures in Arcana capture variables by reference. This means that any modifications to captured variables within the closure will reflect in the outer scope.
- Closures have the same decleration syntax as functions.

#### Lambda Expressions

- Lambda expressions with support for closures.

### Destructuring

- Destructuring of objects, function arguments, and loop variables.

### Variadic Functions(Rest Operator)

- Functions with a variable number of arguments.
- Functions can be declared with the `...` keyword followed by the type of the variable arguments.
- The variable arguments are accessible as an array within the function.

### Spread Operator

- The spread operator `...` can be used to spread an array into individual arguments.
- The spread operator can be used to combine arrays with the syntax `[...array1, ...array2]`.
- The spread operator cannot be used to combine other types of objects.
- The spread operator can be used to copy an array with the syntax `[...array]`.
- The spread operator cannot be used to spread out into function arguments other than variadic functions or array parameters.

### Scope and Lifetime

- Variables are to be initialized before use.
- Variables (fields) declared within a class are accessible to all methods of the class, depending on their access modifiers
- Variables declared within a block have an automatic lifetime and are destroyed when the block is exited.
- Arcana uses reference counting for memory of reference types. Objects are automatically destroyed when their reference count reaches zero.

#### Nested Hierarchy

- In Arcana, every block (defined using {}), function, and class creates a new scope.
- Scopes can be nested within each other, forming a hierarchy. 
- Variables and functions are accessible within the scope they are declared in and any nested scopes.

#### Variable Shadowing

When a variable declared in a nested scope shares a name with a variable in an outer scope, the inner variable "shadows" the outer one within 
the nested scope. This means that any reference to the variable name within the nested scope refers to the inner variable.

#### Accessing Outer Scope Variables

Variables declared in an outer scope are accessible in any nested scope, provided they are not shadowed by a similarly named variable in 
the nested scope. This facilitates the use of global variables or variables from an enclosing scope within inner functions or blocks.

#### Closure Scope

Closures in Arcana capture variables from their enclosing scopes by reference. This means modifications to these variables within the 
closure affect their values in the outer scope. This feature requires careful use to avoid unintended side effects.

The lifetime of captured variables extends as long as the closure exists. This means that even if the outer scope has exited, the variables 
captured by a closure remain alive if the closure itself is still accessible.

Closures use mutable capture. This means that the captured variables can be modified within the closure.

#### Classes and Nested Scopes
Classes in Arcana create their own scope for variables (fields) and functions (methods). The interaction between class scopes and nested function scopes includes:
- Each method in a class defines its own scope. Variables declared within a method are not accessible outside of it, reinforcing encapsulation.
- Methods have access to class fields (variables declared within the class scope), respecting the access modifiers 

#### Global Scope

- Variables can have global scope by using the static keyword.
- Global variables can be accessed from anywhere in the program depending on their access modifiers.
- Global variables must be initialized at the start of the program and are destroyed at the end of the program.

### Error Handling

- `try`, `catch`, `finally` blocks for exception handling.
- Custom exception classes extending a base `Exception`.

### Handling Nullable Values in Arcana

In Arcana, values can be explicitly marked as nullable using the `?` syntax appended to the type. This denotes that a variable can hold either a value of its 
type or `null`. To ensure type safety and clarity, Arcana requires explicit handling of nullable values. The Null-Coalescing Operator (`??`) is the standard 
method for dealing with nullable expressions, providing a clear and concise way to specify default values for `null` however safe casting is also supported.

#### Nullable Types

- Declare a nullable type by appending `?` to any type name.
- Example: `int?` for a nullable integer.

#### Null-Coalescing Operator (`??`)

- The `??` operator provides a default value for a nullable expression if it is `null`.
- Syntax: `nullableExpression ?? defaultValue`

#### Nullable Function Parameters

- Function parameters can also be nullable.
- When invoking functions with nullable parameters, callers must handle the potential for `null` values explicitly.

#### Null Safe Operator (`?.`)

- The `?.` operator can be used to safely access members of a nullable object.
- If the object is `null`, the expression will return `null` instead of throwing a `NullReferenceException`.
- Syntax: `nullableObject?.member`

### Enums

- Enums with optional custom values.
- Enums can be compared directly.
- Enums by default are integers, but can be any type even an object.

### Generics

- Generic type support

### Operator Overloading

- Operator overloading for custom types.
- Allows custom types to be used with arithmetic, comparison, logical operators, bit-wise operators, assignment operators, and [] for indexing.

### Delegates

- Delegates for function signatures.
- Delegates can be passed as arguments to other functions.
- Delegates can be used to implement callbacks.
- Delegates can be used to implement events.

### Events

- First-class support for event handling.
- Utilizes delegates for event signatures.
- Events can be subscribed to and unsubscribed from.
- Events can be raised.
- Internally they are an implementation of the observer pattern.

### Control Structures

- `if`, `else if`, `else` statements.
- `ternary` operator.
- `for`, `while`, `do while` loops.
- `break` and `continue` statements.
- `match` statements.

### Pattern Matching

- `match` statements for pattern matching.
- `match` statements can be used with `if` statements.
- `match` statements can be used with guard clauses.

### Extensions

Arcana introduces extensions, enabling the addition of new methods to existing types without altering their original definitions. 
This feature fosters code modularity and extensibility.

- Utilize the extension keyword followed by the type name to create an extension.
- To use, first import the extension with the extension keyword followed by the type name.
- Extension can be used to add new field, properties, and methods to existing types.
- There is Hypothetical syntax for adding interfaces to existing types, but this is not supported yet. The main concern is how to handle 
  conflicts between multiple extensions. Addionally, this is adding more complexity to the language which may go against the design goals of
  Arcana.
- In general, Arcana needs a way to handle conflicts between multiple extensions between libraries, this hasnt been fully fleshed out yet. 
  One idea is to have the compiler Internally fully qualify the extension methods with the namespace based on the import statement in the
  file where the extension is used. Each file would have its own namespace, so there would be no conflicts between extensions in different
  files, however if two imports in the same file have the same extension method, there would be a conflict. This would be a compile-time error.

### Asynchronous Programming

- `async` and `await` keywords for asynchronous operations.
- Promise syntax for managing asynchronous data flows.

### Interoperability

- Foreign Function Interface (FFI) for calling functions from other languages.
- Will only support C for now, but Examples for other languages will be provided in the code examples section of how they might look.

### Annotations

- Metadata Annotations
- Annotations can be used to add metadata to classes, methods, and properties.
- Custom annotations can be created and used to add metadata to classes, methods, and properties.
- Limited support for custom metadata annotations, in the future if reflection is added, annotations will be expanded upon. Syntax exists for future expansion.

### Reflection
Arcana does not support reflection.

### Terminology Notes
Most of the terminology used in Arcana is defined in the spec, but here is a high-level overview of some of the 
terms used in the spec to use as a quick reference:

#### Function and Method
- Tranditionally, in other languages, methods are functions that are part of a class, and functions are standalone. However
  in Arcana, functions cannot exist outside of a class, so they are all conceptually methods. Therefore in Arcana the terms are 
  used interchangeably, however generally when the term method is used, it is to put emphasis on the fact that it is a function 
  that is part of a class where as when the term function is used, it is to put focus on the function itself. This is not a 
  hard and fast rule, but a general guideline.
- The only outlier to this is a function defined within another function, which is called a closure.
- Parameters are used to refer to the variables that are passed to a function or method.
- Variadic functions are functions that can take a variable number of arguments using the rest operator '...'.

#### Variables

- Local variable: A variable declared within a method or block. They are only accessible within the method or block in which 
  they are declared.
- Global variable: A variable declared outside of any method or block, declared with the `static` keyword. They are accessible
  from anywhere in the program depending on their access modifiers.

#### Members

- Fields are used to refer to variables declared within a class but not within a method.
- Properties are used to refer to fields with getters and setters.
- Members is used to refer to methods, fields and properties that collectively make up a class.

#### Expressions and Statements

- Expressions are used to refer to a combination of values, variables, operators, and functions that are evaluated to produce a
  single value.
- Statements are used to refer to a single line of code that performs an action, such as declaring a variable, invoking a function
  with no return value, or assigning a value to a variable. They do not produce a value.

#### Data Structures

- Structs are used to refer to value types that contain fields and properties but do not support methods or events.	
- Arrays are used to refer to a type of mutable data structure that groups multiple values into a single unit.
- Tuples are used to refer to a type of immutable data structure that groups multiple values into a single unit, they can have 
  named or unnamed elements.

#### Classes, Inheritance, and Polymorphism

- Classes are used to refer to reference types that contain fields, properties, methods, and events.
- Inheritance is used to refer to the ability of a class to inherit fields, properties, methods, and events from a base class.
- Base class is used to refer to the class from which another class inherits.
- Subclass is used to refer to the class that inherits from another class.
- Super is used to refer to the base class from within a subclass.
- Polymorphism is used to refer to the ability of a class to have multiple forms, such as through inheritance and interfaces.
- Overloading is used to refer to the ability to define multiple methods with the same name but different parameters.
- Overriding is used to refer to the ability to define a method in a subclass with the same name and signature as a method 
  in the base class.

#### Interfaces and Abstract Classes

- Interfaces are used to refer to a type of reference type that contains method signatures but no implementation. They can be 
  seen as a contract that a class must adhere to.
- Abstract classes are used to refer to a type of reference type that contains method signatures and some implementation, 
  but not all. They can be seen as a class that is not fully implemented and must be extended to be used.

#### Virtual and Abstract Methods
- Virtual methods are used to refer to methods that can be overridden in a subclass. They are used to provide a default
  implementation that can be overridden.
- Abstract methods are used to refer to methods that must be overridden in a subclass. They are used to define a method
  signature that must be implemented in a subclass.

#### Delegates and Events

- Delegates are used to refer to a type of reference type that contains a method signature. They can be used to reference 
  methods indirectly.
- Events are used to refer to a type of delegate that can be subscribed to and unsubscribed from.

#### Enums

- Enums are used to refer to a type of value type that contains a set of named constants.

#### Control Structures

- Control structures are used to refer to the syntax used to control the flow of a program, such as if statements, loops, and 
  match statements.
- Match statements are used to refer to a type of control structure that is used to compare a value to a set of patterns using
  the pattern matching syntax.

#### Error Handling

- Error handling is used to refer to the syntax used to handle exceptions and errors in a program, such as try, catch, and finally blocks.
- An exception is used to refer to an error that occurs during the execution of a program.
- A custom exception is used to refer to an exception that is defined by the user.

#### Generics

- Generics are templates that allow classes, interfaces, and methods to operate on objects of various types while providing 
  compile-time type safety.

#### Annotations

- Annotations are used to refer to a type of metadata that can be added to classes, methods, and properties.

### In consideration 

- Using/With statements
- Special syntax for reading/writing streams?
- Expand on Error Handling
- Implicit interfaces? Maybe some way to do it optionally?
- Inlining?
- Expand on Interoperability, such as how it would be configured, how it would work, etc
- Standard Lib
- Direct JSON support
- Range and Index Operators
- List comprehension? 
- Linq like syntax as an alternative to list comprehension?
- Support for unit testing
- Standarized doc conventions, have documention built into the spec
- Standardized code conventions
- Standardized project structure
- Stray away from traditional array syntax, instead define a special genric for array: Arr<T>

## Code Examples

### Comments
```// This is a single-line comment
/* This is a
   multi-line comment */
```

### Variable Declaration and Initialization
```
int count = 10
string name = "John"
float balance = 100.50
```

#### Variable Declaration with Nullables
```
int? nullableInt = null
string? nullableString = null
```

### Variable Declaration without Initialization
```
int count
string name
float balance
```
### Definite Assignment
```
int result

if (condition) {
    result = 100
} else {
    result = 200
}

print(result) // This will work because 'result' is definitely assigned in all branches before use.
```

### Tuple Declaration and Initialization
```
(int, string) unnamedTuple = (1, "Alice")
(int id, string name) namedTuple = (id: 1, name: "Alice")
```

### Using Tuples
```
// Named tuple
(int id, string name) tuple = (1, "Alice")
int id = tuple.id
string name = tuple.name
```

// Unnamed tuple
```
(int, string) tuple = (1, "Alice")
int id = tuple.0
```

// Nested tuple
```
((int, string), float) = ((1, "Alice"), 3.14)
int id = tuple.0.0
string name = tuple.0.1
```

### Tuple Comparison
```
(int id, string name) tuple1 = (1, "Alice")
(int id, string name) tuple2 = (1, "Alice")
(int id, string name) tuple3 = (2, "Bob")

bool areEqual = tuple1 == tuple2 // true
bool areEqual = tuple1 == tuple3 // false
```
### Tuple Type Signatures(Might remove this, may as well be an anonymous struct)
```
tupleType PersonInfo = (int id, string name)

// Using Tuple Type Signatures
PersonInfo person = (1, "Alice")
```

### Array Declaration and Initialization
```
int[] numbers = new int[10]
int[] numbers = new int[] { 1, 2, 3 }
int[] numbers = { 1, 2, 3 }
int[][] matrix = new int[10][10]
int[][] matrix = new int[10][]
```

#### Array Length:
```
int length = numbrs.Length
```

### Spread Operator
```
int[] numbers = { 1, 2, 3 }
int[] moreNumbers = { 4, 5, 6 }
int[] allNumbers = [...numbers, ...moreNumbers] // allNumbers = { 1, 2, 3, 4, 5, 6 }
```

### Function Definition
```
 // The func keyword is used to define a function, serves as a visual cue
fpublic unc Sum(int a, int b) int {
    return a + b
}

// Functions can also return multiple values
public func MultiReturn(int a, int b): int, bool {
    return a * b, int
}

// Functions can also be defined without a return type
public func NoReturn(int a, int b): {
	// Implementation
}

// Functions can also take have tuple parameters and return tuple values
public func TupleParams((int, string) tuple): (int, string) {
	return tuple
}
```

### Function with Default Parameters
```
public func Sum(int a, int b = 0): int {
	return a + b
}
```

### Function Invocation
```
int result = Sum(5, 10)
```
### Function Invocation with Named Parameters
```
int result = Sum(b: 5, a: 10)
```

### Function with Multiple Return Values
```
// Option 1: Define types inline
(int product, bool success) = MultiReturn(5, 10)

// Option 2: Define types separately
int product
bool success
(product, success) = MultiReturn(5, 10)
```

### Function Overloading
```
public func Sum(int a, int b): int {
	return a + b
}

public func Sum(int a, int b, int c): int {
	return a + b + c
}
```

### Variadic Functions
```
public func Sum(int... numbers): int {
	int sum = 0
	for (int number in numbers): {
		sum += number
	}
	return sum
}
```

### Closure Support
```
public func ClosureExample() {
	int a = 5
	int b = 10
	int result = 0

	// Closure
	func Add(int x, int y) {
		result += x + y
	}

	Add(a, b) // result = 15
	Add(a, b) // result = 30
}
```

### Explicit Casting
```
39.8::int // 32.8 becomes an int with a value 39
123::float // 123 becomes a float with a value of 123.0
123::string // 123 becomes a string with a value oif "123"
myObject::MyClass() // myObject type in the context becomes MyClass if it makes sense, type errors are caught at compile time
```

### String Interpolation
```
string name = "John"
int age = 30
string greeting = "Hello, $name. You are $age years old."
```

### Null Coalescing Operator
```
string? name = null
string displayName = name ?? "Unknown"
```
### Null Safe Operator
```
string? name = null
string displayName = name?.Trim() // Can be used to safely call methods on nullable objects, returns null if name is null
```

### Casting Objects
```
interface IPrintable {
	func Print()
}

class Person: IPrintable {
	public string name

	public func Person(string name) {
		this.name = name
	}

	public func Print() {
		Print("Name: " + this.name)
	}
}

class Employee: Person {
	public string department

	public func Employee(string name, string department) : super(name) {
		this.department = department
	}

	public func Print() {
		super.Print()
		Print("Department: " + this.department)
	}
}

// Usage
Person person = new Person("John")
Employee employee = new Employee("Alice", "Sales")

// Check if person is an Employee
if (person is Employee) {
	Employee e = person::Employee // Can cast as we checked if person is an Employee
	e.Print()
}

// The following line will result in a compile-time error
Employee e = person::Employee // Cannot cast as we did not check if person is an Employee

// Safe casting
Employee? e = person::?Employee // e will be null as person is not an Employee
if(e != null) {
	e.Print()
}
```
### Function with Nullable Parameters
```
func ProcessInput(string? input): string? {
	if (input == null) return null
	return input.Trim()
}
```

### Null Safe Casting
```
int? nullableInt = GetNullableInt()
int nonNullableInt

if (nullableInt != null) {
    nonNullableInt = nullableInt::int // Safe explicit cast, given the null check, can also use the null coalescing ?? operator here
} else {
    nonNullableInt = 0 // Default value or other handling
}
```

### Invoking Functions with Nullable Arguments
```
string? userInput = GetUserInput() // This might return null
ProcessInput(userInput) // No need for explicit null handling here, handled inside the function
```

### Control Structures

#### If Statement
```
if (condition) {
    // statements
}

// {} optional for single statement
if (condition)
	DoSomething()

if (condition) {
    // statements
} else {
	// Do something else, {} also optional for single statements
}
```

#### loops
```
// for loop
for (int i = 0; i < 10; i++) {
    // loop body
}

// foreach loop
for (int number in numbers) {
	// loop body
}

// while loop
while (condition) {
	// loop body
}

// do while loop
do {
	// loop body
} while (condition)
```

#### Break and Continue
```
for (int i = 0; i < 10; i++) {
	if (i == 5) break // Exit the loop
	if (i == 3) continue // Skip the current iteration
}
```

#### Ternary Operator
```
int age = 20
string category = age >= 18 ? "Adult" : "Minor"
```

#### match statement
```
// match statement with primitive types
match number {
	1 => HandleOne(),
	2 => HandleTwo(),
	_ => HandleDefault(number)
}

// match statement with more complex types
match object {
    TypeA varA => HandleTypeA(varA),
    TypeB varB => HandleTypeB(varB),
    _ => HandleDefault(object)
}
```

#### match integration with if statements
```
class Message {
    public string type
    public string content
}

// Example messages
Message welcomeMessage = { type: "welcome", content: "Welcome to Arcana!" }
Message logoutMessage = { type: "logout", content: "Goodbye!" }

// Function to process messages
public func ProcessMessage(Message message) {
    if match message {
        Message { type: "welcome" } => true,
        Message { type: "logout" } => true,
        _ => false
    } {
        Print("Processing message: " + message.content)
    } else {
        Print("Unhandled message type.")
    }
}


// match statement with guard clauses
match user {
    User u if u.isActive && Now() - u.lastLogin < Duration(7) => Print("Active recently"),
    User u if !u.isActive => Print("Inactive user"),
    _ => Print("Other cases")
}
```

### Class Definition
```
class Person {
    string name // default value optional
    int age = 9 

    public func Person(string name, int age) {
        this.name = name
        this.age = age
    }

    public func Describe() {
        Print("Name: " + this.name + ", Age: " + this.age)
    }
}
```

### Extending a Parent Class and Constructor Chaining
```
class Employee: Person {
    string department

    public func Employee(string name, int age, string department) : super(name, age) { // Constructor chaining
        this.department = department
    }
}
```

### Access Modifiers
```
public class MyClass {
    // Implementation
}

private class HelperClass {
    // Implementation
}

class MyService {
    public func PublicFunction() {
        // Accessible from anywhere
    }

    private func PrivateFunction() {
        // Accessible only inside MyService class
    }

    protected func ProtectedFunction() {
        // Accessible inside MyService class and subclasses
    }

    internal func InternalFunction() {
        // Accessible within the same module
    }
}
```

### Object Instantiation
```
Person person = new Person("John", 30) // Calls the Person constructor
```

### Object Initializer Syntax
```
Person person = { name: "John", age: 30 } // Calls the Person constructor
```
### Sealed Class
```
sealed class PaymentMethod {
	public func ProcessPayment() {
		// Implementation
	}
}

// This will result in a compile-time error
class CreditCard: PaymentMethod {
	// Implementation
}
```

### Static Class
```
static class MathHelper {
	public static func Add(int a, int b): int {
		return a + b
	}
}

// Usage
int result = MathHelper.Add(5, 10) // No instance required

// The following line will result in a compile-time error
MathHelper helper = new MathHelper() // Cannot instantiate a static class
```

### Basic Scope
```
public func ScopeExample() {
    int outerVariable = 20

    if (outerVariable > 10) {
        int innerVariable = 5
        Print("Inner Variable: " + innerVariable)  // Works fine
    }
    
    // Trying to access innerVariable here will result in an error
    // Print("Trying to access Inner Variable: " + innerVariable)  // Error: innerVariable is not accessible here.
}
```

### Function scope
```
public func FunctionScopeExample() {
	int a = 5
	int b = 10

	func Add(int x, int y): int {
		return x + y
	}

	int result = Add(a, b) // result = 15
}
```

### Shadowing
```
public func ShadowingExample() {
	int a = 5
	int b = 10

	if (a > 0) {
		int a = 20 // This is not the same 'a' as the one outside the block
		Print("Inner a: " + a)  // Inner a: 20
	}

	Print("Outer a: " + a)  // Outer a: 5
}
```

### Block Scope
```
public func BlockScopeExample() {
	int a = 5
	int b = 10

	{
		int a = 20 // This is not the same 'a' as the one outside the block
		Print("Inner a: " + a)  // Inner a: 20
	}
}
```

### Global Variables Scope
```
class GlobalVariables {
	public static int globalCount = 0
	private static string globalName = "Global"
}
```

### Global Functions
```
// Accessible from anywhere
public static func GlobalFunction() {
	// Implementation
}

// Accessible only inside the same class
private static func PrivateFunction() {
	// Implementation
}

// Accessible inside the same class and subclasses
protected static func ProtectedFunction() {
	// Implementation
}

// Accessible only inside the same module
internal static func InternalFunction() {
	// Implementation
}
```

### Interface Definition
```
interface Drawable {
    public func Draw()
    func SetColor(string color) // Access modifier optional, implicitly public
}

interface Shape {
	func GetName() str
}
```

### Implementing an Interface
```
class Circle: Drawable, Shape {
    public func Draw() {
        // implementation
    }

    public func SetColor(string color) {
        // implementation
    }

	public func GetName(): string {
        // implementation
    }
}
```

### Abstract Class and Function Syntax
```
abstract class Shape {
    string color

    public abstract func Draw() // Abstract method, no implementation

    public func SetColor(string color) { // Concrete method
        this.color = color
    }
}
```

### Virtual and Abstract Methods
```
class Shape {
	public virtual func Draw() {
		// Default implementation, can be overridden
	}

	public abstract func GetName() string // Abstract method, no implementation. Must be provided by subclass.
}
```

### Overriding Methods
```
class Circle: Shape {
	// No keyword needed, just match the method signature
	public func Draw() {
		// Implementation
	}
}
```

### Class Properties
```
class Person {
    public string name = "Unknown"
        get() = field  // Use 'field' to refer to the backing field
        set(value) {
            field = value.Trim()  // 'field' refers to the internal backing field
        }
}

class Circle {
    public float radius = 1.0
    public float diameter
        get() = field * 2  // 'field' refers to the radius's backing field
}

class User {
    public string email = ""
        get() = field
        set(value) {
            if (value.Contains("@")) field = value  // Validate and set the email
            else Print("Invalid email address.")
        }
}
```

### Variable Destructuring from Objects
```
Message message = { type: "update", content: "New updates available!" }
{ string type: messageType, string content: messageContent } = message
print("Received " + messageType + ": " + messageContent)
```

### Tuple Destructuring
```
(int a, int b) tuple1 = (1, 2)
(int id, string name) tuple2 = SomeFunction()
```

### Function Arguments Destructuring
```
func Greet(Message { string type: messageType, string content: messageContent }) {
    Print("Message Type: " + messageType + ", Content: " + messageContent)
}
```

### Loop Destructuring
```
List<Message> messages = [{ type: "welcome", content: "Hello!" }, { type: "bye", content: "Goodbye!" }]
for ({ string type: messageType, string content: messageContent } in messages) {
    print(messageType + ": " + messageContent)
}
```

### structs
```
struct Point {
    int x
    int y
}

// Usage
Point p = { x: 10, y: 20 }

// Accessing struct fields
int x = p.x

// Struct comparison
Point p1 = { x: 10, y: 20 }
Point p2 = { x: 10, y: 20 }
Point p3 = { x: 20, y: 30 }
bool areEqual = p1 == p2  // true
bool areEqual = p1 == p3  // false
```

### Namespace Declaration
```
namespace Utilities {
	// class definition
}

class Logger {
    public func Log(string message) {
        // implementation
    }
}
```

### Using Namespaces
```
//Option 1: Fully Qualified Name
Utilities.Logger logger = Utilities.Logger()
logger.Log("This is a log message.")

//Option 2: Import Statement
import Utilities

Logger logger = Logger()
logger.Log("This is a log message.")
```

### Constants
```
class MathConstants {
    public const float Pi = 3.14159
    public const float E = 2.71828
}

// Usage
float circumference = 2 * MathConstants.Pi * radius
float exponential = MathConstants.E ^ x

// Constants are immutable and cannot be changed after initialization
MathConstants.Pi = 3.14 // Error: Cannot change the value of a constant
```

### Read-Only Variables
```
class Circle {
	readonly float radius
	readonly float area

	public func Circle(float radius) {
		// Initialize the read-only variables, can only be done in the constructor once
		this.radius = radius
		this.area = MathConstants.Pi * radius * radius

		// Even though area has already been initialized, we are still in the constructor so we can modify it
		this.area = 100

		// Even though CalculateArea() is called in the constructor, area cannot be modified outside of the constructor.
		// The proper way would be to have CalculateArea() return the area and then set it in the constructor
		CalculateArea()
	}

	public func CalculateArea() {
		// The following line will result in a compile-time error
		this.area = MathConstants.Pi * radius * radius
	}
}

// Usage
Circle circle = new Circle(5)
float area = circle.area
```

### Error Handling
```
try {
    // risky operation
} catch (ExceptionTypeA e) {
    // handle exception of type ExceptionTypeA
} catch (ExceptionTypeB e) {
    // handle exception of type ExceptionTypeB
} finally {
	// Always run afterwards regardless if there was an exception
}
```

### Defining Custom Exceptions
```
class FileNotFoundException: Exception {
    public func FileNotFoundException(string message) {
        super(message) // Call the base class constructor with the message
    }
}

class NetworkException: Exception {
    float code // Additional properties specific to this Exception

    public func NetworkException(string message, float code) {
        super(message)
        this.code = code
    }
}
```

### Throwing Exceptions
```
func ReadFile(string path) {
    if (!FileExists(path)) {
        throw FileNotFoundExceptions("File not found: " + path)
    }
    // Read file logic
}
```

### Lambda Expressions
```
// No return value
(int a, int b) => Print("a: " + a + ", b: " + b)
// Single return value
int add = (int a, int b) int => a + b
// Multiple return values
{ int sum, int product } = (int a, int b) (a + b, a * b)
```

### Lambda Expressions with Closures
```
func Main() {
	int a = 5
	int b = 10
	int result = 0

	// Closure
	(int x, int y) => result = x + y

	result = Add(a, b) // result = 15
}

// Usage
import TextUtilities.extension string.toTitleCase
```
### Generics Syntax
```
class List<T> {
    // Implementation of a generic list
    public func Add(T item) {
        // Add item to the list
    }

    public T func Get(int index) {
        // Return item at index
    }
}
```

### Using Generics
```
List<int> numbers = List<int>()
numbers.Add(1)
int firstNumber = numbers.Get(0)
```

### Basic Enum Declaration
```
enum Color {
    Red, // Implicitly 0
    Green, // Implicitly 1
    Blue // Implicitly 2
}
```

### Enum with Custom Values
```
enum HttpStatusCode int {
    Ok = 200,
    NotFound = 404,
    InternalServerError = 500
}

enum UserRole string {
    Admin = "Administrator",
    User = "Standard User",
    Guest = "Guest"
}
```

### Enum Usage
```
HttpStatusCode status = HttpStatusCode.Ok
UserRole role = UserRole.Admin
```

### Polymorphism with Enums
```
enum PrintableColor: IPrintable {
    Red = new ColorValue("Red"),
    Green = new ColorValue("Green"),
    Blue = new ColorValue("Blue")

    // ColorValue is a class that implements IPrintable
    class ColorValue: IPrintable {
        string colorName

        public func ColorValue(string name) {
            this.colorName = name
        }

        public func PrintColor() {
            Print("Color: " + this.colorName)
        }
    }
}
```

### Operator Overloading
```
// Arithmetic operators
class Complex {
    float real
    float imaginary

    // Constructor
    public func Complex(float real, float imaginary) {
        this.real = real
        this.imaginary = imaginary
    }

    // Operator Overloading for '+' (Addition)
    operator +(Complex a, Complex b) Complex {
        return new Complex(a.real + b.real, a.imaginary + b.imaginary)
    }

    // Overloading the '-' (Subtraction) operator
    operator -(Complex a, Complex b) Complex {
        return new Complex(a.real - b.real, a.imaginary - b.imaginary)
    }
}

// Usage
Complex c1 = new Complex(1, 2)
Complex c2 = new Complex(3, 4)
Complex sum = c1 + c2  // Uses the overloaded '+' operator
Complex diff = c1 - c2 // Uses the overloaded '-' operator

// Comparison operators
class Person {
	string name
	int age

	public func Person(string name, int age) {
		this.name = name
		this.age = age
	}

	// Overloading the '==' (Equality) operator
	operator ==(Person a, Person b) bool {
		return a.name == b.name && a.age == b.age
	}

	// Overloading the '!=' (Inequality) operator
	operator !=(Person a, Person b) bool {
		return !(a == b)
	}
}

// Usage
Person p1 = new Person("John", 30)
Person p2 = new Person("John", 30)
Person p3 = new Person("Alice", 25)

bool areEqual = p1 == p2 // true
bool areEqual = p1 == p3 // false

// [] (Indexing) operator

class Matrix {
	int[][] matrix

	public func Matrix(int[][] matrix) {
		this.matrix = matrix
	}

	// Overloading the [] (Indexing) operator
	operator [](int i, int j) int {
		return this.matrix[i][j]
	}

	// Overloading the [] (Indexing) operator for assignment
	operator [](int i, int j, int value) {
		this.matrix[i][j] = value
	}
}

// Usage
Matrix matrix = new Matrix(new int[][] { { 1, 2 }, { 3, 4 } })
int value = matrix[0, 1] // value = 2
matrix[0, 1] = 5 // matrix = { { 1, 5 }, { 3, 4 } }
```

### Delegates
```
delegate (int, float) -> (bool, string) FunctionDelegateName // Creates a delegate type with a signature of (int, float) -> (bool, string)
```
### Delegate Usage
```
func MyDelegate(int a, float b): bool, string {
    // Implementation goes here
}

FunctionDelegate myDelegate = compareNumbers
```

### Delegate Invocation
```
bool result, string message
result, message = MyDelegate(5, 3.14)
```

### Delegate an Lambda functions
```
FunctionDelegate myDelegate = (int a, float b) => (a > b, "a is greater than b")
```
### Delegate as Function Parameter
```
func ProcessNumbers(int a, float b, FunctionDelegate delegate) {
	bool result, string message = delegate(a, b)
	Print(message)
}
```

### Delegate with Generics
```
delegate <T, U> (T) -> (U) GenericDelegate
```

### First class events
```
event FunctionDelegate myEvent

// Subscription
myEvent += SomeFunction

// Unsubscription
myEvent -= SomeFunction

// Raising the event
MyEvent(5, 3.14)
```

### Async/Await
```
async func FetchData(string url): Data {
    // Implementation that fetches data asynchronously
}

async func process() {
    Data data = await FetchData("http://example.com")
    Print("Data processed: " + data)
}

async func SafeProcess() {
    try {
        Data data = await FetchData("http://example.com")
        Print("Data processed: " + data)
    } catch (NetworkError e) {
        Print("Failed to fetch data: " + e.message)
    }
}
```

### Promises Syntax
```
// Definition of a Promise
promise<T> func FetchData(string url) {
    // Implementation that eventually resolves with T or rejects with an error
}

FetchData("http://example.com")
    .then(Data data => ProcessData(data))
    .then(ProcessedData processed => Print("Data: " + processed))
    .catch(NetworkError e => Print("Error: " + e.message))
```

### Integration of Promises with Generics
```
// Async function that returns a promise of type Data
async func FetchData(string url): promise<Data> {
    // Implementation
}

FetchData("http://example.com")
    .then(Data data => Print("Fetched data: " + data))
    .catch(Error e => Print("Fetch error: " + e.message))
```

### Foreign Function Interface (FFI) for language interoperability
```
// Syntax
foreign func <function_name>(<parameters>) -> <return_type> from "<language>:<library_or_function_path>"

// Examples

// Import a Python function
foreign func py_sleep(seconds: float) from "python:time.sleep"

func Main() {
    Print("Waiting for 2 seconds...")
    py_sleep(2) // Calls Python's time.sleep(2)
    Print("Done waiting.")
}

// Import a C++ function
foreign func cpp_sort(array: ref<int[]>, length: int) from "cpp:std::sort"

func Main() {
    int[] numbers = [3, 1, 4, 1, 5, 9, 2, 6]
    cpp_sort(ref numbers, numbers.length)
    Print("Sorted numbers: " + numbers)
}

// Import a C# method
foreign func cs_getCurrentDateTime() -> string from "csharp:System.DateTime.Now.ToString"

func Main() {
    string currentDateTime = cs_getCurrentDateTime() // Calls C# System.DateTime.Now.ToString()
    Print("Current Date and Time: " + currentDateTime)
}
```

### Creating Extensions
```
// Suppose the following class is defined in a library
namespace ShapeLibrary {
	class Circle {
		float radius

		public func Circle(float radius) {
			this.radius = radius
		}

		public func GetArea(): float {
			return MathConstants.Pi * this.radius * this.radius
		}
	}
}

// We can create an extension module to add mew fields, properties, and methods to the Circle class
namespace ShapeLibrary { // Namespace does not have to match the original namespace, but it is recommended
	extension ShapeLibrary.Circle {
		// New field
		public float circumference

		// New property
		public float Circumference {
			get() = 2 * MathConstants.Pi * this.radius
		}

		// New method
		public func GetCircumference(): float {
			return 2 * MathConstants.Pi * this.radius
		}

		// The following line will result in a compile-time error
		public func GetArea(): float {
			return 2 * MathConstants.Pi * this.radius
		}

		// However, with a different signature, it will work
		public func GetArea(float radius): float {
			return 2 * MathConstants.Pi * radius
		}
	}
}

// Usage
import ShapeLibrary // Import the library
import extension ShapeLibrary.Circle // Import the extension module

Circle circle = new Circle(5)
float area = circle.GetArea() // area = 78.53981633974483
float circumference = circle.GetCircumference() // circumference = 31.41592653589793

// Hypothetical syntax for adding interfaces to existing classes
namespace ShapeLibrary {
	interface IShape {
		func GetCircumference(): float
	}
	extension ShapeLibrary.Circle: IShape {
		// Need to implement the interface fully, otherwise it will result in a compile-time error
		public func GetCircumference(): float {
			return 2 * MathConstants.Pi * this.radius
		}
	}
}

// Usage
import ShapeLibrary
import extension ShapeLibrary.Circle

Circle circle = new Circle(5)
IShape shape = circle::IShape // Cast to IShape
```

### Annotations
```
// Basic Annotation(No Attributes)
:export
func MyFunction() {
    // Function implementation
}

// Annotation with Parameters
:range(min=0, max=100)
int percentage
```
### Defining Custom Annotations
```
annotation MyAnnotation {
    string description = "Default description"
    int priority = 5
}

// Usage
:MyAnnotation(description="This is a special function", priority=10)
func SpecialFunction() {
    // Function implementation
}

:range(min=0, max=100)
int percentage

:doc("This function performs an essential task.")
func EssentialTask() {
    // Task implementation
}

:serialize
:doc("Represents a user in the system.")
class User {
    // User implementation
}
```

### Exporting Library Elements
```
:export
class FinancialCalculator {
    int lastResult = 0

    :export
    public func Add(int a, int b): int {
        lastResult = a + b
        return lastResult
    }

    func PrivateHelper() {
        // This method is not exported
    }
}
```

### Grammar in Extended Backus-Naur Form (Not finished)
```
// The following is the grammar for the Arcana language in Extended Backus-Naur Form (EBNF)

//program = { import_statement } , { whitespace } , ( namespace_declaration | class_declaration ) ;

program = {{statement} , { whitespace }} ;

import_statement = "import" , spaces , identifier , { "." , identifier } ;
namespace_declaration = "namespace" ;

class_declaration =  [access_modifier, whitespace], [class_modifier, whitespace], "class", whitespace, identifier, whitespace, [generic_declaration], [class_inheritance], class_body ;
class_inheritance = ":", whitespace, identifier, {whitespace, ",", whitespace, identifier};
class_body = "{", { whitespace }, { class_member }, { whitespace }, "}" ;
access_modifier = "public" | "private" | "protected" | "internal" ;
class_modifier = "static" | "sealed" | "abstract" ;
class_member = ( field_declaration | property_declaration | function_declaration ) ;

field_declaration = [access_modifier, whitespace], [field_modifier, whitespace], type, whitespace, identifier, {whitespace}, [ "=", {whitespace}, expression ];
field_assignment = identifier, whitespace, "=", whitespace, expression;
field_modifier = "readonly" ;

property_declaration = [access_modifier, whitespace], type, whitespace, identifier, whitespace, [ "=", {spaces}, expression ], whitespace, property_body;
property_body = "{", whitespace, getter_declaration, [whitespace, setter_declaration], whitespace, "}" | "{", whitespace, setter_declaration, [whitespace, getter_declaration], whitespace, "}";
getter_declaration = "get", whitespace, "()", whitespace, "=", whitespace, getter_body;
setter_declaration = "set", whitespace, "(", "value", ")", "{", whitespace, setter_body, whitespace, "}" ;
getter_body = expression | getter_custom_body;
setter_body = setter_custom_body;
getter_custom_body = "{", whitespace, "return", whitespace, expression, ";", whitespace, "}" | "field";
setter_custom_body = "{", whitespace, ["if", whitespace, "(", expression, ")", whitespace], "field", whitespace, "=", whitespace, expression, ";", whitespace "}";

function_declaration =  [access_modifier, spaces], [function_modifier, spaces], "func", spaces, identifier, [generic_declaration], [parameter_list], [return_type], [function_body] ;
function_modifier = "async" | "static" | "abstract" | "virtual" ;
function_body = "{", { whitespace }, { statement }, { whitespace }, "}" ;
function_call = qualified_identifier, "(", [ expression { "," expression } ], ")" ;

parameter_list = "(", [parameter, { ",", parameter }], ")" ;
parameter = [parameter_modifier, spaces], type, spaces, identifier ;
parameter_modifier = "ref" | "val" ;
return_type = type, { whitespace, type } ;

statement = field_declaration | field_assignment | function_call | throw_statement | if_statement ;
single_or_block = block | statement ;
block = "{", { whitespace }, { statement }, { whitespace }, "}" ;
if_statement = "if", whitespace, "(", whitespace, expression, whitespace, ")", whitespace, single_or_block, {whitespace, "else if", whitespace, "(", whitespace, expression, whitespace, ")", whitespace, single_or_block}, [whitespace, "else", whitespace, single_or_block] ;
throw_statement = "throw", whitespace, expression ;

expression = 	literal | identifier | "(" expression ")" | unary_expression | 
				binary_expression | ternary_expression |
				array_expression | expression "." identifier |
				identifier "(" [ expression { "," expression } ] ")" |
				object_initialization |
				expression as_op type | "(" type ")" expression;

qualified_identifier = identifier, { ".", identifier } ;

unary_expression = unary_op, whitespace, expression;
binary_expression = expression, whitespace, binary_op, whitespace, expression;
ternary_expression = expression, whitespace, "?", whitespace, expression, whitespace, ":", whitespace, expression;

object_initialization = "new" type "(" [ expression { "," expression } ] ")" ;
array_expression = array_declaration | array_initialization | array_index ;
array_declaration = "new", whitespace, type, whitespace, "[", expression, "]", { "[", expression, "]" } ;
array_initialization = "{", [expression, { ",", expression }], "}" ;
array_index = expression, "[", expression, "]" ;

relational_operator = "==" | "!=" | "<" | ">" | "<=" | ">=";

type = identifier, [generic_declaration] ;
type_cast = expression, "::", type ;
generic_declaration = "<", identifier, { ",", identifier }, ">" ;

identifier = letter , { letter | digit | "_" } ;
string_literal = '"' , { letter | number | single_space | symbol } , '"' ;
literal = string_literal | number | boolean | "null" | tuple_literal ;
tuple_literal = "(", whitespace, [identifier, ":"], whitespace, expression, { whitespace, ",", whitespace, [identifier, ":"], whitespace, expression }, whitespace,")" ;

operator = "+" | "-" | "*" | "/" ;
letter = 	"A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" | 
			"N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" | 
			"a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" | 
			"n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" ;
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;
fractional = ".", digit, { digit } ;
number = digit, { digit }, [ fractional ];
boolean = "true" | "false" ;
unary_op = "-" | "+" | "!" | "~" | "++" | "--";
binary_op = "+" | "-" | "*" | "/" | "%" | "==" | "!=" | ">" | "<" | ">=" | "<=" |
			"&&" | "||" | "&" | "|" | "^" | "<<" | ">>";
as_op = "as";
spaces = single_space { single_space } ;
single_space =  " " ;
whitespace = { single_space | spaces | "\n" | "\t" | "\r" | "\f" | "\b" | line_break } ;
line_break = "\n" | "\r\n";
tab = "\t" ;
symbol = "[" | "]" | "{" | "}" | "(" | ")" | "<" | ">"
       | "'" | '"' | "=" | "|" | "." | "," | ";" | "-" 
       | "+" | "*" | "?" | "\n" | "\t" | "\r" | "\f" | "\b" ;
```

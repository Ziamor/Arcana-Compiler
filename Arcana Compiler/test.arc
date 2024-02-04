import importName
import some.name.space

// Default namespace implied
class Person {
	string name // default value optional
	int age = 9 

	public func Person(string name, int age) {
	}

	func Describe() : int, obj{
	}
}
namespace Utilities {
	class Person {
		int count = 10
		string name = "John"
		float balance = 100.50

		int? nullableInt = 5
		string? nullableString = 5
		Person? nullableObject = null
	}

	// Implement an interface
	class Person: IPrintable {
		
	}

	class Person {
		string name // default value optional
		int age = 9 

		public func Person(string name, int age) {
			int someVar // declaration only
			int someOtherVar = 4 // declaration + assignment
			if(some.name.space.cond) {
				someVar = 2  // assignment
				someVar = 6 // assignment
			}
		}

		func Describe() : int, obj{
		}
	}
}
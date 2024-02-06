// Test different import formats
import single.level
import multi.level.path

// Basic class with properties and constructor
class Vehicle {
    string make
    int year = 2020

    public func Vehicle(string make, int year) {
        // Constructor logic
    }

    func getDescription() : string {
        // Method returning a string
    }
}

namespace Models {
    // Class without default values, implementing an interface
    class Car: IVehicle {
        string model
        int year

        public func Car(string model, int year) {
            // Constructor logic
        }

        func drive() {
            // Method with no return value
        }
    }

    // Class with nullable properties and method overloading
    class ElectricCar: Car {
        float? batteryLevel = null

        func chargeBattery(float amount) {
            // Overloaded method
        }

        func chargeBattery(float amount) : int {
            // Overloaded method
        }

        func chargeBattery() {
            // Overloaded method without parameters
        }
    }
}

class Calculator {
    int result = 0

    func calculate(string expression) : int {
        // Complex conditional logic
        if (expression.contains("+")) {
            result = expression.split("+").map(toInt).sum()
        } else if (expression.contains("-")) {
            result = expression.split("-").map(toInt).reduce(subtract)
        }
        return result
    }

    private func toInt(string number) : int {
        // Conversion logic
    }

    private func subtract(int a, int b) : int {
        // Subtraction logic
    }
}

namespace interfaces {
    interface IVehicle {
        func drive()
        func getYear(): int
    }

    public class ElectricCar : IVehicle {
        int? batteryLevel
        int year

        func ElectricCar(int year) {
            this.year = year
        }

        func drive() {
            // Implementation
        }

        func getYear(): int {
            return year
        }
    }
}

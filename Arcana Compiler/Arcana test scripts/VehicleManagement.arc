import common.utilities.Utility

namespace vehicle.management {
    public class Vehicle {
        int manufactureYear
        Utility utility = new Utility()

        func Vehicle(int year) {
            this.manufactureYear = year
        }

        func getManufactureYear(): int {
            return manufactureYear
        }
    }
}
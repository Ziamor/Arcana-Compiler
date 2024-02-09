import common.utilities.Utility

namespace vehicle.management {
    class Vehicle {
        int manufactureYear
        Utility utility = new Utility()

        func Vehicle(int year) {
            manufactureYear = year
        }

        func getManufactureYear(): int {
            return manufactureYear
        }
    }
}
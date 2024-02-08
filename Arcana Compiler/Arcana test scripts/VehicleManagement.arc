import common.utilities.Utility

namespace vehicle.management {
    class Vehicle {
        int manufactureYear
        Utility utility

        func Vehicle(int year) {
            manufactureYear = year
            utility = new Utility()
        }

        func getManufactureYear(): int {
            return manufactureYear
        }
    }
}
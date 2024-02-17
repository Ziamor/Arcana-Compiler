namespace vehicle {
    public class Transmission {
        int gears

        func Transmission(int gearCount) {
            this.gears = gearCount
        }

        func getGears(): int {
            return gears
        }
    }
}
namespace vehicle {
    public class Sedan {
        Transmission transmission

        func Sedan(int gearCount) {
            this.transmission = new Transmission(gearCount)
        }

        func getTransmissionGears(): int {
            return transmission.getGears()
        }
    }
}
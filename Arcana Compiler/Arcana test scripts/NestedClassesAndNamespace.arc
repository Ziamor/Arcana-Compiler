import common.utilities.Utility

namespace vehicle {
    public class Car {
        int year
        Engine engine

        func Car(int year) {
            this.year = year
            engine = new Engine(200)
        }

        class Engine {
            int horsepower

            func Engine(int hp) {
                horsepower = hp
            }

            func getHorsepower(): int {
                return horsepower
            }
        }
    }
}

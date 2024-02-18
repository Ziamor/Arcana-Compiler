namespace common {
    public static class StringUtils {
        public static func reverse(string text): string {
            // Simple method to reverse a string
            string reversedText = ""
            for(char in text) {
                reversedText = char + reversedText
            }
            return reversedText
        }

        public static func capitalizeWords(string text): string {
            // Method to capitalize the first letter of each word in a string
            string words = text.split(" ")
            //string[] capitalizedWords = []
            for(int i = 0; i < 5; i++) {
                //string capitalizedWord = word[0].toUpperCase() + word.substring(1)
                capitalizedWords.append(capitalizedWord)
            }
            return capitalizedWords.join(" ")
        }

        public static func isPalindrome(string text): bool {
            // Method to check if a string is a palindrome
            bool reversedText = reverse(text) // Utilizing the reverse function defined above
            return text == reversedText
        }
    }
}

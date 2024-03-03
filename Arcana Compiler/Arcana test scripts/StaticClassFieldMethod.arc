namespace common {
    public static class StringUtils {
        public static func reverse(string text): string {
            // Simple method to reverse a string
            string reversedText = ""
            for(char c in text) {
                reversedText = c + reversedText
            }
            return reversedText
        }

        public static func capitalizeWord(string word): string {
            // Simplified method to capitalize just the first letter of a single word
            if(word.length > 0) {
                return word[0].toUpperCase() + word.substring(1)
            }
            return word
        }

        public static func isPalindrome(string text): bool {
            // Method to check if a string is a palindrome
            string reversedText = reverse(text) // Utilizing the reverse function defined above
            return text == reversedText
        }
    }
}
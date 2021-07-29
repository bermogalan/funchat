using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security;

namespace Kritner.OrleansGettingStarted.Client.Helpers
{
    public static class ConsoleHelpers
    {
        public static void WelcomeToFunChat()
        {
            Console.Clear();
            Console.WriteLine($"Welcome to Fun Chat!");
            LineSeparator();
        }

        public static void WelcomeToFunChatUser(string userName)
        {
            Console.Clear();
            Console.WriteLine($"Hi {userName} to start using Fun Chat, please choose from the menu");
            LineSeparator();
        }

        public static void LineSeparator()
        {
            Console.WriteLine($"{Environment.NewLine}-----{Environment.NewLine}");
        }

        public static void DisplayOptionsAskWhatToDo()
        {
            ConsoleHelpers.LineSeparator();
            Console.Write("What do you want to do? ");
        }

        public static void DisplayErrorMessage(string message) 
        {
            var originalForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = originalForegroundColor;
        }

        public static void ReturnToMenu()
        {
            LineSeparator();
            Console.WriteLine("Press any key to return to menu");
            Console.ReadKey();
            Console.Clear();
        }

        public static string GetUserName()
        {
            Console.Write("Enter Username:");
            var input = Console.ReadLine();

            while (!IsValidUserName(input)) 
            {
                WelcomeToFunChat();
                DisplayErrorMessage("Username must be alphanumeric and must be 3-10 characters only.");
                Console.Write("Enter Username:");
                input = Console.ReadLine();
            }

            //validate if username is alphanumeric
            return input;
        }

        public static string GetChannelName()
        {
            Console.Write("Enter Channel:");
            var input = Console.ReadLine();
            //validate if username is alphanumeric
            return input;
        }

        public static ConsoleKeyInfo GetConfirmation(string message)
        {
            Console.Write(message);
            var input = Console.ReadKey();
            //validate if username is alphanumeric
            return input;
        }

        public static bool IsValidUserName(string userName)
        {
            return userName.All(char.IsLetterOrDigit) && userName.Length >= 3 && userName.Length <= 10;
        }

        public static bool GetPassword(string userName)
        {
            var input = getPasswordFromConsole("Enter Password:");

            while (!IsValidPassword(userName, input.ToString()))
            {
                DisplayErrorMessage("Invalid password!");
                input = getPasswordFromConsole("Enter Password:");
            }

            //validate if username is alphanumeric
            return true;
        }

        public static string GetChatRoomPassword()
        {
            var input = getPasswordFromConsole("Enter Chat Room Password:");

            while (!IsValidChatRoomPassword(input.ToString()))
            {
                DisplayErrorMessage("Invalid password!");
                input = getPasswordFromConsole("Enter Chat Room Password:");
            }

            //validate if username is alphanumeric
            return input;
        }

        public static string getPasswordFromConsole(String displayMessage)
        {
            var pass = string.Empty;
            Console.Write(displayMessage);
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            return pass;
        }

        public static bool IsValidPassword(string userName, string password)
        {
            return userName == password;
        }

        public static bool IsValidChatRoomPassword(string password)
        {
            return password.Length >= 6 && password.Length <= 18 && password.All(char.IsLetterOrDigit);
        }
        public static string GenerateRandomAlphaNumericString(int length)
        {
            //Characters used in for Generating AlphaNumeric String
            var chars = "0123456789qazwsxedcrfvtgbyhnujmiklopQAZWSXEDCRFVTGBYHNUJMIKLOP";
            var random = new Random();
            var Result = new string(
            Enumerable.Repeat(chars, length) //Here "length" is the length of the alphaNumeric String
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
            return Convert.ToString(Result);
        }
    }
}

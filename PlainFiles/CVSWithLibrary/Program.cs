// Program.cs
using CVSWithLibrary;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Globalization; // Required for CultureInfo.CurrentCulture.TextInfo.ToTitleCase

// --- GLOBAL CONFIGURATION ---
// User file handler instance
var userHandler = new UserFileHandler();
const string usersFilePath = "Users.txt";
// List of users loaded from file (or created if it doesn't exist)
List<User> users = userHandler.ReadUsers(usersFilePath);

// Person file handler instance
var personHelper = new CvsHelperExample(); // Your helper for people.csv
List<Person> people = personHelper.read(); // Read person data

User? loggedInUser = null; // Variable for the currently logged-in user
const int MAX_LOGIN_ATTEMPTS = 3; // Login attempt limit

// --- LOGGING ---
Logger.Log("Application started.");

// --- INITIAL AUTHENTICATION ---
// This loop repeats until a user successfully authenticates.
while (loggedInUser == null)
{
    Console.Clear();
    Console.WriteLine("=======================================");
    Console.WriteLine("           MANAGEMENT SYSTEM           ");
    Console.WriteLine("=======================================");
    Console.WriteLine("\n--- LOGIN ---");
    Console.Write("Username: ");
    string? usernameInput = Console.ReadLine();
    Console.Write("Password: ");
    string? passwordInput = ReadPassword(); // Function to read password without displaying it
    Console.WriteLine();

    User? userAttempt = users.FirstOrDefault(u => u.Username.Equals(usernameInput, StringComparison.OrdinalIgnoreCase));

    if (userAttempt == null)
    {
        Console.WriteLine("User not found. Please try again.");
        Logger.Log($"Login attempt failed for non-existent user: '{usernameInput}'", "System");
        System.Threading.Thread.Sleep(1500);
        continue;
    }

    if (!userAttempt.IsActive)
    {
        Console.WriteLine($"User '{userAttempt.Username}' is blocked. Contact administrator.");
        Logger.Log($"Login attempt failed for blocked user: '{userAttempt.Username}'", userAttempt.Username);
        System.Threading.Thread.Sleep(2000);
        continue;
    }

    if (userAttempt.Password == passwordInput)
    {
        loggedInUser = userAttempt;
        loggedInUser.LoginAttempts = 0; // Reset attempts on successful login
        userHandler.WriteUsers(usersFilePath, users); // Save updated user status
        Console.WriteLine($"\nWelcome, {loggedInUser.Username}! You have successfully logged in.");
        Logger.Log($"Successful login for user: '{loggedInUser.Username}'", loggedInUser.Username);
        System.Threading.Thread.Sleep(1500);
    }
    else
    {
        userAttempt.LoginAttempts++;
        Console.WriteLine($"Incorrect password for '{userAttempt.Username}'. Remaining attempts: {MAX_LOGIN_ATTEMPTS - userAttempt.LoginAttempts}");
        Logger.Log($"Incorrect password for user '{userAttempt.Username}'. Attempt {userAttempt.LoginAttempts}/{MAX_LOGIN_ATTEMPTS}", userAttempt.Username);

        if (userAttempt.LoginAttempts >= MAX_LOGIN_ATTEMPTS)
        {
            userAttempt.IsActive = false; // Block the user
            userHandler.WriteUsers(usersFilePath, users); // Save status change to file
            Console.WriteLine($"User '{userAttempt.Username}' has been blocked due to multiple failed attempts.");
            Logger.Log($"User '{userAttempt.Username}' blocked after {MAX_LOGIN_ATTEMPTS} failed attempts.", userAttempt.Username);
            System.Threading.Thread.Sleep(2000);
        }
        System.Threading.Thread.Sleep(1500);
    }
}

// --- MAIN MENU AFTER AUTHENTICATION ---
// The rest of the program runs once the user is logged in.
string option;
do
{
    Console.Clear();
    option = MainMenu(); // Your MainMenu() method
    Console.WriteLine("=======================================");

    switch (option)
    {
        case "1": // Show Users (Administration)
            Console.WriteLine("\n--- User List ---");
            foreach (var user in users)
            {
                Console.WriteLine($"User: {user.Username,-15} | Active: {user.IsActive,-5}");
            }
            Logger.Log($"Displaying user list.", loggedInUser.Username);
            break;
        case "2": // Unlock User
            Console.Write("Enter the username to unlock: ");
            string? userToUnlockName = Console.ReadLine();
            UnlockUser(userToUnlockName, loggedInUser.Username); // Your UnlockUser() method
            break;
        case "3": // Show People
            ShowPeople();
            break;
        case "4": // Add Person
            AddPerson();
            break;
        case "5": // Edit Person
            EditPerson();
            break;
        case "6": // Delete Person
            DeletePerson();
            break;
        // Case 7 for GenerateCityBalanceReport (Summary) has been removed
        case "8": // Generate Detailed City Balance Report
            GenerateDetailedCityBalanceReport();
            break;
        case "0":
            Console.WriteLine("Exiting program. Goodbye!");
            Logger.Log($"User '{loggedInUser.Username}' logged out. Application terminated.", loggedInUser.Username);
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();

} while (option != "0");


// --- GLOBAL HELPER METHODS ---

// Method to display the main menu
string MainMenu()
{
    Console.WriteLine("=======================================");
    Console.WriteLine($"CURRENT USER: {loggedInUser?.Username ?? "N/A"}");
    Console.WriteLine("=======================================");
    Console.WriteLine("1. Show Users (Administration)");
    Console.WriteLine("2. Unlock User");
    Console.WriteLine("3. Show People");
    Console.WriteLine("4. Add Person");
    Console.WriteLine("5. Edit Person");
    Console.WriteLine("6. Delete Person");
    // Option 7 for Generate City Balance Report (Summary) has been removed
    Console.WriteLine("8. Generate Detailed City Balance Report"); // Only the detailed report remains
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option: ");
    return Console.ReadLine() ?? "0";
}

// Method to read password without displaying characters
string ReadPassword()
{
    string password = "";
    ConsoleKeyInfo key;
    do
    {
        key = Console.ReadKey(true);
        if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
        {
            password += key.KeyChar;
            Console.Write("*");
        }
        else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
        {
            password = password.Substring(0, (password.Length - 1));
            Console.Write("\b \b");
        }
    }
    while (key.Key != ConsoleKey.Enter);
    return password;
}

// Method to unlock a user
void UnlockUser(string? username, string executedByUsername)
{
    if (string.IsNullOrWhiteSpace(username))
    {
        Console.WriteLine("Username cannot be empty.");
        return;
    }

    User? userToUnlock = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    if (userToUnlock == null)
    {
        Console.WriteLine($"User '{username}' does not exist.");
        Logger.Log($"Unlock attempt failed: user '{username}' not found.", executedByUsername);
        return;
    }

    if (userToUnlock.IsActive)
    {
        Console.WriteLine($"User '{username}' is already active. No need to unlock.");
        Logger.Log($"Unlock attempt: user '{username}' was already active.", executedByUsername);
        return;
    }

    userToUnlock.IsActive = true;
    userToUnlock.LoginAttempts = 0; // Reset attempts
    userHandler.WriteUsers(usersFilePath, users); // Save status change to file
    Console.WriteLine($"User '{username}' has been successfully unlocked.");
    Logger.Log($"User '{username}' unlocked by '{executedByUsername}'.", executedByUsername);
}


// --- PERSON MANAGEMENT METHODS ---

// Method to display all people
void ShowPeople()
{
    Console.WriteLine("\n--- Person List ---");
    if (!people.Any())
    {
        Console.WriteLine("No registered people.");
        Logger.Log($"Displaying person list. (Empty list)", loggedInUser.Username);
        return;
    }

    Console.WriteLine("ID       Names              Last Names         Phone            City               Balance");
    Console.WriteLine("--------------------------------------------------------------------------------------");
    foreach (var person in people)
    {
        // Adjust spacing as needed
        Console.WriteLine($"{person.Id,-7} {person.FirstName,-17} {person.LastName,-17} {person.Phone,-16} {person.City,-17} {person.Balance,-10:C2}");
    }
    Logger.Log($"Displaying person list.", loggedInUser.Username);
}

// Method to add a new person with validations
void AddPerson()
{
    Console.WriteLine("\n--- Add New Person ---");
    int id;
    bool idIsValid = false;
    do
    {
        string? idInput = ReadValidatedInput("Enter ID (unique number, 0 to cancel): ", s => int.TryParse(s, out int result) && result > 0 && !people.Any(p => p.Id == result), "Invalid ID, already exists, or is 0.");
        if (idInput == null) return; // If the user cancels (enters 0)
        id = int.Parse(idInput);
        idIsValid = true;
    } while (!idIsValid);


    string? firstName = ReadValidatedInput("Enter First Name: ", s => !string.IsNullOrWhiteSpace(s), "First name cannot be empty.");
    if (firstName == null) return; // If the user cancels

    string? lastName = ReadValidatedInput("Enter Last Name: ", s => !string.IsNullOrWhiteSpace(s), "Last name cannot be empty.");
    if (lastName == null) return; // If the user cancels

    string? phone = ReadValidatedInput("Enter Phone (e.g., 3001234567, digits only, 0 to cancel): ", IsValidPhoneNumber, "Invalid phone (must be numeric and appropriate length, e.g., 10 digits).");
    if (phone == null) return; // If the user cancels

    string? city = ReadValidatedInput("Enter City: ", s => !string.IsNullOrWhiteSpace(s), "City cannot be empty.");
    if (city == null) return; // If the user cancels
    // Normalize city input to handle case and spacing variations
    city = city.Trim();

    decimal balance;
    bool balanceIsValid = false;
    do
    {
        string? balanceInput = ReadValidatedInput("Enter Balance (positive number, 0 to cancel): ", s => decimal.TryParse(s, out decimal result) && result >= 0, "Invalid balance (must be a positive number).");
        if (balanceInput == null) return; // If the user cancels
        balance = decimal.Parse(balanceInput);
        balanceIsValid = true;
    } while (!balanceIsValid);


    var newPerson = new Person
    {
        Id = id,
        FirstName = firstName,
        LastName = lastName,
        Phone = phone,
        City = city, // Use the normalized city
        Balance = balance
    };

    people.Add(newPerson);
    personHelper.write(people); // Save changes to the people CSV file
    Console.WriteLine($"Person '{newPerson.FirstName} {newPerson.LastName}' added successfully.");
    Logger.Log($"Person with ID {newPerson.Id} added by '{loggedInUser.Username}'.", loggedInUser.Username);
}

// Helper method to read user input with validation and cancel option
string? ReadValidatedInput(string prompt, Func<string, bool> validationFunc, string errorMessage)
{
    string? input;
    bool isValid;
    do
    {
        Console.Write(prompt);
        input = Console.ReadLine();
        // Allow canceling if the user enters "0" and the prompt suggests it (or if it's an ID)
        if (input == "0" && prompt.Contains("cancel"))
        {
            Console.WriteLine("Operation canceled.");
            return null;
        }

        isValid = validationFunc(input ?? string.Empty);
        if (!isValid)
        {
            Console.WriteLine(errorMessage);
        }
    } while (!isValid);
    return input;
}

// Phone validation using Regex
bool IsValidPhoneNumber(string phoneNumber)
{
    // Allows 7 to 15 digits. You can adjust the regular expression as needed.
    return Regex.IsMatch(phoneNumber, @"^\d{7,15}$");
}

// Method to edit a person
void EditPerson()
{
    Console.WriteLine("\n--- Edit Person ---");
    int idToEdit;
    string? idInput = ReadValidatedInput("Enter the ID of the person to edit (0 to cancel): ", s => int.TryParse(s, out int result), "Invalid ID.");
    if (idInput == null || !int.TryParse(idInput, out idToEdit) || idToEdit == 0)
    {
        Console.WriteLine("Edit operation canceled or invalid ID.");
        return;
    }

    Person? personToEdit = people.FirstOrDefault(p => p.Id == idToEdit);

    if (personToEdit == null)
    {
        Console.WriteLine($"No person found with ID {idToEdit}.");
        Logger.Log($"Edit attempt failed: Person with ID {idToEdit} not found.", loggedInUser.Username);
        return;
    }

    Console.WriteLine("\nCurrent person data:");
    Console.WriteLine(personToEdit.ToString());

    Console.WriteLine("\nEnter new data (press ENTER to keep current value):");

    // First Name
    Console.Write($"First Name ({personToEdit.FirstName}): ");
    string? newFirstName = Console.ReadLine();
    if (!string.IsNullOrEmpty(newFirstName))
    {
        personToEdit.FirstName = newFirstName;
    }

    // Last Name
    Console.Write($"Last Name ({personToEdit.LastName}): ");
    string? newLastName = Console.ReadLine();
    if (!string.IsNullOrEmpty(newLastName))
    {
        personToEdit.LastName = newLastName;
    }

    // Phone
    string? newPhone;
    do
    {
        Console.Write($"Phone ({personToEdit.Phone}): ");
        newPhone = Console.ReadLine();
        if (string.IsNullOrEmpty(newPhone)) break; // If ENTER is pressed, keep the current value
        if (!IsValidPhoneNumber(newPhone))
        {
            Console.WriteLine("Invalid phone. Please try again.");
        }
    } while (!IsValidPhoneNumber(newPhone));
    if (!string.IsNullOrEmpty(newPhone))
    {
        personToEdit.Phone = newPhone;
    }

    // City
    Console.Write($"City ({personToEdit.City}): ");
    string? newCity = Console.ReadLine();
    if (!string.IsNullOrEmpty(newCity))
    {
        // Normalize city input to handle case and spacing variations
        personToEdit.City = newCity.Trim();
    }

    // Balance
    string? newBalanceInput;
    decimal newBalance;
    do
    {
        Console.Write($"Balance ({personToEdit.Balance:C2}): ");
        newBalanceInput = Console.ReadLine();
        if (string.IsNullOrEmpty(newBalanceInput)) break; // If ENTER is pressed, keep the current value
        if (decimal.TryParse(newBalanceInput, out newBalance) && newBalance >= 0)
        {
            personToEdit.Balance = newBalance;
            break;
        }
        else
        {
            Console.WriteLine("Invalid balance. Must be a positive number. Please try again.");
        }
    } while (true);


    personHelper.write(people); // Save changes to the people CSV file
    Console.WriteLine($"Person with ID {idToEdit} edited successfully.");
    Logger.Log($"Person with ID {idToEdit} edited by '{loggedInUser.Username}'.", loggedInUser.Username);
}

// --- METHOD TO DELETE A PERSON ---
void DeletePerson()
{
    Console.WriteLine("\n--- Delete Person ---");
    int idToDelete;
    string? idInput = ReadValidatedInput("Enter the ID of the person to delete (0 to cancel): ", s => int.TryParse(s, out int result), "Invalid ID.");
    if (idInput == null || !int.TryParse(idInput, out idToDelete) || idToDelete == 0)
    {
        Console.WriteLine("Delete operation canceled or invalid ID.");
        return;
    }

    Person? personToDelete = people.FirstOrDefault(p => p.Id == idToDelete);

    if (personToDelete == null)
    {
        Console.WriteLine($"No person found with ID {idToDelete}.");
        Logger.Log($"Delete attempt failed: Person with ID {idToDelete} not found.", loggedInUser.Username);
        return;
    }

    Console.WriteLine("\nThe following person will be deleted:");
    Console.WriteLine(personToDelete.ToString());
    Console.Write("Are you sure you want to delete this person? (Yes/No): ");
    string? confirmation = Console.ReadLine()?.Trim().ToLower();

    if (confirmation == "si" || confirmation == "sí" || confirmation == "yes" || confirmation == "y")
    {
        people.Remove(personToDelete);
        personHelper.write(people); // Save changes
        Console.WriteLine($"Persona con ID {idToDelete} deleted successfully.");
        Logger.Log($"Person with ID {idToDelete} deleted by '{loggedInUser.Username}'.", loggedInUser.Username);
    }
    else
    {
        Console.WriteLine("Deletion canceled.");
        Logger.Log($"Deletion of person with ID {idToDelete} canceled by '{loggedInUser.Username}'.", loggedInUser.Username);
    }
}

// The old GenerateCityBalanceReport (Summary) method has been removed.
// It was previously here:
// void GenerateCityBalanceReport() { /* ... */ }


// --- NEW METHOD: GENERATE DETAILED CITY BALANCE REPORT ---
void GenerateDetailedCityBalanceReport()
{
    Console.WriteLine("\n--- Detailed City Balance Report ---");

    if (!people.Any())
    {
        Console.WriteLine("No registered people to generate the detailed report.");
        Logger.Log($"Attempt to generate detailed city balance report. (Empty person list)", loggedInUser.Username);
        return;
    }

    // Group people by city, ensuring consistent grouping
    var peopleByCity = people
        .GroupBy(p => p.City.Trim().ToLower()) // Group by normalized city name
        .OrderBy(group => group.Key) // Order cities alphabetically
        .ToList();

    decimal grandTotalBalance = 0;

    foreach (var cityGroup in peopleByCity)
    {
        // Display city header, capitalizing the first letter
        string displayCityName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cityGroup.Key);
        Console.WriteLine($"\nCiudad: {displayCityName}");
        
        Console.WriteLine("ID       Nombres           Apellidos           Saldo");
        Console.WriteLine("------- ----------------- ----------------- --------------");

        decimal citySubtotal = 0;
        foreach (var person in cityGroup.OrderBy(p => p.LastName).ThenBy(p => p.FirstName)) // Order persons within city
        {
            // Note: Phone number is not included as per your requested format for this report
            Console.WriteLine($"{person.Id,-7} {person.FirstName,-17} {person.LastName,-17} {person.Balance,10:C2}");
            citySubtotal += person.Balance;
        }
        Console.WriteLine("                                            ==============");
        Console.WriteLine($"Total: {displayCityName,-39} {citySubtotal,10:C2}");
        

        grandTotalBalance += citySubtotal;
    }

    Console.WriteLine("\n===============================================================");
    Console.WriteLine($"Total General: {grandTotalBalance,43:C2}"); // Align with city totals
    Console.WriteLine("===============================================================");

    Logger.Log($"Detailed city balance report generated by '{loggedInUser.Username}'. Grand total: {grandTotalBalance:C2}", loggedInUser.Username);
}
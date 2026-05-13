using System;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("============================================");
            Console.WriteLine("     ELECTRICITY BILL CALCULATOR     ");
            Console.WriteLine("============================================");

            UserManager userManager = new UserManager();

            try
            {
                ResidentialUser homeUser = new ResidentialUser(
                    "R001", "Ahmed Khan", "123 Main St", 3000, 4500);
                userManager.AddUser(homeUser);

                CommercialUser shopOwner = new CommercialUser(
                    "C001", "Tech Shop", "456 Business St", 5000, 9000);
                userManager.AddUser(shopOwner);

                IndustrialUser factoryOwner = new IndustrialUser(
                    "I001", "Power Factory", "789 Factory St", 120000, 130000);
                userManager.AddUser(factoryOwner);
            }
            catch (InvalidUserIdException ex)
            {
                Console.WriteLine($"\n  [Setup Error] Could not create a user — {ex.Message}");
            }
            catch (InvalidUserNameException ex)
            {
                Console.WriteLine($"\n  [Setup Error] Could not create a user — {ex.Message}");
            }
            catch (InvalidMeterReadingException ex)
            {
                Console.WriteLine($"\n  [Setup Error] Could not create a user — {ex.Message}");
            }
            catch (CurrentReadingBelowPreviousException ex)
            {
                Console.WriteLine($"\n  [Setup Error] Could not create a user — {ex.Message}");
            }

            Console.WriteLine("\n  Total user of the Electricity: 3.");

            bool isRunning = true;

            while (isRunning)
            {
                try
                {
                    Console.WriteLine("\n============================================");
                    Console.WriteLine("               MAIN MENU                   ");
                    Console.WriteLine("============================================");
                    Console.WriteLine("  1. Show All Users");
                    Console.WriteLine("  2. View Specific User");
                    Console.WriteLine("  3. Calculate Bill");
                    Console.WriteLine("  4. Record Payment");
                    Console.WriteLine("  5. View Payment Summary");
                    Console.WriteLine("  6. View Payment History");
                    Console.WriteLine("  7. Exit");
                    Console.WriteLine(" ");
                    Console.Write("  Enter Choice: ");

                    string userChoice = Console.ReadLine()?.Trim();

                    switch (userChoice)
                    {
                        case "1":
                            userManager.ShowUsersByCategoryColumns();
                            break;

                        case "2":
                            try
                            {
                                Console.Write("\nEnter User ID: ");
                                string searchId = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(searchId))
                                    throw new InvalidUserIdException(searchId ?? "");

                                ElectricityUser foundUser = userManager.FindUser(searchId);

                                if (foundUser != null)
                                {
                                    Console.WriteLine("\n===== USER DETAILS =====");
                                    foundUser.DisplayUserInfo();
                                }
                                else
                                {
                                    throw new UserNotFoundException(searchId.Trim());
                                }
                            }
                            catch (InvalidUserIdException ex)
                            {
                                Console.WriteLine($"\n  [Input Error] {ex.Message}");
                            }
                            catch (UserNotFoundException ex)
                            {
                                Console.WriteLine($"\n  [Not Found] {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"\n  [Unexpected Error] Could not retrieve user details. Details: {ex.Message}");
                            }
                            break;

                        case "3":
                            userManager.CalculateBill();
                            break;

                        case "4":
                            userManager.RecordPayment();
                            break;

                        case "5":
                            userManager.ShowPaymentSummary();
                            break;

                        case "6":
                            userManager.ShowPaymentHistory();
                            break;

                        case "7":
                            Console.WriteLine("\nExiting system. Goodbye!");
                            isRunning = false;
                            break;

                        default:
                            throw new InvalidMenuChoiceException(userChoice ?? "");
                    }
                }
                catch (InvalidMenuChoiceException ex)
                {
                    Console.WriteLine($"\n  [Input Error] {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n  [Unexpected Error] Something went wrong. Details: {ex.Message}");
                    Console.WriteLine("  The system will continue running. Please try again.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  [Critical Error] The application encountered a fatal error and must close.");
            Console.WriteLine($"  Details: {ex.Message}");
            Console.WriteLine("\n  Press any key to exit...");
            Console.ReadKey();
        }
    }

    static void ShowBillBreakdown(BillCalculator calculator)
    {
        try
        {
            Console.WriteLine("  Base Bill  : " + calculator.CalculateBaseBill().ToString("F2") + " BDT");
            Console.WriteLine("  Tax        : " + calculator.CalculateTax().ToString("F2")      + " BDT");
            Console.WriteLine("  Total Bill : " + calculator.CalculateTotalBill().ToString("F2") + " BDT");
        }
        catch (NegativeUnitsConsumedException ex)
        {
            Console.WriteLine($"\n  [Billing Error] {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  [Unexpected Error] Could not display bill breakdown. Details: {ex.Message}");
        }
    }
}
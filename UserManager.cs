using System;
using System.Collections.Generic;
using System.Linq;

public class UserManager
{
    private Dictionary<string, ElectricityUser> registeredUsers =
        new Dictionary<string, ElectricityUser>();

    public void AddUser(ElectricityUser newUser)
    {
        try
        {
            if (newUser == null)
                throw new NullUserException();

            if (registeredUsers.ContainsKey(newUser.UserId))
                throw new DuplicateUserIdException(newUser.UserId);

            registeredUsers[newUser.UserId] = newUser;
        }
        catch (NullUserException ex)
        {
            Console.WriteLine($"\n  [Registration Error] {ex.Message}");
        }
        catch (DuplicateUserIdException ex)
        {
            Console.WriteLine($"\n  [Registration Error] {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  [Unexpected Error] Failed to register user. Details: {ex.Message}");
        }
    }

    private List<ElectricityUser> GetResidentialUsers() =>
        registeredUsers.Values.Where(user => user is ResidentialUser).ToList();

    private List<ElectricityUser> GetCommercialUsers() =>
        registeredUsers.Values.Where(user => user is CommercialUser).ToList();

    private List<ElectricityUser> GetIndustrialUsers() =>
        registeredUsers.Values.Where(user => user is IndustrialUser).ToList();

    public void ShowUsersByCategoryColumns()
    {
        try
        {
            if (registeredUsers.Count == 0)
            {
                Console.WriteLine("\n  [Info] No users have been registered yet.");
                return;
            }

            var residentialUsers = GetResidentialUsers();
            var commercialUsers  = GetCommercialUsers();
            var industrialUsers  = GetIndustrialUsers();

            int maxCategoryCount = Math.Max(residentialUsers.Count,
                                   Math.Max(commercialUsers.Count, industrialUsers.Count));

            Console.WriteLine("\n==========================================================================");
            Console.WriteLine($"  {"RESIDENTIAL",-25} {"COMMERCIAL",-25} {"INDUSTRIAL",-25}");
            Console.WriteLine("==========================================================================");

            for (int rowIndex = 0; rowIndex < maxCategoryCount; rowIndex++)
            {
                string residentialEntry = rowIndex < residentialUsers.Count
                    ? residentialUsers[rowIndex].UserId + " - " + residentialUsers[rowIndex].UserName : "";
                string commercialEntry  = rowIndex < commercialUsers.Count
                    ? commercialUsers[rowIndex].UserId  + " - " + commercialUsers[rowIndex].UserName  : "";
                string industrialEntry  = rowIndex < industrialUsers.Count
                    ? industrialUsers[rowIndex].UserId  + " - " + industrialUsers[rowIndex].UserName  : "";

                Console.WriteLine($"  {residentialEntry,-25} {commercialEntry,-25} {industrialEntry,-25}");
            }

            Console.WriteLine("==========================================================================");
            Console.WriteLine($"  Total: {registeredUsers.Count} user(s) — " +
                              $"Residential: {residentialUsers.Count}, " +
                              $"Commercial: {commercialUsers.Count}, " +
                              $"Industrial: {industrialUsers.Count}");
            Console.WriteLine("==========================================================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  [Unexpected Error] Could not display users. Details: {ex.Message}");
        }
    }

    public ElectricityUser FindUser(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidUserIdException(userId ?? "");

            registeredUsers.TryGetValue(userId.Trim().ToUpper(), out ElectricityUser foundUser);
            return foundUser;
        }
        catch (InvalidUserIdException ex)
        {
            Console.WriteLine($"\n  [Search Error] {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  [Unexpected Error] Could not search for user. Details: {ex.Message}");
            return null;
        }
    }

    public void CalculateBill()
    {
        try
        {
            Console.Write("\nEnter User ID: ");
            string inputUserId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputUserId))
                throw new InvalidUserIdException(inputUserId ?? "");

            registeredUsers.TryGetValue(inputUserId.Trim().ToUpper(), out ElectricityUser selectedUser);

            if (selectedUser == null)
                throw new UserNotFoundException(inputUserId.Trim());

            double unitsConsumed = selectedUser.UnitConsumed();
            double energyCharge  = selectedUser.CalculateBill();
            double serviceCharge = 120.0;
            double vatAmount     = (energyCharge + serviceCharge) * 0.05;
            double totalBill     = energyCharge + serviceCharge + vatAmount;

            string accountTypeLabel  = "General";
            string tariffDescription = "";

            if (selectedUser is ResidentialUser)
            {
                accountTypeLabel  = "Residential";
                var calculator    = new ResidentialBillCalculator(unitsConsumed);
                tariffDescription = "Rate: " + calculator.GetTariffRate() + " BDT/kWh + 10% Tax";
            }
            else if (selectedUser is CommercialUser)
            {
                accountTypeLabel  = "Commercial";
                var calculator    = new CommercialBillCalculator(unitsConsumed);
                tariffDescription = "Rate: " + calculator.GetTariffRate() + " BDT/kWh + 15% Tax";
            }
            else if (selectedUser is IndustrialUser)
            {
                accountTypeLabel  = "Industrial";
                var calculator    = new IndustrialBillCalculator(unitsConsumed);
                tariffDescription = "Rate: " + calculator.GetTariffRate() + " BDT/kWh + 5% Tax";
            }

            selectedUser.PaymentTracker.SetBillAmount(totalBill);

            Console.WriteLine("\n========================================");
            Console.WriteLine("        ELECTRICITY BILL REPORT         ");
            Console.WriteLine("========================================");
            Console.WriteLine($"Customer Name   : {selectedUser.UserName}");
            Console.WriteLine($"Customer ID     : {selectedUser.UserId}");
            Console.WriteLine($"Address         : {selectedUser.Address}");
            Console.WriteLine($"Account Type    : {accountTypeLabel}");
            Console.WriteLine($"Tariff Info     : {tariffDescription}");
            Console.WriteLine($"Billing Month   : {DateTime.Now:MMMM yyyy}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Previous Reading: {selectedUser.PreviousReading} kWh");
            Console.WriteLine($"Current Reading : {selectedUser.CurrentReading} kWh");
            Console.WriteLine($"Units Consumed  : {unitsConsumed} kWh");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Charges:");
            Console.WriteLine($"  Energy Charge : {energyCharge.ToString("F2")} BDT");
            Console.WriteLine($"  Service Charge: {serviceCharge.ToString("F2")} BDT");
            Console.WriteLine($"  VAT (5%)      : {vatAmount.ToString("F2")} BDT");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"  TOTAL BILL    : {totalBill.ToString("F2")} BDT");
            Console.WriteLine("========================================");
            Console.WriteLine($"Due Date        : 20 {DateTime.Now:MMMM yyyy}");
            Console.WriteLine($"Payment Status  : {selectedUser.PaymentTracker.CurrentStatus}");
            Console.WriteLine($"Outstanding     : {selectedUser.PaymentTracker.OutstandingBalance.ToString("F2")} BDT");
            Console.WriteLine("\n  Thank You for using our service!");
            Console.WriteLine("========================================\n");
        }
        catch (InvalidUserIdException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (UserNotFoundException ex)
        {
            Console.WriteLine($"\n  [Not Found] {ex.Message}");
        }
        catch (NegativeUnitsConsumedException ex)
        {
            Console.WriteLine($"\n  [Billing Error] {ex.Message}");
        }
        catch (BillingCalculationException ex)
        {
            Console.WriteLine($"\n  [Billing Error] {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  [Unexpected Error] An unexpected error occurred while generating the bill. Details: {ex.Message}");
        }
    }

    public void RecordPayment()
    {
        try
        {
            Console.Write("\nEnter User ID: ");
            string inputUserId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputUserId))
                throw new InvalidUserIdException(inputUserId ?? "");

            registeredUsers.TryGetValue(inputUserId.Trim().ToUpper(), out ElectricityUser selectedUser);

            if (selectedUser == null)
                throw new UserNotFoundException(inputUserId.Trim());

            if (selectedUser.PaymentTracker.TotalBillAmount == 0)
                throw new NoBillGeneratedException();

            Console.WriteLine("\n----------------------------------------");
            Console.WriteLine($"  User           : {selectedUser.UserName}");
            Console.WriteLine($"  Total Bill     : {selectedUser.PaymentTracker.TotalBillAmount.ToString("F2")} BDT");
            Console.WriteLine($"  Already Paid   : {selectedUser.PaymentTracker.TotalAmountPaid.ToString("F2")} BDT");
            Console.WriteLine($"  Outstanding    : {selectedUser.PaymentTracker.OutstandingBalance.ToString("F2")} BDT");
            Console.WriteLine("----------------------------------------");

            Console.Write("\nEnter Payment Amount (BDT): ");
            string amountInput = Console.ReadLine();
            if (!double.TryParse(amountInput, out double paymentAmount))
                throw new FormatException(
                    $"'{amountInput}' is not a valid number. Please enter a numeric payment amount.");

            Console.WriteLine("\nSelect Payment Method:");
            Console.WriteLine("  1. Cash");
            Console.WriteLine("  2. Bank Transfer");
            Console.WriteLine("  3. Mobile Banking");
            Console.WriteLine("  4. Cheque Payment");
            Console.Write("  Enter choice (1-4): ");
            string methodChoice = Console.ReadLine()?.Trim();

            PaymentMethod selectedMethod;
            switch (methodChoice)
            {
                case "1": selectedMethod = PaymentMethod.Cash;          break;
                case "2": selectedMethod = PaymentMethod.BankTransfer;  break;
                case "3": selectedMethod = PaymentMethod.MobileBanking; break;
                case "4": selectedMethod = PaymentMethod.ChequePayment; break;
                default:
                    throw new InvalidMenuChoiceException(methodChoice ?? "");
            }

            Console.Write("Enter Reference Number (e.g. TXN123456): ");
            string referenceNumber = Console.ReadLine();

            Console.Write("Enter Remarks (optional, press Enter to skip): ");
            string remarks = Console.ReadLine();

            selectedUser.PaymentTracker.RecordPayment(paymentAmount, selectedMethod, referenceNumber, remarks);

            Console.WriteLine("\n========================================");
            Console.WriteLine("        PAYMENT RECORDED                ");
            Console.WriteLine("========================================");
            Console.WriteLine($"  User           : {selectedUser.UserName} ({selectedUser.UserId})");
            Console.WriteLine($"  Amount Paid    : {paymentAmount.ToString("F2")} BDT");
            Console.WriteLine($"  Method         : {selectedMethod}");
            Console.WriteLine($"  Reference No.  : {referenceNumber}");
            Console.WriteLine($"  Date           : {DateTime.Now:dd MMM yyyy  hh:mm tt}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"  Outstanding    : {selectedUser.PaymentTracker.OutstandingBalance.ToString("F2")} BDT");
            Console.WriteLine($"  Payment Status : {selectedUser.PaymentTracker.CurrentStatus}");
            if (selectedUser.PaymentTracker.OverpaidAmount > 0)
                Console.WriteLine($"  Overpaid (credit): {selectedUser.PaymentTracker.OverpaidAmount.ToString("F2")} BDT");
            Console.WriteLine("========================================\n");
        }
        catch (InvalidUserIdException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (UserNotFoundException ex)
        {
            Console.WriteLine($"\n  [Not Found] {ex.Message}");
        }
        catch (NoBillGeneratedException ex)
        {
            Console.WriteLine($"\n  [Payment Error] {ex.Message}");
        }
        catch (InvalidPaymentAmountException ex)
        {
            Console.WriteLine($"\n  [Payment Error] {ex.Message}");
        }
        catch (InvalidPaymentReferenceException ex)
        {
            Console.WriteLine($"\n  [Payment Error] {ex.Message}");
        }
        catch (ExcessiveOverpaymentException ex)
        {
            Console.WriteLine($"\n  [Payment Error] {ex.Message}");
        }
        catch (InvalidMenuChoiceException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  [Unexpected Error] Could not record payment. Details: {ex.Message}");
        }
    }

    public void ShowPaymentSummary()
    {
        try
        {
            Console.Write("\nEnter User ID: ");
            string inputUserId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputUserId))
                throw new InvalidUserIdException(inputUserId ?? "");

            registeredUsers.TryGetValue(inputUserId.Trim().ToUpper(), out ElectricityUser selectedUser);

            if (selectedUser == null)
                throw new UserNotFoundException(inputUserId.Trim());

            Console.WriteLine($"\n  User : {selectedUser.UserName} ({selectedUser.UserId})");
            selectedUser.PaymentTracker.DisplayPaymentSummary();
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
            Console.WriteLine($"\n  [Unexpected Error] Could not display payment summary. Details: {ex.Message}");
        }
    }

    public void ShowPaymentHistory()
    {
        try
        {
            Console.Write("\nEnter User ID: ");
            string inputUserId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputUserId))
                throw new InvalidUserIdException(inputUserId ?? "");

            registeredUsers.TryGetValue(inputUserId.Trim().ToUpper(), out ElectricityUser selectedUser);

            if (selectedUser == null)
                throw new UserNotFoundException(inputUserId.Trim());

            Console.WriteLine($"\n  User : {selectedUser.UserName} ({selectedUser.UserId})");
            selectedUser.PaymentTracker.DisplayPaymentHistory();
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
            Console.WriteLine($"\n  [Unexpected Error] Could not display payment history. Details: {ex.Message}");
        }
    }
}
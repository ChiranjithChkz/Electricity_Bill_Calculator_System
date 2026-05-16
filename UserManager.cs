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
                string commercialEntry = rowIndex < commercialUsers.Count
                    ? commercialUsers[rowIndex].UserId + " - " + commercialUsers[rowIndex].UserName : "";
                string industrialEntry = rowIndex < industrialUsers.Count
                    ? industrialUsers[rowIndex].UserId + " - " + industrialUsers[rowIndex].UserName : "";

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
                throw new FormatException($"'{amountInput}' is not a valid number. Please enter a numeric payment amount.");

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
                case "1": selectedMethod = PaymentMethod.Cash;           break;
                case "2": selectedMethod = PaymentMethod.BankTransfer;   break;
                case "3": selectedMethod = PaymentMethod.MobileBanking;  break;
                case "4": selectedMethod = PaymentMethod.ChequePayment;  break;
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


    public void RegisterNewUser()
    {
        try
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine("         REGISTER NEW USER              ");
            Console.WriteLine("========================================");
            Console.WriteLine("  Select Account Type:");
            Console.WriteLine("  1. Residential");
            Console.WriteLine("  2. Commercial");
            Console.WriteLine("  3. Industrial");
            Console.Write("  Enter choice (1-3): ");
            string typeChoice = Console.ReadLine()?.Trim();

            if (typeChoice != "1" && typeChoice != "2" && typeChoice != "3")
                throw new InvalidMenuChoiceException(typeChoice ?? "");

            Console.WriteLine("\n  --- User Information ---");

            Console.Write("  User ID        : ");
            string userId = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidUserIdException(userId ?? "");

            if (registeredUsers.ContainsKey(userId.ToUpper()))
                throw new DuplicateUserIdException(userId);

            Console.Write("  Full Name      : ");
            string userName = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(userName))
                throw new InvalidUserNameException(userName ?? "");

            Console.Write("  Address        : ");
            string address = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty or blank.");

            Console.Write("  Previous Reading (kWh): ");
            if (!double.TryParse(Console.ReadLine(), out double previousReading))
                throw new FormatException("Previous reading must be a valid number.");
            if (previousReading < 0)
                throw new InvalidMeterReadingException("Previous Reading", previousReading);

            Console.Write("  Current Reading  (kWh): ");
            if (!double.TryParse(Console.ReadLine(), out double currentReading))
                throw new FormatException("Current reading must be a valid number.");
            if (currentReading < 0)
                throw new InvalidMeterReadingException("Current Reading", currentReading);
            if (currentReading < previousReading)
                throw new CurrentReadingBelowPreviousException(previousReading, currentReading);

            ElectricityUser newUser;
            string          accountTypeLabel;
            string[]        requiredDocTypes;

            if (typeChoice == "1")
            {
                Console.Write("\n  Monthly Allowance (BDT, press Enter for 0): ");
                string allowInput = Console.ReadLine()?.Trim();
                double allowance  = string.IsNullOrWhiteSpace(allowInput) ? 0 : double.Parse(allowInput);

                ResidentialUser rUser = new ResidentialUser(userId, userName, address, previousReading, currentReading);
                rUser.MonthlyAllowance = allowance;
                newUser          = rUser;
                accountTypeLabel = "Residential";
                requiredDocTypes = new[] { "National ID", "Utility Ownership Proof" };
            }
            else if (typeChoice == "2")
            {
                Console.Write("\n  License Number (press Enter for N/A): ");
                string licInput = Console.ReadLine()?.Trim();

                CommercialUser cUser = new CommercialUser(userId, userName, address, previousReading, currentReading);
                if (!string.IsNullOrWhiteSpace(licInput))
                    cUser.LicenseNumber = licInput;
                newUser          = cUser;
                accountTypeLabel = "Commercial";
                requiredDocTypes = new[] { "National ID", "Business Registration" };
            }
            else
            {
                Console.Write("\n  Production Capacity (MW, press Enter for 0): ");
                string capInput = Console.ReadLine()?.Trim();
                double capacity = string.IsNullOrWhiteSpace(capInput) ? 0 : double.Parse(capInput);

                IndustrialUser iUser = new IndustrialUser(userId, userName, address, previousReading, currentReading);
                iUser.ProductionCapacity = capacity;
                newUser          = iUser;
                accountTypeLabel = "Industrial";
                requiredDocTypes = new[] { "National ID", "Business Registration", "Industrial License" };
            }

            Console.WriteLine($"\n  --- Required Documents for {accountTypeLabel} Account ---");
            foreach (string doc in requiredDocTypes)
                Console.WriteLine($"    • {doc}");

            Console.WriteLine("\n  You must submit all required documents to complete registration.");
            Console.WriteLine("  Enter each document when prompted.\n");

            bool addingDocuments = true;
            while (addingDocuments)
            {
                Console.WriteLine("  Available Document Types:");
                Console.WriteLine("    1. National ID");
                Console.WriteLine("    2. Passport");
                Console.WriteLine("    3. Driving License");
                Console.WriteLine("    4. Utility Ownership Proof");
                Console.WriteLine("    5. Business Registration");
                Console.WriteLine("    6. Industrial License");
                Console.WriteLine("    0. Done adding documents");
                Console.Write("  Select document type: ");
                string docChoice = Console.ReadLine()?.Trim();

                if (docChoice == "0")
                {
                    addingDocuments = false;
                    continue;
                }

                DocumentType selectedDocType;
                switch (docChoice)
                {
                    case "1": selectedDocType = DocumentType.NationalId;            break;
                    case "2": selectedDocType = DocumentType.Passport;              break;
                    case "3": selectedDocType = DocumentType.Driving;        break;
                    case "4": selectedDocType = DocumentType.UtilityOwnershipProof; break;
                    case "5": selectedDocType = DocumentType.BusinessRegistration;  break;
                    case "6": selectedDocType = DocumentType.IndustrialLicense;     break;
                    default:
                        Console.WriteLine("  [Input Error] Invalid document type. Please enter 0–6.");
                        continue;
                }

                try
                {
                    Console.Write($"  Document Number  : ");
                    string docNumber = Console.ReadLine()?.Trim();

                    Console.Write("  Issue Date (dd/MM/yyyy)  : ");
                    if (!DateTime.TryParseExact(Console.ReadLine()?.Trim(), "dd/MM/yyyy",
                        null, System.Globalization.DateTimeStyles.None, out DateTime issueDate))
                        throw new FormatException("Invalid date format. Use dd/MM/yyyy (e.g. 15/06/2020).");

                    Console.Write("  Expiry Date (dd/MM/yyyy) : ");
                    if (!DateTime.TryParseExact(Console.ReadLine()?.Trim(), "dd/MM/yyyy",
                        null, System.Globalization.DateTimeStyles.None, out DateTime expiryDate))
                        throw new FormatException("Invalid date format. Use dd/MM/yyyy (e.g. 15/06/2030).");

                    Console.Write("  Issuing Authority        : ");
                    string issuingAuthority = Console.ReadLine()?.Trim();

                    UserDocument document = new UserDocument(selectedDocType, docNumber, issueDate, expiryDate, issuingAuthority);
                    newUser.DocumentStore.AddDocument(document);

                    Console.WriteLine($"\n  [OK] {UserDocument.FormatDocumentType(selectedDocType)} added successfully.");

                    int docsAdded = newUser.DocumentStore.DocumentCount;
                    Console.WriteLine($"  Documents submitted so far: {docsAdded}");
                    Console.WriteLine();
                }
                catch (InvalidDocumentNumberException ex)
                {
                    Console.WriteLine($"\n  [Document Error] {ex.Message}\n");
                }
                catch (ExpiredDocumentException ex)
                {
                    Console.WriteLine($"\n  [Document Error] {ex.Message}\n");
                }
                catch (DocumentAlreadyExistsException ex)
                {
                    Console.WriteLine($"\n  [Document Error] {ex.Message}\n");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"\n  [Document Error] {ex.Message}\n");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"\n  [Input Error] {ex.Message}\n");
                }
            }

            try
            {
                if (typeChoice == "1")
                    newUser.DocumentStore.ValidateRequiredDocumentsForResidential();
                else if (typeChoice == "2")
                    newUser.DocumentStore.ValidateRequiredDocumentsForCommercial();
                else
                    newUser.DocumentStore.ValidateRequiredDocumentsForIndustrial();
            }
            catch (MissingRequiredDocumentException ex)
            {
                Console.WriteLine($"\n  [Registration Blocked] {ex.Message}");
                Console.WriteLine("  User was NOT registered. Please restart and submit all required documents.");
                return;
            }

            registeredUsers[newUser.UserId] = newUser;

            Console.WriteLine("\n========================================");
            Console.WriteLine("        USER REGISTERED SUCCESSFULLY    ");
            Console.WriteLine("========================================");
            Console.WriteLine($"  User ID      : {newUser.UserId}");
            Console.WriteLine($"  Name         : {newUser.UserName}");
            Console.WriteLine($"  Account Type : {accountTypeLabel}");
            Console.WriteLine($"  Address      : {newUser.Address}");
            Console.WriteLine($"  Documents    : {newUser.DocumentStore.DocumentCount} submitted");
            Console.WriteLine($"  Total Users  : {registeredUsers.Count}");
            Console.WriteLine("========================================\n");
        }
        catch (InvalidMenuChoiceException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (InvalidUserIdException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (DuplicateUserIdException ex)
        {
            Console.WriteLine($"\n  [Registration Error] {ex.Message}");
        }
        catch (InvalidUserNameException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (InvalidMeterReadingException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (CurrentReadingBelowPreviousException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"\n  [Input Error] {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n  [Unexpected Error] Registration failed. Details: {ex.Message}");
        }
    }

    public void ViewUserDocuments()
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

            Console.WriteLine("\n========================================");
            Console.WriteLine("         USER DOCUMENTS                 ");
            Console.WriteLine("========================================");
            Console.WriteLine($"  User     : {selectedUser.UserName} ({selectedUser.UserId})");
            Console.WriteLine($"  Address  : {selectedUser.Address}");
            Console.WriteLine($"  Total    : {selectedUser.DocumentStore.DocumentCount} document(s)");
            Console.WriteLine("----------------------------------------");
            selectedUser.DocumentStore.DisplayAllDocuments();
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
        catch (Exception ex)
        {
            Console.WriteLine($"\n  [Unexpected Error] Could not display documents. Details: {ex.Message}");
        }
    }
}
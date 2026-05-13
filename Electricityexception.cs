using System;

public class InvalidUserIdException : Exception
{
    public string AttemptedValue { get; }

    public InvalidUserIdException(string attemptedValue)
        : base($"Invalid User ID: '{attemptedValue}'. User ID cannot be empty or blank.")
    {
        AttemptedValue = attemptedValue;
    }
}

public class InvalidUserNameException : Exception
{
    public string AttemptedValue { get; }

    public InvalidUserNameException(string attemptedValue)
        : base($"Invalid User Name: '{attemptedValue}'. User name cannot be empty or blank.")
    {
        AttemptedValue = attemptedValue;
    }
}

public class InvalidMeterReadingException : Exception
{
    public string ReadingLabel { get; }
    public double AttemptedValue { get; }

    public InvalidMeterReadingException(string readingLabel, double attemptedValue)
        : base($"Invalid {readingLabel}: {attemptedValue} kWh. Meter readings cannot be negative.")
    {
        ReadingLabel   = readingLabel;
        AttemptedValue = attemptedValue;
    }
}

public class CurrentReadingBelowPreviousException : Exception
{
    public double PreviousReading { get; }
    public double CurrentReading  { get; }

    public CurrentReadingBelowPreviousException(double previousReading, double currentReading)
        : base($"Current reading ({currentReading} kWh) cannot be less than previous reading " +
               $"({previousReading} kWh). Meter readings must increase over time.")
    {
        PreviousReading = previousReading;
        CurrentReading  = currentReading;
    }
}

public class NegativeUnitsConsumedException : Exception
{
    public double UnitsConsumed { get; }

    public NegativeUnitsConsumedException(double unitsConsumed)
        : base($"Calculated units consumed is negative ({unitsConsumed} kWh). " +
               $"Current reading must be greater than or equal to previous reading.")
    {
        UnitsConsumed = unitsConsumed;
    }
}

public class UserNotFoundException : Exception
{
    public string SearchedUserId { get; }

    public UserNotFoundException(string searchedUserId)
        : base($"No user found with ID '{searchedUserId}'. Please check the ID and try again.")
    {
        SearchedUserId = searchedUserId;
    }
}

public class DuplicateUserIdException : Exception
{
    public string DuplicateId { get; }

    public DuplicateUserIdException(string duplicateId)
        : base($"A user with ID '{duplicateId}' already exists. Each user must have a unique ID.")
    {
        DuplicateId = duplicateId;
    }
}

public class NullUserException : Exception
{
    public NullUserException()
        : base("Cannot register a null user. Please provide a valid ElectricityUser object.")
    {
    }
}

public class InvalidMenuChoiceException : Exception
{
    public string AttemptedChoice { get; }

    public InvalidMenuChoiceException(string attemptedChoice)
        : base($"'{attemptedChoice}' is not a valid menu option. Please enter a number between 1 and 7.")
    {
        AttemptedChoice = attemptedChoice;
    }
}

public class BillingCalculationException : Exception
{
    public string UserId { get; }

    public BillingCalculationException(string userId, Exception innerException)
        : base($"An error occurred while calculating the bill for user '{userId}'. " +
               $"Please check the user's meter readings.", innerException)
    {
        UserId = userId;
    }
}

public class InvalidCustomerIdException : Exception
{
    public string AttemptedValue { get; }

    public InvalidCustomerIdException(string attemptedValue)
        : base($"Invalid Customer ID: '{attemptedValue}'. Customer ID cannot be empty or blank.")
    {
        AttemptedValue = attemptedValue;
    }
}

public class InvalidCustomerNameException : Exception
{
    public string AttemptedValue { get; }

    public InvalidCustomerNameException(string attemptedValue)
        : base($"Invalid Customer Name: '{attemptedValue}'. Customer name cannot be empty or blank.")
    {
        AttemptedValue = attemptedValue;
    }
}

public class InvalidCustomerReadingException : Exception
{
    public string ReadingLabel { get; }
    public double AttemptedValue { get; }

    public InvalidCustomerReadingException(string readingLabel, double attemptedValue)
        : base($"Invalid {readingLabel}: {attemptedValue} kWh. Meter readings cannot be negative.")
    {
        ReadingLabel   = readingLabel;
        AttemptedValue = attemptedValue;
    }
}

public class InvalidPaymentAmountException : Exception
{
    public double AttemptedAmount { get; }

    public InvalidPaymentAmountException(double attemptedAmount)
        : base($"Invalid payment amount: {attemptedAmount:F2} BDT. Payment amount must be greater than zero.")
    {
        AttemptedAmount = attemptedAmount;
    }
}

public class InvalidPaymentReferenceException : Exception
{
    public string AttemptedReference { get; }

    public InvalidPaymentReferenceException(string attemptedReference)
        : base($"Invalid payment reference number: '{attemptedReference}'. " +
               $"Reference number cannot be empty or blank.")
    {
        AttemptedReference = attemptedReference;
    }
}

public class NoBillGeneratedException : Exception
{
    public NoBillGeneratedException()
        : base("No bill has been generated for this user yet. " +
               "Please calculate the bill before recording a payment.")
    {
    }
}

public class ExcessiveOverpaymentException : Exception
{
    public double AttemptedPayment { get; }
    public double TotalBill        { get; }
    public double AlreadyPaid      { get; }

    public ExcessiveOverpaymentException(double attemptedPayment, double totalBill, double alreadyPaid)
        : base($"Payment of {attemptedPayment:F2} BDT would exceed 150% of the total bill " +
               $"({totalBill:F2} BDT). Already paid: {alreadyPaid:F2} BDT. Please verify the amount.")
    {
        AttemptedPayment = attemptedPayment;
        TotalBill        = totalBill;
        AlreadyPaid      = alreadyPaid;
    }
}
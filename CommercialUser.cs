using System;

public class CommercialUser : ElectricityUser
{
    private string _licenseNumber;

    public string LicenseNumber
    {
        get { return _licenseNumber; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("License number cannot be empty or blank.");
            _licenseNumber = value.Trim();
        }
    }

    public CommercialUser(
        string userId,
        string userName,
        string address,
        double previousMeterReading,
        double currentMeterReading)
        : base(userId, userName, address, previousMeterReading, currentMeterReading)
    {
        _licenseNumber = "N/A";
    }

    public override double CalculateBill()
    {
        try
        {
            CommercialBillCalculator calculator = new CommercialBillCalculator(UnitConsumed());
            return calculator.CalculateTotalBill();
        }
        catch (NegativeUnitsConsumedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BillingCalculationException(UserId, ex);
        }
    }

    public override void DisplayUserInfo()
    {
        base.DisplayUserInfo();
        Console.WriteLine("Account Type    : Commercial");
        Console.WriteLine("License Number  : " + LicenseNumber);
        Console.WriteLine("--------------------------------");
    }
}
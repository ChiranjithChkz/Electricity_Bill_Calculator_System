using System;

public class ResidentialUser : ElectricityUser
{
    private double _monthlyAllowance;

    public double MonthlyAllowance
    {
        get { return _monthlyAllowance; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Monthly allowance cannot be negative.");
            _monthlyAllowance = value;
        }
    }

    public ResidentialUser(
        string userId,
        string userName,
        string address,
        double previousMeterReading,
        double currentMeterReading)
        : base(userId, userName, address, previousMeterReading, currentMeterReading)
    {
        MonthlyAllowance = 0;
    }

    public override double CalculateBill()
    {
        try
        {
            ResidentialBillCalculator calculator = new ResidentialBillCalculator(UnitConsumed());
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
        Console.WriteLine("Account Type    : Residential");
        Console.WriteLine("Monthly Allow.  : " + MonthlyAllowance + " BDT");
        Console.WriteLine("--------------------------------");
    }
}
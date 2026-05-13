using System;

public class IndustrialUser : ElectricityUser
{
    private double _productionCapacity;

    public double ProductionCapacity
    {
        get { return _productionCapacity; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Production capacity cannot be negative.");
            _productionCapacity = value;
        }
    }

    public IndustrialUser(
        string userId,
        string userName,
        string address,
        double previousMeterReading,
        double currentMeterReading)
        : base(userId, userName, address, previousMeterReading, currentMeterReading)
    {
        ProductionCapacity = 0;
    }

    public override double CalculateBill()
    {
        try
        {
            IndustrialBillCalculator calculator = new IndustrialBillCalculator(UnitConsumed());
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
        Console.WriteLine("Account Type    : Industrial");
        Console.WriteLine("Production Cap. : " + ProductionCapacity + " MW");
        Console.WriteLine("--------------------------------");
    }
}
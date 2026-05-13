using System;

public abstract class BillCalculator
{
    protected double UnitsConsumed;
    protected double TariffRate;

    protected BillCalculator(double unitsConsumed)
    {
        if (unitsConsumed < 0)
            throw new NegativeUnitsConsumedException(unitsConsumed);

        UnitsConsumed = unitsConsumed;
    }

    public abstract double CalculateBaseBill();
    public abstract double CalculateTax();
    public abstract double GetTariffRate();

    public double CalculateTotalBill()
    {
        try
        {
            double baseBill  = CalculateBaseBill();
            double taxAmount = CalculateTax();
            return baseBill + taxAmount;
        }
        catch (NegativeUnitsConsumedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while calculating the total bill. Please try again.", ex);
        }
    }
}
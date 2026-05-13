using System;

public class IndustrialBillCalculator : BillCalculator
{
    private const double IndustrialRatePerUnit = 6.0;
    private const double TaxPercentage         = 5.0;

    public IndustrialBillCalculator(double unitsConsumed) : base(unitsConsumed)
    {
        TariffRate = IndustrialRatePerUnit;
    }

    public override double CalculateBaseBill()
    {
        try
        {
            return UnitsConsumed * IndustrialRatePerUnit;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while calculating the industrial base bill.", ex);
        }
    }

    public override double CalculateTax()
    {
        try
        {
            double baseAmount = CalculateBaseBill();
            return baseAmount * (TaxPercentage / 100.0);
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while calculating the industrial tax.", ex);
        }
    }

    public override double GetTariffRate()
    {
        return IndustrialRatePerUnit;
    }
}
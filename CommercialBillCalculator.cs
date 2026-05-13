using System;

public class CommercialBillCalculator : BillCalculator
{
    private const double CommercialRatePerUnit = 8.0;
    private const double TaxPercentage         = 15.0;

    public CommercialBillCalculator(double unitsConsumed) : base(unitsConsumed)
    {
        TariffRate = CommercialRatePerUnit;
    }

    public override double CalculateBaseBill()
    {
        try
        {
            return UnitsConsumed * CommercialRatePerUnit;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while calculating the commercial base bill.", ex);
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
                "An unexpected error occurred while calculating the commercial tax.", ex);
        }
    }

    public override double GetTariffRate()
    {
        return CommercialRatePerUnit;
    }
}
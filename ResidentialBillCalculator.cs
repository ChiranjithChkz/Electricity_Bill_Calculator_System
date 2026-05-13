using System;

public class ResidentialBillCalculator : BillCalculator
{
    private const double ResidentialRatePerUnit = 5.0;
    private const double TaxPercentage          = 10.0;

    public ResidentialBillCalculator(double unitsConsumed) : base(unitsConsumed)
    {
        TariffRate = ResidentialRatePerUnit;
    }

    public override double CalculateBaseBill()
    {
        try
        {
            return UnitsConsumed * ResidentialRatePerUnit;
        }
        catch (Exception ex)
        {
            throw new Exception(
                "An unexpected error occurred while calculating the residential base bill.", ex);
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
                "An unexpected error occurred while calculating the residential tax.", ex);
        }
    }

    public override double GetTariffRate()
    {
        return ResidentialRatePerUnit;
    }
}
public class CustomerValidator : ICustomerValidator
{
    public bool ValidateId(string customerId)
    {
        return !string.IsNullOrWhiteSpace(customerId);
    }

    public bool ValidateName(string customerName)
    {
        return !string.IsNullOrWhiteSpace(customerName);
    }

    public bool ValidateReading(double meterReading)
    {
        return meterReading >= 0;
    }
}
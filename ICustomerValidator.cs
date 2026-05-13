public interface ICustomerValidator
{
    bool ValidateId(string customerId);
    bool ValidateName(string customerName);
    bool ValidateReading(double meterReading);
}
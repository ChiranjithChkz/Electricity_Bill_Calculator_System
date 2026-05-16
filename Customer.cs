public class Customer
{
    private string _customerId   = "";
    private string _customerName = "";
    private double _previousMeterReading;
    private double _currentMeterReading;

    private ICustomerValidator _validator;

    public Customer(ICustomerValidator validator)
    {
        if (validator == null)
            throw new ArgumentNullException(nameof(validator),
                "Must be provided to create a Customer.");
        _validator = validator;
    }

    public string CustomerId
    {
        get { return _customerId; }
        set
        {
            if (!_validator.ValidateId(value))
                throw new InvalidCustomerIdException(value ?? "");
            _customerId = value;
        }
    }

    public string CustomerName
    {
        get { return _customerName; }
        set
        {
            if (!_validator.ValidateName(value))
                throw new InvalidCustomerNameException(value ?? "");
            _customerName = value;
        }
    }

    public string Address { get; set; } = "N/A";

    public double PreviousReading
    {
        get { return _previousMeterReading; }
        set
        {
            if (!_validator.ValidateReading(value))
                throw new InvalidCustomerReadingException("Previous Reading", value);
            _previousMeterReading = value;
        }
    }

    public double CurrentReading
    {
        get { return _currentMeterReading; }
        set
        {
            if (!_validator.ValidateReading(value))
                throw new InvalidCustomerReadingException("Current Reading", value);
            if (value < _previousMeterReading)
                throw new CurrentReadingBelowPreviousException(_previousMeterReading, value);
            _currentMeterReading = value;
        }
    }
}
using System;

public class ElectricityUser
{
    private string _userId;
    private string _userName;
    private string _address;
    private int    _connectionYear;
    private double _previousMeterReading;
    private double _currentMeterReading;

    public string UserId
    {
        get { return _userId; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidUserIdException(value ?? "");
            _userId = value.Trim();
        }
    }

    public string UserName
    {
        get { return _userName; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidUserNameException(value ?? "");
            _userName = value.Trim();
        }
    }

    public string Address
    {
        get { return _address; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Address cannot be empty or blank.");
            _address = value.Trim();
        }
    }

    public int ConnectionYear
    {
        get { return _connectionYear; }
        set
        {
            if (value < 1900 || value > DateTime.Now.Year)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Connection year must be between 1900 and {DateTime.Now.Year}. Provided: {value}.");
            _connectionYear = value;
        }
    }

    public double PreviousReading
    {
        get { return _previousMeterReading; }
        set
        {
            if (value < 0)
                throw new InvalidMeterReadingException("Previous Reading", value);
            _previousMeterReading = value;
        }
    }

    public double CurrentReading
    {
        get { return _currentMeterReading; }
        set
        {
            if (value < 0)
                throw new InvalidMeterReadingException("Current Reading", value);
            if (value < _previousMeterReading)
                throw new CurrentReadingBelowPreviousException(_previousMeterReading, value);
            _currentMeterReading = value;
        }
    }

    public PaymentTracker PaymentTracker { get; }

    public ElectricityUser(
        string userId,
        string userName,
        string address,
        double previousMeterReading,
        double currentMeterReading)
    {
        UserId          = userId;
        UserName        = userName;
        Address         = address;
        PreviousReading = previousMeterReading;
        CurrentReading  = currentMeterReading;
        ConnectionYear  = 2024;
        PaymentTracker  = new PaymentTracker();
    }

    public double UnitConsumed()
    {
        double units = CurrentReading - PreviousReading;
        if (units < 0)
            throw new NegativeUnitsConsumedException(units);
        return units;
    }

    public virtual double CalculateBill()
    {
        return UnitConsumed() * 5;
    }

    public virtual void DisplayUserInfo()
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine("User ID         : " + UserId);
        Console.WriteLine("Name            : " + UserName);
        Console.WriteLine("Address         : " + Address);
        Console.WriteLine("Connection Year : " + ConnectionYear);
        Console.WriteLine("Previous Reading: " + PreviousReading + " kWh");
        Console.WriteLine("Current Reading : " + CurrentReading  + " kWh");
        Console.WriteLine("Units Consumed  : " + UnitConsumed()  + " kWh");
        Console.WriteLine("--------------------------------");
    }
}
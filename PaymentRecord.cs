using System;

public class PaymentRecord
{
    private double _amountPaid;
    private string _referenceNumber;
    private string _remarks;

    public DateTime      PaymentDate { get; }
    public PaymentMethod Method      { get; }

    public double AmountPaid
    {
        get { return _amountPaid; }
    }

    public string ReferenceNumber
    {
        get { return _referenceNumber; }
    }

    public string Remarks
    {
        get { return _remarks; }
    }

    public PaymentRecord(double amountPaid, PaymentMethod method, string referenceNumber, string remarks)
    {
        if (amountPaid <= 0)
            throw new InvalidPaymentAmountException(amountPaid);

        if (string.IsNullOrWhiteSpace(referenceNumber))
            throw new InvalidPaymentReferenceException(referenceNumber ?? "");

        _amountPaid      = amountPaid;
        _referenceNumber = referenceNumber.Trim();
        _remarks         = string.IsNullOrWhiteSpace(remarks) ? "N/A" : remarks.Trim();
        Method           = method;
        PaymentDate      = DateTime.Now;
    }

    public void DisplayPaymentRecord()
    {
        Console.WriteLine("  Reference No.  : " + ReferenceNumber);
        Console.WriteLine("  Amount Paid    : " + AmountPaid.ToString("F2") + " BDT");
        Console.WriteLine("  Payment Method : " + Method.ToString().Replace("Payment", " Payment"));
        Console.WriteLine("  Payment Date   : " + PaymentDate.ToString("dd MMM yyyy  hh:mm tt"));
        Console.WriteLine("  Remarks        : " + Remarks);
    }
}
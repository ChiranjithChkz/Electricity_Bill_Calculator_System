using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class PaymentTracker
{
    private readonly List<PaymentRecord> _paymentHistory;
    private double        _totalBillAmount;
    private PaymentStatus _currentStatus;

    public ReadOnlyCollection<PaymentRecord> PaymentHistory
    {
        get { return _paymentHistory.AsReadOnly(); }
    }

    public double TotalBillAmount
    {
        get { return _totalBillAmount; }
    }

    public double TotalAmountPaid
    {
        get
        {
            double total = 0;
            foreach (PaymentRecord record in _paymentHistory)
                total += record.AmountPaid;
            return total;
        }
    }

    public double OutstandingBalance
    {
        get
        {
            double balance = _totalBillAmount - TotalAmountPaid;
            return balance < 0 ? 0 : balance;
        }
    }

    public double OverpaidAmount
    {
        get
        {
            double overpaid = TotalAmountPaid - _totalBillAmount;
            return overpaid < 0 ? 0 : overpaid;
        }
    }

    public PaymentStatus CurrentStatus
    {
        get { return _currentStatus; }
    }

    public int TotalPaymentCount
    {
        get { return _paymentHistory.Count; }
    }

    public PaymentTracker()
    {
        _paymentHistory  = new List<PaymentRecord>();
        _totalBillAmount = 0;
        _currentStatus   = PaymentStatus.Unpaid;
    }

    public void SetBillAmount(double billAmount)
    {
        if (billAmount < 0)
            throw new InvalidPaymentAmountException(billAmount);

        _totalBillAmount = billAmount;
        RefreshPaymentStatus();
    }

    public void RecordPayment(double amountPaid, PaymentMethod method, string referenceNumber, string remarks)
    {
        if (_totalBillAmount == 0)
            throw new NoBillGeneratedException();

        PaymentRecord newRecord = new PaymentRecord(amountPaid, method, referenceNumber, remarks);

        if (TotalAmountPaid + amountPaid > _totalBillAmount * 1.5)
            throw new ExcessiveOverpaymentException(amountPaid, _totalBillAmount, TotalAmountPaid);

        _paymentHistory.Add(newRecord);
        RefreshPaymentStatus();
    }

    private void RefreshPaymentStatus()
    {
        if (_totalBillAmount == 0)
        {
            _currentStatus = PaymentStatus.Unpaid;
            return;
        }

        if (TotalAmountPaid == 0)
            _currentStatus = PaymentStatus.Unpaid;
        else if (TotalAmountPaid >= _totalBillAmount)
            _currentStatus = PaymentStatus.Paid;
        else
            _currentStatus = PaymentStatus.PartiallyPaid;
    }

    public void MarkAsOverdue()
    {
        if (_currentStatus == PaymentStatus.Unpaid || _currentStatus == PaymentStatus.PartiallyPaid)
            _currentStatus = PaymentStatus.Overdue;
    }

    public void DisplayPaymentSummary()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("         PAYMENT SUMMARY                ");
        Console.WriteLine("========================================");
        Console.WriteLine("  Total Bill     : " + TotalBillAmount.ToString("F2")    + " BDT");
        Console.WriteLine("  Total Paid     : " + TotalAmountPaid.ToString("F2")    + " BDT");
        Console.WriteLine("  Outstanding    : " + OutstandingBalance.ToString("F2") + " BDT");

        if (OverpaidAmount > 0)
            Console.WriteLine("  Overpaid       : " + OverpaidAmount.ToString("F2") +
                              " BDT (credit for next bill)");

        Console.WriteLine("  Payment Status : " + FormatStatus(_currentStatus));
        Console.WriteLine("  No. of Payments: " + TotalPaymentCount);
        Console.WriteLine("========================================");
    }

    public void DisplayPaymentHistory()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("         PAYMENT HISTORY                ");
        Console.WriteLine("========================================");

        if (_paymentHistory.Count == 0)
        {
            Console.WriteLine("  No payments have been recorded yet.");
            Console.WriteLine("========================================");
            return;
        }

        for (int index = 0; index < _paymentHistory.Count; index++)
        {
            Console.WriteLine($"  --- Payment #{index + 1} ---");
            _paymentHistory[index].DisplayPaymentRecord();
            Console.WriteLine();
        }

        Console.WriteLine("----------------------------------------");
        Console.WriteLine("  Total Paid : " + TotalAmountPaid.ToString("F2") + " BDT");
        Console.WriteLine("  Status     : " + FormatStatus(_currentStatus));
        Console.WriteLine("========================================");
    }

    private string FormatStatus(PaymentStatus status)
    {
        switch (status)
        {
            case PaymentStatus.Paid:          return "PAID";
            case PaymentStatus.PartiallyPaid: return "PARTIALLY PAID";
            case PaymentStatus.Overdue:       return "OVERDUE";
            default:                          return "UNPAID";
        }
    }
}
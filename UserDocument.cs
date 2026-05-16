using System;

public class UserDocument
{
    private readonly DocumentType _documentType;
    private readonly string       _documentNumber;
    private readonly DateTime     _issueDate;
    private readonly DateTime     _expiryDate;
    private readonly string       _issuingAuthority;
    private readonly DateTime     _submittedAt;

    public DocumentType DocumentType
    {
        get { return _documentType; }
    }

    public string DocumentNumber
    {
        get { return _documentNumber; }
    }

    public DateTime IssueDate
    {
        get { return _issueDate; }
    }

    public DateTime ExpiryDate
    {
        get { return _expiryDate; }
    }

    public string IssuingAuthority
    {
        get { return _issuingAuthority; }
    }

    public DateTime SubmittedAt
    {
        get { return _submittedAt; }
    }

    public bool IsExpired
    {
        get { return DateTime.Today > _expiryDate; }
    }

    public int DaysUntilExpiry
    {
        get { return (int)(_expiryDate - DateTime.Today).TotalDays; }
    }

    public UserDocument(
        DocumentType documentType,
        string       documentNumber,
        DateTime     issueDate,
        DateTime     expiryDate,
        string       issuingAuthority)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new InvalidDocumentNumberException(documentNumber ?? "");

        if (issueDate > DateTime.Today)
            throw new ArgumentException(
                $"Issue date ({issueDate:dd MMM yyyy}) cannot be in the future.");

        if (expiryDate <= issueDate)
            throw new ArgumentException(
                $"Expiry date ({expiryDate:dd MMM yyyy}) must be after the issue date ({issueDate:dd MMM yyyy}).");

        if (expiryDate < DateTime.Today)
            throw new ExpiredDocumentException(documentType, expiryDate);

        if (string.IsNullOrWhiteSpace(issuingAuthority))
            throw new ArgumentException("Issuing authority cannot be empty or blank.");

        _documentType     = documentType;
        _documentNumber   = documentNumber.Trim().ToUpper();
        _issueDate        = issueDate;
        _expiryDate       = expiryDate;
        _issuingAuthority = issuingAuthority.Trim();
        _submittedAt      = DateTime.Now;
    }

    public void DisplayDocument()
    {
        Console.WriteLine($"  Document Type   : {FormatDocumentType(_documentType)}");
        Console.WriteLine($"  Document Number : {_documentNumber}");
        Console.WriteLine($"  Issue Date      : {_issueDate:dd MMM yyyy}");
        Console.WriteLine($"  Expiry Date     : {_expiryDate:dd MMM yyyy}" +
                          (DaysUntilExpiry <= 30 ? $"  ⚠ Expires in {DaysUntilExpiry} day(s)" : ""));
        Console.WriteLine($"  Issued By       : {_issuingAuthority}");
        Console.WriteLine($"  Submitted At    : {_submittedAt:dd MMM yyyy  hh:mm tt}");
        Console.WriteLine($"  Status          : {(IsExpired ? "EXPIRED ✗" : "VALID ✓")}");
    }

    public static string FormatDocumentType(DocumentType type)
    {
        switch (type)
        {
            case DocumentType.NationalId:             return "National ID";
            case DocumentType.Passport:               return "Passport";
            case DocumentType.Driving:         return "Driving License";
            case DocumentType.UtilityOwnershipProof:  return "Utility Ownership Proof";
            case DocumentType.BusinessRegistration:   return "Business Registration";
            case DocumentType.IndustrialLicense:      return "Industrial License";
            default:                                   return type.ToString();
        }
    }
}
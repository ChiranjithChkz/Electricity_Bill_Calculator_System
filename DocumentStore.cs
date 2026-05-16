using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DocumentStore
{
    private readonly List<UserDocument> _documents;

    public ReadOnlyCollection<UserDocument> Documents
    {
        get { return _documents.AsReadOnly(); }
    }

    public int DocumentCount
    {
        get { return _documents.Count; }
    }

    public DocumentStore()
    {
        _documents = new List<UserDocument>();
    }

    public void AddDocument(UserDocument document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document), "Document cannot be null.");

        foreach (UserDocument existing in _documents)
        {
            if (existing.DocumentType == document.DocumentType)
                throw new DocumentAlreadyExistsException(document.DocumentType);
        }

        _documents.Add(document);
    }

    public bool HasDocument(DocumentType type)
    {
        foreach (UserDocument doc in _documents)
        {
            if (doc.DocumentType == type)
                return true;
        }
        return false;
    }

    public void ValidateRequiredDocumentsForResidential()
    {
        if (!HasDocument(DocumentType.NationalId))
            throw new MissingRequiredDocumentException(DocumentType.NationalId, "Residential");
        if (!HasDocument(DocumentType.UtilityOwnershipProof))
            throw new MissingRequiredDocumentException(DocumentType.UtilityOwnershipProof, "Residential");
    }

    public void ValidateRequiredDocumentsForCommercial()
    {
        if (!HasDocument(DocumentType.NationalId))
            throw new MissingRequiredDocumentException(DocumentType.NationalId, "Commercial");
        if (!HasDocument(DocumentType.BusinessRegistration))
            throw new MissingRequiredDocumentException(DocumentType.BusinessRegistration, "Commercial");
    }

    public void ValidateRequiredDocumentsForIndustrial()
    {
        if (!HasDocument(DocumentType.NationalId))
            throw new MissingRequiredDocumentException(DocumentType.NationalId, "Industrial");
        if (!HasDocument(DocumentType.BusinessRegistration))
            throw new MissingRequiredDocumentException(DocumentType.BusinessRegistration, "Industrial");
        if (!HasDocument(DocumentType.IndustrialLicense))
            throw new MissingRequiredDocumentException(DocumentType.IndustrialLicense, "Industrial");
    }

    public void DisplayAllDocuments()
    {
        if (_documents.Count == 0)
        {
            Console.WriteLine("  No documents have been submitted.");
            return;
        }

        for (int index = 0; index < _documents.Count; index++)
        {
            Console.WriteLine($"  --- Document {index + 1} ---");
            _documents[index].DisplayDocument();
            Console.WriteLine();
        }
    }
}
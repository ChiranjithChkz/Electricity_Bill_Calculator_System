# ⚡ Electricity Bill Calculator

<div align="center">

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_6.0+-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![OOP](https://img.shields.io/badge/OOP-4_Pillars-orange?style=for-the-badge)
![Exceptions](https://img.shields.io/badge/Custom_Exceptions-21-red?style=for-the-badge)
![Files](https://img.shields.io/badge/Source_Files-21-blue?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-Complete-brightgreen?style=for-the-badge)

**A production-quality, fully object-oriented electricity billing system built in C# (.NET) — covering interactive user registration with document verification, tariff-based billing, payment tracking, encapsulated data protection, and a complete custom exception framework.**

[Features](#-features) · [OOP Concepts](#-oop-concepts-applied) · [Architecture](#-project-architecture) · [Document System](#-document-verification-system) · [Billing Logic](#-billing--tariff-logic) · [Payment System](#-payment-tracking-system) · [Exceptions](#-exception-handling-system) · [Encapsulation](#-encapsulation-deep-dive) · [Getting Started](#-getting-started) · [Sample Output](#-sample-output)

</div>

---

## 📖 About the Project

The **Electricity Bill Calculator** is a console-based application that simulates a real-world electricity billing system used by utility companies. It handles three distinct customer account types — **Residential**, **Commercial**, and **Industrial** — each with their own tariff rates, tax percentages, and billing rules.

What makes this project stand apart from a basic calculator is its **depth and completeness**. Beyond generating a number, it implements:

- A fully interactive **user registration system** with step-by-step console input, type-specific required documents, and document validation (expiry, duplicates, required fields)
- A **document verification module** with immutable document records, per-account-type requirements, and a read-only document store
- A **payment tracking module** that records payments, tracks outstanding balances, handles partial payments, detects overpayments, and maintains an immutable payment history
- **21 custom exception classes** — one for every meaningful failure scenario — so nothing ever fails silently or with a meaningless generic message
- **4 OOP pillars** applied throughout: Encapsulation, Inheritance, Polymorphism and Abstraction design
- **Private backing fields and validated setters on every single property** in every single class — anonymous code simply cannot corrupt data

This is not a project that does one thing. It is a layered system that models how real billing software works.

---

## 🏆 Why This Project Is Best for Users

### 🔒 Your Data Cannot Be Corrupted
Every field in every class is `private`. Properties validate input before accepting it. You cannot assign a negative meter reading, an empty user ID, a negative payment, an expired document, or an impossible connection year — the system rejects it immediately with a specific message telling you exactly what is wrong and why.

### 📄 Document-Verified Registration
New users cannot be registered without submitting all required documents. The system enforces different document requirements per account type — residential users need a National ID and Utility Ownership Proof, commercial users need a National ID and Business Registration, and industrial users need all three plus an Industrial License. Submitting an expired document, an empty document number, or a duplicate document type is blocked at the point of entry.

### 💬 Errors Are Always Meaningful
Instead of crashing or printing "Error occurred", every failure has its own named exception class. The message tells the user what they tried to do, what value they used, and what the correct behaviour should be. No user is ever left confused about what went wrong.

### 💳 Full Payment Lifecycle
Most billing tools just calculate a number. This system takes that number and lets the user record payments against it — with a method, reference number, date, and remarks — then tracks the outstanding balance, detects when bills are fully paid, flags overpayments, and keeps a permanent history. The entire financial lifecycle of a customer account is managed in one place.

### 🛡️ Impossible to Misuse
The internal list of payments is exposed only as a read-only collection. The document store is also read-only externally. `UserManager`'s dictionary is private. Billing calculator logic is sealed inside each subclass. At every level, the system is designed so that misuse is not just discouraged — it is impossible.

### 🧭 Intuitive 9-Option Menu
The system guides users through a numbered menu that handles all error cases gracefully. Typing an invalid menu choice, an empty user ID, a non-numeric payment amount, or an expired document number never crashes the application — it always prints a clear, labelled error message and returns the user to the next prompt.

---

## ✨ Features

| Feature | Detail |
|---|---|
| 👥 **Interactive User Registration** | Full step-by-step console form: account type → user info → documents → validation → save |
| 📄 **Document Verification** | Required documents enforced per account type — expired, duplicate, or empty docs rejected |
| 🗂️ **Category Column Display** | View all registered users sorted into three side-by-side columns |
| 🔍 **User Lookup** | Find any user by ID — case-insensitive matching |
| 🧮 **Itemised Bill Generation** | Energy charge + tax + fixed service charge + 5% VAT |
| 💳 **Payment Recording** | Record payments with method, reference number, date, and remarks |
| 📊 **Payment Summary** | View total billed, paid, outstanding balance, and current status |
| 📜 **Payment History** | Chronological list of every payment for a given user |
| 📋 **Document Viewer** | View all submitted documents for any user with full detail and validity status |
| ⚠️ **Overdue Marking** | Flag unpaid or partially paid accounts as overdue |
| 🔐 **Full Encapsulation** | Every field private — validated setters enforce all business rules |
| 🚨 **21 Custom Exceptions** | One named exception per failure scenario, never a generic crash |
| 🔄 **Automatic Status Updates** | Payment status refreshes automatically after every payment |
| 🧾 **Overpayment Detection** | Warns when payment exceeds 150% of the bill and shows credit |
| 📅 **Expiry Warning** | Documents expiring within 30 days are flagged automatically |

---

## 🎓 OOP Concepts Applied

All four pillars of Object-Oriented Programming are present and deliberate in this project — not just named in a list but actually demonstrated through concrete implementation.

---

### 1 · Encapsulation

**What it means:** Hiding internal data and controlling access through validated properties.

**How it is applied here:** Every single field in every class is `private`. Public access is only granted through properties whose setters enforce business rules before writing to the backing field. It is structurally impossible for outside code to put bad data into the system.

```csharp
// ElectricityUser.cs
private double _previousMeterReading;

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
```

This pattern is applied to every property in `ElectricityUser`, `Customer`, `ResidentialUser`, `CommercialUser`, `IndustrialUser`, `PaymentRecord`, `PaymentTracker`, `UserDocument`, and `DocumentStore`.

---


### 2 · Polymorphism

**What it means:** The same method call produces different behaviour depending on the actual type of the object at runtime.

**How it is applied here:** `CalculateBill()` and `DisplayUserInfo()` are declared `virtual` in `ElectricityUser` and `override`d in all three subclasses. `UserManager` calls these methods without knowing — or caring — whether it is working with a residential, commercial, or industrial user. The correct version is selected automatically.

```csharp
// UserManager.cs — one line, three different behaviours depending on actual type
double energyCharge = selectedUser.CalculateBill();
```

If `selectedUser` is a `ResidentialUser`, the `ResidentialBillCalculator` runs at 5 BDT/kWh with 10% tax. If it is a `CommercialUser`, the `CommercialBillCalculator` runs at 8 BDT/kWh with 15% tax. The caller writes one line of code.

---

### 3 · Abstraction

**What it means:** Defining a contract that subclasses must fulfil, hiding implementation details behind a shared interface.

**How it is applied here:** `BillCalculator` is declared `abstract`. It forces every concrete calculator to implement three methods. The shared `CalculateTotalBill()` is implemented once in the base class and calls those abstract methods — it does not need to know how tax is calculated, only that each subclass will calculate it correctly.

```csharp
// BillCalculator.cs
public abstract double CalculateBaseBill();
public abstract double CalculateTax();
public abstract double GetTariffRate();

public double CalculateTotalBill()      // implemented once, used by all three subclasses
{
    double baseBill  = CalculateBaseBill();
    double taxAmount = CalculateTax();
    return baseBill + taxAmount;
}
```

A class that does not implement all three abstract methods will not compile. The contract is enforced by the language itself.

---

### 4 · Interface

**What it means:** Defining a strict contract that any class can implement, enabling loose coupling between components.

**How it is applied here:** `ICustomerValidator` defines exactly what a validator must be able to do. `Customer` depends on the interface — not on `CustomerValidator` directly. This means any class that implements `ICustomerValidator` can be passed to `Customer` — a stricter validator, a test mock, a database-backed validator — without changing a single line inside `Customer`.

```csharp
// ICustomerValidator.cs
public interface ICustomerValidator
{
    bool ValidateId(string customerId);
    bool ValidateName(string customerName);
    bool ValidateReading(double meterReading);
}

// Customer.cs constructor — depends on the interface, not the concrete class
public Customer(ICustomerValidator validator)
{
    if (validator == null)
        throw new ArgumentNullException(nameof(validator),
            "A validator must be provided to create a Customer.");
    _validator = validator;
}
```

---

###  · Inheritance (a category of class)

**What it means:** Child classes inherit shared behaviour from a parent and only override what is specific to them.

**How it is applied here:** Two clean, parallel inheritance trees — one for users, one for calculators. No code is duplicated anywhere.

```
ElectricityUser          ← holds all shared user data and virtual methods
├── ResidentialUser      ← adds MonthlyAllowance
├── CommercialUser       ← adds LicenseNumber
└── IndustrialUser       ← adds ProductionCapacity

BillCalculator (abstract)   ← defines the billing contract
├── ResidentialBillCalculator   ← 5.00 BDT/kWh, 10% tax
├── CommercialBillCalculator    ← 8.00 BDT/kWh, 15% tax
└── IndustrialBillCalculator    ← 6.00 BDT/kWh, 5% tax
```

Each subclass inherits shared logic from its parent and overrides only what differs — rates, tax percentages, display fields, and type-specific properties.

---

## 🏗️ Project Architecture

The project is organised in a strict dependency order. Each layer depends only on layers below it — nothing in the lower layers knows about the layers above.

```
┌─────────────────────────────────────────────────────────────────┐
│                          Program.cs                             │
│                   Entry point · Main menu loop (9 options)      │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                        UserManager.cs                           │
│   Registers users · Documents · Bills · Payments · Display      │
└──────┬───────────────────────┬──────────────────┬──────────────┘
       │                       │                  │
┌──────▼──────────┐  ┌─────────▼────────┐  ┌─────▼────────────────┐
│ ElectricityUser │  │  BillCalculator  │  │   PaymentTracker     │
│  (base class)   │  │ (abstract base)  │  │    (per-user)        │
│  + DocumentStore│  └──┬──────┬─────┬──┘  └──────┬───────────────┘
└──┬────┬────┬────┘     │      │     │            │
   │    │    │       Resid.  Comm. Indus.    PaymentRecord
Resid. Comm. Indus.  Calc.  Calc. Calc.      (immutable)
User   User  User
       │
  DocumentStore
       │
  UserDocument[]
  (immutable)

── Foundation Layer ──────────────────────────────────────────────
  PaymentStatus (enum)    PaymentMethod (enum)    DocumentType (enum)
  ElectricityExceptions (21 custom exception classes)
  ICustomerValidator → CustomerValidator → Customer
```

---

## 📁 Project File Structure

```
Electricity_Bill_Calculator/
│
│  ── Enums ──────────────────────────────────────────────────────
├── PaymentStatus.cs                # Unpaid / PartiallyPaid / Paid / Overdue
├── PaymentMethod.cs                # Cash / BankTransfer / MobileBanking / ChequePayment
├── DocumentType.cs                 # NationalId / Passport / DrivingLicense / ...
│
│  ── Exception Framework ────────────────────────────────────────
├── ElectricityExceptions.cs        # 21 named exception classes in one file
│
│  ── Validation Layer ───────────────────────────────────────────
├── ICustomerValidator.cs           # Interface — defines the validation contract
├── CustomerValidator.cs            # Implements ICustomerValidator
├── Customer.cs                     # Validated customer data model
│
│  ── Document System ────────────────────────────────────────────
├── UserDocument.cs                 # Single immutable document record
├── DocumentStore.cs                # Per-user document collection + requirement rules
│
│  ── Payment System ─────────────────────────────────────────────
├── PaymentRecord.cs                # Single immutable payment entry
├── PaymentTracker.cs               # Full payment lifecycle manager
│
│  ── Billing Engine ─────────────────────────────────────────────
├── BillCalculator.cs               # Abstract base — defines the billing contract
├── ResidentialBillCalculator.cs    # 5.00 BDT/kWh + 10% tax
├── CommercialBillCalculator.cs     # 8.00 BDT/kWh + 15% tax
├── IndustrialBillCalculator.cs     # 6.00 BDT/kWh + 5% tax
│
│  ── User Hierarchy ─────────────────────────────────────────────
├── ElectricityUser.cs              # Base class — shared user data + virtual methods
├── ResidentialUser.cs              # Adds MonthlyAllowance
├── CommercialUser.cs               # Adds LicenseNumber
├── IndustrialUser.cs               # Adds ProductionCapacity
│
│  ── Application ────────────────────────────────────────────────
├── UserManager.cs                  # All operations and business logic
└── Program.cs                      # Entry point and main menu loop
```

**Total: 21 source files**

---

## 📄 Document Verification System

Every new user registered through the system must submit the required documents for their account type before registration is finalised. Documents are validated at the point of entry — the registration form does not abort if one document fails; it shows the error, lets you correct it, and continues collecting documents until you are done.

### Supported Document Types

| # | Document Type | Who Typically Submits It |
|---|---|---|
| 1 | National ID | All account types (mandatory for all) |
| 2 | Passport | Optional identity alternative |
| 3 | Driving License | Optional identity alternative |
| 4 | Utility Ownership Proof | Required for Residential accounts |
| 5 | Business Registration | Required for Commercial and Industrial accounts |
| 6 | Industrial License | Required for Industrial accounts only |

### Required Documents Per Account Type

| Account Type | Required Documents |
|---|---|
| 🏠 **Residential** | National ID + Utility Ownership Proof |
| 🏢 **Commercial** | National ID + Business Registration |
| 🏭 **Industrial** | National ID + Business Registration + Industrial License |

Registration is **blocked** if any required document is missing. The system tells you exactly which document is still needed.

### Document Validation Rules

Every document submitted is checked against five rules before being accepted:

- **Document number** cannot be empty or whitespace
- **Issue date** cannot be in the future
- **Expiry date** must be strictly after the issue date
- **Document must not already be expired** at the time of submission
- **Issuing authority** cannot be empty or whitespace
- **Same document type** cannot be submitted twice for the same user

### Document Safety Design

- `UserDocument` is **fully immutable** — all fields are `private readonly` with no setters. Once created, a document record can never be changed.
- `DocumentStore` exposes its list only as `ReadOnlyCollection<UserDocument>` — external code can read but not modify the document history.
- Each user owns their own `DocumentStore` created at construction time and accessible only through the read-only `DocumentStore` property on `ElectricityUser`.

### Expiry Warning

If a valid document expires within 30 days, the system displays an automatic warning next to the expiry date when the document is viewed:

```
  Expiry Date     : 15 Jun 2026  ⚠ Expires in 28 day(s)
```

---

## 💰 Billing & Tariff Logic

### Tariff Rates by Account Type

| Account Type | Who Uses It | Rate (BDT/kWh) | Tax | Service Charge | VAT |
|---|---|---|---|---|---|
| 🏠 Residential | Home users | 5.00 | 10% | 120 BDT | 5% |
| 🏢 Commercial | Shops, businesses | 8.00 | 15% | 120 BDT | 5% |
| 🏭 Industrial | Factories, plants | 6.00 | 5% | 120 BDT | 5% |

### Bill Formula — Step by Step

```
Units Consumed   =  Current Reading  −  Previous Reading

Energy Charge    =  Units Consumed   ×  Rate per kWh
Tax              =  Energy Charge    ×  Tax Percentage
Service Charge   =  120.00 BDT  (fixed for all account types)
VAT (5%)         =  (Energy Charge + Tax + Service Charge) × 0.05

─────────────────────────────────────────────────────────────────
TOTAL BILL       =  Energy Charge + Tax + Service Charge + VAT
```

### Worked Example — Residential (Ahmed Khan)

```
Previous Reading :  3,000 kWh
Current Reading  :  4,500 kWh
Units Consumed   :  1,500 kWh

Energy Charge    :  1,500 × 5.00          =  7,500.00 BDT
Tax (10%)        :  7,500 × 0.10          =    750.00 BDT
Service Charge   :                            120.00 BDT
VAT (5%)         :  (7,500+750+120)×0.05  =    418.50 BDT
───────────────────────────────────────────────────────
TOTAL BILL       :                         8,788.50 BDT
```

---

## 💳 Payment Tracking System

The payment module manages the entire financial lifecycle of each user's bill — from generation through to final settlement.

### Supported Payment Methods

| Option | Method |
|---|---|
| 1 | Cash |
| 2 | Bank Transfer |
| 3 | Mobile Banking |
| 4 | Cheque Payment |

### Payment Status Flow

```
Bill Generated ──► UNPAID
                      │
          First partial payment made
                      │
                      ▼
               PARTIALLY PAID
                      │
            Full amount paid           Due date passed without full payment
                      │                          │
                      ▼                          ▼
                    PAID ✓                    OVERDUE ⚠
```

Status updates **automatically** after every payment — no manual status setting is required.

### What PaymentTracker Tracks

| Property | Description |
|---|---|
| `TotalBillAmount` | The bill amount set when the bill is calculated |
| `TotalAmountPaid` | Sum of all recorded payments — computed dynamically |
| `OutstandingBalance` | What is still owed — never returns a negative value |
| `OverpaidAmount` | Amount paid beyond the bill — displayed as a credit |
| `CurrentStatus` | Auto-refreshed after every payment |
| `TotalPaymentCount` | Number of individual payments recorded |
| `PaymentHistory` | Read-only collection — cannot be modified externally |

### Payment Safety Rules

- Payment amount must be **greater than zero** — otherwise `InvalidPaymentAmountException`
- A **reference number is mandatory** — otherwise `InvalidPaymentReferenceException`
- A **bill must exist** before any payment can be recorded — otherwise `NoBillGeneratedException`
- A single payment cannot push total paid beyond **150% of the bill** — otherwise `ExcessiveOverpaymentException`

---

## 🚨 Exception Handling System

Every possible failure scenario has its own named exception class. Nothing ever fails silently, crashes with a stack trace, or shows a generic "An error occurred" message to the user.

### Full Exception Reference

#### User & Registration Exceptions

| Exception Class | Thrown When |
|---|---|
| `InvalidUserIdException` | User ID is empty or whitespace |
| `InvalidUserNameException` | User name is empty or whitespace |
| `InvalidMeterReadingException` | Any meter reading is negative |
| `CurrentReadingBelowPreviousException` | Current reading is less than previous reading |
| `NegativeUnitsConsumedException` | Calculated consumption is negative |
| `UserNotFoundException` | No user found for the given ID |
| `DuplicateUserIdException` | A user with the same ID is already registered |
| `NullUserException` | A null object is passed to AddUser |
| `InvalidMenuChoiceException` | Input is outside the 1–9 menu range |
| `BillingCalculationException` | Any unexpected error during bill calculation |

#### Customer Validation Exceptions

| Exception Class | Thrown When |
|---|---|
| `InvalidCustomerIdException` | Customer ID fails validation |
| `InvalidCustomerNameException` | Customer name fails validation |
| `InvalidCustomerReadingException` | Customer meter reading is negative |

#### Payment Exceptions

| Exception Class | Thrown When |
|---|---|
| `InvalidPaymentAmountException` | Payment amount is zero or negative |
| `InvalidPaymentReferenceException` | Reference number is empty or whitespace |
| `NoBillGeneratedException` | Payment attempted before a bill is generated |
| `ExcessiveOverpaymentException` | Payment would exceed 150% of the total bill |

#### Document Exceptions

| Exception Class | Thrown When |
|---|---|
| `InvalidDocumentNumberException` | Document number is empty or whitespace |
| `ExpiredDocumentException` | Document's expiry date is already in the past |
| `MissingRequiredDocumentException` | A mandatory document type was not submitted before registration |
| `DocumentAlreadyExistsException` | Same document type submitted twice for one user |

### Exception Design Pattern

Every custom exception follows the same structure — it extends `Exception`, carries a specific descriptive message, and exposes the bad value as a readable property so catch blocks can work with it programmatically.

```csharp
public class ExpiredDocumentException : Exception
{
    public DocumentType DocumentType { get; }
    public DateTime     ExpiryDate   { get; }

    public ExpiredDocumentException(DocumentType documentType, DateTime expiryDate)
        : base($"{UserDocument.FormatDocumentType(documentType)} expired on " +
               $"{expiryDate:dd MMM yyyy}. Only valid documents can be submitted.")
    {
        DocumentType = documentType;
        ExpiryDate   = expiryDate;
    }
}
```

### How Exceptions Are Displayed to the User

Every catch block in `UserManager` and `Program` prefixes the message with a label so the user knows the category of the error instantly:

```
  [Input Error]         — something typed was invalid
  [Not Found]           — the requested user does not exist
  [Registration Error]  — a duplicate or null user was submitted
  [Registration Blocked]— required documents are missing; user was not saved
  [Billing Error]       — something went wrong during bill/payment calculation
  [Payment Error]       — a payment-specific rule was violated
  [Document Error]      — a document validation rule was violated
  [Unexpected Error]    — a truly unknown error occurred (with details)
  [Setup Error]         — a pre-loaded sample user failed to initialise
  [Critical Error]      — a fatal application-level failure
```

---

## 🔒 Encapsulation Deep Dive

Encapsulation in this project is not a decoration — it is a complete design decision applied at six distinct levels.

### Level 1 — Private Backing Fields Everywhere

No class has a public field. Every piece of data is stored in a `private` variable and accessed only through a property.

```csharp
// ElectricityUser.cs — all six fields are private
private string _userId;
private string _userName;
private string _address;
private int    _connectionYear;
private double _previousMeterReading;
private double _currentMeterReading;
```

### Level 2 — Validated Setters on Every Property

Every writable property enforces at least one business rule before assigning the value. Bad data throws a named exception before it ever reaches the backing field.

```csharp
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
```

### Level 3 — Immutable Payment Records

Once a `PaymentRecord` is created, it cannot be changed. There are no setters — only getters. The payment date, amount, method, and reference number are fixed at construction time and permanent.

```csharp
// PaymentRecord.cs — no setters exist
public double   AmountPaid      { get { return _amountPaid; } }
public string   ReferenceNumber { get { return _referenceNumber; } }
public DateTime PaymentDate     { get; }
public PaymentMethod Method     { get; }
```

### Level 4 — Immutable Document Records

`UserDocument` applies the same principle as `PaymentRecord`. All six fields are `private readonly`. No setter exists on any property. A document record, once submitted, is a permanent and unalterable entry.

```csharp
// UserDocument.cs — all fields are private readonly
private readonly DocumentType _documentType;
private readonly string       _documentNumber;
private readonly DateTime     _issueDate;
private readonly DateTime     _expiryDate;
private readonly string       _issuingAuthority;
private readonly DateTime     _submittedAt;
```

### Level 5 — Read-Only Collections

Both `PaymentTracker` and `DocumentStore` expose their internal lists only as `ReadOnlyCollection<T>`. External code can iterate and read but cannot add, remove, or replace entries.

```csharp
// DocumentStore.cs
private readonly List<UserDocument> _documents;

public ReadOnlyCollection<UserDocument> Documents
{
    get { return _documents.AsReadOnly(); }
}
```

### Level 6 — Private Internals in UserManager

The registered user dictionary and all category filter methods are `private`. External code has no direct path to the dictionary — it must go through the defined public methods.

```csharp
private Dictionary<string, ElectricityUser> registeredUsers = ...;
private List<ElectricityUser> GetResidentialUsers() => ...;
private List<ElectricityUser> GetCommercialUsers()  => ...;
private List<ElectricityUser> GetIndustrialUsers()  => ...;
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK 6.0 or later](https://dotnet.microsoft.com/download) — free to download and install

### Clone and Run

```bash
# 1. Clone the repository
git clone https://github.com/your-username/electricity-bill-calculator.git

# 2. Navigate into the project folder
cd electricity-bill-calculator

# 3. Run the application
dotnet run
```

> Replace `your-username` with your actual GitHub username.

### Pre-loaded Test Users

Three users are registered automatically at startup so you can test billing and payment features immediately:

| User ID | Name | Account Type |
|---|---|---|
| `R001` | Ahmed Khan | Residential |
| `C001` | Tech Shop | Commercial |
| `I001` | Power Factory | Industrial |

> User IDs are **case-insensitive** — `r001`, `R001`, and `r001` all work identically.

---

## 🗺️ Usage Walkthrough

### Main Menu

```
============================================
               MAIN MENU
============================================
  1. Show All Users
  2. View Specific User
  3. Calculate Bill
  4. Record Payment
  5. View Payment Summary
  6. View Payment History
  7. Register New User
  8. View User Documents
  9. Exit
```

### Recommended Workflow — Billing

```
Option 3  →  Calculate Bill          Generate the bill (must do this first)
Option 4  →  Record Payment          Pay part or all of the bill
Option 5  →  View Payment Summary    Check outstanding balance and status
Option 6  →  View Payment History    See every individual payment recorded
```

### Recommended Workflow — New User Registration

```
Option 7  →  Register New User
              Step 1: Select account type (Residential / Commercial / Industrial)
              Step 2: Enter user details (ID, name, address, meter readings)
              Step 3: Enter type-specific field (allowance / license / capacity)
              Step 4: Submit required documents one by one
                        - Select document type (1–6)
                        - Enter document number
                        - Enter issue date (dd/MM/yyyy)
                        - Enter expiry date (dd/MM/yyyy)
                        - Enter issuing authority
                        - Repeat or type 0 to finish
              Step 5: System validates required documents
                        - If all present → user is registered ✓
                        - If any missing → registration is blocked ✗
Option 8  →  View User Documents     See all submitted documents for any user
```

---

## 🖥️ Sample Output

### Register New User (Option 7)

```
========================================
         REGISTER NEW USER
========================================
  Select Account Type:
  1. Residential
  2. Commercial
  3. Industrial
  Enter choice (1-3): 1

  --- User Information ---
  User ID        : R002
  Full Name      : Sara Islam
  Address        : 45 Lake View Road
  Previous Reading (kWh): 1200
  Current Reading  (kWh): 1950

  Monthly Allowance (BDT, press Enter for 0): 500

  --- Required Documents for Residential Account ---
    • National ID
    • Utility Ownership Proof

  Available Document Types:
    1. National ID   2. Passport   3. Driving License
    4. Utility Ownership Proof   5. Business Registration
    6. Industrial License        0. Done
  Select document type: 1
  Document Number  : NID-882345
  Issue Date (dd/MM/yyyy)  : 10/03/2018
  Expiry Date (dd/MM/yyyy) : 10/03/2028
  Issuing Authority        : Bangladesh Election Commission

  [OK] National ID added successfully.
  Documents submitted so far: 1

  Select document type: 4
  Document Number  : UTIL-200456
  Issue Date (dd/MM/yyyy)  : 01/06/2021
  Expiry Date (dd/MM/yyyy) : 01/06/2031
  Issuing Authority        : DESCO

  [OK] Utility Ownership Proof added successfully.
  Documents submitted so far: 2

  Select document type: 0

========================================
        USER REGISTERED SUCCESSFULLY
========================================
  User ID      : R002
  Name         : Sara Islam
  Account Type : Residential
  Address      : 45 Lake View Road
  Documents    : 2 submitted
  Total Users  : 4
========================================
```

### View User Documents (Option 8)

```
========================================
         USER DOCUMENTS
========================================
  User     : Sara Islam (R002)
  Address  : 45 Lake View Road
  Total    : 2 document(s)
----------------------------------------
  --- Document 1 ---
  Document Type   : National ID
  Document Number : NID-882345
  Issue Date      : 10 Mar 2018
  Expiry Date     : 10 Mar 2028
  Issued By       : Bangladesh Election Commission
  Submitted At    : 16 May 2026  10:32 AM
  Status          : VALID ✓

  --- Document 2 ---
  Document Type   : Utility Ownership Proof
  Document Number : UTIL-200456
  Issue Date      : 01 Jun 2021
  Expiry Date     : 01 Jun 2031
  Issued By       : DESCO
  Submitted At    : 16 May 2026  10:33 AM
  Status          : VALID ✓

========================================
```

### Registration Blocked — Missing Document

```
  [Registration Blocked] Missing required document: 'Utility Ownership Proof'
  is mandatory for Residential accounts.
  User was NOT registered. Please restart and submit all required documents.
```

### Document Error During Submission

```
  [Document Error] National ID expired on 01 Jan 2024.
  Only valid documents can be submitted.

  [Document Error] A document of type 'National ID' has already been
  submitted for this user. Each document type can only be added once.
```

### Bill Report (Option 3)

```
========================================
        ELECTRICITY BILL REPORT
========================================
Customer Name   : Ahmed Khan
Customer ID     : R001
Address         : 123 Main St
Account Type    : Residential
Tariff Info     : Rate: 5 BDT/kWh + 10% Tax
Billing Month   : May 2026
----------------------------------------
Previous Reading: 3000 kWh
Current Reading : 4500 kWh
Units Consumed  : 1500 kWh
----------------------------------------
Charges:
  Energy Charge : 7500.00 BDT
  Service Charge: 120.00 BDT
  VAT (5%)      : 381.00 BDT
----------------------------------------
  TOTAL BILL    : 8001.00 BDT
========================================
Due Date        : 20 May 2026
Payment Status  : Unpaid
Outstanding     : 8001.00 BDT

  Thank You for using our service!
========================================
```

### Payment Recorded (Option 4)

```
========================================
        PAYMENT RECORDED
========================================
  User           : Ahmed Khan (R001)
  Amount Paid    : 5000.00 BDT
  Method         : MobileBanking
  Reference No.  : TXN987654
  Date           : 13 May 2026  09:45 AM
----------------------------------------
  Outstanding    : 3001.00 BDT
  Payment Status : PartiallyPaid
========================================
```

---

## 🧠 Design Decisions Explained

### Why a Separate Document System?

Separating document storage (`DocumentStore`, `UserDocument`) from user data (`ElectricityUser`) follows the **Single Responsibility Principle**. `ElectricityUser` knows about meter readings and billing. `DocumentStore` knows about what documents a user holds and whether requirements are met. Neither class needs to know about the other's internals. This makes both independently testable and extendable.

### Why Block Registration on Missing Documents?

In a real utility company, a customer cannot receive a connection without identity verification and proof of ownership or business registration. Enforcing this in the system — rather than making documents optional — means the data model accurately reflects reality. The `ValidateRequiredDocuments` methods in `DocumentStore` are the enforcement layer, and they throw a `MissingRequiredDocumentException` with the exact document name and account type, so the user knows precisely what is still needed.

### Why Immutable UserDocument?

A submitted document is a legal record. It should not be editable after submission — the same way a signed form cannot be quietly modified later. Making all fields `private readonly` and removing all setters enforces this permanently at the language level, not just through convention.

### Why Separate BillCalculator and ElectricityUser Hierarchies?

Keeping billing logic out of user classes follows the **Single Responsibility Principle**. `ResidentialUser` knows about the customer — their ID, address, readings. `ResidentialBillCalculator` knows about billing rates and tax. Neither class has any reason to reach into the other's domain.

### Why 21 Exception Classes?

A single `throw new Exception("something went wrong")` fails on three counts — it tells the developer nothing about what failed, it tells the user nothing actionable, and it is impossible to catch specifically. Named exceptions solve all three: each catch block is precise, each message is accurate, and adding a new failure mode means adding a new class — nothing existing is disturbed.

### Why ReadOnlyCollection for Both Payment History and Document Store?

Both payment history and document history are permanent records. It should never be possible for external code to insert, delete, or reorder entries in either list. `ReadOnlyCollection<T>` enforces this at compile time — the wrapper is built into the .NET type system.

### Why Interface for Validation?

`ICustomerValidator` decouples `Customer` from its validator. In a production system it would mean you could swap in a validator backed by a database, an API, or a regex library without touching the `Customer` class at all.

---

## 🛠️ Bugs Fixed During Development

This project was carefully debugged and improved from its original version. Every issue was identified, diagnosed, and corrected.

| # | Bug Found | Root Cause | Fix Applied |
|---|---|---|---|
| 1 | `CalculatorTax()` typo in all three calculator classes | Method name did not match the abstract base — it was never overriding anything | Renamed to `CalculateTax()` across all three subclasses |
| 2 | `CalculateBill()` in user classes used hardcoded multiplication | The BillCalculator hierarchy was completely unused | Each user class now instantiates the correct calculator and delegates to it |
| 3 | `Customer` class was missing `Address` and `PreviousReading` | Properties were never defined — only `CurrentReading` existed | Both added with private backing fields and validated setters |
| 4 | Irrelevant `using` directives in multiple files | Likely copied from unrelated code | All unused directives removed from all files |
| 5 | `MonthlyAllowance`, `LicenseNumber`, `ProductionCapacity` were unprotected auto-properties | No validation — anyone could set them to invalid values | Replaced with private backing fields and validated setters |
| 6 | `Address` and `ConnectionYear` in `ElectricityUser` were unprotected | No validation — blank addresses and impossible years were accepted | Both now have private fields with validated setters |
| 7 | All errors used `throw new Exception(...)` | No meaningful distinction between failure types | Replaced with 21 domain-specific custom exception classes |

---

## 🛠️ Tech Stack

| Technology | Version | Why Used |
|---|---|---|
| C# | 10+ | Primary language — strong OOP support, clean syntax |
| .NET | 6.0+ | Cross-platform runtime — runs on Windows, macOS, Linux |
| `System.Collections.ObjectModel` | Built-in | `ReadOnlyCollection<T>` for immutable payment history and document store |
| `System.Collections.Generic` | Built-in | `Dictionary<K,V>` for fast user lookup, `List<T>` for records |
| `System.Linq` | Built-in | Category filtering in `UserManager` |
| `System.Globalization` | Built-in | `DateTime.TryParseExact` for strict date input parsing in registration |

---

## 📄 License

This project can be used for educational purpose.

---

<div align="center">

**⚡ Electricity Bill Calculator**

*Clean Architecture · 5 OOP Pillars · 21 Custom Exceptions · Document Verification · Full Payment Lifecycle · Complete Encapsulation*

Built as an Object-Oriented Programming university project in C#.

</div>



## For Push in gitub
```
git init
git add README.md
git commit -m "first commit"
git branch -M main
git remote add origin https://github.com/ChiranjithChkz/Electricity_Bill_Calculator_System.git
git push -u origin main
```
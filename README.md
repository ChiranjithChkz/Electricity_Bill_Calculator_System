# ⚡ Electricity Bill Calculator

<div align="center">

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_6.0+-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![OOP](https://img.shields.io/badge/OOP-5_Pillars-orange?style=for-the-badge)
![Exceptions](https://img.shields.io/badge/Custom_Exceptions-17-red?style=for-the-badge)
![Files](https://img.shields.io/badge/Source_Files-18-blue?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-Complete-brightgreen?style=for-the-badge)

**A production-quality, fully object-oriented electricity billing system built in C# (.NET) — covering user management, tariff-based billing, payment tracking, encapsulated data protection, and a complete custom exception framework.**

[Features](#-features) · [OOP Concepts](#-oop-concepts-applied) · [Architecture](#-project-architecture) · [Billing Logic](#-billing--tariff-logic) · [Payment System](#-payment-tracking-system) · [Exceptions](#-exception-handling-system) · [Encapsulation](#-encapsulation-deep-dive) · [Getting Started](#-getting-started) · [Sample Output](#-sample-output)

</div>

---

## 📖 About the Project

The **Electricity Bill Calculator** is a console-based application that simulates a real-world electricity billing system used by utility companies. It handles three distinct customer account types — **Residential**, **Commercial**, and **Industrial** — each with their own tariff rates, tax percentages, and billing rules.

What makes this project stand apart from a basic calculator is its **depth and completeness**. Beyond generating a number, it implements:

- A fully validated **user management system** with encapsulated data at every layer
- A **payment tracking module** that records payments, tracks outstanding balances, handles partial payments, detects overpayments, and maintains an immutable payment history
- **17 custom exception classes** — one for every meaningful failure scenario — so nothing ever fails silently or with a meaningless generic message
- **5 OOP pillars** applied throughout: Encapsulation, Inheritance, Polymorphism, Abstraction, and Interface-based design
- **Private backing fields and validated setters on every single property** in every single class — anonymous code simply cannot corrupt data

This is not a project that does one thing. It is a layered system that models how real billing software works.

---

## 🏆 Why This Project Is Best for Users

### 🔒 Your Data Cannot Be Corrupted
Every field in every class is `private`. Properties validate input before accepting it. You cannot assign a negative meter reading, an empty user ID, a negative payment, or an impossible connection year — the system rejects it immediately with a specific message telling you exactly what is wrong and why.

### 💬 Errors Are Always Meaningful
Instead of crashing or printing "Error occurred", every failure has its own named exception class. The message tells the user what they tried to do, what value they used, and what the correct behaviour should be. No user is ever left confused about what went wrong.

### 💳 Full Payment Lifecycle
Most billing tools just calculate a number. This system takes that number and lets the user record payments against it — with a method, reference number, date, and remarks — then tracks the outstanding balance, detects when bills are fully paid, flags overpayments, and keeps a permanent history. The entire financial lifecycle of a customer account is managed in one place.

### 🛡️ Impossible to Misuse
The internal list of payments is exposed only as a read-only collection. The payment history cannot be modified from outside `PaymentTracker`. `UserManager`'s dictionary is private. Billing calculator logic is sealed inside each subclass. At every level, the system is designed so that misuse is not just discouraged — it is impossible.

### 🧭 Intuitive 7-Option Menu
The system guides users through a numbered menu that handles all error cases gracefully. Typing an invalid menu choice, an empty user ID, or a non-numeric payment amount never crashes the application — it always prints a clear, labelled error message and returns the user to the menu.

---

## ✨ Features

| Feature | Detail |
|---|---|
| 👥 **Multi-type User Management** | Register and manage Residential, Commercial, and Industrial users |
| 🗂️ **Category Column Display** | View all registered users sorted into three side-by-side columns |
| 🔍 **User Lookup** | Find any user by ID — case-insensitive matching |
| 🧮 **Itemised Bill Generation** | Energy charge + tax + fixed service charge + 5% VAT |
| 💳 **Payment Recording** | Record payments with method, reference number, date, and remarks |
| 📊 **Payment Summary** | View total billed, paid, outstanding balance, and current status |
| 📜 **Payment History** | Chronological list of every payment for a given user |
| ⚠️ **Overdue Marking** | Flag unpaid or partially paid accounts as overdue |
| 🔐 **Full Encapsulation** | Every field private — validated setters enforce all business rules |
| 🚨 **17 Custom Exceptions** | One named exception per failure scenario, never a generic crash |
| 🔄 **Automatic Status Updates** | Payment status refreshes automatically after every payment |
| 🧾 **Overpayment Detection** | Warns when payment exceeds 150% of the bill and shows credit |

---

## 🎓 OOP Concepts Applied

All five pillars of Object-Oriented Programming are present and deliberate in this project — not just named in a list but actually demonstrated through concrete implementation.

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

This pattern is applied to every property in `ElectricityUser`, `Customer`, `ResidentialUser`, `CommercialUser`, `IndustrialUser`, `PaymentRecord`, and `PaymentTracker`.

---

### 2 · Inheritance

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

### 3 · Polymorphism

**What it means:** The same method call produces different behaviour depending on the actual type of the object at runtime.

**How it is applied here:** `CalculateBill()` and `DisplayUserInfo()` are declared `virtual` in `ElectricityUser` and `override`d in all three subclasses. `UserManager` calls these methods without knowing — or caring — whether it is working with a residential, commercial, or industrial user. The correct version is selected automatically.

```csharp
// UserManager.cs — one line, three different behaviours depending on actual type
double energyCharge = selectedUser.CalculateBill();
```

If `selectedUser` is a `ResidentialUser`, the `ResidentialBillCalculator` runs at 5 BDT/kWh with 10% tax. If it is a `CommercialUser`, the `CommercialBillCalculator` runs at 8 BDT/kWh with 15% tax. The caller writes one line of code.

---

### 4 · Abstraction

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

### 5 · Interface

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

## 🏗️ Project Architecture

The project is organised in a strict dependency order. Each layer depends only on layers below it — nothing in the lower layers knows about the layers above.

```
┌─────────────────────────────────────────────────────────────────┐
│                          Program.cs                             │
│                   Entry point · Main menu loop                  │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                        UserManager.cs                           │
│     Registers users · Calculates bills · Handles payments       │
└──────┬──────────────────────────┬───────────────────┬───────────┘
       │                          │                   │
┌──────▼──────────┐  ┌────────────▼──────────┐  ┌────▼─────────────┐
│ ElectricityUser │  │    BillCalculator      │  │  PaymentTracker  │
│   (base class)  │  │   (abstract base)      │  │  (per-user)      │
└──┬────┬────┬────┘  └──┬─────────┬──────┬───┘  └──────┬───────────┘
   │    │    │           │         │      │             │
Residential  │       Residential   │   Industrial  PaymentRecord
 User   Commercial  Calculator Commercial  Calculator  (immutable)
        User         IndustrialCalculator

── Foundation Layer ──────────────────────────────────────────────
  PaymentStatus (enum)    PaymentMethod (enum)
  ElectricityExceptions (17 custom exception classes)
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
│
│  ── Exception Framework ────────────────────────────────────────
├── ElectricityExceptions.cs        # 17 named exception classes in one file
│
│  ── Validation Layer ───────────────────────────────────────────
├── ICustomerValidator.cs           # Interface — defines the validation contract
├── CustomerValidator.cs            # Implements ICustomerValidator
├── Customer.cs                     # Validated customer data model
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

**Total: 18 source files**

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

Energy Charge    :  1,500 × 5.00        =  7,500.00 BDT
Tax (10%)        :  7,500 × 0.10        =    750.00 BDT
Service Charge   :                         120.00 BDT
VAT (5%)         :  (7,500+750+120)×0.05 =  418.50 BDT
─────────────────────────────────────────────────────
TOTAL BILL       :                       8,788.50 BDT
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
| `InvalidMenuChoiceException` | Input is outside the 1–7 menu range |
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

### Exception Design Pattern

Every custom exception follows the same structure — it extends `Exception`, carries a specific descriptive message, and exposes the bad value as a readable property so catch blocks can work with it programmatically.

```csharp
public class CurrentReadingBelowPreviousException : Exception
{
    public double PreviousReading { get; }
    public double CurrentReading  { get; }

    public CurrentReadingBelowPreviousException(double previousReading, double currentReading)
        : base($"Current reading ({currentReading} kWh) cannot be less than previous reading " +
               $"({previousReading} kWh). Meter readings must increase over time.")
    {
        PreviousReading = previousReading;
        CurrentReading  = currentReading;
    }
}
```

### How Exceptions Are Displayed to the User

Every catch block in `UserManager` and `Program` prefixes the message with a label so the user knows the category of the error instantly:

```
  [Input Error]       — something typed was invalid
  [Not Found]         — the requested user does not exist
  [Registration Error]— a duplicate or null user was submitted
  [Billing Error]     — something went wrong during bill/payment calculation
  [Payment Error]     — a payment-specific rule was violated
  [Unexpected Error]  — a truly unknown error occurred (with details)
  [Setup Error]       — a pre-loaded sample user failed to initialise
  [Critical Error]    — a fatal application-level failure
```

---

## 🔒 Encapsulation Deep Dive

Encapsulation in this project is not a decoration — it is a complete design decision applied at five distinct levels.

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
public string   Remarks         { get { return _remarks; } }
public DateTime PaymentDate     { get; }   // init-only, set in constructor
public PaymentMethod Method     { get; }   // init-only, set in constructor
```

### Level 4 — Read-Only Payment History

The list of `PaymentRecord` objects inside `PaymentTracker` is `private readonly`. External code receives it as a `ReadOnlyCollection` — it can iterate and read but cannot add, remove, or replace entries.

```csharp
private readonly List<PaymentRecord> _paymentHistory;

public ReadOnlyCollection<PaymentRecord> PaymentHistory
{
    get { return _paymentHistory.AsReadOnly(); }
}
```

### Level 5 — Private Internals in UserManager

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

Three users are registered automatically at startup so you can test all features immediately:

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
  7. Exit
```

### Recommended Workflow

```
Option 3  →  Calculate Bill          Generate the bill (must do this first)
Option 4  →  Record Payment          Pay part or all of the bill
Option 5  →  View Payment Summary    Check outstanding balance and status
Option 6  →  View Payment History    See every individual payment recorded
```

You can also use **Option 1** to see all users in a columnar layout, and **Option 2** to look up any specific user's full profile at any time.

---

## 🖥️ Sample Output

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

### Payment Summary (Option 5)

```
========================================
         PAYMENT SUMMARY
========================================
  Total Bill     : 8001.00 BDT
  Total Paid     : 5000.00 BDT
  Outstanding    : 3001.00 BDT
  Payment Status : PARTIALLY PAID
  No. of Payments: 1
========================================
```

### Payment History (Option 6)

```
========================================
         PAYMENT HISTORY
========================================
  --- Payment #1 ---
  Reference No.  : TXN987654
  Amount Paid    : 5000.00 BDT
  Payment Method : MobileBanking
  Payment Date   : 13 May 2026  09:45 AM
  Remarks        : First partial payment

----------------------------------------
  Total Paid : 5000.00 BDT
  Status     : PARTIALLY PAID
========================================
```

### User-Friendly Error Messages

```
  [Input Error]   Invalid User ID: ''. User ID cannot be empty or blank.

  [Not Found]     No user found with ID 'X999'. Please check the ID and try again.

  [Billing Error] Current reading (2000 kWh) cannot be less than previous
                  reading (3000 kWh). Meter readings must increase over time.

  [Payment Error] No bill has been generated for this user yet. Please
                  calculate the bill before recording a payment.
```

---

## 🧠 Design Decisions Explained

### Why Separate BillCalculator and ElectricityUser Hierarchies?

Keeping billing logic out of user classes follows the **Single Responsibility Principle**. `ResidentialUser` knows about the customer — their ID, address, readings. `ResidentialBillCalculator` knows about billing rates and tax. Neither class has any reason to reach into the other's domain. This makes each class independently testable, extendable, and replaceable.

### Why 17 Exception Classes?

A single `throw new Exception("something went wrong")` fails on three counts — it tells the developer nothing about what failed, it tells the user nothing actionable, and it is impossible to catch specifically. Named exceptions solve all three: each catch block is precise, each message is accurate, and adding a new failure mode means adding a new class — nothing existing is disturbed.

### Why ReadOnlyCollection for Payment History?

The payment history is a financial record. It should never be possible for code outside `PaymentTracker` to add a fake payment, remove a real one, or reorder the list. Exposing it as `ReadOnlyCollection<PaymentRecord>` enforces this at compile time — the wrapper is built into the .NET type system.

### Why Interface for Validation?

`ICustomerValidator` decouples `Customer` from its validator. In a university project this is elegant design. In a production system it would mean you could swap in a validator backed by a database, an API, or a regex library without touching the `Customer` class at all. The interface makes the dependency explicit and the substitution trivial.

### Why Private Category Filter Methods in UserManager?

`GetResidentialUsers()`, `GetCommercialUsers()`, and `GetIndustrialUsers()` are internal helpers. External code never needs to call them — only `UserManager`'s own public methods do. Keeping them `private` prevents external code from depending on implementation details that could change.

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
| 7 | All errors used `throw new Exception(...)` | No meaningful distinction between failure types | Replaced with 17 domain-specific custom exception classes |

---

## 🛠️ Tech Stack

| Technology | Version | Why Used |
|---|---|---|
| C# | 10+ | Primary language — strong OOP support, clean syntax |
| .NET | 6.0+ | Cross-platform runtime — runs on Windows, macOS, Linux |
| `System.Collections.ObjectModel` | Built-in | `ReadOnlyCollection<T>` for immutable payment history |
| `System.Collections.Generic` | Built-in | `Dictionary<K,V>` for fast user lookup, `List<T>` for payment records |
| `System.Linq` | Built-in | Category filtering in `UserManager` |

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

---

<div align="center">

**⚡ Electricity Bill Calculator**

*Clean Architecture · 5 OOP Pillars · 17 Custom Exceptions · Full Payment Lifecycle · Complete Encapsulation*

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
# Finance App

A comprehensive personal finance management application built with .NET 10 Web API backend and React frontend. Track your accounts, transactions, budgets, and get insights into your spending habits.

## Features

### Account Management
- Create and manage multiple accounts (checking, savings, e-wallet, cash, investment)
- View total balance across all accounts
- Color-coded accounts for easy identification
- Real-time balance updates

### Transaction Management
- Record income, expenses, and transfers
- Categorize transactions (Makanan, Transport, Bil Utilities, etc.)
- Transfer money between accounts with automatic balance updates
- Transaction history with filtering by date
- Detailed transaction notes

### Budget Tracking
- Set monthly budgets by category
- Track spending against budgets
- View budget utilization percentages
- Monitor remaining budget in real-time

### Dashboard & Reports
- Visual dashboard with key financial metrics
- Monthly income and expense summaries
- Expense breakdown by category
- Recent transaction history
- Net savings calculations

### User Authentication
- Secure JWT-based authentication
- User registration and login
- Protected API endpoints

## Tech Stack

### Backend (.NET 10 Web API)
- **Framework:** ASP.NET Core 10
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** JWT (JSON Web Tokens)
- **Architecture:** Clean Architecture with Services pattern
- **Features:**
  - RESTful API design
  - Database transactions for data consistency
  - Input validation
  - CORS enabled for frontend integration

### Frontend (React + Vite)
- **Framework:** React 18
- **Build Tool:** Vite
- **Styling:** Tailwind CSS
- **HTTP Client:** Axios
- **Routing:** React Router
- **State Management:** Context API
- **UI Components:** Custom components with responsive design

## Project Structure

```
FinanceApp/
├── FinanceApp.API/              # Backend API
│   ├── Controllers/             # API endpoints
│   ├── Services/                # Business logic
│   ├── Models/                  # Data models
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Data/                    # Database context
│   └── Migrations/              # EF Core migrations
│
└── finance-app-client/          # Frontend React app
    ├── src/
    │   ├── components/          # React components
    │   ├── pages/               # Page components
    │   ├── contexts/            # Context providers
    │   └── services/            # API service layer
    └── public/                  # Static assets
```

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18 or higher)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server Express

### Backend Setup

1. Navigate to the API project:
```bash
cd FinanceApp.API
```

2. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FinanceAppDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

3. Run database migrations:
```bash
dotnet ef database update
```

4. Start the API:
```bash
dotnet run
```

The API will be available at `https://localhost:7180` (or the port specified in `launchSettings.json`)

### Frontend Setup

1. Navigate to the client project:
```bash
cd finance-app-client
```

2. Install dependencies:
```bash
npm install
```

3. Update the API URL in `src/services/api.js` if needed:
```javascript
const API_URL = 'https://localhost:7180/api';
```

4. Start the development server:
```bash
npm run dev
```

The app will be available at `http://localhost:5173`

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Accounts
- `GET /api/accounts` - Get all accounts
- `GET /api/accounts/{id}` - Get account by ID
- `POST /api/accounts` - Create new account
- `PUT /api/accounts/{id}` - Update account
- `DELETE /api/accounts/{id}` - Delete account
- `GET /api/accounts/total-balance` - Get total balance

### Transactions
- `GET /api/transactions` - Get all transactions (with optional date filtering)
- `GET /api/transactions/{id}` - Get transaction by ID
- `POST /api/transactions` - Create transaction (income/expense/transfer)
- `PUT /api/transactions/{id}` - Update transaction
- `DELETE /api/transactions/{id}` - Delete transaction
- `GET /api/transactions/account/{accountId}` - Get transactions by account
- `GET /api/transactions/expenses-by-category` - Get expenses grouped by category

### Dashboard
- `GET /api/dashboard/summary` - Get dashboard summary with all metrics

## Transfer Money Between Accounts

To transfer money between accounts, create a transaction with type `"transfer"`:

```json
POST /api/transactions
{
  "accountId": 1,
  "toAccountId": 2,
  "description": "Transfer to savings",
  "category": "Transfer",
  "type": "transfer",
  "amount": 100.00,
  "transactionDate": "2025-11-26T10:30:00Z",
  "notes": "Monthly savings"
}
```

The system will:
- Validate both accounts exist
- Check sufficient balance
- Deduct from source account
- Add to destination account
- Create a single transaction record
- Use database transactions for atomicity

## Key Features Implementation

### Transfer Functionality
Located in `FinanceApp.API/Services/TransactionService.cs:130-147`, the transfer feature:
- Validates source and destination accounts
- Ensures sufficient balance
- Updates both account balances atomically
- Records the transfer with proper linking

### Authentication Flow
- JWT tokens with configurable expiration
- Password hashing with BCrypt
- Claims-based authorization
- User ID extraction from token claims

### Database Transactions
- Uses execution strategy for retry logic
- Implements transaction rollback on errors
- Ensures data consistency across operations

## Default Categories

### Expense Categories
- Makanan (Food)
- Transport
- Bil Utilities (Utility Bills)
- Entertainment
- Shopping
- Healthcare
- Education
- Others

### Account Types
- Checking
- Savings
- E-wallet
- Cash
- Investment

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.

## Author

Nasruddin Shafie

## Support

For issues and questions, please open an issue on GitHub.

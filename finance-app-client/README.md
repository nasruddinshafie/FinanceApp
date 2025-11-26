# Finance App - React Frontend

A modern React application for managing personal finances, built with Vite, React Router, and TailwindCSS.

## Features

- User Authentication (Login/Register)
- Dashboard with financial summary
- Account management
- Transaction tracking
- Responsive design with TailwindCSS

## Prerequisites

- Node.js 16+ and npm
- Finance App API running on `http://localhost:5000`

## Getting Started

### 1. Install Dependencies

```bash
npm install
```

### 2. Configure API URL

The API base URL is set in `src/services/api.js`. Default is `http://localhost:5000/api`.

Update it if your API runs on a different port.

### 3. Run Development Server

```bash
npm run dev
```

The app will be available at `http://localhost:5173`

### 4. Build for Production

```bash
npm run build
```

## Project Structure

```
src/
├── contexts/
│   └── AuthContext.jsx       # Authentication state management
├── pages/
│   ├── Login.jsx             # Login page
│   ├── Register.jsx          # Registration page
│   └── Dashboard.jsx         # Main dashboard
├── services/
│   └── api.js                # API service & endpoints
├── App.jsx                   # Main app with routing
└── main.jsx                  # App entry point
```

## Available Routes

- `/` - Redirects to dashboard (or login if not authenticated)
- `/login` - User login
- `/register` - User registration
- `/dashboard` - Main dashboard (protected)

## Technologies Used

- **React 18** - UI library
- **Vite** - Build tool
- **React Router v6** - Routing
- **Axios** - HTTP client
- **TailwindCSS** - Styling

## API Integration

The app connects to the Finance App API with the following endpoints:

- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `GET /api/dashboard/summary` - Dashboard data
- `GET /api/accounts` - Get all accounts
- `GET /api/transactions` - Get all transactions

JWT tokens are automatically included in requests after login.

## Development Notes

- Protected routes redirect to login if not authenticated
- Public routes (login/register) redirect to dashboard if already logged in
- Auth state persists in localStorage
- 401 responses automatically trigger logout

## License

MIT

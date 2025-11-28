import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { dashboardAPI, accountAPI } from '../services/api';
import Navigation from '../components/Navigation';
import AddAccountModal from '../components/AddAccountModal';

const Dashboard = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const response = await dashboardAPI.getSummary();
      setSummary(response.data);
      setError('');
    } catch (err) {
      setError('Failed to load dashboard data');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleAddAccount = () => {
    setIsModalOpen(true);
  };

  const handleSubmitAccount = async (formData) => {
    try {
      await accountAPI.create(formData);
      setIsModalOpen(false);
      fetchDashboardData(); // Refresh dashboard data
    } catch (err) {
      console.error('Failed to create account:', err);
      setError('Failed to create account. Please try again.');
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      <Navigation />

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6 lg:py-8">
        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-3 sm:px-4 py-2.5 sm:py-3 rounded text-sm sm:text-base mb-4 sm:mb-6">
            {error}
          </div>
        )}

        {/* Summary Cards */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6 mb-6 sm:mb-8">
          {/* Total Balance Card */}
          <div className="bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl sm:rounded-2xl shadow-lg p-4 sm:p-6 text-white transform hover:scale-105 transition-all duration-200 hover:shadow-xl">
            <div className="flex items-center justify-between mb-3 sm:mb-4">
              <div className="p-2 sm:p-3 bg-white/20 rounded-lg sm:rounded-xl backdrop-blur-sm">
                <svg className="w-5 h-5 sm:w-6 sm:h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 6l3 1m0 0l-3 9a5.002 5.002 0 006.001 0M6 7l3 9M6 7l6-2m6 2l3-1m-3 1l-3 9a5.002 5.002 0 006.001 0M18 7l3 9m-3-9l-6-2m0-2v2m0 16V5m0 16H9m3 0h3" />
                </svg>
              </div>
            </div>
            <h3 className="text-xs sm:text-sm font-semibold text-blue-100 uppercase tracking-wide">Total Balance</h3>
            <p className="text-2xl sm:text-3xl font-bold mt-1.5 sm:mt-2">
              RM {summary?.totalBalance?.toFixed(2) || '0.00'}
            </p>
          </div>

          {/* Monthly Income Card */}
          <div className="bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl sm:rounded-2xl shadow-lg p-4 sm:p-6 text-white transform hover:scale-105 transition-all duration-200 hover:shadow-xl">
            <div className="flex items-center justify-between mb-3 sm:mb-4">
              <div className="p-2 sm:p-3 bg-white/20 rounded-lg sm:rounded-xl backdrop-blur-sm">
                <svg className="w-5 h-5 sm:w-6 sm:h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 11l5-5m0 0l5 5m-5-5v12" />
                </svg>
              </div>
            </div>
            <h3 className="text-xs sm:text-sm font-semibold text-green-100 uppercase tracking-wide">Monthly Income</h3>
            <p className="text-2xl sm:text-3xl font-bold mt-1.5 sm:mt-2">
              RM {summary?.monthlyIncome?.toFixed(2) || '0.00'}
            </p>
          </div>

          {/* Monthly Expense Card */}
          <div className="bg-gradient-to-br from-red-500 to-rose-600 rounded-xl sm:rounded-2xl shadow-lg p-4 sm:p-6 text-white transform hover:scale-105 transition-all duration-200 hover:shadow-xl">
            <div className="flex items-center justify-between mb-3 sm:mb-4">
              <div className="p-2 sm:p-3 bg-white/20 rounded-lg sm:rounded-xl backdrop-blur-sm">
                <svg className="w-5 h-5 sm:w-6 sm:h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 13l-5 5m0 0l-5-5m5 5V6" />
                </svg>
              </div>
            </div>
            <h3 className="text-xs sm:text-sm font-semibold text-red-100 uppercase tracking-wide">Monthly Expense</h3>
            <p className="text-2xl sm:text-3xl font-bold mt-1.5 sm:mt-2">
              RM {summary?.monthlyExpense?.toFixed(2) || '0.00'}
            </p>
          </div>

          {/* Net Savings Card */}
          <div className="bg-gradient-to-br from-purple-500 to-indigo-600 rounded-xl sm:rounded-2xl shadow-lg p-4 sm:p-6 text-white transform hover:scale-105 transition-all duration-200 hover:shadow-xl">
            <div className="flex items-center justify-between mb-3 sm:mb-4">
              <div className="p-2 sm:p-3 bg-white/20 rounded-lg sm:rounded-xl backdrop-blur-sm">
                <svg className="w-5 h-5 sm:w-6 sm:h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 8h6m-5 0a3 3 0 110 6H9l3 3m-3-6h6m6 1a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
            </div>
            <h3 className="text-xs sm:text-sm font-semibold text-purple-100 uppercase tracking-wide">Net Savings</h3>
            <p className="text-2xl sm:text-3xl font-bold mt-1.5 sm:mt-2">
              RM {summary?.netSavings?.toFixed(2) || '0.00'}
            </p>
          </div>
        </div>

        {/* Accounts Section */}
        <div className="bg-white/90 backdrop-blur-sm rounded-xl sm:rounded-2xl shadow-lg mb-6 sm:mb-8 border border-gray-200/50">
          <div className="px-4 sm:px-6 py-4 sm:py-5 border-b border-gray-200 bg-gradient-to-r from-gray-50 to-white rounded-t-xl sm:rounded-t-2xl">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-2 sm:space-x-3">
                <div className="p-1.5 sm:p-2 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg">
                  <svg className="w-4 h-4 sm:w-5 sm:h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                  </svg>
                </div>
                <h2 className="text-lg sm:text-xl font-bold text-gray-900">My Accounts</h2>
              </div>
              <button
                onClick={handleAddAccount}
                className="flex items-center space-x-1 sm:space-x-2 px-3 sm:px-4 py-2 bg-gradient-to-r from-indigo-600 to-purple-600 text-white rounded-lg sm:rounded-xl hover:from-indigo-700 hover:to-purple-700 transition-all duration-200 shadow-md hover:shadow-lg font-semibold text-sm"
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                </svg>
                <span className="hidden sm:inline">Add Account</span>
              </button>
            </div>
          </div>
          <div className="p-4 sm:p-6">
            {summary?.accounts?.length > 0 ? (
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-5">
                {summary.accounts
                  .sort((a, b) => {
                    // Define green color
                    const greenColor = '#10b981';
                    const isAGreen = a.color?.toLowerCase() === greenColor.toLowerCase();
                    const isBGreen = b.color?.toLowerCase() === greenColor.toLowerCase();

                    // Green accounts come first
                    if (isAGreen && !isBGreen) return -1;
                    if (!isAGreen && isBGreen) return 1;

                    // Otherwise maintain original order
                    return 0;
                  })
                  .map((account) => (
                  <div
                    key={account.id}
                    className="relative border-2 border-gray-200 rounded-lg sm:rounded-xl p-4 sm:p-5 hover:shadow-xl hover:border-indigo-300 transition-all duration-200 transform hover:-translate-y-1 bg-gradient-to-br from-white to-gray-50"
                  >
                    <div className="flex items-center justify-between mb-2 sm:mb-3">
                      <h3 className="font-bold text-gray-900 text-base sm:text-lg">{account.name}</h3>
                      <span
                        className="w-4 h-4 sm:w-5 sm:h-5 rounded-full shadow-md border-2 border-white"
                        style={{ backgroundColor: account.color }}
                      ></span>
                    </div>
                    <p className="text-xs sm:text-sm text-gray-500 font-medium mb-2 sm:mb-3 uppercase tracking-wide">{account.type}</p>
                    <p className="text-xl sm:text-2xl font-bold bg-gradient-to-r from-indigo-600 to-purple-600 bg-clip-text text-transparent">
                      RM {account.balance.toFixed(2)}
                    </p>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-8 sm:py-12">
                <div className="inline-flex items-center justify-center w-12 h-12 sm:w-16 sm:h-16 bg-gray-100 rounded-full mb-3 sm:mb-4">
                  <svg className="w-6 h-6 sm:w-8 sm:h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                  </svg>
                </div>
                <p className="text-gray-500 font-medium text-sm sm:text-base">No accounts yet. Create your first account!</p>
              </div>
            )}
          </div>
        </div>

        {/* Recent Transactions */}
        <div className="bg-white/90 backdrop-blur-sm rounded-xl sm:rounded-2xl shadow-lg border border-gray-200/50">
          <div className="px-4 sm:px-6 py-4 sm:py-5 border-b border-gray-200 bg-gradient-to-r from-gray-50 to-white rounded-t-xl sm:rounded-t-2xl">
            <div className="flex items-center space-x-2 sm:space-x-3">
              <div className="p-1.5 sm:p-2 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg">
                <svg className="w-4 h-4 sm:w-5 sm:h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                </svg>
              </div>
              <h2 className="text-lg sm:text-xl font-bold text-gray-900">Recent Transactions</h2>
            </div>
          </div>
          <div className="p-4 sm:p-6">
            {summary?.recentTransactions?.length > 0 ? (
              <div className="space-y-2 sm:space-y-3">
                {summary.recentTransactions.map((transaction) => (
                  <div
                    key={transaction.id}
                    className="flex items-center justify-between p-3 sm:p-4 rounded-lg sm:rounded-xl border-2 border-gray-100 hover:border-indigo-200 hover:shadow-md transition-all duration-200 bg-gradient-to-r from-white to-gray-50"
                  >
                    <div className="flex items-center space-x-2 sm:space-x-4 flex-1 min-w-0">
                      <div className={`p-2 sm:p-3 rounded-lg sm:rounded-xl flex-shrink-0 ${
                        transaction.type === 'income'
                          ? 'bg-green-100'
                          : transaction.type === 'expense'
                          ? 'bg-red-100'
                          : 'bg-blue-100'
                      }`}>
                        {transaction.type === 'income' ? (
                          <svg className="w-4 h-4 sm:w-6 sm:h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 11l5-5m0 0l5 5m-5-5v12" />
                          </svg>
                        ) : transaction.type === 'expense' ? (
                          <svg className="w-4 h-4 sm:w-6 sm:h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 13l-5 5m0 0l-5-5m5 5V6" />
                          </svg>
                        ) : (
                          <svg className="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
                          </svg>
                        )}
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="font-bold text-gray-900 text-sm sm:text-base truncate">{transaction.description}</p>
                        <p className="text-xs sm:text-sm text-gray-500 font-medium flex flex-wrap items-center gap-1 sm:gap-2">
                          <span className="inline-flex items-center px-1.5 sm:px-2 py-0.5 rounded-md bg-gray-100 text-gray-700 text-xs">
                            {transaction.category}
                          </span>
                          <span className="hidden sm:inline">•</span>
                          <span className="text-xs">
                            {new Date(transaction.transactionDate).toLocaleDateString('en-US', {
                              month: 'short',
                              day: 'numeric',
                              year: 'numeric'
                            })}
                          </span>
                        </p>
                        <p className="text-xs sm:text-sm text-gray-500 font-medium flex flex-wrap items-center gap-1 sm:gap-2">
                           <span className="inline-flex items-center px-1.5 sm:px-2 py-0.5 rounded-md bg-gray-100 text-gray-700 text-xs">
                              From account : {transaction.accountName}
                          </span>
                          
                        </p>
                      </div>
                    </div>
                    <p
                      className={`text-base sm:text-xl font-bold flex-shrink-0 ml-2 ${
                        transaction.type === 'income'
                          ? 'text-green-600'
                          : transaction.type === 'expense'
                          ? 'text-red-600'
                          : 'text-blue-600'
                      }`}
                    >
                      {transaction.type === 'income' ? '+' : '-'}RM {transaction.amount.toFixed(2)}
                    </p>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-8 sm:py-12">
                <div className="inline-flex items-center justify-center w-12 h-12 sm:w-16 sm:h-16 bg-gray-100 rounded-full mb-3 sm:mb-4">
                  <svg className="w-6 h-6 sm:w-8 sm:h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                  </svg>
                </div>
                <p className="text-gray-500 font-medium text-sm sm:text-base">No transactions yet. Start tracking your finances!</p>
              </div>
            )}
          </div>
        </div>
      </main>

      {/* Add Account Modal */}
      <AddAccountModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleSubmitAccount}
      />
    </div>
  );
};

export default Dashboard;

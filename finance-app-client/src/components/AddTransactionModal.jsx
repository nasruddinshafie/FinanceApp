import { useState, useEffect } from 'react';
import { accountAPI } from '../services/api';

const AddTransactionModal = ({ isOpen, onClose, onSubmit, transaction = null }) => {
  const [formData, setFormData] = useState({
    description: '',
    type: 'expense',
    category: '',
    amount: '',
    transactionDate: new Date().toISOString().split('T')[0],
    accountId: '',
    toAccountId: '', // For transfers
  });

  const [accounts, setAccounts] = useState([]);
  const [errors, setErrors] = useState({});

  const transactionTypes = [
    { value: 'income', label: 'Income', color: 'green' },
    { value: 'expense', label: 'Expense', color: 'red' },
    { value: 'transfer', label: 'Transfer', color: 'blue' },
  ];

  const expenseCategories = [
    'Food & Dining',
    'Transportation',
    'Shopping',
    'Entertainment',
    'Bills & Utilities',
    'Healthcare',
    'Education',
    'Travel',
    'Personal Care',
    'Other',
  ];

  const incomeCategories = [
    'Salary',
    'Freelance',
    'Investment',
    'Gift',
    'Bonus',
    'Refund',
    'Other',
  ];

  const transferCategories = ['Account Transfer'];

  useEffect(() => {
    if (isOpen) {
      fetchAccounts();
    }
  }, [isOpen]);

  useEffect(() => {
    if (transaction) {
      setFormData({
        description: transaction.description || '',
        type: transaction.type || 'expense',
        category: transaction.category || '',
        amount: transaction.amount?.toString() || '',
        transactionDate: transaction.transactionDate
          ? new Date(transaction.transactionDate).toISOString().split('T')[0]
          : new Date().toISOString().split('T')[0],
        accountId: transaction.accountId || '',
        toAccountId: transaction.toAccountId || '',
      });
    } else {
      setFormData({
        description: '',
        type: 'expense',
        category: '',
        amount: '',
        transactionDate: new Date().toISOString().split('T')[0],
        accountId: '',
        toAccountId: '',
      });
    }
    setErrors({});
  }, [transaction, isOpen]);

  const fetchAccounts = async () => {
    try {
      const response = await accountAPI.getAll();
      setAccounts(response.data);
      if (response.data.length > 0 && !formData.accountId && !transaction) {
        setFormData(prev => ({ ...prev, accountId: response.data[0].id }));
      }
    } catch (err) {
      console.error('Failed to fetch accounts:', err);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const handleTypeChange = (type) => {
    setFormData(prev => ({
      ...prev,
      type,
      category: '' // Reset category when type changes
    }));
    if (errors.type) {
      setErrors(prev => ({ ...prev, type: '' }));
    }
  };

  const getCategoriesForType = () => {
    switch (formData.type) {
      case 'income':
        return incomeCategories;
      case 'expense':
        return expenseCategories;
      case 'transfer':
        return transferCategories;
      default:
        return [];
    }
  };

  const validate = () => {
    const newErrors = {};

    if (!formData.description.trim()) {
      newErrors.description = 'Description is required';
    }

    if (!formData.category) {
      newErrors.category = 'Category is required';
    }

    if (!formData.amount || isNaN(formData.amount) || parseFloat(formData.amount) <= 0) {
      newErrors.amount = 'Please enter a valid amount';
    }

    if (!formData.accountId) {
      newErrors.accountId = 'Please select an account';
    }

    // For transfers, validate toAccountId
    if (formData.type === 'transfer') {
      if (!formData.toAccountId) {
        newErrors.toAccountId = 'Please select destination account';
      } else if (formData.accountId === formData.toAccountId) {
        newErrors.toAccountId = 'Cannot transfer to the same account';
      }
    }

    if (!formData.transactionDate) {
      newErrors.transactionDate = 'Date is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (validate()) {
      const submitData = {
        description: formData.description.trim(),
        type: formData.type,
        category: formData.category,
        amount: parseFloat(formData.amount),
        transactionDate: formData.transactionDate,
        accountId: parseInt(formData.accountId),
      };

      // Add toAccountId for transfers
      if (formData.type === 'transfer' && formData.toAccountId) {
        submitData.toAccountId = parseInt(formData.toAccountId);
      }

      onSubmit(submitData);
    }
  };

  if (!isOpen) return null;

  const selectedType = transactionTypes.find(t => t.value === formData.type);

  return (
    <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-lg transform transition-all max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="px-6 py-5 border-b border-gray-200 bg-gradient-to-r from-indigo-500 to-purple-600 rounded-t-2xl sticky top-0 z-10">
          <div className="flex items-center justify-between">
            <h2 className="text-xl font-bold text-white">
              {transaction ? 'Edit Transaction' : 'Add New Transaction'}
            </h2>
            <button
              onClick={onClose}
              className="text-white hover:bg-white/20 rounded-lg p-2 transition-colors"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="px-6 py-6">
          <div className="space-y-5">
            {/* Transaction Type */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Transaction Type
              </label>
              <div className="grid grid-cols-3 gap-2">
                {transactionTypes.map((type) => (
                  <button
                    key={type.value}
                    type="button"
                    onClick={() => handleTypeChange(type.value)}
                    className={`py-3 px-4 rounded-xl font-semibold transition-all ${
                      formData.type === type.value
                        ? type.color === 'green'
                          ? 'bg-green-100 text-green-700 ring-2 ring-green-500'
                          : type.color === 'red'
                          ? 'bg-red-100 text-red-700 ring-2 ring-red-500'
                          : 'bg-blue-100 text-blue-700 ring-2 ring-blue-500'
                        : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                    }`}
                  >
                    {type.label}
                  </button>
                ))}
              </div>
            </div>

            {/* Description */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Description
              </label>
              <input
                type="text"
                name="description"
                value={formData.description}
                onChange={handleChange}
                placeholder="e.g., Grocery shopping, Salary, etc."
                className={`w-full px-4 py-3 rounded-xl border-2 ${
                  errors.description ? 'border-red-500' : 'border-gray-200'
                } focus:border-indigo-500 focus:outline-none transition-colors`}
              />
              {errors.description && (
                <p className="mt-1 text-sm text-red-500">{errors.description}</p>
              )}
            </div>

            {/* Category */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Category
              </label>
              <select
                name="category"
                value={formData.category}
                onChange={handleChange}
                className={`w-full px-4 py-3 rounded-xl border-2 ${
                  errors.category ? 'border-red-500' : 'border-gray-200'
                } focus:border-indigo-500 focus:outline-none transition-colors bg-white`}
              >
                <option value="">Select a category</option>
                {getCategoriesForType().map(category => (
                  <option key={category} value={category}>{category}</option>
                ))}
              </select>
              {errors.category && (
                <p className="mt-1 text-sm text-red-500">{errors.category}</p>
              )}
            </div>

            {/* Amount */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Amount
              </label>
              <div className="relative">
                <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 font-medium">
                  RM
                </span>
                <input
                  type="number"
                  name="amount"
                  value={formData.amount}
                  onChange={handleChange}
                  placeholder="0.00"
                  step="0.01"
                  className={`w-full pl-14 pr-4 py-3 rounded-xl border-2 ${
                    errors.amount ? 'border-red-500' : 'border-gray-200'
                  } focus:border-indigo-500 focus:outline-none transition-colors`}
                />
              </div>
              {errors.amount && (
                <p className="mt-1 text-sm text-red-500">{errors.amount}</p>
              )}
            </div>

            {/* Account (From Account for transfers) */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                {formData.type === 'transfer' ? 'From Account' : 'Account'}
              </label>
              <select
                name="accountId"
                value={formData.accountId}
                onChange={handleChange}
                className={`w-full px-4 py-3 rounded-xl border-2 ${
                  errors.accountId ? 'border-red-500' : 'border-gray-200'
                } focus:border-indigo-500 focus:outline-none transition-colors bg-white`}
              >
                <option value="">Select an account</option>
                {accounts.map(account => (
                  <option key={account.id} value={account.id}>
                    {account.name} (RM {account.balance.toFixed(2)})
                  </option>
                ))}
              </select>
              {errors.accountId && (
                <p className="mt-1 text-sm text-red-500">{errors.accountId}</p>
              )}
              {accounts.length === 0 && (
                <p className="mt-1 text-sm text-amber-600">
                  Please create an account first before adding transactions.
                </p>
              )}
            </div>

            {/* To Account (only for transfers) */}
            {formData.type === 'transfer' && (
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  To Account
                </label>
                <select
                  name="toAccountId"
                  value={formData.toAccountId}
                  onChange={handleChange}
                  className={`w-full px-4 py-3 rounded-xl border-2 ${
                    errors.toAccountId ? 'border-red-500' : 'border-gray-200'
                  } focus:border-indigo-500 focus:outline-none transition-colors bg-white`}
                >
                  <option value="">Select destination account</option>
                  {accounts.map(account => (
                    <option
                      key={account.id}
                      value={account.id}
                      disabled={formData.accountId === account.id.toString()}
                    >
                      {account.name} (RM {account.balance.toFixed(2)})
                    </option>
                  ))}
                </select>
                {errors.toAccountId && (
                  <p className="mt-1 text-sm text-red-500">{errors.toAccountId}</p>
                )}
              </div>
            )}

            {/* Date */}
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Date
              </label>
              <input
                type="date"
                name="transactionDate"
                value={formData.transactionDate}
                onChange={handleChange}
                className={`w-full px-4 py-3 rounded-xl border-2 ${
                  errors.transactionDate ? 'border-red-500' : 'border-gray-200'
                } focus:border-indigo-500 focus:outline-none transition-colors`}
              />
              {errors.transactionDate && (
                <p className="mt-1 text-sm text-red-500">{errors.transactionDate}</p>
              )}
            </div>
          </div>

          {/* Buttons */}
          <div className="flex space-x-3 mt-8">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 px-4 py-3 bg-gray-100 text-gray-700 rounded-xl font-semibold hover:bg-gray-200 transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={accounts.length === 0}
              className={`flex-1 px-4 py-3 rounded-xl font-semibold transition-all shadow-lg hover:shadow-xl ${
                accounts.length === 0
                  ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                  : 'bg-gradient-to-r from-indigo-600 to-purple-600 text-white hover:from-indigo-700 hover:to-purple-700'
              }`}
            >
              {transaction ? 'Update' : 'Add'} Transaction
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddTransactionModal;

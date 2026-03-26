export const ROLES = {
  ADMIN: 'Admin',
  HR: 'HR',
  EMPLOYEE: 'Employee',
};

export const STATUS_COLORS = {
  Available: 'bg-green-100 text-green-800',
  Assigned: 'bg-blue-100 text-blue-800',
  InRepair: 'bg-yellow-100 text-yellow-800',
  Retired: 'bg-red-100 text-red-800',
  Pending: 'bg-orange-100 text-orange-800',
  HRApproved: 'bg-teal-100 text-teal-800',
  AdminApproved: 'bg-indigo-100 text-indigo-800',
  Rejected: 'bg-red-100 text-red-800',
  Cancelled: 'bg-gray-100 text-gray-800',
};

export const CONDITIONS = ['Good', 'Fair', 'Poor', 'UnderRepair', 'Damaged'];
export const ASSET_STATUSES = ['Available', 'Assigned', 'InRepair', 'Retired'];
export const REQUEST_STATUSES = ['Pending', 'HRApproved', 'AdminApproved', 'Assigned', 'Rejected', 'Cancelled'];

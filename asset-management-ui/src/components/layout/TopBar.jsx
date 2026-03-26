import React from 'react';
import useAuthStore from '../../store/authStore';
import useNotificationStore from '../../store/notificationStore';
import { Bell, User, LogOut } from 'lucide-react';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '../ui/dropdown-menu';
import { Link } from 'react-router-dom';

export const TopBar = () => {
  const { user, logout } = useAuthStore();
  const { unreadCount, fetchNotifications } = useNotificationStore();

  React.useEffect(() => {
    if (user) {
      fetchNotifications();
    }
  }, [user]);

  return (
    <div className="h-16 bg-white border-b border-slate-200 flex items-center justify-end px-6 shadow-sm">
      <div className="flex items-center space-x-4">
        <Link to="/notifications" className="relative p-2 text-slate-400 hover:text-slate-500 transition-colors">
          <Bell className="w-6 h-6" />
          {unreadCount > 0 && (
            <span className="absolute top-1 right-1 inline-flex items-center justify-center w-4 h-4 text-xs font-bold leading-none text-white bg-red-600 rounded-full">
              {unreadCount}
            </span>
          )}
        </Link>
        <DropdownMenu>
          <DropdownMenuTrigger className="flex items-center space-x-2 focus:outline-none">
            <div className="w-8 h-8 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-700 font-bold">
              {user?.fullName?.charAt(0) || 'U'}
            </div>
            <span className="text-sm font-medium text-slate-700">{user?.fullName} ({user?.role})</span>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-48">
            <DropdownMenuItem onClick={logout} className="text-red-600 cursor-pointer">
              <LogOut className="w-4 h-4 mr-2" />
              Sign out
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  );
};

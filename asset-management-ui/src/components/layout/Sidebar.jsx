import React, { useEffect, useState } from 'react';
import { NavLink } from 'react-router-dom';
import useAuthStore from '../../store/authStore';
import { LayoutDashboard, Package, GitPullRequest, Settings, Users, FileBarChart, Bell } from 'lucide-react';

const navigation = [
  { name: 'Dashboard', to: '/', icon: LayoutDashboard, roles: ['Admin'] },
  { name: 'Assets', to: '/assets', icon: Package, roles: ['Admin', 'HR'] },
  { name: 'My Assets', to: '/my-assets', icon: Package, roles: ['Employee'] },
  { name: 'Requests', to: '/requests', icon: GitPullRequest, roles: ['Admin', 'HR', 'Employee'] },
  { name: 'System Config', to: '/config/categories', icon: Settings, roles: ['Admin'] },
  { name: 'Stock Report', to: '/reports/stock', icon: FileBarChart, roles: ['Admin', 'HR'] },
];

export const Sidebar = () => {
  const { user } = useAuthStore();
  const [items, setItems] = useState([]);

  useEffect(() => {
    if (user) {
      setItems(navigation.filter((item) => item.roles.includes(user.role)));
    }
  }, [user]);

  return (
    <div className="w-64 bg-slate-900 h-screen text-slate-300 flex flex-col">
      <div className="h-16 flex items-center px-6 font-bold text-white text-xl tracking-wide border-b border-slate-800">
        Asset Management
      </div>
      <nav className="flex-1 py-4 space-y-1">
        {items.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.name}
              to={item.to}
              className={({ isActive }) =>
                `flex items-center px-6 py-3 text-sm font-medium transition-colors ${isActive ? 'bg-indigo-600 text-white' : 'hover:bg-slate-800 hover:text-white'
                }`
              }
            >
              <Icon className="w-5 h-5 mr-3" />
              {item.name}
            </NavLink>
          );
        })}
      </nav>
      <div className="p-4 border-t border-slate-800 text-xs text-slate-500">
        v1.0.0 &copy; 2026
      </div>
    </div>
  );
};

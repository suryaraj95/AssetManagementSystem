import React from 'react';
import { NavLink, Outlet } from 'react-router-dom';

export default function ConfigLayout() {
  const tabs = [
    { name: 'Categories', path: '/config/categories' },
    { name: 'Asset Types', path: '/config/asset-types' },
    { name: 'Branches', path: '/config/branches' },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">System Configuration</h1>
        <p className="text-muted-foreground">Manage organization settings and classifications.</p>
      </div>
      
      <div className="border-b border-border">
        <nav className="-mb-px flex space-x-8" aria-label="Tabs">
          {tabs.map((tab) => (
            <NavLink
              key={tab.name}
              to={tab.path}
              className={({ isActive }) =>
                `whitespace-nowrap border-b-2 py-4 px-1 text-sm font-medium ${
                  isActive
                    ? 'border-indigo-500 text-indigo-600'
                    : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'
                }`
              }
            >
              {tab.name}
            </NavLink>
          ))}
        </nav>
      </div>

      <div className="pt-4">
        <Outlet />
      </div>
    </div>
  );
}

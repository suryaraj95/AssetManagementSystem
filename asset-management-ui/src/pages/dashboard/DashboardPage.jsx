import React, { useMemo } from 'react';
import useAuthStore from '../../store/authStore';
import { useDashboardSummary } from '../../hooks/useDashboard';
import { useAssets } from '../../hooks/useAssets';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../../components/ui/card';
import { PieChart, Pie, Cell, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, AreaChart, Area } from 'recharts';

export default function DashboardPage() {
  const { user } = useAuthStore();
  const { data: summary } = useDashboardSummary();
  const { data: assetsData, isLoading } = useAssets({ page: 1, size: 5000 });
  const assets = assetsData?.items || [];

  const { statusData, categoryData, conditionData } = useMemo(() => {
    const statusCounts = { Available: 0, Assigned: 0, Maintenance: 0, Retired: 0 };
    const condCounts = { Good: 0, Fair: 0, Poor: 0, Broken: 0 };
    const catCounts = {};

    assets.forEach(a => {
      // Status
      if (statusCounts[a.status] !== undefined) statusCounts[a.status]++;
      // Condition
      if (condCounts[a.condition] !== undefined) condCounts[a.condition]++;
      // Category
      const cat = a.categoryName || 'Uncategorized';
      catCounts[cat] = (catCounts[cat] || 0) + 1;
    });

    return {
      statusData: Object.entries(statusCounts).map(([name, value]) => ({ name, value })),
      conditionData: Object.entries(condCounts).map(([name, value]) => ({ name, value })),
      categoryData: Object.entries(catCounts).map(([name, value]) => ({ name, value })).sort((a, b) => b.value - a.value).slice(0, 10) // Top 10 categories
    };
  }, [assets]);

  const STATUS_COLORS = ['#10b981', '#6366f1', '#f59e0b', '#94a3b8']; // Available, Assigned, Maintenance, Retired
  const COND_COLORS = ['#10b981', '#3b82f6', '#f59e0b', '#ef4444']; // Good, Fair, Poor, Broken

  if (isLoading) return <div className="p-8 text-center text-muted-foreground animate-pulse">Computing system visualizations...</div>;

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Assets Dashboard</h1>
        <p className="text-muted-foreground">Welcome back, {user?.fullName}. Here are your real-time ecosystem telemetry graphs.</p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        {/* Status Distribution */}
        <Card className="shadow-sm border-slate-200">
          <CardHeader>
            <CardTitle>Asset Allocation Status</CardTitle>
            <CardDescription>Ratio of equipment actively deployed vs available</CardDescription>
          </CardHeader>
          <CardContent className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie data={statusData} cx="50%" cy="50%" innerRadius={80} outerRadius={110} paddingAngle={5} dataKey="value" stroke="none">
                  {statusData.map((entry, index) => <Cell key={`cell-${index}`} fill={STATUS_COLORS[index % STATUS_COLORS.length]} />)}
                </Pie>
                <Tooltip wrapperClassName="rounded-md shadow-lg border-0" contentStyle={{ borderRadius: '8px' }} />
                <Legend verticalAlign="bottom" height={36} iconType="circle" />
              </PieChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>

        {/* Condition Distribution */}
        <Card className="shadow-sm border-slate-200">
          <CardHeader>
            <CardTitle>Hardware Health Status</CardTitle>
            <CardDescription>General lifecycle condition breakdown</CardDescription>
          </CardHeader>
          <CardContent className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie data={conditionData} cx="50%" cy="50%" innerRadius={0} outerRadius={110} dataKey="value" stroke="white" strokeWidth={2}>
                  {conditionData.map((entry, index) => <Cell key={`cell-${index}`} fill={COND_COLORS[index % COND_COLORS.length]} />)}
                </Pie>
                <Tooltip wrapperClassName="rounded-md shadow-lg border-0" contentStyle={{ borderRadius: '8px' }} />
                <Legend verticalAlign="bottom" height={36} iconType="diamond" />
              </PieChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>

      {/* Category Volumes */}
      <Card className="shadow-sm border-slate-200">
        <CardHeader>
          <CardTitle>Inventory by Category</CardTitle>
          <CardDescription>Top deployed asset categories globally</CardDescription>
        </CardHeader>
        <CardContent className="h-[400px]">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={categoryData} margin={{ top: 20, right: 30, left: 0, bottom: 5 }}>
              <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#E2E8F0" />
              <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{ fill: '#64748b' }} />
              <YAxis axisLine={false} tickLine={false} tick={{ fill: '#64748b' }} />
              <Tooltip cursor={{ fill: '#f1f5f9' }} wrapperClassName="rounded-md shadow-md" />
              <Bar dataKey="value" fill="#818cf8" radius={[4, 4, 0, 0]} name="Total Units" barSize={45}>
                {categoryData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={`hsl(226, 70%, ${50 + (index * 4)}%)`} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </CardContent>
      </Card>

      {user?.role !== 'Employee' && summary?.recentActivity && (
        <Card className="shadow-sm">
          <CardHeader>
            <CardTitle>Activity Ledger</CardTitle>
            <CardDescription>Chronological sequence of database modifications</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {summary.recentActivity.map((activity, index) => (
                <div key={index} className="flex items-center">
                  <span className="w-2 h-2 rounded-full bg-blue-500 mr-3"></span>
                  <div className="text-sm flex-1">
                    <p className="font-medium leading-none">{activity.action}</p>
                    <p className="text-xs text-muted-foreground mt-1">
                      {new Date(activity.createdAt).toLocaleString()} by {activity.performedByName || 'System'}
                    </p>
                  </div>
                </div>
              ))}
              {summary.recentActivity.length === 0 && (
                <p className="text-sm text-muted-foreground">No recent activity detected.</p>
              )}
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}

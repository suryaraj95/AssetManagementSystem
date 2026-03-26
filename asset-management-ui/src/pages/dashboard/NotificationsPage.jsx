import React from 'react';
import { useNotifications, useMarkNotificationRead, useMarkAllNotificationsRead } from '../../hooks/useDashboard';
import { Button } from '../../components/ui/button';
import { Card, CardContent } from '../../components/ui/card';

export default function NotificationsPage() {
  const { data: notifications = [], isLoading } = useNotifications();
  const markRead = useMarkNotificationRead();
  const markAllRead = useMarkAllNotificationsRead();

  if (isLoading) return <div className="p-8 text-center text-muted-foreground">Loading Notifications...</div>;

  const unreadCount = notifications.filter(n => !n.isRead).length;

  return (
    <div className="space-y-6 max-w-4xl max-w-2xl mx-auto">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold tracking-tight">Notifications</h1>
        {unreadCount > 0 && (
          <Button variant="outline" size="sm" onClick={() => markAllRead.mutate()}>
            Mark All as Read
          </Button>
        )}
      </div>

      {notifications.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground border rounded-md border-dashed">
          You have no notifications.
        </div>
      ) : (
        <div className="space-y-4">
          {notifications.map((notif) => (
            <Card key={notif.id} className={notif.isRead ? 'opacity-75 bg-slate-50' : 'border-l-4 border-indigo-500 shadow-sm'}>
              <CardContent className="p-4 flex items-center justify-between">
                <div>
                  <h3 className="font-medium text-sm text-foreground">{notif.title}</h3>
                  <p className="text-sm text-muted-foreground mt-1">{notif.message}</p>
                  <p className="text-xs text-slate-400 mt-2">{new Date(notif.createdAt).toLocaleString()}</p>
                </div>
                {!notif.isRead && (
                  <Button variant="ghost" size="sm" onClick={() => markRead.mutate(notif.id)}>
                    Dismiss
                  </Button>
                )}
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}

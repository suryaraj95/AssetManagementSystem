import React, { useState } from 'react';
import useAuthStore from '../../store/authStore';
import { useRequests, useCreateRequest, useAdminApproveRequest, useRejectRequest } from '../../hooks/useRequests';
import { useAssetTypes } from '../../hooks/useConfig';
import { useAssets } from '../../hooks/useAssets';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Badge } from '../../components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '../../components/ui/dialog';
import { Label } from '../../components/ui/label';
import { toast } from 'sonner';

export default function RequestListPage() {
  const { user } = useAuthStore();
  const [filterStatus, setFilterStatus] = useState('');
  
  const { data: requests = [], isLoading } = useRequests({ status: filterStatus });
  
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [adminAssignReq, setAdminAssignReq] = useState(null);
  const [rejectReq, setRejectReq] = useState(null);

  const statusColors = {
    'Pending': 'secondary',
    'Assigned': 'success',
    'Rejected': 'destructive'
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold tracking-tight">Asset Requests</h1>
        {user?.role === 'Employee' && (
          <Button onClick={() => setIsCreateOpen(true)}>Create Request</Button>
        )}
      </div>

      <div className="flex items-center space-x-4">
        <select 
          className="h-10 rounded-md border border-input bg-transparent px-3 py-2 text-sm"
          value={filterStatus}
          onChange={(e) => setFilterStatus(e.target.value)}
        >
          <option value="">All Statuses</option>
          <option value="Pending">Pending</option>
          <option value="Assigned">Assigned</option>
          <option value="Rejected">Rejected</option>
        </select>
      </div>

      <div className="border rounded-md bg-white">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Request No.</TableHead>
              <TableHead>Requested By</TableHead>
              <TableHead>Type/Item</TableHead>
              <TableHead>Date</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={6} className="text-center">Loading...</TableCell></TableRow>
            ) : requests.length === 0 ? (
              <TableRow><TableCell colSpan={6} className="text-center">No requests found</TableCell></TableRow>
            ) : (
              requests.map((req) => (
                <TableRow key={req.id}>
                  <TableCell className="font-mono font-medium">{req.requestNumber}</TableCell>
                  <TableCell>{req.requestedByName}</TableCell>
                  <TableCell>
                    <div className="font-medium">{req.assetTypeName || 'N/A'}</div>
                    <div className="text-xs text-muted-foreground">{req.requestType}</div>
                  </TableCell>
                  <TableCell>{new Date(req.createdAt).toLocaleDateString()}</TableCell>
                  <TableCell>
                    <Badge variant={statusColors[req.status] || 'default'}>{req.status}</Badge>
                  </TableCell>
                  <TableCell className="text-right space-x-2">
                    {req.status === 'Pending' && user?.role === 'Admin' && (
                      <Button variant="default" size="sm" onClick={() => setAdminAssignReq(req)}>Assign Asset</Button>
                    )}

                    {req.status === 'Pending' && (user?.role === 'HR' || user?.role === 'Admin') && (
                      <Button variant="destructive" size="sm" onClick={() => setRejectReq(req)}>Reject</Button>
                    )}
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>

      {isCreateOpen && <CreateRequestModal isOpen={isCreateOpen} onClose={() => setIsCreateOpen(false)} />}
      
      {adminAssignReq && (
        <AdminAssignModal 
          request={adminAssignReq} 
          isOpen={!!adminAssignReq} 
          onClose={() => setAdminAssignReq(null)} 
        />
      )}

      {rejectReq && (
        <RejectModal 
          request={rejectReq} 
          isOpen={!!rejectReq} 
          onClose={() => setRejectReq(null)} 
        />
      )}
    </div>
  );
}

// -------------------------------------------------------------
// Sub-components
// -------------------------------------------------------------

function CreateRequestModal({ isOpen, onClose }) {
  const { data: types = [] } = useAssetTypes();
  const createMutation = useCreateRequest();

  const [formData, setFormData] = useState({
    requestType: 'New',
    assetTypeId: '',
    reason: ''
  });

  const handleSubmit = (e) => {
    e.preventDefault();
    createMutation.mutate(formData, {
      onSuccess: () => {
        toast.success('Request submitted successfully');
        onClose();
      }
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>File New Request</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label>Type of Request</Label>
            <select className="flex h-10 w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm"
              value={formData.requestType} onChange={(e) => setFormData({...formData, requestType: e.target.value})}>
              <option value="New">New Asset</option>
              <option value="Replacement">Replacement</option>
            </select>
          </div>
          
          <div className="space-y-2">
            <Label>Asset Category / Type</Label>
            <select className="flex h-10 w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm"
              value={formData.assetTypeId} onChange={(e) => setFormData({...formData, assetTypeId: e.target.value})} required>
              <option value="" disabled>Select Type</option>
              {types.map(t => <option key={t.id} value={t.id}>{t.categoryName} - {t.name}</option>)}
            </select>
          </div>
          
          <div className="space-y-2">
            <Label>Business Justification / Reason</Label>
            <Input value={formData.reason} onChange={(e) => setFormData({...formData, reason: e.target.value})} required />
          </div>

          <DialogFooter className="mt-6">
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button type="submit" disabled={createMutation.isPending}>Submit</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

function AdminAssignModal({ request, isOpen, onClose }) {
  const { data: assetsData } = useAssets({ page: 1, size: 50, status: 'Available' });
  // Filter assets that match the requested asset type
  const availableAssets = (assetsData?.items || []).filter(a => a.assetTypeId === request.assetTypeId);
  
  const adminApprove = useAdminApproveRequest();
  const [assignedAssetId, setAssignedAssetId] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!assignedAssetId) {
      toast.error('Please select an asset to assign');
      return;
    }
    adminApprove.mutate({ id: request.id, assignedAssetId }, {
      onSuccess: () => {
        toast.success('Asset assigned and request approved');
        onClose();
      }
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Assign Asset</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <p className="text-sm text-muted-foreground">
            Select an available <strong>{request.assetTypeName}</strong> to fulfill this request.
          </p>

          <div className="space-y-2">
            <Label>Available Assets</Label>
            <select className="flex h-10 w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm"
              value={assignedAssetId} onChange={(e) => setAssignedAssetId(e.target.value)} required>
              <option value="" disabled>Select Asset</option>
              {availableAssets.map(a => (
                <option key={a.id} value={a.id}>{a.assetId} - {a.brandName} ({a.serialNumber})</option>
              ))}
            </select>
            {availableAssets.length === 0 && (
              <p className="text-xs text-red-500 mt-1">No available items found for this asset type.</p>
            )}
          </div>

          <DialogFooter className="mt-6">
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button type="submit" disabled={adminApprove.isPending || availableAssets.length === 0}>Complete Request</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

function RejectModal({ request, isOpen, onClose }) {
  const rejectMutation = useRejectRequest();
  const [rejectionReason, setRejectionReason] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    rejectMutation.mutate({ id: request.id, rejectionReason }, {
      onSuccess: () => {
        toast.success('Request rejected');
        onClose();
      }
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Reject Request</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label>Reason for Rejection</Label>
            <Input value={rejectionReason} onChange={(e) => setRejectionReason(e.target.value)} required />
          </div>
          <DialogFooter className="mt-6">
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button type="submit" variant="destructive" disabled={rejectMutation.isPending}>Reject Request</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

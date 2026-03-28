import React, { useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useAuthStore from '../../store/authStore';
import { useAsset, useAssignAsset, useUnassignAsset, useSaveDispatch, useDownloadDispatchReceipt } from '../../hooks/useAssets';
import { useEmployees } from '../../hooks/useEmployees';
import { Button } from '../../components/ui/button';
import { Badge } from '../../components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '../../components/ui/dialog';
import { Label } from '../../components/ui/label';
import { toast } from 'sonner';
import { Truck, Download, PaperclipIcon, CalendarDays } from 'lucide-react';

export default function AssetDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuthStore();
  const { data: asset, isLoading } = useAsset(id);

  const [isAssignOpen, setIsAssignOpen] = useState(false);
  const [isDispatchOpen, setIsDispatchOpen] = useState(false);
  const unassignMutation = useUnassignAsset();
  const downloadReceipt = useDownloadDispatchReceipt();

  if (isLoading) return <div className="p-8 text-center">Loading Asset...</div>;
  if (!asset) return <div className="p-8 text-center text-red-500">Asset not found.</div>;

  const canManage = user?.role === 'Admin' || user?.role === 'HR';

  const handleUnassign = () => {
    if (confirm('Are you sure you want to unassign this asset?')) {
      unassignMutation.mutate(asset.id, {
        onSuccess: () => toast.success('Asset unassigned successfully'),
        onError: (err) => toast.error(err.response?.data?.message || err.message),
      });
    }
  };

  const handleDownloadReceipt = () => {
    downloadReceipt.mutate({ id: asset.id }, {
      onError: () => toast.error('Failed to download receipt.'),
    });
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center space-x-4">
        <Button variant="outline" onClick={() => navigate('/assets')}>&larr; Back to Assets</Button>
        <h1 className="text-2xl font-bold tracking-tight">Asset Details: {asset.assetId}</h1>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Basic Info */}
        <div className="bg-white p-6 rounded-md border shadow-sm space-y-4">
          <h2 className="text-lg font-semibold border-b pb-2">Basic Info</h2>
          <div className="grid grid-cols-2 gap-y-4 text-sm">
            <div><span className="text-muted-foreground">Type:</span> <p className="font-medium">{asset.assetTypeName}</p></div>
            <div><span className="text-muted-foreground">Branch:</span> <p className="font-medium">{asset.branchName}</p></div>
            <div><span className="text-muted-foreground">Brand:</span> <p className="font-medium">{asset.brandName || 'N/A'}</p></div>
            <div><span className="text-muted-foreground">Serial No:</span> <p className="font-medium font-mono">{asset.serialNumber || 'N/A'}</p></div>
            <div><span className="text-muted-foreground">Status:</span> <div className="mt-1"><Badge variant={asset.status === 'Available' ? 'default' : 'secondary'}>{asset.status}</Badge></div></div>
            <div><span className="text-muted-foreground">Condition:</span> <p className="font-medium">{asset.condition}</p></div>
            <div><span className="text-muted-foreground">Warranty:</span> <p className="font-medium">{asset.warrantyExpiry ? new Date(asset.warrantyExpiry).toLocaleDateString() : 'Not Applicable'}</p></div>
          </div>
        </div>

        {/* Assignments */}
        <div className="bg-white p-6 rounded-md border shadow-sm space-y-4">
          <h2 className="text-lg font-semibold border-b pb-2">Assignments</h2>
          <div className="text-sm flex items-center justify-between">
            <div>
              <span className="text-muted-foreground">Currently Assigned To:</span>
              <p className="font-medium text-lg mt-1">{asset.assignedEmployeeName || 'Unassigned'}</p>
            </div>
            {canManage && (
              <div>
                {asset.status === 'Available' && (
                  <Button size="sm" onClick={() => setIsAssignOpen(true)}>Assign Asset</Button>
                )}
                {asset.status === 'Assigned' && (
                  <Button size="sm" variant="destructive" onClick={handleUnassign} disabled={unassignMutation.isPending}>Unassign</Button>
                )}
              </div>
            )}
          </div>
        </div>

        {/* Specifications */}
        <div className="bg-white p-6 rounded-md border shadow-sm space-y-4 md:col-span-2">
          <h2 className="text-lg font-semibold border-b pb-2">Specifications</h2>
          {asset.specs && asset.specs.length > 0 ? (
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
              {asset.specs.map(spec => (
                <div key={spec.specDefinitionId}>
                  <span className="text-muted-foreground block">{spec.specName}</span>
                  <span className="font-medium">{spec.value || 'N/A'}</span>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">No specifications mapped to this asset.</p>
          )}
        </div>

        {/* Dispatch Details */}
        <div className="bg-white p-6 rounded-md border shadow-sm space-y-4 md:col-span-2">
          <div className="flex items-center justify-between border-b pb-2">
            <h2 className="text-lg font-semibold flex items-center gap-2">
              <Truck className="w-5 h-5 text-slate-500" /> Dispatch Details
            </h2>
            {canManage && asset.status === 'Assigned' && (
              <Button size="sm" variant="outline" onClick={() => setIsDispatchOpen(true)}>
                {asset.dispatchDate ? 'Update Dispatch Details' : 'Add Dispatch Details'}
              </Button>
            )}
            {canManage && asset.status !== 'Assigned' && (
              <Button size="sm" variant="outline" disabled title="Only available for Assigned assets">
                Add Dispatch Details
              </Button>
            )}
          </div>

          {asset.status !== 'Assigned' && !asset.dispatchDate ? (
            <p className="text-sm text-muted-foreground">Dispatch details can only be added for Assigned assets.</p>
          ) : asset.dispatchDate ? (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
              <div className="flex items-start gap-2">
                <CalendarDays className="w-4 h-4 text-slate-400 mt-0.5" />
                <div>
                  <span className="text-muted-foreground block">Dispatch Date</span>
                  <span className="font-medium">{new Date(asset.dispatchDate).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })}</span>
                </div>
              </div>
              <div className="flex items-start gap-2">
                <PaperclipIcon className="w-4 h-4 text-slate-400 mt-0.5" />
                <div>
                  <span className="text-muted-foreground block">Receipt</span>
                  {asset.hasDispatchReceipt ? (
                    <Button
                      size="sm"
                      variant="outline"
                      className="mt-1 h-7 text-xs"
                      onClick={handleDownloadReceipt}
                      disabled={downloadReceipt.isPending}
                    >
                      <Download className="w-3 h-3 mr-1" />
                      {downloadReceipt.isPending ? 'Downloading...' : 'Download Receipt'}
                    </Button>
                  ) : (
                    <span className="text-muted-foreground">No receipt uploaded</span>
                  )}
                </div>
              </div>
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">No dispatch details recorded yet.</p>
          )}
        </div>
      </div>

      <AssignAssetModal isOpen={isAssignOpen} onClose={() => setIsAssignOpen(false)} assetId={asset.id} />
      <DispatchModal isOpen={isDispatchOpen} onClose={() => setIsDispatchOpen(false)} assetId={asset.id} />
    </div>
  );
}

/* ─── Assign Modal (unchanged) ─── */
function AssignAssetModal({ isOpen, onClose, assetId }) {
  const { data: employeesData } = useEmployees({ size: 100 });
  const employees = Array.isArray(employeesData) ? employeesData : (employeesData?.items || []);
  const assignMutation = useAssignAsset();
  const [selectedEmployee, setSelectedEmployee] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!selectedEmployee) return;
    assignMutation.mutate({ id: assetId, employeeId: selectedEmployee }, {
      onSuccess: () => {
        toast.success('Asset assigned successfully!');
        onClose();
        setSelectedEmployee('');
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
          <div className="space-y-2">
            <Label>Select Employee</Label>
            <select
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
              value={selectedEmployee}
              onChange={(e) => setSelectedEmployee(e.target.value)}
              required
            >
              <option value="" disabled>Select an employee</option>
              {employees.filter(emp => emp.role === 'Employee').map(emp => (
                <option key={emp.id} value={emp.id}>{emp.fullName} ({emp.employeeId})</option>
              ))}
            </select>
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button type="submit" disabled={assignMutation.isPending || !selectedEmployee}>Assign</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

/* ─── Dispatch Modal ─── */
function DispatchModal({ isOpen, onClose, assetId }) {
  const saveDispatch = useSaveDispatch();
  const [dispatchDate, setDispatchDate] = useState('');
  const [receiptFile, setReceiptFile] = useState(null);
  const [fileError, setFileError] = useState('');
  const fileInputRef = useRef(null);

  const MAX_MB = 1;
  const ALLOWED_TYPES = ['image/jpeg', 'image/png', 'application/pdf'];

  const handleFileChange = (e) => {
    const file = e.target.files?.[0];
    setFileError('');
    if (!file) { setReceiptFile(null); return; }

    if (!ALLOWED_TYPES.includes(file.type)) {
      setFileError('Only JPEG, PNG, and PDF files are accepted.');
      e.target.value = '';
      setReceiptFile(null);
      return;
    }
    if (file.size > MAX_MB * 1024 * 1024) {
      setFileError(`File must be ${MAX_MB} MB or smaller.`);
      e.target.value = '';
      setReceiptFile(null);
      return;
    }
    setReceiptFile(file);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!dispatchDate) return;

    saveDispatch.mutate({ id: assetId, dispatchDate, receiptFile }, {
      onSuccess: () => {
        toast.success('Dispatch details saved successfully!');
        onClose();
        setDispatchDate('');
        setReceiptFile(null);
        if (fileInputRef.current) fileInputRef.current.value = '';
      },
      onError: (err) => {
        toast.error(err.response?.data || err.message || 'Failed to save dispatch details.');
      },
    });
  };

  const handleClose = () => {
    setDispatchDate('');
    setReceiptFile(null);
    setFileError('');
    if (fileInputRef.current) fileInputRef.current.value = '';
    onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Truck className="w-5 h-5" /> Dispatch Details
          </DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-5">
          <div className="space-y-2">
            <Label htmlFor="dispatch-date">
              Dispatch Date <span className="text-red-500">*</span>
            </Label>
            <input
              id="dispatch-date"
              type="date"
              required
              max={new Date().toISOString().slice(0, 10)}
              value={dispatchDate}
              onChange={(e) => setDispatchDate(e.target.value)}
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="dispatch-receipt">
              Receipt Attachment <span className="text-muted-foreground text-xs">(optional — JPEG, PNG, PDF · max 1 MB)</span>
            </Label>
            <input
              id="dispatch-receipt"
              ref={fileInputRef}
              type="file"
              accept=".jpg,.jpeg,.png,.pdf"
              onChange={handleFileChange}
              className="flex w-full rounded-md border border-input bg-transparent px-3 py-1.5 text-sm shadow-sm file:border-0 file:bg-transparent file:text-sm file:font-medium cursor-pointer"
            />
            {fileError && <p className="text-xs text-red-500">{fileError}</p>}
            {receiptFile && !fileError && (
              <p className="text-xs text-emerald-600 flex items-center gap-1">
                <PaperclipIcon className="w-3 h-3" /> {receiptFile.name} ({(receiptFile.size / 1024).toFixed(1)} KB)
              </p>
            )}
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={handleClose}>Cancel</Button>
            <Button type="submit" disabled={saveDispatch.isPending || !dispatchDate || !!fileError}>
              {saveDispatch.isPending ? 'Saving...' : 'Save Dispatch'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

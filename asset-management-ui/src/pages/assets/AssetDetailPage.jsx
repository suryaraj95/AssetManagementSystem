import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useAuthStore from '../../store/authStore';
import { useAsset, useAssignAsset, useUnassignAsset } from '../../hooks/useAssets';
import { useEmployees } from '../../hooks/useEmployees';
import { Button } from '../../components/ui/button';
import { Badge } from '../../components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '../../components/ui/dialog';
import { Label } from '../../components/ui/label';
import { toast } from 'sonner';

export default function AssetDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuthStore();
  const { data: asset, isLoading } = useAsset(id);
  
  const [isAssignOpen, setIsAssignOpen] = useState(false);
  const unassignMutation = useUnassignAsset();

  if (isLoading) return <div className="p-8 text-center">Loading Asset...</div>;
  if (!asset) return <div className="p-8 text-center text-red-500">Asset not found.</div>;

  const handleUnassign = () => {
    if (confirm('Are you sure you want to unassign this asset?')) {
      unassignMutation.mutate(asset.id, {
        onSuccess: () => {
          toast.success('Asset unassigned successfully');
        },
        onError: (err) => {
          toast.error(err.response?.data?.message || err.message);
        }
      });
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center space-x-4">
        <Button variant="outline" onClick={() => navigate('/assets')}>&larr; Back to Assets</Button>
        <h1 className="text-2xl font-bold tracking-tight">Asset Details: {asset.assetId}</h1>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
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

        <div className="bg-white p-6 rounded-md border shadow-sm space-y-4">
          <h2 className="text-lg font-semibold border-b pb-2">Assignments</h2>
          <div className="text-sm flex items-center justify-between">
            <div>
              <span className="text-muted-foreground">Currently Assigned To:</span>
              <p className="font-medium text-lg mt-1">{asset.assignedEmployeeName || 'Unassigned'}</p>
            </div>
            {user?.role === 'Admin' && (
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
      </div>
      
      <AssignAssetModal isOpen={isAssignOpen} onClose={() => setIsAssignOpen(false)} assetId={asset.id} />
    </div>
  );
}

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

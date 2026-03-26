import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAssets } from '../../hooks/useAssets';
import { Button } from '../../components/ui/button';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Badge } from '../../components/ui/badge';
import useAuthStore from '../../store/authStore';

export default function MyAssetsPage() {
  const navigate = useNavigate();
  const { user } = useAuthStore();
  
  // The backend now securely overrides the filter internally for 'Employee' 
  // so we don't have to worry about them passing the wrong employeeId, 
  // but we pass standard bounds to be safe and logical.
  const { data: assetsData, isLoading } = useAssets({ page: 1, size: 50, employeeId: user?.id });
  const assets = assetsData?.items || [];

  const statusColors = {
    'Available': 'default',
    'Assigned': 'success',
    'Maintenance': 'destructive',
    'Retired': 'secondary'
  };

  const conditionColors = {
    'Good': 'success',
    'Fair': 'secondary',
    'Poor': 'destructive',
    'Broken': 'destructive'
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold tracking-tight">My Assigned Assets</h1>
      </div>

      <div className="border rounded-md bg-white">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Asset ID</TableHead>
              <TableHead>Type/Category</TableHead>
              <TableHead>Brand & S/N</TableHead>
              <TableHead>Assigned Date</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Condition</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={7} className="text-center py-6">Loading your assets...</TableCell></TableRow>
            ) : assets.length === 0 ? (
              <TableRow><TableCell colSpan={7} className="text-center py-6 text-muted-foreground">You currently have no assets assigned from the company.</TableCell></TableRow>
            ) : (
              assets.map((asset) => (
                <TableRow key={asset.id}>
                  <TableCell className="font-medium font-mono">{asset.assetId}</TableCell>
                  <TableCell>
                    <div className="font-semibold">{asset.assetTypeName}</div>
                    <div className="text-xs text-muted-foreground">{asset.categoryName}</div>
                  </TableCell>
                  <TableCell>
                    <div>{asset.brandName || 'Unknown Brand'}</div>
                    <div className="text-xs text-muted-foreground">SN: {asset.serialNumber || 'N/A'}</div>
                  </TableCell>
                  <TableCell>{asset.assignedAt ? new Date(asset.assignedAt).toLocaleDateString() : 'N/A'}</TableCell>
                  <TableCell>
                    <Badge variant={statusColors[asset.status] || 'secondary'}>{asset.status}</Badge>
                  </TableCell>
                  <TableCell>
                    <Badge variant={conditionColors[asset.condition] || 'outline'}>{asset.condition}</Badge>
                  </TableCell>
                  <TableCell className="text-right">
                    <Button variant="outline" size="sm" onClick={() => navigate(`/assets/${asset.id}`)}>View Details</Button>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}

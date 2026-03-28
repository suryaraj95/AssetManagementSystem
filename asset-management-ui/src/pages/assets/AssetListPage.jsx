import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import useAuthStore from '../../store/authStore';
import { useAssets, useCreateAsset, useAssignAsset, useUnassignAsset, useUpdateAsset, useDeleteAsset, useAssetFilters, useDownloadTemplate, useImportAssets, useExportAssets } from '../../hooks/useAssets';
import { useEmployees } from '../../hooks/useEmployees';
import { useAssetTypes, useBranches, useSpecs, useCategories } from '../../hooks/useConfig';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '../../components/ui/dialog';
import { Label } from '../../components/ui/label';
import { Badge } from '../../components/ui/badge';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '../../components/ui/dropdown-menu';
import { MoreHorizontal, Eye, Edit, Trash2 } from 'lucide-react';
import { toast } from 'sonner';

const getWarrantyStatus = (date) => {
  if (!date) return { label: 'Not Applicable', color: 'bg-slate-200' };
  const expiry = new Date(date);
  const now = new Date();
  const diffTime = expiry.getTime() - now.getTime();
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

  if (diffDays < 0) return { label: new Date(date).toLocaleDateString(), color: 'bg-red-500' };
  if (diffDays <= 30) return { label: new Date(date).toLocaleDateString(), color: 'bg-yellow-500' };
  return { label: new Date(date).toLocaleDateString(), color: 'bg-green-500' };
};

export default function AssetListPage() {
  const navigate = useNavigate();
  const { user } = useAuthStore();
  const [page, setPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');

  const [filterCategory, setFilterCategory] = useState('');
  const [filterType, setFilterType] = useState('');
  const [filterBranch, setFilterBranch] = useState('');
  const [filterEmployeeId, setFilterEmployeeId] = useState('');
  const [filterStatus, setFilterStatus] = useState('');
  const [filterCondition, setFilterCondition] = useState('');

  const { data: filtersData } = useAssetFilters();
  const filterOptions = filtersData || { categories: [], types: [], branches: [], assignees: [], statuses: [], conditions: [] };

  const { data: assetsData, isLoading } = useAssets({
    page,
    size: 50,
    search: searchTerm,
    categoryId: filterCategory || undefined,
    assetTypeId: filterType || undefined,
    branchId: filterBranch || undefined,
    employeeId: filterEmployeeId || undefined,
    status: filterStatus || undefined,
    condition: filterCondition || undefined
  });
  const assets = assetsData?.items || [];

  const [isAddOpen, setIsAddOpen] = useState(false);
  const [isImportOpen, setIsImportOpen] = useState(false);
  const [assignAssetId, setAssignAssetId] = useState(null);
  const [editAsset, setEditAsset] = useState(null);
  const unassignMutation = useUnassignAsset();
  const deleteMutation = useDeleteAsset();
  const exportMutation = useExportAssets();

  const handleExport = () => {
    exportMutation.mutate({
      categoryId: filterCategory || undefined,
      assetTypeId: filterType || undefined,
      branchId: filterBranch || undefined,
      employeeId: filterEmployeeId || undefined,
      status: filterStatus || undefined,
      condition: filterCondition || undefined,
      search: searchTerm || undefined
    }, {
      onSuccess: () => toast.success('Assets exported successfully'),
      onError: (err) => toast.error(err.response?.data?.message || 'Export failed')
    });
  };

  const handleUnassign = (id) => {
    if (confirm('Are you sure you want to unassign this asset? It will be marked as Available.')) {
      unassignMutation.mutate(id, {
        onSuccess: () => toast.success('Asset unassigned successfully'),
        onError: (err) => toast.error(err.response?.data?.message || err.message)
      });
    }
  };

  const handleDelete = (id) => {
    if (confirm('Are you sure you want to completely delete this asset? This action cannot be undone.')) {
      deleteMutation.mutate(id, {
        onSuccess: () => toast.success('Asset deleted successfully'),
        onError: (err) => toast.error(err.response?.data?.message || err.message)
      });
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold tracking-tight">Assets</h1>
        <div className="flex gap-2">
          <Button variant="outline" onClick={handleExport} disabled={exportMutation.isPending}>
            {exportMutation.isPending ? 'Exporting...' : 'Assets Export'}
          </Button>
          {user?.role === 'Admin' && <Button variant="outline" onClick={() => setIsImportOpen(true)}>Assets Import</Button>}
          <Button onClick={() => setIsAddOpen(true)}>Add Asset</Button>
        </div>
      </div>

      <div className="flex flex-wrap items-center gap-2">
        <Input
          placeholder="Search assets..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="max-w-xs"
        />
        <select className="flex h-9 rounded-md border text-sm px-3" value={filterCategory} onChange={(e) => { setFilterCategory(e.target.value); setFilterType(''); }}>
          <option value="">Category</option>
          {filterOptions.categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>
        <select className="flex h-9 rounded-md border text-sm px-3" value={filterType} onChange={(e) => setFilterType(e.target.value)} disabled={!filterCategory}>
          <option value="">Asset Type</option>
          {filterOptions.types.filter(t => t.categoryId?.toLowerCase() === filterCategory?.toLowerCase()).map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
        </select>
        <select className="flex h-9 rounded-md border text-sm px-3" value={filterEmployeeId} onChange={(e) => setFilterEmployeeId(e.target.value)}>
          <option value="">Assignee</option>
          {filterOptions.assignees.map(e => <option key={e.id} value={e.id}>{e.fullName} - {e.employeeId}</option>)}
        </select>
        <select className="flex h-9 rounded-md border text-sm px-3" value={filterBranch} onChange={(e) => setFilterBranch(e.target.value)}>
          <option value="">Location</option>
          {filterOptions.branches.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
        </select>
        <select className="flex h-9 rounded-md border text-sm px-3" value={filterStatus} onChange={(e) => setFilterStatus(e.target.value)}>
          <option value="">Status</option>
          {filterOptions.statuses.map(s => <option key={s} value={s}>{s}</option>)}
        </select>
        <select className="flex h-9 rounded-md border text-sm px-3" value={filterCondition} onChange={(e) => setFilterCondition(e.target.value)}>
          <option value="">Condition</option>
          {filterOptions.conditions.map(c => <option key={c} value={c}>{c}</option>)}
        </select>
      </div>

      <div className="border rounded-md bg-white">
        <Table>
          <TableHeader>
            <TableRow>
              {user?.role === 'Admin' && <TableHead className="w-[100px]">Mapping</TableHead>}
              <TableHead>Asset ID</TableHead>
              <TableHead>Category</TableHead>
              <TableHead>Type</TableHead>
              <TableHead>Brand & Serial</TableHead>
              <TableHead>Location</TableHead>
              <TableHead>Assignee</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Condition</TableHead>
              <TableHead>Warranty</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={7} className="text-center">Loading...</TableCell></TableRow>
            ) : assets.length === 0 ? (
              <TableRow><TableCell colSpan={9} className="text-center">No assets found</TableCell></TableRow>
            ) : (
              assets.map((asset) => (
                <TableRow key={asset.id} className="cursor-pointer hover:bg-slate-100 transition-colors" onClick={() => navigate(`/assets/${asset.id}`)}>
                  {user?.role === 'Admin' && (
                    <TableCell>
                      {asset.status === 'Available' && (
                        <Button size="sm" onClick={(e) => { e.stopPropagation(); setAssignAssetId(asset.id); }}>Assign</Button>
                      )}
                      {asset.status === 'Assigned' && (
                        <Button size="sm" variant="destructive" onClick={(e) => { e.stopPropagation(); handleUnassign(asset.id); }} disabled={unassignMutation.isPending}>Unassign</Button>
                      )}
                    </TableCell>
                  )}
                  <TableCell className="font-mono font-medium">{asset.assetId}</TableCell>
                  <TableCell>{asset.categoryName}</TableCell>
                  <TableCell>{asset.assetTypeName}</TableCell>
                  <TableCell>
                    {asset.brandName || 'N/A'} <br />
                    <span className="text-xs text-muted-foreground">{asset.serialNumber}</span>
                  </TableCell>
                  <TableCell>{asset.branchName}</TableCell>
                  <TableCell>
                    {asset.assignedEmployeeName ? (
                      <div>
                        {asset.assignedEmployeeName} <br />
                        <span className="text-xs text-muted-foreground">{asset.assignedEmployeeEmpId}</span>
                      </div>
                    ) : <span className="text-muted-foreground">Unassigned</span>}
                  </TableCell>
                  <TableCell>
                    <Badge variant={asset.status === 'Available' ? 'default' : 'secondary'}>
                      {asset.status}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-sm font-medium">{asset.condition}</TableCell>
                  <TableCell className="text-sm">
                    {(() => {
                      const wStatus = getWarrantyStatus(asset.warrantyExpiry);
                      return (
                        <div className="flex items-center gap-2">
                          {asset.warrantyExpiry && <span className={`w-3 h-3 rounded-full ${wStatus.color}`}></span>}
                          <span className={!asset.warrantyExpiry ? "text-slate-400" : ""}>{wStatus.label}</span>
                        </div>
                      );
                    })()}
                  </TableCell>
                  <TableCell className="text-right">
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" className="h-8 w-8 p-0" onClick={(e) => e.stopPropagation()}>
                          <span className="sr-only">Open menu</span>
                          <MoreHorizontal className="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem onClick={(e) => { e.stopPropagation(); navigate(`/assets/${asset.id}`); }}>
                          <Eye className="mr-2 h-4 w-4" />
                          View Details
                        </DropdownMenuItem>
                        {user?.role === 'Admin' && (
                          <>
                            <DropdownMenuItem onClick={(e) => { e.stopPropagation(); setEditAsset(asset); }}>
                              <Edit className="mr-2 h-4 w-4" />
                              Edit Asset
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={(e) => { e.stopPropagation(); handleDelete(asset.id); }} disabled={deleteMutation.isPending} className="text-red-600 focus:text-red-600">
                              <Trash2 className="mr-2 h-4 w-4" />
                              Delete Asset
                            </DropdownMenuItem>
                          </>
                        )}
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>

      <AddAssetModal isOpen={isAddOpen} onClose={() => setIsAddOpen(false)} />
      <ImportAssetsModal isOpen={isImportOpen} onClose={() => setIsImportOpen(false)} />
      {assignAssetId && (
        <AssignAssetModal
          isOpen={!!assignAssetId}
          onClose={() => setAssignAssetId(null)}
          assetId={assignAssetId}
        />
      )}
      {editAsset && (
        <EditAssetModal
          isOpen={!!editAsset}
          onClose={() => setEditAsset(null)}
          asset={editAsset}
        />
      )}
    </div>
  );
}

function AddAssetModal({ isOpen, onClose }) {
  const { data: types = [] } = useAssetTypes();
  const { data: categories = [] } = useCategories();
  const { data: branches = [] } = useBranches();
  const createMutation = useCreateAsset();

  const [selectedCategoryId, setSelectedCategoryId] = useState('');

  const [formData, setFormData] = useState({
    assetTypeId: '',
    branchId: '',
    brandName: '',
    serialNumber: '',
    status: 'Available',
    condition: 'Good',
    hasWarranty: true,
    warrantyExpiry: '',
    specValues: {}
  });

  const { data: specs = [] } = useSpecs(formData.assetTypeId);

  const handleSubmit = (e) => {
    e.preventDefault();
    const payload = {
      ...formData,
      warrantyExpiry: formData.hasWarranty && formData.warrantyExpiry ? new Date(formData.warrantyExpiry).toISOString() : null
    };
    createMutation.mutate(payload, {
      onSuccess: () => {
        toast.success('Asset created successfully!');
        onClose();
      }
    });
  };

  const handleSpecChange = (specId, value) => {
    setFormData(prev => ({
      ...prev,
      specValues: { ...prev.specValues, [specId]: value }
    }));
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Add New Asset</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Category</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
                value={selectedCategoryId}
                onChange={(e) => {
                  setSelectedCategoryId(e.target.value);
                  setFormData({ ...formData, assetTypeId: '' });
                }} required>
                <option value="" disabled>Select Category</option>
                {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </div>
            <div className="space-y-2">
              <Label>Asset Type</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
                value={formData.assetTypeId}
                onChange={(e) => setFormData({ ...formData, assetTypeId: e.target.value })} disabled={!selectedCategoryId} required>
                <option value="" disabled>Select Type</option>
                {types.filter(t => t.categoryId?.toLowerCase() === selectedCategoryId?.toLowerCase()).map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
              </select>
            </div>
            <div className="space-y-2">
              <Label>Branch</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
                value={formData.branchId}
                onChange={(e) => setFormData({ ...formData, branchId: e.target.value })} required>
                <option value="" disabled>Select Branch</option>
                {branches.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
              </select>
            </div>
            <div className="space-y-2">
              <Label>Brand Name</Label>
              <Input value={formData.brandName} onChange={(e) => setFormData({ ...formData, brandName: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label>Serial Number</Label>
              <Input value={formData.serialNumber} onChange={(e) => setFormData({ ...formData, serialNumber: e.target.value })} />
            </div>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2 flex flex-col justify-end">
              <div className="flex items-center space-x-2 py-2">
                <input
                  type="checkbox"
                  id="hasWarranty"
                  checked={formData.hasWarranty}
                  onChange={(e) => setFormData({ ...formData, hasWarranty: e.target.checked, warrantyExpiry: e.target.checked ? formData.warrantyExpiry : '' })}
                  className="rounded border-gray-300 w-4 h-4 cursor-pointer text-indigo-600 focus:ring-indigo-600"
                />
                <Label htmlFor="hasWarranty" className="font-normal cursor-pointer">Has Warranty</Label>
              </div>
            </div>
            <div className="space-y-2">
              <Label>Warranty Expiry</Label>
              <Input
                type="date"
                disabled={!formData.hasWarranty}
                value={formData.warrantyExpiry}
                onChange={(e) => setFormData({ ...formData, warrantyExpiry: e.target.value })}
                required={formData.hasWarranty}
              />
            </div>
          </div>

          {specs.length > 0 && (
            <div className="pt-4 border-t mt-4">
              <h3 className="font-semibold text-sm mb-3 text-muted-foreground">Specifications</h3>
              <div className="grid grid-cols-2 gap-4">
                {specs.map(spec => (
                  <div key={spec.id} className="space-y-2">
                    <Label>{spec.specName} {spec.isRequired && <span className="text-red-500">*</span>}</Label>
                    <Input
                      required={spec.isRequired}
                      value={formData.specValues[spec.id] || ''}
                      onChange={(e) => handleSpecChange(spec.id, e.target.value)}
                    />
                  </div>
                ))}
              </div>
            </div>
          )}


          <DialogFooter className="mt-6">
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button type="submit" disabled={createMutation.isPending}>Save Asset</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
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

function EditAssetModal({ isOpen, onClose, asset }) {
  const updateMutation = useUpdateAsset();
  const _date = asset.warrantyExpiry ? new Date(asset.warrantyExpiry).toISOString().split('T')[0] : '';
  const [formData, setFormData] = useState({
    brandName: asset.brandName || '',
    serialNumber: asset.serialNumber || '',
    status: asset.status || 'Available',
    condition: asset.condition || 'Good',
    notes: asset.notes || '',
    hasWarranty: !!asset.warrantyExpiry,
    warrantyExpiry: _date,
    specValues: {}
  });

  const handleSubmit = (e) => {
    e.preventDefault();
    const payload = {
      ...formData,
      warrantyExpiry: formData.hasWarranty && formData.warrantyExpiry ? new Date(formData.warrantyExpiry).toISOString() : null
    };
    updateMutation.mutate({ id: asset.id, payload: payload }, {
      onSuccess: () => {
        toast.success('Asset updated successfully!');
        onClose();
      },
      onError: (err) => {
        toast.error(err.response?.data?.message || err.message || 'Update failed');
      }
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>Edit Asset</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Brand Name</Label>
              <Input value={formData.brandName} onChange={(e) => setFormData({ ...formData, brandName: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label>Serial Number</Label>
              <Input value={formData.serialNumber} onChange={(e) => setFormData({ ...formData, serialNumber: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label>Status</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1"
                value={formData.status} onChange={(e) => setFormData({ ...formData, status: e.target.value })}>
                <option value="Available">Available</option>
                <option value="Maintenance">Maintenance</option>
                <option value="Retired">Retired</option>
              </select>
            </div>
            <div className="space-y-2">
              <Label>Condition</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1"
                value={formData.condition} onChange={(e) => setFormData({ ...formData, condition: e.target.value })}>
                <option value="Good">Good</option>
                <option value="Fair">Fair</option>
                <option value="Poor">Poor</option>
                <option value="Broken">Broken</option>
              </select>
            </div>
            <div className="space-y-2 flex flex-col justify-end">
              <div className="flex items-center space-x-2 py-2">
                <input
                  type="checkbox"
                  id="editHasWarranty"
                  checked={formData.hasWarranty}
                  onChange={(e) => setFormData({ ...formData, hasWarranty: e.target.checked, warrantyExpiry: e.target.checked ? formData.warrantyExpiry : '' })}
                  className="rounded border-gray-300 w-4 h-4 cursor-pointer text-indigo-600 focus:ring-indigo-600"
                />
                <Label htmlFor="editHasWarranty" className="font-normal cursor-pointer">Has Warranty</Label>
              </div>
            </div>
            <div className="space-y-2">
              <Label>Warranty Expiry</Label>
              <Input
                type="date"
                disabled={!formData.hasWarranty}
                value={formData.warrantyExpiry}
                onChange={(e) => setFormData({ ...formData, warrantyExpiry: e.target.value })}
                required={formData.hasWarranty}
              />
            </div>
          </div>
          <DialogFooter className="mt-6">
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button type="submit" disabled={updateMutation.isPending}>Update Asset</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

function ImportAssetsModal({ isOpen, onClose }) {
  const downloadMutation = useDownloadTemplate();
  const importMutation = useImportAssets();
  const [file, setFile] = useState(null);
  const [errors, setErrors] = useState([]);

  const handleDownload = () => {
    downloadMutation.mutate(undefined, {
      onSuccess: () => toast.success('Template downloaded successfully!'),
      onError: (err) => toast.error('Failed to download template')
    });
  };

  const handleImport = (e) => {
    e.preventDefault();
    if (!file) return;
    setErrors([]);
    importMutation.mutate(file, {
      onSuccess: (data) => {
        toast.success(`Successfully imported ${data.uploadedCount} assets.`);
        onClose();
        setFile(null);
      },
      onError: (err) => {
        const errData = err.response?.data;
        if (errData && errData.errors) {
          setErrors(errData.errors);
          toast.error('Import failed due to validation errors.');
        } else {
          toast.error(err.response?.data?.message || err.message || 'Import failed');
        }
      }
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={(open) => { if (!open) { onClose(); setFile(null); setErrors([]); } }}>
      <DialogContent className="max-w-xl">
        <DialogHeader>
          <DialogTitle>Import Assets</DialogTitle>
        </DialogHeader>
        <div className="space-y-6">
          <div className="p-4 border rounded-md bg-slate-50 flex items-center justify-between">
            <div>
              <h4 className="font-medium">Step 1: Download Template</h4>
              <p className="text-sm text-slate-500">Get the exact columns needed for import.</p>
            </div>
            <Button variant="secondary" onClick={handleDownload} disabled={downloadMutation.isPending}>Download</Button>
          </div>
          <form onSubmit={handleImport} className="p-4 border rounded-md space-y-4">
            <div>
              <h4 className="font-medium mb-1">Step 2: Upload Excel</h4>
              <p className="text-sm text-slate-500 mb-3">Fill the downloaded template and upload it here. Follow All or Nothing validation.</p>
              <Input type="file" accept=".xlsx,.xls" onChange={(e) => setFile(e.target.files[0])} />
            </div>
            {errors.length > 0 && (
              <div className="bg-red-50 text-red-600 p-3 rounded-md text-sm max-h-32 overflow-y-auto">
                <ul className="list-disc pl-4">
                  {errors.map((err, i) => <li key={i}>{err}</li>)}
                </ul>
              </div>
            )}
            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => { onClose(); setFile(null); setErrors([]); }}>Cancel</Button>
              <Button type="submit" disabled={!file || importMutation.isPending}>Import Assets</Button>
            </DialogFooter>
          </form>
        </div>
      </DialogContent>
    </Dialog>
  );
}

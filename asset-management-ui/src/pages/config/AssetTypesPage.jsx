import React, { useState } from 'react';
import { useAssetTypes, useCreateAssetType, useUpdateAssetType, useCategories } from '../../hooks/useConfig';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '../../components/ui/dialog';
import { Label } from '../../components/ui/label';
import { Badge } from '../../components/ui/badge';
import { toast } from 'sonner';

export default function AssetTypesPage() {
  const { data: assetTypes = [], isLoading } = useAssetTypes();
  const { data: categories = [] } = useCategories();
  const createMutation = useCreateAssetType();
  const updateMutation = useUpdateAssetType();
  
  const [isOpen, setIsOpen] = useState(false);
  const [editingItem, setEditingItem] = useState(null);
  
  const [formData, setFormData] = useState({ name: '', categoryId: '', isActive: true });

  const handleOpen = (item = null) => {
    if (item) {
      setEditingItem(item);
      setFormData({ name: item.name, categoryId: item.categoryId, isActive: item.isActive });
    } else {
      setEditingItem(null);
      setFormData({ name: '', categoryId: categories[0]?.id || '', isActive: true });
    }
    setIsOpen(true);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!formData.categoryId) {
      toast.error('Please select a category');
      return;
    }
    if (editingItem) {
      updateMutation.mutate({ id: editingItem.id, payload: formData }, {
        onSuccess: () => {
          toast.success('Asset Type updated');
          setIsOpen(false);
        }
      });
    } else {
      createMutation.mutate(formData, {
        onSuccess: () => {
          toast.success('Asset Type created');
          setIsOpen(false);
        }
      });
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold tracking-tight">Asset Types</h1>
        <Button onClick={() => handleOpen()} disabled={categories.length === 0}>
          {categories.length === 0 ? 'Create a Category First' : 'Add Asset Type'}
        </Button>
      </div>

      <div className="border rounded-md bg-white">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Type Name</TableHead>
              <TableHead>Category</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={4} className="text-center">Loading...</TableCell></TableRow>
            ) : assetTypes.length === 0 ? (
              <TableRow><TableCell colSpan={4} className="text-center">No asset types found</TableCell></TableRow>
            ) : (
              assetTypes.map((item) => (
                <TableRow key={item.id}>
                  <TableCell className="font-medium">{item.name}</TableCell>
                  <TableCell>{item.categoryName || 'Unknown'}</TableCell>
                  <TableCell className="text-right">
                    <Button variant="outline" size="sm" onClick={() => handleOpen(item)}>Edit</Button>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>

      <Dialog open={isOpen} onOpenChange={setIsOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editingItem ? 'Edit Asset Type' : 'Create Asset Type'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label>Type Name</Label>
              <Input value={formData.name} onChange={(e) => setFormData({...formData, name: e.target.value})} required />
            </div>
            <div className="space-y-2">
              <Label>Category</Label>
              <select 
                className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
                value={formData.categoryId} 
                onChange={(e) => setFormData({...formData, categoryId: e.target.value})}
                required
              >
                <option value="" disabled>Select Category</option>
                {categories.map(c => (
                  <option key={c.id} value={c.id}>{c.name}</option>
                ))}
              </select>
            </div>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => setIsOpen(false)}>Cancel</Button>
              <Button type="submit">Save</Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}

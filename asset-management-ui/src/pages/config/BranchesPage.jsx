import React, { useState } from 'react';
import { useBranches, useCreateBranch, useUpdateBranch } from '../../hooks/useConfig';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '../../components/ui/dialog';
import { Label } from '../../components/ui/label';
import { Badge } from '../../components/ui/badge';
import { toast } from 'sonner';

export default function BranchesPage() {
  const { data: branches = [], isLoading } = useBranches();
  const createMutation = useCreateBranch();
  const updateMutation = useUpdateBranch();
  
  const [isOpen, setIsOpen] = useState(false);
  const [editingItem, setEditingItem] = useState(null);
  
  const [formData, setFormData] = useState({ name: '', code: '', address: '', isActive: true });

  const handleOpen = (item = null) => {
    if (item) {
      setEditingItem(item);
      setFormData({ name: item.name, code: item.code, address: item.address || '', isActive: item.isActive });
    } else {
      setEditingItem(null);
      setFormData({ name: '', code: '', address: '', isActive: true });
    }
    setIsOpen(true);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (editingItem) {
      updateMutation.mutate({ id: editingItem.id, payload: formData }, {
        onSuccess: () => {
          toast.success('Branch updated');
          setIsOpen(false);
        }
      });
    } else {
      createMutation.mutate(formData, {
        onSuccess: () => {
          toast.success('Branch created');
          setIsOpen(false);
        }
      });
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold tracking-tight">Branches</h1>
        <Button onClick={() => handleOpen()}>Add Branch</Button>
      </div>

      <div className="border rounded-md bg-white">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Code</TableHead>
              <TableHead>Name</TableHead>
              <TableHead>Address</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={5} className="text-center">Loading...</TableCell></TableRow>
            ) : branches.length === 0 ? (
              <TableRow><TableCell colSpan={5} className="text-center">No branches found</TableCell></TableRow>
            ) : (
              branches.map((item) => (
                <TableRow key={item.id}>
                  <TableCell className="font-mono text-sm">{item.code}</TableCell>
                  <TableCell className="font-medium">{item.name}</TableCell>
                  <TableCell>{item.address}</TableCell>
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
            <DialogTitle>{editingItem ? 'Edit Branch' : 'Create Branch'}</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>Branch Code</Label>
                <Input value={formData.code} onChange={(e) => setFormData({...formData, code: e.target.value.toUpperCase()})} required maxLength={10} />
              </div>
              <div className="space-y-2">
                <Label>Branch Name</Label>
                <Input value={formData.name} onChange={(e) => setFormData({...formData, name: e.target.value})} required />
              </div>
            </div>
            <div className="space-y-2">
              <Label>Address</Label>
              <Input value={formData.address} onChange={(e) => setFormData({...formData, address: e.target.value})} />
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

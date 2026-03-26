import React, { useState } from 'react';
import { useEmployees, useUpdateEmployee, useCreateEmployee } from '../../hooks/useEmployees';
import { useBranches } from '../../hooks/useConfig';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Badge } from '../../components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '../../components/ui/dialog';
import { Label } from '../../components/ui/label';
import { toast } from 'sonner';

export default function EmployeeListPage() {
  const [searchTerm, setSearchTerm] = useState('');
  const [roleFilter, setRoleFilter] = useState('');

  const { data: employees = [], isLoading } = useEmployees({ search: searchTerm, role: roleFilter });
  const [editingItem, setEditingItem] = useState(null);
  const [isAddOpen, setIsAddOpen] = useState(false);

  const handleEdit = (employee) => {
    setEditingItem(employee);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold tracking-tight">Employees</h1>
        <Button onClick={() => setIsAddOpen(true)}>Add Employee</Button>
      </div>

      <div className="flex items-center space-x-4">
        <Input 
          placeholder="Search by name, email, or ID..." 
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="max-w-sm"
        />
        <select 
          className="flex h-9 rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
          value={roleFilter}
          onChange={(e) => setRoleFilter(e.target.value)}
        >
          <option value="">All Roles</option>
          <option value="Admin">Admin</option>
          <option value="HR">HR</option>
          <option value="Employee">Employee</option>
        </select>
      </div>

      <div className="border rounded-md bg-white">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Employee ID</TableHead>
              <TableHead>Name & Contact</TableHead>
              <TableHead>Role</TableHead>
              <TableHead>Branch</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={6} className="text-center">Loading...</TableCell></TableRow>
            ) : employees.length === 0 ? (
              <TableRow><TableCell colSpan={6} className="text-center">No employees found</TableCell></TableRow>
            ) : (
              employees.map((emp) => (
                <TableRow key={emp.id}>
                  <TableCell className="font-mono font-medium">{emp.employeeId || 'N/A'}</TableCell>
                  <TableCell>
                    <div className="font-medium">{emp.fullName}</div>
                    <div className="text-xs text-muted-foreground">{emp.email}</div>
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline">{emp.role}</Badge>
                  </TableCell>
                  <TableCell>{emp.branchName}</TableCell>
                  <TableCell>
                    <Badge variant={emp.status === 'Active' ? 'default' : 'secondary'}>
                      {emp.status}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-right">
                    <Button variant="outline" size="sm" onClick={() => handleEdit(emp)}>Edit Profile</Button>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>

      {editingItem && (
        <EditEmployeeModal 
          employee={editingItem} 
          isOpen={!!editingItem} 
          onClose={() => setEditingItem(null)} 
        />
      )}

      {isAddOpen && <AddEmployeeModal isOpen={isAddOpen} onClose={() => setIsAddOpen(false)} />}
    </div>
  );
}

function AddEmployeeModal({ isOpen, onClose }) {
  const { data: branches = [] } = useBranches();
  const createMutation = useCreateEmployee();

  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    password: '',
    employeeId: '',
    role: 'Employee',
    branchId: '',
    department: '',
    phone: ''
  });

  const handleSubmit = (e) => {
    e.preventDefault();
    createMutation.mutate(formData, {
      onSuccess: () => {
        toast.success('Employee created successfully');
        onClose();
      },
      onError: (error) => {
        toast.error('Failed to create account. Check logs.');
        console.error(error);
      }
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-xl">
        <DialogHeader>
          <DialogTitle>Add New Employee / User</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Full Name</Label>
              <Input value={formData.fullName} onChange={(e) => setFormData({...formData, fullName: e.target.value})} required />
            </div>
            <div className="space-y-2">
              <Label>Email</Label>
              <Input type="email" value={formData.email} onChange={(e) => setFormData({...formData, email: e.target.value})} required />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Initial Password</Label>
              <Input type="password" value={formData.password} onChange={(e) => setFormData({...formData, password: e.target.value})} required minLength={6} placeholder="e.g. Pass@123" />
            </div>
            <div className="space-y-2">
              <Label>Employee ID (Optional)</Label>
              <Input value={formData.employeeId} onChange={(e) => setFormData({...formData, employeeId: e.target.value})} />
            </div>
          </div>
          
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Role</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
                value={formData.role} onChange={(e) => setFormData({...formData, role: e.target.value})} required>
                <option value="Employee">Employee</option>
                <option value="HR">HR</option>
                <option value="Admin">Admin</option>
              </select>
            </div>
            
            <div className="space-y-2">
              <Label>Branch</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
                value={formData.branchId} onChange={(e) => setFormData({...formData, branchId: e.target.value ? e.target.value : null})}>
                <option value="">No Branch (Global)</option>
                {branches.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
              </select>
            </div>
          </div>

          <DialogFooter className="mt-6">
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button type="submit" disabled={createMutation.isPending}>Create User</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

function EditEmployeeModal({ employee, isOpen, onClose }) {
  const { data: branches = [] } = useBranches();
  const updateMutation = useUpdateEmployee();

  const [formData, setFormData] = useState({
    fullName: employee.fullName,
    role: employee.role,
    branchId: employee.branchId || '',
    status: employee.status || 'Active',
    department: employee.department || '',
    phone: employee.phone || ''
  });

  const handleSubmit = (e) => {
    e.preventDefault();
    updateMutation.mutate({ id: employee.id, payload: formData }, {
      onSuccess: () => {
        toast.success('Employee updated successfully');
        onClose();
      }
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Edit Profile: {employee.fullName}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label>Full Name</Label>
            <Input value={formData.fullName} onChange={(e) => setFormData({...formData, fullName: e.target.value})} required />
          </div>
          
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Role</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
                value={formData.role} onChange={(e) => setFormData({...formData, role: e.target.value})} required>
                <option value="Admin">Admin</option>
                <option value="HR">HR</option>
                <option value="Employee">Employee</option>
              </select>
            </div>
            
            <div className="space-y-2">
              <Label>Branch</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
                value={formData.branchId || ''} onChange={(e) => setFormData({...formData, branchId: e.target.value ? e.target.value : null})}>
                <option value="">No Branch (Global)</option>
                {branches.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
              </select>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Department</Label>
              <Input value={formData.department} onChange={(e) => setFormData({...formData, department: e.target.value})} />
            </div>

            <div className="space-y-2">
              <Label>Status</Label>
              <select className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm"
                value={formData.status} onChange={(e) => setFormData({...formData, status: e.target.value})} required>
                <option value="Active">Active</option>
                <option value="Inactive">Inactive</option>
                <option value="Suspended">Suspended</option>
              </select>
            </div>
          </div>

          <DialogFooter className="mt-6">
            <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
            <Button type="submit" disabled={updateMutation.isPending}>Save Changes</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

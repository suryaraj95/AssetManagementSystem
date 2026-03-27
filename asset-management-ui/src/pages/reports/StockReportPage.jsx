import React, { useMemo } from 'react';
import { useAssets } from '../../hooks/useAssets';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Button } from '../../components/ui/button';
import { FileDown } from 'lucide-react';
import * as XLSX from 'xlsx';

export default function StockReportPage() {
  const { data: assetsData, isLoading } = useAssets({ page: 1, size: 5000 });
  const assets = assetsData?.items || [];

  // Group assets internally by Category -> Asset Type -> Status
  const reportData = useMemo(() => {
    if (!assets.length) return [];
    
    const grouping = {};

    assets.forEach((asset) => {
      const cat = asset.categoryName || 'Uncategorized';
      const type = asset.assetTypeName || 'Unknown Type';

      if (!grouping[cat]) grouping[cat] = {};
      if (!grouping[cat][type]) {
        grouping[cat][type] = {
          total: 0,
          available: 0,
          assigned: 0,
          maintenance: 0,
          retired: 0
        };
      }

      grouping[cat][type].total += 1;
      
      if (asset.status === 'Available') grouping[cat][type].available += 1;
      else if (asset.status === 'Assigned') grouping[cat][type].assigned += 1;
      else if (asset.status === 'Maintenance') grouping[cat][type].maintenance += 1;
      else if (asset.status === 'Retired') grouping[cat][type].retired += 1;
    });

    const rows = [];
    Object.keys(grouping).sort().forEach(cat => {
      Object.keys(grouping[cat]).sort().forEach(type => {
        rows.push({
          category: cat,
          type: type,
          stats: grouping[cat][type]
        });
      });
    });

    return rows;
  }, [assets]);

  const handleExport = () => {
    if (!reportData || reportData.length === 0) return;

    const exportData = reportData.map(row => ({
      Category: row.category,
      'Asset Type': row.type,
      'Available Stock': row.stats.available,
      Assigned: row.stats.assigned,
      'In Maintenance': row.stats.maintenance,
      Retired: row.stats.retired,
      'Total Units': row.stats.total
    }));

    const worksheet = XLSX.utils.json_to_sheet(exportData);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Stock Report");
    XLSX.writeFile(workbook, "asset_details.xlsx");
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Real-Time Stock Report</h1>
          <p className="text-muted-foreground mt-1">Real-time breakdown of asset allocation and availability across the system.</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={handleExport}><FileDown className="w-4 h-4 mr-2" /> Export Excel</Button>
        </div>
      </div>

      <div className="border rounded-md bg-white">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Category</TableHead>
              <TableHead>Asset Type</TableHead>
              <TableHead className="text-right">Available Stock</TableHead>
              <TableHead className="text-right">Assigned</TableHead>
              <TableHead className="text-right">In Maintenance</TableHead>
              <TableHead className="text-right">Retired</TableHead>
              <TableHead className="text-right font-bold text-slate-800">Total Units</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={7} className="text-center py-6">Compiling stock data...</TableCell></TableRow>
            ) : reportData.length === 0 ? (
              <TableRow><TableCell colSpan={7} className="text-center py-6 text-muted-foreground">No assets found in inventory.</TableCell></TableRow>
            ) : (
              reportData.map((row, index) => (
                <TableRow key={index} className="hover:bg-slate-50 transition-colors">
                  <TableCell className="font-semibold text-slate-900">{row.category}</TableCell>
                  <TableCell className="text-slate-700">{row.type}</TableCell>
                  <TableCell className="text-right font-medium text-emerald-600">{row.stats.available}</TableCell>
                  <TableCell className="text-right text-indigo-600">{row.stats.assigned}</TableCell>
                  <TableCell className="text-right text-amber-600">{row.stats.maintenance}</TableCell>
                  <TableCell className="text-right text-slate-400">{row.stats.retired}</TableCell>
                  <TableCell className="text-right font-bold text-slate-800 bg-slate-50/50">{row.stats.total}</TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import apiClient from '../api/client';

export const useAssets = (params) => {
  return useQuery({
    queryKey: ['assets', params],
    queryFn: async () => {
      const { data } = await apiClient.get('/assets', { params });
      return data;
    },
  });
};

export const useAssetFilters = () => {
  return useQuery({
    queryKey: ['assetFilters'],
    queryFn: async () => {
      const { data } = await apiClient.get('/assets/filters');
      return data;
    },
  });
};

export const useAsset = (id) => {
  return useQuery({
    queryKey: ['asset', id],
    queryFn: async () => {
      const { data } = await apiClient.get(`/assets/${id}`);
      return data;
    },
    enabled: !!id,
  });
};

export const useCreateAsset = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (payload) => {
      const { data } = await apiClient.post('/assets', payload);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['assets'] });
    },
  });
};

export const useUpdateAsset = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, payload }) => {
      const { data } = await apiClient.put(`/assets/${id}`, payload);
      return data;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['assets'] });
      queryClient.invalidateQueries({ queryKey: ['asset', variables.id] });
    },
  });
};

export const useAssignAsset = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, employeeId }) => {
      await apiClient.post(`/assets/${id}/assign`, { employeeId });
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['assets'] });
      queryClient.invalidateQueries({ queryKey: ['asset', variables.id] });
    },
  });
};

export const useUnassignAsset = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id) => {
      await apiClient.post(`/assets/${id}/unassign`, {});
    },
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['assets'] });
      queryClient.invalidateQueries({ queryKey: ['asset', id] });
    },
  });
};

export const useDeleteAsset = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id) => {
      await apiClient.delete(`/assets/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['assets'] });
    },
  });
};

export const useChangeAssetStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, status, condition, remarks }) => {
      await apiClient.post(`/assets/${id}/status`, { status, condition, remarks });
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['assets'] });
      queryClient.invalidateQueries({ queryKey: ['asset', variables.id] });
    },
  });
};

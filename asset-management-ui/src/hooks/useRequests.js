import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import apiClient from '../api/client';

export const useRequests = (params) => {
  return useQuery({
    queryKey: ['requests', params],
    queryFn: async () => {
      const { data } = await apiClient.get('/requests', { params });
      return data;
    },
  });
};

export const useCreateRequest = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (payload) => {
      const { data } = await apiClient.post('/requests', payload);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['requests'] });
    },
  });
};

export const useHrApproveRequest = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (id) => {
      const { data } = await apiClient.put(`/requests/${id}/hr-approve`);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['requests'] });
    },
  });
};

export const useAdminApproveRequest = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, assignedAssetId }) => {
      const { data } = await apiClient.put(`/requests/${id}/admin-approve`, { assignedAssetId });
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['requests'] });
      queryClient.invalidateQueries({ queryKey: ['assets'] });
    },
  });
};

export const useRejectRequest = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, rejectionReason }) => {
      const { data } = await apiClient.put(`/requests/${id}/reject`, { rejectionReason });
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['requests'] });
    },
  });
};

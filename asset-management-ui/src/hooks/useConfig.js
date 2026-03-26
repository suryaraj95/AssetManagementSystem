import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import apiClient from '../api/client';

// -- Categories --
export const useCategories = () => {
  return useQuery({
    queryKey: ['categories'],
    queryFn: async () => {
      const { data } = await apiClient.get('/categories');
      return data;
    },
  });
};

export const useCreateCategory = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (payload) => {
      const { data } = await apiClient.post('/categories', payload);
      return data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categories'] }),
  });
};

export const useUpdateCategory = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, payload }) => {
      const { data } = await apiClient.put(`/categories/${id}`, payload);
      return data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categories'] }),
  });
};

// -- Branches --
export const useBranches = () => {
  return useQuery({
    queryKey: ['branches'],
    queryFn: async () => {
      const { data } = await apiClient.get('/branches');
      return data;
    },
  });
};

export const useCreateBranch = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (payload) => {
      const { data } = await apiClient.post('/branches', payload);
      return data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['branches'] }),
  });
};

export const useUpdateBranch = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, payload }) => {
      const { data } = await apiClient.put(`/branches/${id}`, payload);
      return data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['branches'] }),
  });
};

// -- Asset Types --
export const useAssetTypes = () => {
  return useQuery({
    queryKey: ['assetTypes'],
    queryFn: async () => {
      const { data } = await apiClient.get('/asset-types');
      return data;
    },
  });
};

export const useCreateAssetType = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (payload) => {
      const { data } = await apiClient.post('/asset-types', payload);
      return data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['assetTypes'] }),
  });
};

export const useUpdateAssetType = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ id, payload }) => {
      const { data } = await apiClient.put(`/asset-types/${id}`, payload);
      return data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['assetTypes'] }),
  });
};

// -- Specs --
export const useSpecs = (assetTypeId) => {
  return useQuery({
    queryKey: ['specs', assetTypeId],
    queryFn: async () => {
      const { data } = await apiClient.get('/specs', { params: { assetTypeId } });
      return data;
    },
    enabled: !!assetTypeId,
  });
};

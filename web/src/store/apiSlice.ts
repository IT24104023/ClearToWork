import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { RootState } from './index';
import type {
  PermitDetails,
  ValidationReport,
  Worker,
  Asset,
  Zone,
  SafetyAnalytics,
  UserSession,
} from '../types';

export const apiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    baseUrl: '/api',
    prepareHeaders: (headers, { getState }) => {
      const token = (getState() as RootState).auth.token;
      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }
      return headers;
    },
  }),
  tagTypes: ['Permit', 'Worker', 'Equipment', 'Zone', 'Analytics'],
  endpoints: (builder) => ({
    // Auth Endpoints
    login: builder.mutation<
      { token: string; id: string; fullName: string; email: string; role: string; contractorId: string | null },
      { email: string; password: string }
    >({
      query: (credentials) => ({
        url: '/auth/login',
        method: 'POST',
        body: credentials,
      }),
    }),
    getMe: builder.query<UserSession, void>({
      query: () => '/auth/me',
    }),

    // Permit Endpoints
    getPermits: builder.query<
      PermitDetails[],
      { status?: string; contractorId?: string; zoneId?: string } | void
    >({
      query: (params) => {
        const queryParams = new URLSearchParams();
        if (params && params.status) queryParams.append('status', params.status);
        if (params && params.contractorId) queryParams.append('contractorId', params.contractorId);
        if (params && params.zoneId) queryParams.append('zoneId', params.zoneId);
        return `/permits?${queryParams.toString()}`;
      },
      providesTags: ['Permit'],
    }),
    getPermitById: builder.query<PermitDetails, string>({
      query: (id) => `/permits/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'Permit', id }],
    }),
    createPermitDraft: builder.mutation<PermitDetails, any>({
      query: (body) => ({
        url: '/permits',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Permit'],
    }),
    submitPermitForAiReview: builder.mutation<ValidationReport, string>({
      query: (permitId) => ({
        url: `/permits/${permitId}/submit`,
        method: 'POST',
      }),
      invalidatesTags: ['Permit', 'Analytics'],
    }),
    recordDecision: builder.mutation<
      PermitDetails,
      { permitId: string; decision: string; decisionNotes: string }
    >({
      query: ({ permitId, decision, decisionNotes }) => ({
        url: `/permits/${permitId}/decision`,
        method: 'POST',
        body: { decision, decisionNotes },
      }),
      invalidatesTags: ['Permit', 'Equipment', 'Analytics'],
    }),

    // Workforce Endpoints
    getWorkers: builder.query<Worker[], { trade?: string; activeOnly?: boolean } | void>({
      query: (params) => {
        const queryParams = new URLSearchParams();
        if (params && params.trade) queryParams.append('trade', params.trade);
        if (params && params.activeOnly !== undefined)
          queryParams.append('activeOnly', String(params.activeOnly));
        return `/workforce?${queryParams.toString()}`;
      },
      providesTags: ['Worker'],
    }),
    getExpiryForecast: builder.query<any[], void>({
      query: () => '/workforce/expiry-forecast',
      providesTags: ['Worker'],
    }),

    // Equipment Endpoints
    getEquipment: builder.query<Asset[], { category?: string; status?: string } | void>({
      query: (params) => {
        const queryParams = new URLSearchParams();
        if (params && params.category) queryParams.append('category', params.category);
        if (params && params.status) queryParams.append('status', params.status);
        return `/equipment?${queryParams.toString()}`;
      },
      providesTags: ['Equipment'],
    }),

    // Hazards, Zones & Analytics
    getZones: builder.query<Zone[], void>({
      query: () => '/hazardzone/zones',
      providesTags: ['Zone'],
    }),
    checkZoneConflicts: builder.mutation<any, any>({
      query: (body) => ({
        url: '/hazardzone/zones/conflict-check',
        method: 'POST',
        body,
      }),
    }),
    getSafetyAnalytics: builder.query<SafetyAnalytics, void>({
      query: () => '/hazardzone/analytics/safety-summary',
      providesTags: ['Analytics'],
    }),
  }),
});

export const {
  useLoginMutation,
  useGetMeQuery,
  useGetPermitsQuery,
  useGetPermitByIdQuery,
  useCreatePermitDraftMutation,
  useSubmitPermitForAiReviewMutation,
  useRecordDecisionMutation,
  useGetWorkersQuery,
  useGetExpiryForecastQuery,
  useGetEquipmentQuery,
  useGetZonesQuery,
  useCheckZoneConflictsMutation,
  useGetSafetyAnalyticsQuery,
} = apiSlice;

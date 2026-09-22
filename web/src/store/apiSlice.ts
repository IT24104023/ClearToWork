import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { RootState } from './index';
import type {
  PermitDetails,
  ValidationReport,
  Worker,
  WorkerCertificate,
  Asset,
  Zone,
  SafetyAnalytics,
  UserSession,
  Contractor,
  CertificateType,
  CreateWorkerRequest,
  UpdateWorkerRequest,
  CreateCertificateRequest,
  CreateAssetRequest,
  UpdateAssetRequest,
  CreateInspectionRequest,
  CreateCalibrationRequest,
  IsolationPoint,
  CreateIsolationPointRequest,
  UpdateIsolationPointStateRequest,
  UpdatePermitRequest,
  Observation,
  CreateObservationRequest,
  UpdateObservationRequest,
  HazardType,
  CreateHazardTypeRequest,
  UpdateHazardTypeRequest,
  ControlMeasure,
  CreateControlMeasureRequest,
  UpdateControlMeasureRequest,
  IncompatibilityRule,
  CreateIncompatibilityRuleRequest,
  UpdateIncompatibilityRuleRequest,
  ZoneAdjacency,
  AddZoneAdjacencyRequest,
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
  tagTypes: ['Permit', 'Worker', 'Equipment', 'Zone', 'Analytics', 'Contractor', 'CertificateType', 'IsolationPoint', 'HazardType', 'IncompatibilityRule', 'Observation'],
  endpoints: (builder) => ({
    // Auth Endpoints
    login: builder.mutation<
      { token: string; id?: string; userId?: string; fullName: string; email: string; role: string; contractorId: string | null },
      { email: string; password: string }
    >({
      query: (credentials) => ({
        url: '/auth/login',
        method: 'POST',
        body: credentials,
      }),
    }),
    register: builder.mutation<
      { token: string; userId: string; fullName: string; email: string; role: string; contractorId: string | null },
      { fullName: string; email: string; password: string; role: string; contractorId?: string | null }
    >({
      query: (data) => ({
        url: '/auth/register',
        method: 'POST',
        body: data,
      }),
    }),
    getMe: builder.query<UserSession, void>({
      query: () => '/auth/me',
    }),

    // Permit Endpoints (Student 3)
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
      invalidatesTags: ['Permit', 'Analytics'],
    }),
    updatePermitDraft: builder.mutation<PermitDetails, { id: string; body: UpdatePermitRequest }>({
      query: ({ id, body }) => ({
        url: `/permits/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: ['Permit', 'Analytics'],
    }),
    deletePermitDraft: builder.mutation<{ message: string; id: string }, string>({
      query: (id) => ({
        url: `/permits/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Permit', 'Analytics'],
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

    // Workforce Endpoints (Student 1)
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
    getWorkerById: builder.query<Worker, string>({
      query: (id) => `/workforce/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'Worker', id }],
    }),
    createWorker: builder.mutation<Worker, CreateWorkerRequest>({
      query: (body) => ({
        url: '/workforce',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Worker'],
    }),
    updateWorker: builder.mutation<Worker, { id: string; body: UpdateWorkerRequest }>({
      query: ({ id, body }) => ({
        url: `/workforce/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: ['Worker'],
    }),
    deleteWorker: builder.mutation<{ message: string; id: string }, string>({
      query: (id) => ({
        url: `/workforce/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Worker'],
    }),
    addWorkerCertificate: builder.mutation<WorkerCertificate, { workerId: string; body: CreateCertificateRequest }>({
      query: ({ workerId, body }) => ({
        url: `/workforce/${workerId}/certificates`,
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Worker'],
    }),
    deleteWorkerCertificate: builder.mutation<{ message: string; certificateId: string }, string>({
      query: (certId) => ({
        url: `/workforce/certificates/${certId}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Worker'],
    }),
    getContractors: builder.query<Contractor[], void>({
      query: () => '/workforce/contractors',
      providesTags: ['Contractor'],
    }),
    getCertificateTypes: builder.query<CertificateType[], void>({
      query: () => '/workforce/certificate-types',
      providesTags: ['CertificateType'],
    }),
    getExpiryForecast: builder.query<any[], void>({
      query: () => '/workforce/expiry-forecast',
      providesTags: ['Worker'],
    }),

    // Equipment & Isolation Points Endpoints (Student 2)
    getEquipment: builder.query<Asset[], { category?: string; status?: string } | void>({
      query: (params) => {
        const queryParams = new URLSearchParams();
        if (params && params.category) queryParams.append('category', params.category);
        if (params && params.status) queryParams.append('status', params.status);
        return `/equipment?${queryParams.toString()}`;
      },
      providesTags: ['Equipment'],
    }),
    getEquipmentById: builder.query<Asset, string>({
      query: (id) => `/equipment/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'Equipment', id }],
    }),
    createEquipment: builder.mutation<Asset, CreateAssetRequest>({
      query: (body) => ({
        url: '/equipment',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Equipment'],
    }),
    updateEquipment: builder.mutation<Asset, { id: string; body: UpdateAssetRequest }>({
      query: ({ id, body }) => ({
        url: `/equipment/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: ['Equipment'],
    }),
    deleteEquipment: builder.mutation<{ message: string; id: string }, string>({
      query: (id) => ({
        url: `/equipment/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Equipment'],
    }),
    addInspectionRecord: builder.mutation<{ message: string }, { id: string; body: CreateInspectionRequest }>({
      query: ({ id, body }) => ({
        url: `/equipment/${id}/inspections`,
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Equipment'],
    }),
    addCalibrationRecord: builder.mutation<{ message: string }, { id: string; body: CreateCalibrationRequest }>({
      query: ({ id, body }) => ({
        url: `/equipment/${id}/calibrations`,
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Equipment'],
    }),
    getIsolationPoints: builder.query<IsolationPoint[], string>({
      query: (zoneId) => `/equipment/isolation-points/${zoneId}`,
      providesTags: ['IsolationPoint'],
    }),
    createIsolationPoint: builder.mutation<IsolationPoint, CreateIsolationPointRequest>({
      query: (body) => ({
        url: '/equipment/isolation-points',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['IsolationPoint'],
    }),
    updateIsolationState: builder.mutation<IsolationPoint, { id: string; body: UpdateIsolationPointStateRequest }>({
      query: ({ id, body }) => ({
        url: `/equipment/isolation-points/${id}/state`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: ['IsolationPoint'],
    }),

    // Hazards, Zones & Analytics (Student 4)
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

    // Safety Observations (Student 4)
    getObservations: builder.query<Observation[], { zoneId?: string; category?: string } | void>({
      query: (params) => {
        const queryParams = new URLSearchParams();
        if (params && params.zoneId) queryParams.append('zoneId', params.zoneId);
        if (params && params.category && params.category !== 'ALL') queryParams.append('category', params.category);
        return `/hazardzone/observations?${queryParams.toString()}`;
      },
      providesTags: ['Observation'],
    }),
    getObservationById: builder.query<Observation, string>({
      query: (id) => `/hazardzone/observations/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'Observation', id }],
    }),
    createObservation: builder.mutation<Observation, CreateObservationRequest>({
      query: (body) => ({
        url: '/hazardzone/observations',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Observation', 'Analytics'],
    }),
    updateObservation: builder.mutation<Observation, { id: string; body: UpdateObservationRequest }>({
      query: ({ id, body }) => ({
        url: `/hazardzone/observations/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: ['Observation', 'Analytics'],
    }),
    deleteObservation: builder.mutation<{ message: string; id: string }, string>({
      query: (id) => ({
        url: `/hazardzone/observations/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Observation', 'Analytics'],
    }),

    // Rulebook Hazard Types & Control Measures (Student 4)
    getHazardTypes: builder.query<HazardType[], void>({
      query: () => '/hazardzone/hazard-types',
      providesTags: ['HazardType'],
    }),
    getHazardTypeById: builder.query<HazardType, string>({
      query: (id) => `/hazardzone/hazard-types/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'HazardType', id }],
    }),
    createHazardType: builder.mutation<HazardType, CreateHazardTypeRequest>({
      query: (body) => ({
        url: '/hazardzone/hazard-types',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['HazardType'],
    }),
    updateHazardType: builder.mutation<HazardType, { id: string; body: UpdateHazardTypeRequest }>({
      query: ({ id, body }) => ({
        url: `/hazardzone/hazard-types/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: ['HazardType'],
    }),
    deleteHazardType: builder.mutation<{ message: string; id: string }, string>({
      query: (id) => ({
        url: `/hazardzone/hazard-types/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['HazardType'],
    }),

    createControlMeasure: builder.mutation<ControlMeasure, CreateControlMeasureRequest>({
      query: (body) => ({
        url: '/hazardzone/control-measures',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['HazardType'],
    }),
    updateControlMeasure: builder.mutation<ControlMeasure, { id: string; body: UpdateControlMeasureRequest }>({
      query: ({ id, body }) => ({
        url: `/hazardzone/control-measures/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: ['HazardType'],
    }),
    deleteControlMeasure: builder.mutation<{ message: string; id: string }, string>({
      query: (id) => ({
        url: `/hazardzone/control-measures/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['HazardType'],
    }),

    // SIMOPS Incompatibility Rules & Zone Adjacencies (Student 4)
    getIncompatibilityRules: builder.query<IncompatibilityRule[], void>({
      query: () => '/hazardzone/incompatibility-rules',
      providesTags: ['IncompatibilityRule'],
    }),
    createIncompatibilityRule: builder.mutation<IncompatibilityRule, CreateIncompatibilityRuleRequest>({
      query: (body) => ({
        url: '/hazardzone/incompatibility-rules',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['IncompatibilityRule'],
    }),
    updateIncompatibilityRule: builder.mutation<IncompatibilityRule, { id: string; body: UpdateIncompatibilityRuleRequest }>({
      query: ({ id, body }) => ({
        url: `/hazardzone/incompatibility-rules/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: ['IncompatibilityRule'],
    }),
    deleteIncompatibilityRule: builder.mutation<{ message: string; id: string }, string>({
      query: (id) => ({
        url: `/hazardzone/incompatibility-rules/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['IncompatibilityRule'],
    }),

    getZoneAdjacencies: builder.query<ZoneAdjacency[], void>({
      query: () => '/hazardzone/zone-adjacencies',
      providesTags: ['Zone'],
    }),
    addZoneAdjacency: builder.mutation<{ message: string }, AddZoneAdjacencyRequest>({
      query: (body) => ({
        url: '/hazardzone/zone-adjacencies',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Zone'],
    }),
    removeZoneAdjacency: builder.mutation<{ message: string }, { zoneId: string; adjacentZoneId: string }>({
      query: ({ zoneId, adjacentZoneId }) => ({
        url: `/hazardzone/zone-adjacencies/${zoneId}/${adjacentZoneId}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Zone'],
    }),
  }),
});

export const {
  useLoginMutation,
  useRegisterMutation,
  useGetMeQuery,
  useGetPermitsQuery,
  useGetPermitByIdQuery,
  useCreatePermitDraftMutation,
  useUpdatePermitDraftMutation,
  useDeletePermitDraftMutation,
  useSubmitPermitForAiReviewMutation,
  useRecordDecisionMutation,
  useGetWorkersQuery,
  useGetWorkerByIdQuery,
  useCreateWorkerMutation,
  useUpdateWorkerMutation,
  useDeleteWorkerMutation,
  useAddWorkerCertificateMutation,
  useDeleteWorkerCertificateMutation,
  useGetContractorsQuery,
  useGetCertificateTypesQuery,
  useGetExpiryForecastQuery,
  useGetEquipmentQuery,
  useGetEquipmentByIdQuery,
  useCreateEquipmentMutation,
  useUpdateEquipmentMutation,
  useDeleteEquipmentMutation,
  useAddInspectionRecordMutation,
  useAddCalibrationRecordMutation,
  useGetIsolationPointsQuery,
  useCreateIsolationPointMutation,
  useUpdateIsolationStateMutation,
  useGetZonesQuery,
  useCheckZoneConflictsMutation,
  useGetSafetyAnalyticsQuery,
  useGetObservationsQuery,
  useGetObservationByIdQuery,
  useCreateObservationMutation,
  useUpdateObservationMutation,
  useDeleteObservationMutation,
  useGetHazardTypesQuery,
  useGetHazardTypeByIdQuery,
  useCreateHazardTypeMutation,
  useUpdateHazardTypeMutation,
  useDeleteHazardTypeMutation,
  useCreateControlMeasureMutation,
  useUpdateControlMeasureMutation,
  useDeleteControlMeasureMutation,
  useGetIncompatibilityRulesQuery,
  useCreateIncompatibilityRuleMutation,
  useUpdateIncompatibilityRuleMutation,
  useDeleteIncompatibilityRuleMutation,
  useGetZoneAdjacenciesQuery,
  useAddZoneAdjacencyMutation,
  useRemoveZoneAdjacencyMutation,
} = apiSlice;


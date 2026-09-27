import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export interface User {
  id: string;
  fullName: string;
  name?: string;
  email: string;
  role: string;
  contractorId: string | null;
  badgeNumber?: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
}

const initialUser: User = {
  id: 'usr-001',
  fullName: 'Mohammed Zakee',
  name: 'Mohammed Zakee',
  email: 'IT24104023@my.sliit.lk',
  role: 'Lead Safety Officer',
  contractorId: 'CTR-OFFSHORE-01',
  badgeNumber: 'BADGE-9901',
};

const initialState: AuthState = {
  user: initialUser,
  token: 'dev-session-token-clear-to-work-2026',
  isAuthenticated: true,
};

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{ user: User; token: string }>
    ) => {
      state.user = action.payload.user;
      state.token = action.payload.token;
      state.isAuthenticated = true;
    },
    logout: (state) => {
      state.user = null;
      state.token = null;
      state.isAuthenticated = false;
    },
  },
});

export const { setCredentials, logout } = authSlice.actions;
export default authSlice.reducer;

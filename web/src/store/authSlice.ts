import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import type { UserSession } from '../types';

interface AuthState {
  user: UserSession | null;
  token: string | null;
  isAuthenticated: boolean;
}

const savedToken = localStorage.getItem('ctw_token');
const savedUser = localStorage.getItem('ctw_user');

const initialState: AuthState = {
  token: savedToken,
  user: savedUser ? JSON.parse(savedUser) : null,
  isAuthenticated: !!savedToken,
};

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{ user: UserSession; token: string }>
    ) => {
      state.user = action.payload.user;
      state.token = action.payload.token;
      state.isAuthenticated = true;
      localStorage.setItem('ctw_token', action.payload.token);
      localStorage.setItem('ctw_user', JSON.stringify(action.payload.user));
    },
    logout: (state) => {
      state.user = null;
      state.token = null;
      state.isAuthenticated = false;
      localStorage.removeItem('ctw_token');
      localStorage.removeItem('ctw_user');
    },
  },
});

export const { setCredentials, logout } = authSlice.actions;
export default authSlice.reducer;

import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import type { UserSession } from '../types';

/**
 * Interface representing the global authentication state in Redux store.
 */
interface AuthState {
  /** Authenticated user profile data or null if logged out. */
  user: UserSession | null;
  /** JWT Bearer token issued by backend API or null. */
  token: string | null;
  /** Boolean flag indicating whether active session exists. */
  isAuthenticated: boolean;
}

// Retrieve persisted credentials from localStorage on initial app boot
const savedToken = localStorage.getItem('ctw_token');
const savedUser = localStorage.getItem('ctw_user');

const initialState: AuthState = {
  token: savedToken,
  user: savedUser ? JSON.parse(savedUser) : null,
  isAuthenticated: !!savedToken,
};

/**
 * Redux slice for managing user authentication, JWT tokens, and login/logout state.
 */
export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    /**
     * Stores user session and JWT token in Redux state and persists to browser localStorage.
     */
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
    /**
     * Clears authentication credentials from Redux state and removes tokens from localStorage.
     */
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

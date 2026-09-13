# ADR-001: React State Management Strategy

## Status
Accepted

## Context
The React web application serves Safety Officers and Site Administrators. Key requirements include:
- Consuming the authoritative ASP.NET Core REST API.
- Live polling or updating of asynchronous agent workflow execution traces.
- Complex tabular views (search, multi-column sort, filters, pagination).
- Role-based route guards and persistent authentication tokens.

Options considered:
1. **Redux Toolkit (RTK) with RTK Query**: Standardized slice pattern, built-in normalized caching, auto-refetching, request deduplication, and devtools.
2. **Zustand**: Minimalist and lightweight, but lacks built-in declarative server-state caching and deduplication out-of-the-box (requires TanStack Query combination).
3. **React Context API**: Native, but causes unnecessary re-renders in rapidly updating states (such as live agent execution step tracking).

## Decision
We choose **Redux Toolkit with RTK Query**. 
- RTK Query provides automatic request caching, tag-based invalidation (e.g., automatically refetching permit lists when a permit is approved), and loading/error states out of the box.
- Global slices cleanly separate UI state (sidebar, filters, modal dialogs) from auth session data.

## Consequences
- **Positive**: Eliminates boilerplate for fetching/caching; deterministic state transitions easily inspected via Redux DevTools during the viva; avoids over-fetching.
- **Negative**: Slight initial boilerplate compared to Zustand, mitigated by RTK Query's createApi.

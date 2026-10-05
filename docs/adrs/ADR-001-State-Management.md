# ADR-001: State Management Architecture in React and Flutter Clients

* **Status**: ACCEPTED  
* **Date**: 2026-09-12  
* **Deciders**: ClearToWork AI Engineering Team (Zakee, Chemini, Dinithi, Oshini)  
* **Technical Area**: Web & Mobile Client Architecture (SE3090 LO4)

---

## 1. Context & Problem Statement

ClearToWork AI requires client-side state synchronization across two platforms:
1. **React Web Application**: Requires global authentication state, active permit selection, multi-lingual localization toggles (EN / SI / TA), theme switching (Light / Dark), and live polling for AI clearance updates.
2. **Flutter Mobile Application**: Operates on offshore / refinery decks with intermittent connectivity, necessitating local camera QR state, offline permit draft caching, and real-time form validation.

The team needed to select justified, maintainable state management libraries for both platforms that prevent prop-drilling, guarantee deterministic re-renders, and separate UI from data fetching.

---

## 2. Options Considered

### React Web Client:
* **Option A: React Context API + Local `useState`**: Native to React, but lacks built-in caching, normalization, and automatic background re-fetching.
* **Option B: Redux Toolkit (RTK) + RTK Query**: Industry standard for enterprise React applications; provides centralized store, predictable reducer pipelines, and automated server cache management.
* **Option C: Zustand**: Lightweight and minimalist, but lacks out-of-the-box standardized query caching and API lifecycle middlewares.

### Flutter Mobile Client:
* **Option A: Native `setState`**: Simple for prototype screens, but leads to tightly coupled UI logic and unmaintainable state sharing across multi-step wizard screens.
* **Option B: Provider / Riverpod Pattern**: Recommended by Google; reactive dependency injection, testable ChangeNotifier state, and straightforward offline cache integration.
* **Option C: BLoC (Business Logic Component)**: Powerful reactive streams with RxDart, but introduces unnecessary boilerplate for the current mobile scope.

---

## 3. Decision

1. **React Web Application**: We selected **Redux Toolkit (RTK) with RTK Query** alongside React Context for lightweight visual themes (ThemeContext for Light/Dark and I18nContext for Trilingual localization).
2. **Flutter Mobile Application**: We selected the **Provider (ChangeNotifier) pattern**, isolating permit draft creation, GPS coordinate telemetry, and QR token decoding into dedicated service models.

---

## 4. Consequences & Trade-offs

### Positive Consequences:
* **RTK Query** automatically caches API responses from `cleartowork-backend`, eliminating redundant network requests and providing out-of-the-box loading, error, and refetch states.
* React Context cleanly separates presentation concerns (theme tokens and i18n dictionaries) from business entity state.
* **Flutter Provider** ensures predictable state updates and facilitates mock unit testing without instantiating platform widgets.

### Mitigations:
* Standardized slice naming conventions and typed hooks (`useAppDispatch`, `useAppSelector`) to enforce strict TypeScript safety.

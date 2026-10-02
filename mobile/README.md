# ClearToWork AI — Flutter Mobile Application (`mobile/`)

Mobile Field Operations application for **ClearToWork AI Industrial Permit-to-Work Safety System**. Built with **Flutter**, **Dart**, and **Provider**, directly consuming the shared **ASP.NET Core REST API backend** (`https://cleartowork-backend-h0pr.onrender.com/api`).

---

## 📱 Features & Assignment Specification Alignment

* **Shared ASP.NET Core REST API Integration**: Consumes `/api/Auth`, `/api/Permits`, `/api/Workforce`, `/api/Equipment`, and `/api/HazardZone`.
* **JWT Authentication & Demo Role Quick Select**: Persistent login state with SharedPreferences and one-click role switching for **HSE Safety Officer**, **Contractor Supervisor**, **Area Supervisor**, and **System Administrator**.
* **Device Features (Camera & QR Scanner)**:
  * **Digital Worker Badge QR Scanner**: Scans worker badges to audit OPITO/CompEx certifications on-site.
  * **LOTO Lockout Tag Scanner**: Scans physical lock tags and verifies zero-energy isolation.
* **Gas Telemetry Logger**: Allows field technicians to log H2S (ppm), LEL (%), and O2 (%) atmospheric readings with instant threshold violation alerts.
* **LangGraph 5-Agent Telemetry**: View 5-agent LangGraph workflow execution step traces and trigger real-time AI clearance checks directly from the mobile app.

---

## 👥 Student Component Breakdown (Mobile App)

| Student Member | Mobile Module / Feature | Technical Implementation |
| :--- | :--- | :--- |
| **Student 1 (Mohammed Zakee)** | Worker Profile & Digital Badge QR Scanner | `lib/screens/qr_scanner_screen.dart` |
| **Student 2 (Chemini Perera)** | Gas Reading Logger & Atmospheric Telemetry | `lib/screens/gas_monitor_screen.dart` |
| **Student 3 (Dinithi Silva)** | Permits Register & On-Site Digital Authorization | `lib/screens/permit_list_screen.dart` |
| **Student 4 (Oshini)** | Equipment Readiness & AI Agent Clearance Status | `lib/screens/permit_detail_screen.dart` |

---

## 🛠️ How to Import into Google Project IDX / Android Studio / VS Code

### Option A: Importing in Google Project IDX (Browser IDE)
1. Open [Project IDX](https://idx.dev/) in your browser.
2. Click **Import Repository** and enter your GitHub repository URL:
   `https://github.com/IT24104023/ClearToWork.git`
3. Select **Flutter** as the workspace template.
4. When IDX launches, open a terminal in the `mobile/` directory:
   ```bash
   cd mobile
   flutter pub get
   flutter run -d web-server --web-port 8080
   ```

### Option B: Building Android APK locally with Flutter CLI
```bash
cd mobile
flutter pub get
flutter build apk --release
```
The compiled Android APK will be generated at:
`mobile/build/app/outputs/flutter-apk/app-release.apk`

---

## 🤖 Prompts for AI Tools (Google Project IDX / Cursor / Gemini)

If you use Google Project IDX AI, Cursor, or Gemini in Android Studio, use these exact prompts:

### Prompt 1: Building and Running the Flutter Mobile App
```text
I have imported the ClearToWork AI repository containing the Flutter app in the `mobile/` folder.
Please analyze `mobile/pubspec.yaml` and `mobile/lib/main.dart`.
Run `flutter pub get` in `mobile/` and start the app on a web preview or Android emulator.
Ensure it connects to the live ASP.NET Core API at https://cleartowork-backend-h0pr.onrender.com/api.
```

### Prompt 2: Generating the Android APK File
```text
Please build a release Android APK for the ClearToWork AI Flutter mobile application located in `mobile/`.
Execute `flutter build apk --release` and provide the absolute path to the generated app-release.apk file.
```

### Prompt 3: Adding a New Device Feature (Camera / GPS)
```text
In the ClearToWork AI Flutter app (`mobile/lib/`), add a device GPS location verification widget using geolocator to verify that the mobile user is physically located inside Zone A1 (Lat: 24.5000, Lon: 54.3000) before allowing on-site permit activation.
```

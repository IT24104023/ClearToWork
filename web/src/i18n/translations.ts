export type Language = 'en' | 'si' | 'ta';

export interface Translations {
  // Navigation
  nav_home: string;
  nav_platform: string;
  nav_about: string;
  nav_contact: string;
  nav_register: string;
  nav_dashboard: string;
  nav_permits: string;
  nav_workforce: string;
  nav_equipment: string;
  nav_hazard_rules: string;
  nav_analytics: string;
  nav_agent_command: string;
  nav_database_admin: string;
  nav_profile: string;
  nav_documentation: string;
  nav_logout: string;
  nav_login: string;


  // Header & User
  header_title: string;
  header_subtitle: string;
  role_supervisor: string;
  role_safety_officer: string;
  role_admin: string;
  role_area_supervisor: string;

  // Common Actions & Statuses
  theme_toggle: string;
  theme_dark: string;
  theme_light: string;
  status_draft: string;
  status_submitted: string;
  status_ai_review: string;
  status_pending_approval: string;
  status_approved: string;
  status_active: string;
  status_closed: string;
  status_refused: string;
  status_expired: string;

  // Agent Command Center & QChat
  agent_center_title: string;
  agent_center_subtitle: string;
  agent_status_online: string;
  agent_status_processing: string;
  agent_kpi_total_runs: string;
  agent_kpi_safe_failures: string;
  agent_kpi_clear_permits: string;
  agent_kpi_avg_latency: string;
  agent_active_pipeline: string;
  agent_step_planning: string;
  agent_step_competency: string;
  agent_step_equipment: string;
  agent_step_hazard: string;
  agent_step_validation: string;
  qchat_title: string;
  qchat_placeholder: string;
  qchat_send: string;
  qchat_simulation_notice: string;
  qchat_remediation: string;

  // Database Admin
  db_admin_title: string;
  db_admin_subtitle: string;
  db_table_name: string;
  db_record_count: string;
  db_action_seed: string;
  db_action_reset: string;
  db_status_healthy: string;
  db_confirm_reset: string;

  // Profile & Permissions
  profile_title: string;
  profile_subtitle: string;
  profile_full_name: string;
  profile_email: string;
  profile_role: string;
  profile_phone: string;
  profile_department: string;
  profile_bio: string;
  profile_avatar_url: string;
  profile_choose_avatar: string;
  profile_save_changes: string;
  profile_permissions_title: string;
  profile_permissions_desc: string;

  // Documentation
  docs_title: string;
  docs_subtitle: string;
  docs_tab_architecture: string;
  docs_tab_agents: string;
  docs_tab_security: string;
  docs_tab_database: string;
}

export const translations: Record<Language, Translations> = {
  en: {
    nav_home: 'Home',
    nav_platform: 'Platform & Agents',
    nav_about: 'About Us',
    nav_contact: 'Contact & Ops',
    nav_register: 'Register',
    nav_dashboard: 'Dashboard',
    nav_permits: 'Safety Permits',

    nav_workforce: 'Workforce & Certs',
    nav_equipment: 'Safety Equipment',
    nav_hazard_rules: 'Hazard Zones & SIMOPS',
    nav_analytics: 'Safety Analytics',
    nav_agent_command: 'QChat & Agent Command',
    nav_database_admin: 'Database Management',
    nav_profile: 'User Profile & Security',
    nav_documentation: 'Technical Documentation',
    nav_logout: 'Sign Out',
    nav_login: 'Sign In',

    header_title: 'ClearToWork AI',
    header_subtitle: 'Industrial Permit-to-Work & Agentic Clearance Engine',
    role_supervisor: 'Contractor Supervisor',
    role_safety_officer: 'HSE Safety Officer',
    role_admin: 'System Administrator',
    role_area_supervisor: 'Area Operations Supervisor',

    theme_toggle: 'Toggle Color Theme',
    theme_dark: 'Dark Mode',
    theme_light: 'Light Mode',
    status_draft: 'Draft',
    status_submitted: 'Submitted',
    status_ai_review: 'AI Multi-Agent Review',
    status_pending_approval: 'Pending Human Sign-off',
    status_approved: 'Approved',
    status_active: 'Active On-Site',
    status_closed: 'Closed Out',
    status_refused: 'Refused (Safe Failure)',
    status_expired: 'Expired',

    agent_center_title: 'QChat & Agent Command Center',
    agent_center_subtitle: 'Real-time observability, pipeline tracking, and interactive safety reasoning',
    agent_status_online: 'ONLINE',
    agent_status_processing: 'EVALUATING',
    agent_kpi_total_runs: 'Total Agent Evaluations',
    agent_kpi_safe_failures: 'Safe Failures Caught',
    agent_kpi_clear_permits: 'Clear Clearances',
    agent_kpi_avg_latency: 'Mean Pipeline Latency',
    agent_active_pipeline: 'LangGraph 5-Agent Execution Pipeline',
    agent_step_planning: 'Planning & Coordination',
    agent_step_competency: 'Personnel & Competency',
    agent_step_equipment: 'Resource & Isolation',
    agent_step_hazard: 'Site Conditions & SIMOPS',
    agent_step_validation: 'Validation & Safety Guardrail',
    qchat_title: 'QChat Multi-Agent Safety Assistant',
    qchat_placeholder: 'Ask the safety agents a clearance query or simulate a permit condition...',
    qchat_send: 'Run Simulation',
    qchat_simulation_notice: 'Queries are evaluated dynamically through the live LangGraph multi-agent DAG with fail-safe verification.',
    qchat_remediation: 'Automated Remediation Fix',

    db_admin_title: 'Database Management & Schema Control',
    db_admin_subtitle: 'Inspect relational entity tables, review storage metrics, seed test environments, or trigger schema resets.',
    db_table_name: 'Database Table / Entity',
    db_record_count: 'Record Count',
    db_action_seed: 'Re-Seed Benchmark Data',
    db_action_reset: 'Reset & Recreate Schema',
    db_status_healthy: 'Database Status: Operational & Verified',
    db_confirm_reset: 'Are you sure you want to recreate the database? All tables will be refreshed with seed data.',

    profile_title: 'User Profile & Access Control',
    profile_subtitle: 'Manage your credentials, safety profile photo, and inspect role-based operational permissions.',
    profile_full_name: 'Full Legal Name',
    profile_email: 'Corporate Email Address',
    profile_role: 'Assigned System Role',
    profile_phone: 'Emergency Contact Number',
    profile_department: 'Operational Department',
    profile_bio: 'Professional Bio & Safety Qualifications',
    profile_avatar_url: 'Custom Profile Picture URL',
    profile_choose_avatar: 'Select Preset Avatar',
    profile_save_changes: 'Update Safety Profile',
    profile_permissions_title: 'Active Role-Based Permissions (RBAC)',
    profile_permissions_desc: 'These cryptographic permission claims are embedded in your JWT token and enforced across all API endpoints.',

    docs_title: 'ClearToWork AI Technical Documentation',
    docs_subtitle: 'Complete architectural specification, multi-agent workflows, security boundaries, and relational schema.',
    docs_tab_architecture: 'System Architecture',
    docs_tab_agents: 'Agentic Workflow (LangGraph)',
    docs_tab_security: 'Security & Auth Matrix',
    docs_tab_database: 'Database Schema & ERD'
  },
  si: {
    nav_home: 'මුල් පිටුව',
    nav_platform: 'වේදිකාව සහ නියෝජිතයන්',
    nav_about: 'අප ගැන',
    nav_contact: 'සම්බන්ධ වන්න',
    nav_register: 'ලියාපදිංචි වන්න',
    nav_dashboard: 'පාලක පුවරුව',
    nav_permits: 'ආරක්ෂිත බලපත්‍ර',

    nav_workforce: 'සේවක මණ්ඩලය සහ සහතික',
    nav_equipment: 'ආරක්ෂිත උපකරණ',
    nav_hazard_rules: 'අන්තරායකර කලාප සහ SIMOPS',
    nav_analytics: 'ආරක්ෂිත විශ්ලේෂණ',
    nav_agent_command: 'QChat සහ නියෝජිත විධාන මධ්‍යස්ථානය',
    nav_database_admin: 'දත්ත සමුදාය කළමනාකරණය',
    nav_profile: 'පරිශීලක පැතිකඩ සහ ආරක්ෂාව',
    nav_documentation: 'තාක්ෂණික ලියකියවිලි',
    nav_logout: 'ඉවත් වන්න',
    nav_login: 'ඇතුළු වන්න',

    header_title: 'ක්ලියර්ටුවර්ක් AI (ClearToWork AI)',
    header_subtitle: 'කාර්මික වැඩ බලපත්‍ර සහ නියෝජිත ආරක්ෂණ පද්ධතිය',
    role_supervisor: 'කොන්ත්‍රාත්කරු අධීක්ෂක',
    role_safety_officer: 'HSE ආරක්ෂක නිලධාරී',
    role_admin: 'පද්ධති පරිපාලක',
    role_area_supervisor: 'ක්ෂේත්‍ර මෙහෙයුම් අධීක්ෂක',

    theme_toggle: 'වර්ණ තේමාව මාරු කරන්න',
    theme_dark: 'අඳුරු මාදිලිය (Dark)',
    theme_light: 'ආලෝක මාදිලිය (Light)',
    status_draft: 'කෙටුම්පත',
    status_submitted: 'ඉදිරිපත් කරන ලදි',
    status_ai_review: 'AI නියෝජිත සමාලෝචනය',
    status_pending_approval: 'අනුමැතිය අපේක්ෂාවෙන්',
    status_approved: 'අනුමතයි',
    status_active: 'ක්‍රියාත්මකයි',
    status_closed: 'අවසන් කරන ලදි',
    status_refused: 'ප්‍රතික්ෂේප විය (ආරක්ෂිත අසමත් වීම)',
    status_expired: 'කල් ඉකුත් විය',

    agent_center_title: 'QChat සහ නියෝජිත විධාන මධ්‍යස්ථානය',
    agent_center_subtitle: 'තථ්‍ය කාලීන නිරීක්ෂණය, ක්‍රියාවලි ලුහුබැඳීම සහ බුද්ධිමත් ආරක්ෂණ තීරණ',
    agent_status_online: 'සක්‍රියයි',
    agent_status_processing: 'තක්සේරු කරමින් පවතී',
    agent_kpi_total_runs: 'මුළු නියෝජිත තක්සේරු',
    agent_kpi_safe_failures: 'හඳුනාගත් ආරක්ෂිත අසමත් වීම්',
    agent_kpi_clear_permits: 'සම්පූර්ණ අනුමැතීන්',
    agent_kpi_avg_latency: 'සාමාන්‍ය ප්‍රතිචාර කාලය',
    agent_active_pipeline: 'LangGraph නියෝජිතයන් 5 දෙනාගේ ක්‍රියාවලි පද්ධතිය',
    agent_step_planning: 'සැලසුම් සහ සම්බන්ධීකරණය',
    agent_step_competency: 'සේවක සහතික සහ නිපුණතාවය',
    agent_step_equipment: 'උපකරණ සහ හුදකලා කිරීම්',
    agent_step_hazard: 'ක්ෂේත්‍ර අවදානම් සහ SIMOPS',
    agent_step_validation: 'අවසාන ආරක්ෂිත තහවුරු කිරීම',
    qchat_title: 'QChat බහු-නියෝජිත ආරක්ෂණ සහයක',
    qchat_placeholder: 'ආරක්ෂණ විමසුමක් හෝ වැඩ බලපත්‍රයක් අනුකරණය කිරීමට මෙහි ලියන්න...',
    qchat_send: 'අනුකරණය ධාවනය කරන්න',
    qchat_simulation_notice: 'විමසුම් සෘජුවම LangGraph බහු-නියෝජිත පද්ධතිය හරහා පරීක්ෂා කර ආරක්ෂිත පිළිතුරු සපයයි.',
    qchat_remediation: 'ස්වයංක්‍රීය විසඳුම් සහ නිර්දේශ',

    db_admin_title: 'දත්ත සමුදාය සහ ව්‍යුහ පාලනය',
    db_admin_subtitle: 'වගු වාර්තා පරීක්ෂා කිරීම, ආදර්ශක දත්ත ඇතුළත් කිරීම හෝ දත්ත සමුදාය නැවත සැකසීම.',
    db_table_name: 'දත්ත වගුව',
    db_record_count: 'වාර්තා ගණන',
    db_action_seed: 'ආදර්ශක දත්ත ඇතුළත් කරන්න',
    db_action_reset: 'දත්ත සමුදාය නැවත සකසන්න',
    db_status_healthy: 'දත්ත සමුදා තත්වය: සක්‍රිය සහ සත්‍යාපිතයි',
    db_confirm_reset: 'ඔබට මෙම දත්ත සමුදාය මුල සිට නැවත සැකසීමට අවශ්‍ය බව සහතිකද?',

    profile_title: 'පරිශීලක පැතිකඩ සහ ආරක්ෂාව',
    profile_subtitle: 'ඔබගේ තොරතුරු, පැතිකඩ ඡායාරූපය සහ පද්ධති අවසර පරීක්ෂා කරන්න.',
    profile_full_name: 'සම්පූර්ණ නම',
    profile_email: 'විද්‍යුත් තැපෑල',
    profile_role: 'පවරා ඇති කාර්යභාරය',
    profile_phone: 'හදිසි ඇමතුම් අංකය',
    profile_department: 'දෙපාර්තමේන්තුව',
    profile_bio: 'වෘත්තීය විස්තරය සහ සුදුසුකම්',
    profile_avatar_url: 'පැතිකඩ පින්තූර සබැඳිය (URL)',
    profile_choose_avatar: 'පෙරනිමි අවතාරයක් තෝරන්න',
    profile_save_changes: 'තොරතුරු යාවත්කාලීන කරන්න',
    profile_permissions_title: 'භූමිකාව පදනම් කරගත් අවසර (RBAC)',
    profile_permissions_desc: 'මෙම අවසර ඔබගේ JWT ටෝකනය සමඟ සංකේතනය කර ඇති අතර සියලුම API මඟින් බලාත්මක කරනු ලැබේ.',

    docs_title: 'ClearToWork AI තාක්ෂණික ලියකියවිලි',
    docs_subtitle: 'පද්ධති ගෘහ නිර්මාණ ශිල්පය, නියෝජිත ක්‍රියාවලි සහ දත්ත සමුදා ව්‍යුහය.',
    docs_tab_architecture: 'පද්ධති ගෘහ නිර්මාණ ශිල්පය',
    docs_tab_agents: 'නියෝජිත ක්‍රියාවලිය (LangGraph)',
    docs_tab_security: 'ආරක්ෂාව සහ අවසර',
    docs_tab_database: 'දත්ත සමුදා ER සටහන'
  },
  ta: {
    nav_home: 'முகப்பு',
    nav_platform: 'தளம் & முகவர்கள்',
    nav_about: 'எங்களை பற்றி',
    nav_contact: 'தொடர்பு கொள்ள',
    nav_register: 'பதிவு செய்க',
    nav_dashboard: 'டாஷ்போர்டு',
    nav_permits: 'பாதுகாப்பு அனுமதிகள்',

    nav_workforce: 'பணியாளர்கள் & சான்றிதழ்கள்',
    nav_equipment: 'பாதுகாப்பு உபகரணங்கள்',
    nav_hazard_rules: 'ஆபத்து மண்டலங்கள் & SIMOPS',
    nav_analytics: 'பாதுகாப்பு பகுப்பாய்வு',
    nav_agent_command: 'QChat & முகவர் கட்டளை மையம்',
    nav_database_admin: 'தரவுத்தள மேலாண்மை',
    nav_profile: 'பயனர் சுயவிவரம் & பாதுகாப்பு',
    nav_documentation: 'தொழில்நுட்ப ஆவணங்கள்',
    nav_logout: 'வெளியேறு',
    nav_login: 'உள்நுழைக',

    header_title: 'க்ளியர்டூவர்க் AI (ClearToWork AI)',
    header_subtitle: 'தொழில்துறை பணி அனுமதி & முகவர் பாதுகாப்பு அனுமதி இயந்திரம்',
    role_supervisor: 'ஒப்பந்த மேற்பார்வையாளர்',
    role_safety_officer: 'HSE பாதுகாப்பு அதிகாரி',
    role_admin: 'கணினி நிர்வாகி',
    role_area_supervisor: 'பகுதி செயல்பாட்டு மேற்பார்வையாளர்',

    theme_toggle: 'வண்ண தீமை மாற்றவும்',
    theme_dark: 'இருண்ட பயன்முறை (Dark)',
    theme_light: 'ஒளி பயன்முறை (Light)',
    status_draft: 'வரைவு',
    status_submitted: 'சமர்ப்பிக்கப்பட்டது',
    status_ai_review: 'AI முகவர் மதிப்பாய்வு',
    status_pending_approval: 'ஒப்புதலுக்காக காத்திருக்கிறது',
    status_approved: 'அங்கீகரிக்கப்பட்டது',
    status_active: 'தளத்தில் செயலில் உள்ளது',
    status_closed: 'மூடப்பட்டது',
    status_refused: 'நிராகரிக்கப்பட்டது (பாதுகாப்பான தோல்வி)',
    status_expired: 'காலாவதியானது',

    agent_center_title: 'QChat & முகவர் கட்டளை மையம்',
    agent_center_subtitle: 'நிகழ்நேர கண்காணிப்பு, செயல்முறை கண்காணிப்பு மற்றும் புத்திசாலித்தனமான பாதுகாப்பு பகுத்தறிவு',
    agent_status_online: 'செயலில் உள்ளது',
    agent_status_processing: 'மதிப்பிடுகிறது',
    agent_kpi_total_runs: 'மொத்த முகவர் மதிப்பீடுகள்',
    agent_kpi_safe_failures: 'கண்டறியப்பட்ட பாதுகாப்பான தோல்விகள்',
    agent_kpi_clear_permits: 'முழு அனுமதிகள்',
    agent_kpi_avg_latency: 'சராசரி மறுமொழி நேரம்',
    agent_active_pipeline: 'LangGraph 5-முகவர் செயல்படுத்தும் பைப்லைன்',
    agent_step_planning: 'திட்டமிடல் & ஒருங்கிணைப்பு',
    agent_step_competency: 'பணியாளர் சான்றிதழ்கள் & தகுதி',
    agent_step_equipment: 'உபகரணங்கள் & தனிமைப்படுத்தல்',
    agent_step_hazard: 'தள அபாயங்கள் & SIMOPS',
    agent_step_validation: 'இறுதி பாதுகாப்பு சரிபார்ப்பு',
    qchat_title: 'QChat பல முகவர் பாதுகாப்பு உதவியாளர்',
    qchat_placeholder: 'பாதுகாப்பு வினவல் அல்லது பணி அனுமதி உருவகப்படுத்துதலை இங்கே உள்ளிடவும்...',
    qchat_send: 'உருவகப்படுத்துதலை இயக்கவும்',
    qchat_simulation_notice: 'வினவல்கள் நேரடியாக நேரடி LangGraph பல முகவர் DAG வழியாக தோல்வி-பாதுகாப்புடன் மதிப்பிடப்படுகின்றன.',
    qchat_remediation: 'தானியங்கி தீர்வு & பரிந்துரை',

    db_admin_title: 'தரவுத்தள மேலாண்மை & அமைப்பு கட்டுப்பாடு',
    db_admin_subtitle: 'அட்டவணை பதிவுகளை ஆய்வு செய்தல், மாதிரி தரவை உள்ளிடுதல் அல்லது தரவுத்தளத்தை மீட்டமைத்தல்.',
    db_table_name: 'தரவுத்தள அட்டவணை',
    db_record_count: 'பதிவுகளின் எண்ணிக்கை',
    db_action_seed: 'மாதிரி தரவை மீண்டும் உள்ளிடவும்',
    db_action_reset: 'தரவுத்தளத்தை மீட்டமைக்கவும்',
    db_status_healthy: 'தரவுத்தள நிலை: செயலில் மற்றும் சரிபார்க்கப்பட்டது',
    db_confirm_reset: 'இந்த தரவுத்தளத்தை மீண்டும் புதிதாக உருவாக்க விரும்புகிறீர்களா?',

    profile_title: 'பயனர் சுயவிவரம் & பாதுகாப்பு அணுகல்',
    profile_subtitle: 'உங்கள் விவரங்கள், சுயவிவரப் படம் மற்றும் பங்கு அடிப்படையிலான அனுமதிகளை நிர்வகிக்கவும்.',
    profile_full_name: 'முழு பெயர்',
    profile_email: 'மின்னஞ்சல் முகவரி',
    profile_role: 'ஒதுக்கப்பட்ட பங்கு',
    profile_phone: 'அவசர தொடர்பு எண்',
    profile_department: 'துறை',
    profile_bio: 'தொழில்முறை சுயவிவரம் & தகுதிகள்',
    profile_avatar_url: 'சுயவிவரப் பட இணைப்பு (URL)',
    profile_choose_avatar: 'முன்னமைக்கப்பட்ட அவதாரத்தைத் தேர்ந்தெடுக்கவும்',
    profile_save_changes: 'சுயவிவரத்தைப் புதுப்பிக்கவும்',
    profile_permissions_title: 'பங்கு அடிப்படையிலான அனுமதிகள் (RBAC)',
    profile_permissions_desc: 'இந்த அனுமதிகள் உங்கள் JWT டோக்கனுடன் குறியாக்கம் செய்யப்பட்டு அனைத்து APIகளாலும் செயல்படுத்தப்படுகின்றன.',

    docs_title: 'ClearToWork AI தொழில்நுட்ப ஆவணங்கள்',
    docs_subtitle: 'கணினி கட்டமைப்பு விவரக்குறிப்பு, முகவர் பணிப்பாய்வு மற்றும் தரவுத்தள அமைப்பு.',
    docs_tab_architecture: 'கணினி கட்டமைப்பு',
    docs_tab_agents: 'முகவர் பணிப்பாய்வு (LangGraph)',
    docs_tab_security: 'பாதுகாப்பு & அங்கீகாரம்',
    docs_tab_database: 'தரவுத்தள ER வரைபடம்'
  }
};

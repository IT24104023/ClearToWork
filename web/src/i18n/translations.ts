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

  // Homepage Specific Keys
  home_badge: string;
  home_hero_title_1: string;
  home_hero_title_2: string;
  home_hero_subtitle: string;
  home_btn_launch: string;
  home_btn_register: string;
  home_pixel_compare: string;
  home_legacy_title: string;
  home_legacy_heading: string;
  home_legacy_desc: string;
  home_legacy_tag1: string;
  home_legacy_tag2: string;
  home_ai_title: string;
  home_ai_heading: string;
  home_ai_desc: string;
  home_ai_tag1: string;
  home_ai_tag2: string;
  home_stat_fail_closed: string;
  home_stat_agents: string;
  home_stat_latency: string;
  home_stat_languages: string;
  home_agents_title: string;
  home_agents_heading: string;
  home_agents_desc: string;
  home_agent_1_title: string;
  home_agent_1_desc: string;
  home_agent_2_title: string;
  home_agent_2_desc: string;
  home_agent_3_title: string;
  home_agent_3_desc: string;
  home_agent_4_title: string;
  home_agent_4_desc: string;
  home_agent_5_title: string;
  home_agent_5_desc: string;
  home_cta_heading: string;
  home_cta_desc: string;
  home_cta_signin: string;
  home_cta_about: string;
  home_footer_desc: string;
  home_footer_copy: string;

  // About Page Specific Keys
  about_badge: string;
  about_title: string;
  about_subtitle: string;
  about_team_badge: string;
  about_team_heading: string;
  about_team_desc: string;
  about_team_stat: string;
  about_problem_badge: string;
  about_problem_heading: string;
  about_problem_desc: string;
  about_problem_1_title: string;
  about_problem_1_desc: string;
  about_problem_2_title: string;
  about_problem_2_desc: string;
  about_problem_3_title: string;
  about_problem_3_desc: string;
  about_solution_badge: string;
  about_solution_heading: string;
  about_solution_desc: string;
  about_ownership_badge: string;
  about_ownership_heading: string;
  about_ownership_desc: string;
  about_s1_title: string;
  about_s1_desc: string;
  about_s2_title: string;
  about_s2_desc: string;
  about_s3_title: string;
  about_s3_desc: string;
  about_s4_title: string;
  about_s4_desc: string;

  // Contact Page Specific Keys
  contact_badge: string;
  contact_title: string;
  contact_subtitle: string;
  contact_emergency_title: string;
  contact_emergency_desc: string;
  contact_form_title: string;
  contact_form_desc: string;
  contact_field_name: string;
  contact_field_email: string;
  contact_field_facility: string;
  contact_field_subject: string;
  contact_field_message: string;
  contact_btn_submit: string;
  contact_success_title: string;
  contact_success_desc: string;
  contact_hq_title: string;
  contact_hq_address: string;
  contact_response_title: string;
  contact_response_desc: string;

  // Register Page Specific Keys
  reg_title: string;
  reg_subtitle: string;
  reg_name: string;
  reg_email: string;
  reg_role: string;
  reg_password: string;
  reg_confirm_password: string;
  reg_submit: string;
  reg_have_account: string;
  reg_signin: string;

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

  // Workforce Domain (Student 1)
  workforce_title: string;
  workforce_subtitle: string;
  workforce_forecast_title: string;
  workforce_forecast_desc: string;
  workforce_forecast_valid_all: string;
  workforce_search_placeholder: string;
  workforce_th_name: string;
  workforce_th_badge: string;
  workforce_th_cert: string;
  workforce_th_expiry: string;
  workforce_th_remaining: string;
  workforce_th_status: string;
  workforce_accredited_certs: string;
  workforce_no_certs: string;
  workforce_status_valid: string;
  workforce_status_expired: string;
  workforce_status_expiring: string;

  // Equipment & LOTO Domain (Student 2)
  equipment_title: string;
  equipment_subtitle: string;
  equipment_loto_title: string;
  equipment_loto_desc: string;
  equipment_search_placeholder: string;
  equipment_th_tag: string;
  equipment_th_name: string;
  equipment_th_category: string;
  equipment_th_inspection: string;
  equipment_th_calibration: string;
  equipment_th_readiness: string;
  equipment_status_cleared: string;
  equipment_status_restricted: string;
  equipment_status_indate: string;
  equipment_status_overdue: string;
  equipment_status_certified: string;

  // Hazard Rules & SIMOPS Domain (Student 4)
  hazard_title: string;
  hazard_subtitle: string;
  hazard_simops_title: string;
  hazard_simops_desc: string;
  hazard_primary: string;
  hazard_conflicting: string;
  hazard_weather_title: string;
  hazard_zones_title: string;
  hazard_adjacent_transfer: string;

  // Safety Analytics & Refusal Audit
  analytics_title: string;
  analytics_subtitle: string;
  analytics_kpi_approved: string;
  analytics_kpi_approved_sub: string;
  analytics_kpi_refused: string;
  analytics_kpi_refused_sub: string;
  analytics_kpi_active: string;
  analytics_kpi_active_sub: string;
  analytics_kpi_mtta: string;
  analytics_kpi_mtta_sub: string;
  analytics_causes_title: string;
  analytics_causes_subtitle: string;
  analytics_zone_dist_title: string;
  analytics_zone_dist_subtitle: string;

  // Permit Dossier & Detail
  permit_list_title: string;
  permit_list_subtitle: string;
  permit_new_btn: string;
  permit_filter_all: string;
  permit_filter_pending: string;
  permit_filter_approved: string;
  permit_filter_refused: string;
  permit_detail_back: string;
  permit_detail_run_ai: string;
  permit_detail_signoff: string;
  permit_assigned_personnel: string;
  permit_assigned_equipment: string;
  permit_no_workers: string;
  permit_no_equipment: string;
  permit_timeline_title: string;
  permit_timeline_latency: string;
  permit_remediation_title: string;
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

    // Homepage
    home_badge: 'Autonomous Industrial Safety Clearance Engine',
    home_hero_title_1: 'Zero Incidents.',
    home_hero_title_2: 'Multi-Agent AI Clearance.',
    home_hero_subtitle: 'Eliminate simultaneous hazardous clashes (SIMOPS), uncalibrated safety gear, and unqualified crews across petrochemical plants and refineries with deterministic LangGraph multi-agent evaluation.',
    home_btn_launch: 'Launch Safety Portal',
    home_btn_register: 'Register New Contractor',
    home_pixel_compare: 'Hover or Click to Compare Paradigm Shift',
    home_legacy_title: 'Legacy Vulnerabilities (Manual PTW)',
    home_legacy_heading: 'Paper Permits & Spreadsheet Blindspots',
    home_legacy_desc: 'Manual paperwork leads to simultaneous incompatible hot work in adjacent solvent zones, overdue gas monitor calibrations, and expired welder credentials.',
    home_legacy_tag1: '❌ High SIMOPS Explosion Risk',
    home_legacy_tag2: '❌ 3-4 Hour Manual Review Delay',
    home_ai_title: 'ClearToWork AI Autonomous Clearance',
    home_ai_heading: '5-Agent Fail-Closed Clearance Engine',
    home_ai_desc: 'Instant spatial-temporal conflict checking, 90-day equipment calibration audit, certified worker replacement synthesis, and live 35km/h wind gust guardrails.',
    home_ai_tag1: '✓ <80ms LangGraph Consensus',
    home_ai_tag2: '✓ Fail-Safe Remediation Guidance',
    home_stat_fail_closed: 'Fail-Closed Safety',
    home_stat_agents: 'LangGraph Agents',
    home_stat_latency: 'Evaluation Latency',
    home_stat_languages: 'Native Languages (EN/SI/TA)',
    home_agents_title: 'Autonomous Safety Consensus',
    home_agents_heading: '5 Specialized Agent Nodes in Directed Graph',
    home_agents_desc: 'Each agent executes deterministic domain-specific verification rules over a strongly-typed shared state model before human sign-off.',
    home_agent_1_title: 'Planning & Coordination',
    home_agent_1_desc: 'Decomposes permit objectives, enforces hazard duration ceilings, and mandates fire watches for hot work operations.',
    home_agent_2_title: 'Personnel & Competency',
    home_agent_2_desc: 'Audits worker badges, verifies trade qualifications, forecasts 30-day credential expiries, and auto-recommends certified replacements.',
    home_agent_3_title: 'Resource & Isolation',
    home_agent_3_desc: 'Verifies equipment safety inspection readiness, flags 90-day gas monitor calibration tags, and validates physical LOTO isolation.',
    home_agent_4_title: 'Site Conditions & Hazard',
    home_agent_4_desc: 'Queries adjacent zone permits for SIMOPS clashes, and checks live Open-Meteo weather forecasts to enforce a 35 km/h wind gust ceiling.',
    home_agent_5_title: 'Validation & Safety Guardrail',
    home_agent_5_desc: 'Enforces authoritative fail-closed consensus. If any safety violation is detected across any preceding node, marks REFUSED_SAFE_FAILURE and synthesizes an automated remediation package.',
    home_cta_heading: 'Ready to Upgrade Industrial Safety Clearance?',
    home_cta_desc: 'Sign in to experience the live clearance pipeline or explore our comprehensive system architecture documentation.',
    home_cta_signin: 'Sign In to Safety Portal',
    home_cta_about: 'About the Platform →',
    home_footer_desc: 'Autonomous Safety Clearance & SIMOPS Engine',
    home_footer_copy: '© 2026 ClearToWork AI · Fail-Closed Industrial Safety Standard',

    // About Page
    about_badge: 'Industrial Safety Engineering',
    about_title: 'About ClearToWork AI',
    about_subtitle: 'Pioneering autonomous, multi-agent Permit-to-Work (PTW) verification and real-time SIMOPS safety clearance for high-hazard industrial environments.',
    about_team_badge: 'ClearToWork AI Core Engineering Team',
    about_team_heading: 'Integrated Full-Stack & Agentic AI Team',
    about_team_desc: 'Engineering next-generation deterministic industrial safety clearance, SIMOPS spatial conflict engines, and fail-closed multi-agent consensus.',
    about_team_stat: '4 Engineers · 5 Agents',
    about_problem_badge: 'The Industrial Problem',
    about_problem_heading: 'Why Legacy PTW Systems Fail',
    about_problem_desc: 'In high-hazard facilities like refineries and chemical plants, catastrophic incidents routinely occur due to three root causes:',
    about_problem_1_title: 'SIMOPS Clashes:',
    about_problem_1_desc: 'Simultaneous incompatible activities (e.g. Hot welding next to volatile solvent lines) going unnoticed in separate paper permits.',
    about_problem_2_title: 'Uncalibrated Gear:',
    about_problem_2_desc: 'Expired 90-day multi-gas monitors and extinguishers passing physical inspections without cryptographic verification.',
    about_problem_3_title: 'Unqualified Crews:',
    about_problem_3_desc: 'Expired trade certifications for high-risk enclosed welding or confined space entries.',
    about_solution_badge: 'The Autonomous Solution',
    about_solution_heading: 'Deterministic 5-Agent Pipeline',
    about_solution_desc: 'ClearToWork AI executes a Directed Acyclic Graph (DAG) with five autonomous agents in LangGraph that enforce a Fail-Closed Safety Policy.',
    about_ownership_badge: 'Engineering Team Architecture',
    about_ownership_heading: 'System Component Ownership',
    about_ownership_desc: 'Aligned with enterprise safety standards and modular microservice division.',
    about_s1_title: 'Student 1: Workforce & Competency',
    about_s1_desc: 'Worker profiles, badge scanning, trade qualification tracking (Welder, Confined Space), and 30-day credential expiry forecasting.',
    about_s2_title: 'Student 2: Equipment & LOTO',
    about_s2_desc: 'Safety asset registry (extinguishers, gas monitors), 90-day calibration logs, and physical Lock-Out / Tag-Out isolation points.',
    about_s3_title: 'Student 3: Permit Lifecycle',
    about_s3_desc: 'JWT authentication, permit drafting state machines, digital QR token clearance issuance, and fire watch requirements.',
    about_s4_title: 'Student 4: SIMOPS & Weather',
    about_s4_desc: 'Geospatial plant zones, spatial-temporal conflict detection matrix, and live Open-Meteo meteorological integration.',

    // Contact Page
    contact_badge: '24/7 Safety & Plant Operations Support',
    contact_title: 'Contact & Operations Dispatch',
    contact_subtitle: 'Get in touch with the ClearToWork AI engineering support team or reach the Central HSE Control Room for high-hazard emergencies.',
    contact_emergency_title: '24/7 Plant Emergency Hotline',
    contact_emergency_desc: 'Dedicated HSE Safety Officer line for live SIMOPS halt, gas alarms, and evacuation orders.',
    contact_form_title: 'Send an Inquiry',
    contact_form_desc: 'Submit requests for enterprise pilot access, contractor onboarding, or custom safety rule integrations.',
    contact_field_name: 'Full Name',
    contact_field_email: 'Work Email',
    contact_field_facility: 'Facility / Site',
    contact_field_subject: 'Subject',
    contact_field_message: 'Message / Operational Requirement',
    contact_btn_submit: 'Submit Inquiry',
    contact_success_title: 'Message Dispatched!',
    contact_success_desc: 'Thank you for contacting ClearToWork AI. Our safety operations team will respond shortly.',
    contact_hq_title: 'Central Operations HQ',
    contact_hq_address: 'Petrochemical Complex Corridor, Block 7, Industrial Zone',
    contact_response_title: 'Operating Response Windows',
    contact_response_desc: 'Emergency SIMOPS escalation calls are routed deterministically in < 30 seconds. Standard pilot and enterprise inquiries are answered within 2 business hours.',

    // Register Page
    reg_title: 'Register User Profile',
    reg_subtitle: 'Create an authorized account for ClearToWork AI Permit-to-Work Clearance',
    reg_name: 'Full Name',
    reg_email: 'Work Email',
    reg_role: 'Operational Role',
    reg_password: 'Password',
    reg_confirm_password: 'Confirm Password',
    reg_submit: 'Complete Registration',
    reg_have_account: 'Already registered?',
    reg_signin: 'Sign in here →',

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
    docs_tab_database: 'Database Schema & ERD',

    // Workforce Domain (Student 1)
    workforce_title: 'Workforce Competency & Certification Register',
    workforce_subtitle: 'Active trade qualifications, compliance tracking, and 30-day proactive expiry forecasting.',
    workforce_forecast_title: '30-Day Proactive Certification Expiry Forecast (§5 Non-CRUD Operation)',
    workforce_forecast_desc: 'Automated foresight identifies certificates expiring or lapsed within 30 days to prevent job site clearance stoppages.',
    workforce_forecast_valid_all: 'All active personnel certificates are valid beyond 30 days.',
    workforce_search_placeholder: 'Search workers by name, badge, trade, or contractor...',
    workforce_th_name: 'Worker Name',
    workforce_th_badge: 'Badge #',
    workforce_th_cert: 'Certificate',
    workforce_th_expiry: 'Expiry Date',
    workforce_th_remaining: 'Days Remaining',
    workforce_th_status: 'Status',
    workforce_accredited_certs: 'Accredited Certifications',
    workforce_no_certs: 'No certifications registered.',
    workforce_status_valid: 'Valid',
    workforce_status_expired: 'Expired',
    workforce_status_expiring: 'Expiring Soon',

    // Equipment & LOTO Domain (Student 2)
    equipment_title: 'Equipment, Isolation & Asset Readiness Register',
    equipment_subtitle: 'Gas monitor calibration, fire extinguisher inspection records, and zone isolation points.',
    equipment_loto_title: 'Zone B3 Mezzanine — Active Lock-Out / Tag-Out (LOTO) Points',
    equipment_loto_desc: 'Prior to permit clearance, physical isolation manifolds are verified in safe state.',
    equipment_search_placeholder: 'Search equipment by tag, name, or category...',
    equipment_th_tag: 'Asset Tag',
    equipment_th_name: 'Equipment Name',
    equipment_th_category: 'Category',
    equipment_th_inspection: 'Inspection Status',
    equipment_th_calibration: 'Gas Calibration',
    equipment_th_readiness: 'Readiness Status',
    equipment_status_cleared: 'CLEARED',
    equipment_status_restricted: 'RESTRICTED',
    equipment_status_indate: 'In-Date',
    equipment_status_overdue: 'OVERDUE',
    equipment_status_certified: 'Certified',

    // Hazard Rules & SIMOPS Domain (Student 4)
    hazard_title: 'Hazard Rulebook & SIMOPS Incompatibility Matrix',
    hazard_subtitle: 'Atmospheric collision rules, simultaneous operations (SIMOPS), and weather envelope constraints.',
    hazard_simops_title: 'SIMOPS Incompatibility Rule HR-07 (High Explosive Risk)',
    hazard_simops_desc: 'Hot work (welding, grinding, open sparks) is strictly forbidden in any zone immediately adjacent to volatile solvent spray operations or solvent storage.',
    hazard_primary: 'Primary Hazard',
    hazard_conflicting: 'Conflicting Hazard',
    hazard_weather_title: 'Environmental Operational Limits (Open-Meteo Integration)',
    hazard_zones_title: 'Industrial Site Zones & Spatial Adjacency Graph',
    hazard_adjacent_transfer: 'Adjacent Risk Transfer Zones:',

    // Safety Analytics & Refusal Audit
    analytics_title: 'HSE Plant Safety Analytics & Refusal Audit',
    analytics_subtitle: 'Historical trends, root-cause refusal rankings, and Mean Time to Approval (MTTA).',
    analytics_kpi_approved: 'Permits Approved',
    analytics_kpi_approved_sub: 'Passed deterministic checks',
    analytics_kpi_refused: 'Safe Failures / Refused',
    analytics_kpi_refused_sub: 'Incidents prevented before site entry',
    analytics_kpi_active: 'Active Hot Work',
    analytics_kpi_active_sub: 'Currently undergoing execution on-site',
    analytics_kpi_mtta: 'Mean Time to Approval',
    analytics_kpi_mtta_sub: 'Sub-2h turnaround with AI assist',
    analytics_causes_title: 'Root-Cause Safe Failure Ranking (Incident Prevention)',
    analytics_causes_subtitle: 'Categorization of unsafe conditions caught by LangGraph agents & deterministic rules before permit sign-off.',
    analytics_zone_dist_title: 'Permit Workload Distribution by Plant Zone',
    analytics_zone_dist_subtitle: 'Active operational intensity across industrial sectors.',

    // Permit Dossier & Detail
    permit_list_title: 'Safety Permits Register',
    permit_list_subtitle: 'Real-time multi-agent safety permit clearance and risk validation dashboard.',
    permit_new_btn: 'Draft New Permit',
    permit_filter_all: 'All Permits',
    permit_filter_pending: 'Pending AI Review',
    permit_filter_approved: 'HSE Approved',
    permit_filter_refused: 'Refused (Safe Failure)',
    permit_detail_back: 'Back to Permit Register',
    permit_detail_run_ai: 'Run Agentic AI Clearance',
    permit_detail_signoff: 'Sign-Off Safety Clearance',
    permit_assigned_personnel: 'Assigned Personnel',
    permit_assigned_equipment: 'Safety Equipment',
    permit_no_workers: 'No workers assigned yet.',
    permit_no_equipment: 'No equipment assigned.',
    permit_timeline_title: 'Multi-Agent Workflow Execution Trace',
    permit_timeline_latency: 'Total Latency:',
    permit_remediation_title: 'Agent Recommended Safe Mitigation & Substitution',
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

    // Homepage
    home_badge: 'ස්වයංක්‍රීය කාර්මික ආරක්ෂණ සහතික කිරීමේ පද්ධතිය',
    home_hero_title_1: 'අනතුරු ශුන්‍යයි.',
    home_hero_title_2: 'බහු-නියෝජිත AI ආරක්ෂණ සහතිකය.',
    home_hero_subtitle: 'පෙට්‍රෝකෙමිකල් සහ පිරිපහදු ශාලා වල එකවර සිදුවන පරස්පර අවදානම් (SIMOPS), ක්‍රමාංකනය නොකළ උපකරණ සහ නුසුදුසු සේවක ගැටළු LangGraph මගින් ස්වයංක්‍රීයව වළක්වන්න.',
    home_btn_launch: 'ආරක්ෂිත පද්ධතියට පිවිසෙන්න',
    home_btn_register: 'නව කොන්ත්‍රාත්කරුවෙකු ලියාපදිංචි කරන්න',
    home_pixel_compare: 'පැරණි සහ නව පද්ධති සංසන්දනයට ක්ලික් කරන්න',
    home_legacy_title: 'පැරණි ක්‍රමයේ අවදානම් (Manual PTW)',
    home_legacy_heading: 'කඩදාසි බලපත්‍ර සහ පැතුරුම්පත් ගැටළු',
    home_legacy_desc: 'කඩදාසි ලේඛන මගින් යාබද කලාප වල එකවර සිදුවන අනතුරුදායක වෙල්ඩින්, ක්‍රමාංකනය නොකළ ගෑස් අනාවරක සහ කල් ඉකුත් වූ සහතික මග හැරේ.',
    home_legacy_tag1: '❌ ඉහළ SIMOPS පිපිරුම් අවදානම',
    home_legacy_tag2: '❌ පැය 3-4ක අතින් පරීක්ෂා කිරීමේ ප්‍රමාදය',
    home_ai_title: 'ClearToWork AI ස්වයංක්‍රීය අනුමැතිය',
    home_ai_heading: 'නියෝජිතයන් 5 දෙනාගේ Fail-Closed පද්ධතිය',
    home_ai_desc: 'ක්ෂණික භූගෝලීය පරස්පරතා පරීක්ෂාව, දින 90 උපකරණ ක්‍රමාංකන විගණනය, සහතික කළ සේවක ආදේශන සහ තථ්‍ය කාලීන 35km/h සුළං වේග ආරක්ෂණය.',
    home_ai_tag1: '✓ මිලි තත්පර 80ට අඩු LangGraph තීරණය',
    home_ai_tag2: '✓ ස්වයංක්‍රීය ආරක්ෂිත විසඳුම් මඟපෙන්වීම',
    home_stat_fail_closed: 'අනතුරු-ආරක්ෂිත ප්‍රතිපත්තිය',
    home_stat_agents: 'LangGraph නියෝජිතයන්',
    home_stat_latency: 'තක්සේරු කාලය',
    home_stat_languages: 'ස්වදේශීය භාෂා ත්‍රිත්වය (EN/SI/TA)',
    home_agents_title: 'ස්වයංක්‍රීය ආරක්ෂණ සම්මුතිය',
    home_agents_heading: 'විශේෂඥ නියෝජිත නෝඩ් 5ක්',
    home_agents_desc: 'මිනිස් අනුමැතියට පෙර සෑම නියෝජිතයෙකුම දැඩි ආරක්ෂණ නීති පද්ධතියක් ඔස්සේ තීරණ ලබා ගනී.',
    home_agent_1_title: 'සැලසුම් සහ සම්බන්ධීකරණය',
    home_agent_1_desc: 'වැඩ බලපත්‍ර අරමුණු විශ්ලේෂණය, කාල සීමා පැනවීම සහ ගිනි මුරකරුවන් අනිවාර්ය කිරීම.',
    home_agent_2_title: 'සේවක මණ්ඩලය සහ නිපුණතාවය',
    home_agent_2_desc: 'සේවක බැජ් විගණනය, වෘත්තීය සුදුසුකම් පරීක්ෂාව, සහතික කල් ඉකුත්වීම් පුරෝකථනය සහ සුදුසු සේවකයින් නිර්දේශය.',
    home_agent_3_title: 'උපකරණ සහ හුදකලා කිරීම් (LOTO)',
    home_agent_3_desc: 'ආරක්ෂිත උපකරණ පරීක්ෂාව, දින 90 ගෑස් අනාවරක ටැග් පරීක්ෂාව සහ භෞතික LOTO තහවුරු කිරීම.',
    home_agent_4_title: 'ක්ෂේත්‍ර තත්වයන් සහ අවදානම් (SIMOPS)',
    home_agent_4_desc: 'යාබද කලාප අවදානම් පරීක්ෂාව සහ Open-Meteo කාලගුණ දත්ත මගින් 35km/h සුළං සීමාව පැනවීම.',
    home_agent_5_title: 'අවසාන ආරක්ෂිත තහවුරු කිරීම',
    home_agent_5_desc: 'යම් හෝ ආරක්ෂිත උල්ලංඝනයක් ඇත්නම් බලපත්‍රය ප්‍රතික්ෂේප කර ස්වයංක්‍රීය පිළියම් සහ විකල්ප සේවකයින් ලබා දීම.',
    home_cta_heading: 'කාර්මික ආරක්ෂාව උසස් කිරීමට සූදානම්ද?',
    home_cta_desc: 'තථ්‍ය කාලීන ආරක්ෂණ පද්ධතිය අත්විඳීමට හෝ තාක්ෂණික ලියකියවිලි ගවේෂණය කිරීමට පිවිසෙන්න.',
    home_cta_signin: 'ආරක්ෂිත පද්ධතියට පිවිසෙන්න',
    home_cta_about: 'අපේ පද්ධතිය ගැන විස්තර →',
    home_footer_desc: 'ස්වයංක්‍රීය වැඩ බලපත්‍ර සහ SIMOPS ආරක්ෂණ පද්ධතිය',
    home_footer_copy: '© 2026 ClearToWork AI · Fail-Closed කාර්මික ආරක්ෂණ ප්‍රමිතිය',

    // About Page
    about_badge: 'කාර්මික ආරක්ෂණ ඉංජිනේරු විද්‍යාව',
    about_title: 'ClearToWork AI ගැන',
    about_subtitle: 'අවදානම් සහිත කාර්මික පරිසරයන් සඳහා ස්වයංක්‍රීය, බහු-නියෝජිත වැඩ බලපත්‍ර සත්‍යාපනය සහ SIMOPS ආරක්ෂණ පද්ධතිය.',
    about_team_badge: 'ClearToWork AI ප්‍රධාන ඉංජිනේරු කණ්ඩායම',
    about_team_heading: 'Full-Stack සහ Agentic AI සංවර්ධන කණ්ඩායම',
    about_team_desc: 'නවතම පරම්පරාවේ ආරක්ෂිත වැඩ බලපත්‍ර, අවකාශීය පරස්පරතා එන්ජිම සහ බහු-නියෝජිත ආරක්ෂණ සම්මුතිය නිර්මාණය කිරීම.',
    about_team_stat: 'ඉංජිනේරුවන් 4ක් · නියෝජිතයන් 5ක්',
    about_problem_badge: 'කාර්මික ගැටළුව',
    about_problem_heading: 'පැරණි PTW පද්ධති අසමත් වන්නේ ඇයි?',
    about_problem_desc: 'පිරිපහදු සහ රසායනික කර්මාන්තශාලා වල ප්‍රධාන හේතු තුනක් නිසා බරපතල අනතුරු සිදුවේ:',
    about_problem_1_title: 'SIMOPS පරස්පරතා:',
    about_problem_1_desc: 'යාබද කලාප වල එකවර සිදුවන අනතුරුදායක ක්‍රියාකාරකම් කඩදාසි පද්ධති මගින් හඳුනා නොගැනීම.',
    about_problem_2_title: 'ක්‍රමාංකනය නොකළ උපකරණ:',
    about_problem_2_desc: 'දින 90 ගෑස් අනාවරක සහ ගිනි නිවන උපකරණ විධිමත් පරීක්ෂාවකින් තොරව භාවිත කිරීම.',
    about_problem_3_title: 'නුසුදුසු සේවකයින්:',
    about_problem_3_desc: 'කල් ඉකුත් වූ වෘත්තීය සහතික ඇති සේවකයින් අනතුරුදායක වෙල්ඩින් හෝ සංවෘත අවකාශ වලට යෙදවීම.',
    about_solution_badge: 'ස්වයංක්‍රීය විසඳුම',
    about_solution_heading: 'නියෝජිතයන් 5 දෙනාගේ ක්‍රියාවලි පද්ධතිය',
    about_solution_desc: 'ClearToWork AI පද්ධතිය LangGraph ඔස්සේ Fail-Closed ආරක්ෂණ ප්‍රතිපත්තියක් ක්‍රියාත්මක කරයි.',
    about_ownership_badge: 'ඉංජිනේරු කණ්ඩායමේ ව්‍යුහය',
    about_ownership_heading: 'පද්ධති සංරචක හිමිකාරිත්වය',
    about_ownership_desc: 'ව්‍යවසාය මට්ටමේ ආරක්ෂණ ප්‍රමිතීන්ට අනුකූලව බෙදා වෙන් කර ඇත.',
    about_s1_title: 'ශිෂ්‍ය 1: සේවක මණ්ඩලය සහ නිපුණතාවය',
    about_s1_desc: 'සේවක පැතිකඩ, බැජ් ස්කෑන් කිරීම, සුදුසුකම් ලුහුබැඳීම සහ දින 30 කල් ඉකුත්වීම් පුරෝකථනය.',
    about_s2_title: 'ශිෂ්‍ය 2: උපකරණ සහ LOTO හුදකලා කිරීම',
    about_s2_desc: 'ආරක්ෂිත වත්කම් ලේඛනය, දින 90 ක්‍රමාංකන ලොග සහ භෞතික LOTO හුදකලා ස්ථාන.',
    about_s3_title: 'ශිෂ්‍ය 3: බලපත්‍ර ජීවන චක්‍රය',
    about_s3_desc: 'JWT සත්‍යාපනය, බලපත්‍ර කෙටුම්පත් කිරීම, ඩිජිටල් QR ටෝකන් නිකුත් කිරීම සහ ගිනි මුර අවශ්‍යතා.',
    about_s4_title: 'ශිෂ්‍ය 4: SIMOPS සහ කාලගුණය',
    about_s4_desc: 'භූගෝලීය කලාප, අවකාශීය ගැටුම් හඳුනාගැනීම සහ Open-Meteo සජීවී කාලගුණ ඒකාබද්ධතාවය.',

    // Contact Page
    contact_badge: '24/7 ආරක්ෂක සහ මෙහෙයුම් සහාය',
    contact_title: 'සම්බන්ධ වන්න සහ මෙහෙයුම් පාලනය',
    contact_subtitle: 'ClearToWork AI ඉංජිනේරු සහාය කණ්ඩායම හෝ හදිසි අවස්ථා සඳහා මධ්‍යම HSE පාලක මැදිරිය අමතන්න.',
    contact_emergency_title: '24/7 හදිසි ආරක්ෂක ඇමතුම් අංකය',
    contact_emergency_desc: 'හදිසි SIMOPS නැවැත්වීම්, ගෑස් අනතුරු ඇඟවීම් සහ ඉවත් කිරීමේ නියෝග සඳහා සෘජු ආරක්ෂක නිලධාරී මාර්ගය.',
    contact_form_title: 'විමසුමක් එවන්න',
    contact_form_desc: 'පද්ධති නියමු ප්‍රවේශය, කොන්ත්‍රාත්කරු ඇතුළත් කිරීම හෝ ආරක්ෂණ නීති ඒකාබද්ධ කිරීම සඳහා විමසන්න.',
    contact_field_name: 'සම්පූර්ණ නම',
    contact_field_email: 'විද්‍යුත් තැපෑල',
    contact_field_facility: 'කර්මාන්තශාලාව / පරිශ්‍රය',
    contact_field_subject: 'විෂය',
    contact_field_message: 'පණිවිඩය / මෙහෙයුම් අවශ්‍යතාවය',
    contact_btn_submit: 'විමසුම ඉදිරිපත් කරන්න',
    contact_success_title: 'පණිවිඩය සාර්ථකව යවන ලදි!',
    contact_success_desc: 'ClearToWork AI හා සම්බන්ධ වීම ගැන ස්තූතියි. අපගේ ආරක්ෂක මෙහෙයුම් කණ්ඩායම ඉක්මනින් ප්‍රතිචාර දක්වනු ඇත.',
    contact_hq_title: 'ප්‍රධාන මෙහෙයුම් මූලස්ථානය',
    contact_hq_address: 'පෙට්‍රෝකෙමිකල් සංකීර්ණ තීරය, බ්ලොක් 7, කාර්මික කලාපය',
    contact_response_title: 'ප්‍රතිචාර කාල සීමාවන්',
    contact_response_desc: 'හදිසි SIMOPS ඇමතුම් තත්පර 30කට අඩු කාලයකදී යොමු කෙරේ. සාමාන්‍ය විමසුම් ව්‍යාපාරික පැය 2ක් ඇතුළත විසඳනු ලැබේ.',

    // Register Page
    reg_title: 'පරිශීලක පැතිකඩ ලියාපදිංචි කිරීම',
    reg_subtitle: 'ClearToWork AI ආරක්ෂණ බලපත්‍ර පද්ධතිය සඳහා බලයලත් ගිණුමක් සාදන්න',
    reg_name: 'සම්පූර්ණ නම',
    reg_email: 'විද්‍යුත් තැපෑල',
    reg_role: 'මෙහෙයුම් කාර්යභාරය',
    reg_password: 'මුරපදය',
    reg_confirm_password: 'මුරපදය තහවුරු කරන්න',
    reg_submit: 'ලියාපදිංචිය සම්පූර්ණ කරන්න',
    reg_have_account: 'දැනටමත් ගිණුමක් තිබේද?',
    reg_signin: 'මෙහිදී ඇතුළු වන්න →',

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
    docs_tab_database: 'දත්ත සමුදා ER සටහන',

    // Workforce Domain (Student 1)
    workforce_title: 'සේවක නිපුණතා සහ සහතික කිරීමේ ලේඛනය',
    workforce_subtitle: 'ක්‍රියාකාරී වෘත්තීය සුදුසුකම්, අනුකූලතා ලුහුබැඳීම සහ දින 30ක පූර්ව කල් ඉකුත්වීම් පුරෝකථනය.',
    workforce_forecast_title: 'දින 30 කල් ඉකුත්වීම් පූර්ව අනතුරු ඇඟවීම (§5 Non-CRUD ක්‍රියාවලිය)',
    workforce_forecast_desc: 'වැඩබිම් නිෂ්කාශන බාධා වැළැක්වීම සඳහා දින 30ක් ඇතුළත කල් ඉකුත් වන සහතික ස්වයංක්‍රීයව හඳුනා ගනී.',
    workforce_forecast_valid_all: 'සියලුම සක්‍රීය සේවක සහතික දින 30කට වඩා වලංගු වේ.',
    workforce_search_placeholder: 'නම, බැජ් අංකය, වෘත්තිය හෝ කොන්ත්‍රාත්කරු අනුව සොයන්න...',
    workforce_th_name: 'සේවකයාගේ නම',
    workforce_th_badge: 'බැජ් #',
    workforce_th_cert: 'සහතිකය',
    workforce_th_expiry: 'කල් ඉකුත් වන දිනය',
    workforce_th_remaining: 'ඉතිරි දින ගණන',
    workforce_th_status: 'තත්ත්වය',
    workforce_accredited_certs: 'පිළිගත් සහතිකපත්',
    workforce_no_certs: 'ලියාපදිංචි සහතික නොමැත.',
    workforce_status_valid: 'වලංගුයි',
    workforce_status_expired: 'කල් ඉකුත් වී ඇත',
    workforce_status_expiring: 'ළඟදීම කල් ඉකුත් වේ',

    // Equipment & LOTO Domain (Student 2)
    equipment_title: 'ආරක්ෂිත උපකරණ, LOTO සහ වත්කම් සූදානම',
    equipment_subtitle: 'ගෑස් මොනිටර් ක්‍රමාංකනය, ගිනි නිවන උපකරණ පරීක්ෂණ සහ කලාප හුදකලා ස්ථාන.',
    equipment_loto_title: 'Zone B3 මෙසානින් — සක්‍රීය LOTO අගුලු දැමීමේ ස්ථාන',
    equipment_loto_desc: 'බලපත්‍ර නිෂ්කාශනයට පෙර භෞතික හුදකලා කපාට ආරක්ෂිත තත්ත්වයේ පවතින බව තහවුරු කරයි.',
    equipment_search_placeholder: 'ටැගය, නම හෝ කාණ්ඩය අනුව සොයන්න...',
    equipment_th_tag: 'වත්කම් ටැගය',
    equipment_th_name: 'උපකරණයේ නම',
    equipment_th_category: 'කාණ්ඩය',
    equipment_th_inspection: 'පරීක්ෂණ තත්ත්වය',
    equipment_th_calibration: 'ගෑස් ක්‍රමාංකනය',
    equipment_th_readiness: 'සූදානම තත්ත්වය',
    equipment_status_cleared: 'අවසර ලත්',
    equipment_status_restricted: 'සීමා කර ඇත',
    equipment_status_indate: 'වලංගුයි',
    equipment_status_overdue: 'කල් ඉකුත් වී ඇත',
    equipment_status_certified: 'සහතික කළ',

    // Hazard Rules & SIMOPS Domain (Student 4)
    hazard_title: 'අන්තරායකාරී නීති සංග්‍රහය සහ SIMOPS ගැටුම් න්‍යාසය',
    hazard_subtitle: 'වායුගෝලීය ඝට්ටන නීති, සමගාමී මෙහෙයුම් (SIMOPS) සහ කාලගුණ සීමා.',
    hazard_simops_title: 'SIMOPS ගැටුම් නීතිය HR-07 (ඉහළ පුපුරන සුළු අවදානම)',
    hazard_simops_desc: 'ද්‍රාවක ස්ප්‍රේ මෙහෙයුම් හෝ ද්‍රාවක ගබඩා ආසන්න කලාපවල උණුසුම් වැඩ (වෙල්ඩින්, ග්‍රයින්ඩින්) සපුරා තහනම්ය.',
    hazard_primary: 'ප්‍රාථමික අන්තරාය',
    hazard_conflicting: 'ගැටෙන අන්තරාය',
    hazard_weather_title: 'පාරිසරික මෙහෙයුම් සීමාවන් (Open-Meteo ඒකාබද්ධතාවය)',
    hazard_zones_title: 'කාර්මික අඩවි කලාප සහ අවකාශීය සිතියම',
    hazard_adjacent_transfer: 'යාබද අවදානම් මාරු කලාප:',

    // Safety Analytics & Refusal Audit
    analytics_title: 'HSE කාර්මික ආරක්ෂණ විශ්ලේෂණ සහ ප්‍රතික්ෂේප කිරීම් විගණනය',
    analytics_subtitle: 'ඓතිහාසික ප්‍රවණතා, ප්‍රතික්ෂේප කිරීම් වර්ගීකරණය සහ අනුමත කිරීමට ගතවන සාමාන්‍ය කාලය (MTTA).',
    analytics_kpi_approved: 'අනුමත කළ බලපත්‍ර',
    analytics_kpi_approved_sub: 'නිරවද්‍යතා පරීක්ෂණ සමත් විය',
    analytics_kpi_refused: 'ආරක්ෂිතව වැළැක්වූ / ප්‍රතික්ෂේපිත',
    analytics_kpi_refused_sub: 'වැඩබිමට ඇතුළුවීමට පෙර අනතුරු වළක්වන ලදී',
    analytics_kpi_active: 'සක්‍රීය උණුසුම් වැඩ',
    analytics_kpi_active_sub: 'දැනට වැඩබිමෙහි ක්‍රියාත්මක වේ',
    analytics_kpi_mtta: 'සාමාන්‍ය අනුමත කාලය',
    analytics_kpi_mtta_sub: 'AI සහාය ඇතිව පැය 2කට අඩු කාලයකින්',
    analytics_causes_title: 'මූලික හේතු වර්ගීකරණය (අනතුරු වැළැක්වීම)',
    analytics_causes_subtitle: 'LangGraph නියෝජිතයින් විසින් හසුකරගත් අනාරක්ෂිත තත්ත්වයන්.',
    analytics_zone_dist_title: 'කලාපය අනුව බලපත්‍ර ව්‍යාප්තිය',
    analytics_zone_dist_subtitle: 'කාර්මික අංශ හරහා සක්‍රීය මෙහෙයුම් තීව්‍රතාව.',

    // Permit Dossier & Detail
    permit_list_title: 'ආරක්ෂිත බලපත්‍ර ලේඛනය',
    permit_list_subtitle: 'තථ්‍ය කාලීන බහු-නියෝජිත ආරක්ෂිත වැඩ බලපත්‍ර නිෂ්කාශනය සහ අවදානම් පාලන පුවරුව.',
    permit_new_btn: 'නව බලපත්‍රයක් කෙටුම්පත් කරන්න',
    permit_filter_all: 'සියලු බලපත්‍ර',
    permit_filter_pending: 'AI සමාලෝචනය අපේක්ෂිත',
    permit_filter_approved: 'HSE අනුමත',
    permit_filter_refused: 'ප්‍රතික්ෂේපිත (ආරක්ෂිත වැළැක්වීම)',
    permit_detail_back: 'නැවත බලපත්‍ර ලේඛනයට',
    permit_detail_run_ai: 'නියෝජිත AI නිෂ්කාශනය ක්‍රියාත්මක කරන්න',
    permit_detail_signoff: 'ආරක්ෂණ අනුමැතිය සහතික කරන්න',
    permit_assigned_personnel: 'යොදවා ඇති සේවකයින්',
    permit_assigned_equipment: 'ආරක්ෂිත උපකරණ',
    permit_no_workers: 'සේවකයින් තවම පවරා නොමැත.',
    permit_no_equipment: 'උපකරණ පවරා නොමැත.',
    permit_timeline_title: 'බහු-නියෝජිත ක්‍රියාකාරී විගණන ලොගය',
    permit_timeline_latency: 'මුළු කාල ප්‍රමාදය:',
    permit_remediation_title: 'නියෝජිතයන් විසින් නිර්දේශිත ආරක්ෂිත විකල්ප සහ විසඳුම්',
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

    // Homepage
    home_badge: 'தானியங்கி தொழில்துறை பாதுகாப்பு அனுமதி இயந்திரம்',
    home_hero_title_1: 'விபத்துகள் பூஜ்ஜியம்.',
    home_hero_title_2: 'பல முகவர் AI பாதுகாப்பு அனுமதி.',
    home_hero_subtitle: 'பெட்ரோகெமிக்கல் மற்றும் சுத்திகரிப்பு நிலையங்களில் ஒரே நேரத்தில் ஏற்படும் முரண்பாடான ஆபத்துகள் (SIMOPS), அளவீடு செய்யப்படாத உபகரணங்கள் மற்றும் தகுதியற்ற குழுக்களை LangGraph மூலம் தவிர்க்கவும்.',
    home_btn_launch: 'பாதுகாப்பு தளத்திற்குள் நுழையவும்',
    home_btn_register: 'புதிய ஒப்பந்ததாரரை பதிவு செய்யவும்',
    home_pixel_compare: 'பழைய மற்றும் புதிய அமைப்பை ஒப்பிட கிளிக் செய்யவும்',
    home_legacy_title: 'பழைய காகித முறையின் குறைபாடுகள் (Manual PTW)',
    home_legacy_heading: 'காகித அனுமதிகள் & விரிதாள் குருட்டுப் புள்ளிகள்',
    home_legacy_desc: 'காகித வேலைகள் அருகிலுள்ள கரைப்பான் மண்டலங்களில் ஒரே நேரத்தில் வெல்டிங் செய்தல், காலாவதியான வாயு கண்காணிப்பு அளவீடுகள் போன்ற ஆபத்துகளை புறக்கணிக்கிறது.',
    home_legacy_tag1: '❌ அதிக SIMOPS வெடிப்பு ஆபத்து',
    home_legacy_tag2: '❌ 3-4 மணிநேர மனித மதிப்பாய்வு தாமதம்',
    home_ai_title: 'ClearToWork AI தானியங்கி அனுமதி',
    home_ai_heading: '5-முகவர் தோல்வி-பாதுகாப்பு அனுமதி இயந்திரம்',
    home_ai_desc: 'உடனடி இடஞ்சார்ந்த மோதல் சரிபார்ப்பு, 90 நாள் உபகரண அளவீட்டு தணிக்கை, சான்றளிக்கப்பட்ட தொழிலாளர் மாற்றீடு மற்றும் நேரடி 35km/h காற்று வேக வரம்பு.',
    home_ai_tag1: '✓ <80ms LangGraph ஒருமித்த முடிவு',
    home_ai_tag2: '✓ தோல்வி-பாதுகாப்பான தீர்வு வழிகாட்டுதல்',
    home_stat_fail_closed: 'தோல்வி-பாதுகாப்பான கொள்கை',
    home_stat_agents: 'LangGraph முகவர்கள்',
    home_stat_latency: 'மதிப்பீட்டு வேகம்',
    home_stat_languages: 'பூர்வீக 3 மொழிகள் (EN/SI/TA)',
    home_agents_title: 'தானியங்கி பாதுகாப்பு ஒருமித்த கருத்து',
    home_agents_heading: 'இயக்கப்பட்ட வரைபடத்தில் 5 சிறப்பு முகவர்கள்',
    home_agents_desc: 'மனித கையொப்பத்திற்கு முன் ஒவ்வொரு முகவரும் கடுமையான பாதுகாப்பு விதிகளை தானாகவே மதிப்பீடு செய்கிறது.',
    home_agent_1_title: 'திட்டமிடல் & ஒருங்கிணைப்பு',
    home_agent_1_desc: 'பணி நோக்கங்களை பகுப்பாய்வு செய்தல், நேர வரம்புகளை விதித்தல் மற்றும் தீ கண்காணிப்பாளர்களை கட்டாயப்படுத்துதல்.',
    home_agent_2_title: 'பணியாளர்கள் & தகுதி',
    home_agent_2_desc: 'தொழிலாளர் பேட்ஜ் தணிக்கை, தகுதி சரிபார்ப்பு, 30 நாள் காலாவதி கணிப்பு மற்றும் தகுதியான தொழிலாளர் பரிந்துரை.',
    home_agent_3_title: 'உபகரணங்கள் & தனிமைப்படுத்தல் (LOTO)',
    home_agent_3_desc: 'பாதுகாப்பு உபகரண சரிபார்ப்பு, 90 நாள் வாயு கண்காணிப்பு குறிச்சொற்கள் மற்றும் உடல் LOTO பூட்டுதல் சரிபார்ப்பு.',
    home_agent_4_title: 'தள நிலைமைகள் & அபாயங்கள் (SIMOPS)',
    home_agent_4_desc: 'அருகிலுள்ள மண்டல மோதல்களை சரிபார்த்தல் மற்றும் Open-Meteo மூலம் 35 km/h காற்று வேக வரம்பை செயல்படுத்துதல்.',
    home_agent_5_title: 'இறுதி பாதுகாப்பு சரிபார்ப்பு',
    home_agent_5_desc: 'ஏதேனும் பாதுகாப்பு மீறல் கண்டறியப்பட்டால் அனுமதியை நிராகரித்து, மாற்று தொழிலாளர்கள் மற்றும் நேரத்தை தானாகவே பரிந்துரைக்கிறது.',
    home_cta_heading: 'தொழில்துறை பாதுகாப்பை மேம்படுத்த தயாரா?',
    home_cta_desc: 'நேரடி பாதுகாப்பு அமைப்பை அனுபவிக்க உள்நுழையவும் அல்லது தொழில்நுட்ப ஆவணங்களை ஆராயவும்.',
    home_cta_signin: 'பாதுகாப்பு தளத்தில் உள்நுழையவும்',
    home_cta_about: 'எங்கள் தளம் பற்றி அறிய →',
    home_footer_desc: 'தானியங்கி பணி அனுமதி & SIMOPS பாதுகாப்பு இயந்திரம்',
    home_footer_copy: '© 2026 ClearToWork AI · தோல்வி-பாதுகாப்பான தொழில்துறை தரம்',

    // About Page
    about_badge: 'தொழில்துறை பாதுகாப்பு பொறியியல்',
    about_title: 'ClearToWork AI பற்றி',
    about_subtitle: 'அதிவேக மற்றும் ஆபத்தான தொழில்துறை சூழல்களுக்கான தானியங்கி, பல முகவர் பணி அனுமதி (PTW) சரிபார்ப்பு மற்றும் SIMOPS பாதுகாப்பு அமைப்பு.',
    about_team_badge: 'ClearToWork AI முதன்மை பொறியியல் குழு',
    about_team_heading: 'Full-Stack மற்றும் Agentic AI மேம்பாட்டுக் குழு',
    about_team_desc: 'அடுத்த தலைமுறை தொழில்துறை பாதுகாப்பு அனுமதி, இடஞ்சார்ந்த மோதல் இயந்திரம் மற்றும் பல முகவர் ஒருமித்த அமைப்பை உருவாக்குகிறது.',
    about_team_stat: '4 பொறியாளர்கள் · 5 முகவர்கள்',
    about_problem_badge: 'தொழில்துறை பிரச்சனை',
    about_problem_heading: 'பழைய PTW அமைப்புகள் ஏன் தோல்வியடைகின்றன?',
    about_problem_desc: 'சுத்திகரிப்பு மற்றும் ரசாயன ஆலைகளில் மூன்று முக்கிய காரணங்களால் பெரும் விபத்துகள் ஏற்படுகின்றன:',
    about_problem_1_title: 'SIMOPS மோதல்கள்:',
    about_problem_1_desc: 'அருகிலுள்ள மண்டலங்களில் ஒரே நேரத்தில் நடக்கும் ஆபத்தான வெல்டிங் வேலைகள் காகித அமைப்பால் கவனிக்கப்படாமல் போவது.',
    about_problem_2_title: 'அளவீடு செய்யப்படாத உபகரணங்கள்:',
    about_problem_2_desc: '90 நாள் வாயு கண்காணிப்பாளர்கள் மற்றும் தீயணைப்பு கருவிகள் முறையான சரிபார்ப்பு இல்லாமல் பயன்படுத்தப்படுவது.',
    about_problem_3_title: 'தகுதியற்ற பணியாளர்கள்:',
    about_problem_3_desc: 'காலாவதியான சான்றிதழ்களைக் கொண்ட தொழிலாளர்கள் ஆபத்தான பணிகளில் ஈடுபடுத்தப்படுவது.',
    about_solution_badge: 'தானியங்கி தீர்வு',
    about_solution_heading: '5-முகவர் செயல்முறை பைப்லைன்',
    about_solution_desc: 'ClearToWork AI அமைப்பு LangGraph வழியாக கடுமையான தோல்வி-பாதுகாப்புக் கொள்கையை செயல்படுத்துகிறது.',
    about_ownership_badge: 'பொறியியல் குழு கட்டமைப்பு',
    about_ownership_heading: 'கணினி கூறுகளின் உரிமை',
    about_ownership_desc: 'நிறுவன பாதுகாப்பு தரநிலைகளுக்கு ஏற்ப பிரிக்கப்பட்டுள்ளது.',
    about_s1_title: 'மாணவர் 1: பணியாளர்கள் & தகுதி',
    about_s1_desc: 'தொழிலாளர் விவரங்கள், பேட்ஜ் ஸ்கேனிங், தகுதி கண்காணிப்பு மற்றும் 30 நாள் காலாவதி கணிப்பு.',
    about_s2_title: 'மாணவர் 2: உபகரணங்கள் & LOTO தனிமைப்படுத்தல்',
    about_s2_desc: 'பாதுகாப்பு சொத்து பதிவேடு, 90 நாள் அளவீட்டு பதிவுகள் மற்றும் உடல் LOTO தனிமைப்படுத்தல் புள்ளிகள்.',
    about_s3_title: 'மாணவர் 3: பணி அனுமதி வாழ்க்கை சுழற்சி',
    about_s3_desc: 'JWT அங்கீகாரம், அனுமதி வரைவு நிலை, டிஜிட்டல் QR டோக்கன் வெளியீடு மற்றும் தீ கண்காணிப்பு தேவைகள்.',
    about_s4_title: 'மாணவர் 4: SIMOPS & வானிலை',
    about_s4_desc: 'இடஞ்சார்ந்த மண்டலங்கள், மோதல் கண்டறிதல் மற்றும் Open-Meteo நேரடி வானிலை ஒருங்கிணைப்பு.',

    // Contact Page
    contact_badge: '24/7 பாதுகாப்பு & செயல்பாட்டு ஆதரவு',
    contact_title: 'தொடர்பு & செயல்பாட்டு கட்டுப்பாடு',
    contact_subtitle: 'ClearToWork AI பொறியியல் ஆதரவு குழு அல்லது அவசர தேவைகளுக்கு மத்திய HSE கட்டுப்பாட்டு அறையை தொடர்பு கொள்ளவும்.',
    contact_emergency_title: '24/7 அவசர பாதுகாப்பு உதவி எண்',
    contact_emergency_desc: 'நேரடி SIMOPS நிறுத்தம், வாயு எச்சரிக்கைகள் மற்றும் வெளியேற்ற உத்தரவுகளுக்கான பிரத்யேக பாதுகாப்பு அதிகாரி எண்.',
    contact_form_title: 'ஒரு வினவலை அனுப்பவும்',
    contact_form_desc: 'முன்னோடி அணுகல், ஒப்பந்ததாரர் பதிவு அல்லது பாதுகாப்பு விதிகளை ஒருங்கிணைக்க கோரிக்கைகளை சமர்ப்பிக்கவும்.',
    contact_field_name: 'முழு பெயர்',
    contact_field_email: 'மின்னஞ்சல் முகவரி',
    contact_field_facility: 'ஆலை / தளம்',
    contact_field_subject: 'பொருள்',
    contact_field_message: 'செய்தி / செயல்பாட்டு தேவை',
    contact_btn_submit: 'வினவலை சமர்ப்பிக்கவும்',
    contact_success_title: 'செய்தி வெற்றிகரமாக அனுப்பப்பட்டது!',
    contact_success_desc: 'ClearToWork AI ஐ தொடர்பு கொண்டதற்கு நன்றி. எங்கள் பாதுகாப்பு குழு விரைவில் பதிலளிக்கும்.',
    contact_hq_title: 'மத்திய செயல்பாட்டு தலைமையகம்',
    contact_hq_address: 'பெட்ரோகெமிக்கல் வளாகம், பிளாக் 7, தொழில்துறை மண்டலம்',
    contact_response_title: 'பதில் அளிக்கும் நேரங்கள்',
    contact_response_desc: 'அவசர SIMOPS அழைப்புகள் 30 வினாடிகளுக்குள் இணைக்கப்படுகின்றன. நிலையான வினவல்கள் 2 வணிக மணிநேரத்திற்குள் தீர்க்கப்படும்.',

    // Register Page
    reg_title: 'பயனர் சுயவிவரத்தை பதிவு செய்யவும்',
    reg_subtitle: 'ClearToWork AI பணி அனுமதி அமைப்புக்கு அங்கீகரிக்கப்பட்ட கணக்கை உருவாக்கவும்',
    reg_name: 'முழு பெயர்',
    reg_email: 'மின்னஞ்சல் முகவரி',
    reg_role: 'செயல்பாட்டு பங்கு',
    reg_password: 'கடவுச்சொல்',
    reg_confirm_password: 'கடவுச்சொல்லை உறுதிப்படுத்தவும்',
    reg_submit: 'பதிவை முடிக்கவும்',
    reg_have_account: 'ஏற்கனவே கணக்கு உள்ளதா?',
    reg_signin: 'இங்கே உள்நுழையவும் →',

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
    docs_tab_database: 'தரவுத்தள ER வரைபடம்',

    // Workforce Domain (Student 1)
    workforce_title: 'பணியாளர் தகுதி மற்றும் சான்றிதழ் பதிவு',
    workforce_subtitle: 'செயலில் உள்ள தொழில் தகுதிகள், இணக்க கண்காணிப்பு மற்றும் 30 நாள் முன்கூட்டியே காலாவதி கணிப்பு.',
    workforce_forecast_title: '30-நாள் முன்கூட்டியே சான்றிதழ் காலாவதி கணிப்பு (§5 Non-CRUD செயல்பாடு)',
    workforce_forecast_desc: 'வேலை தள அனுமதி தாமதங்களை தடுக்க 30 நாட்களுக்குள் காலாவதியாகும் சான்றிதழ்களை தானாகவே கண்டறிகிறது.',
    workforce_forecast_valid_all: 'அனைத்து செயலில் உள்ள பணியாளர் சான்றிதழ்களும் 30 நாட்களுக்கு மேல் செல்லுபடியாகும்.',
    workforce_search_placeholder: 'பெயர், பேட்ஜ் எண், தொழில் அல்லது ஒப்பந்ததாரர் மூலம் தேடுங்கள்...',
    workforce_th_name: 'பணியாளர் பெயர்',
    workforce_th_badge: 'பேட்ஜ் #',
    workforce_th_cert: 'சான்றிதழ்',
    workforce_th_expiry: 'காலாவதி தேதி',
    workforce_th_remaining: 'மீதமுள்ள நாட்கள்',
    workforce_th_status: 'நிலை',
    workforce_accredited_certs: 'அங்கீகரிக்கப்பட்ட சான்றிதழ்கள்',
    workforce_no_certs: 'பதிவுசெய்யப்பட்ட சான்றிதழ்கள் எதுவும் இல்லை.',
    workforce_status_valid: 'செல்லுபடியாகும்',
    workforce_status_expired: 'காலாவதியானது',
    workforce_status_expiring: 'விரைவில் காலாவதியாகிறது',

    // Equipment & LOTO Domain (Student 2)
    equipment_title: 'பாதுகாப்பு உபகரணங்கள், LOTO மற்றும் தயார்நிலை பதிவு',
    equipment_subtitle: 'வாயு மானிட்டர் அளவீடு, தீயணைப்பு ஆய்வு பதிவுகள் மற்றும் மண்டல தனிமைப்படுத்தல் புள்ளிகள்.',
    equipment_loto_title: 'மண்டலம் B3 மெஸ்ஸானைன் — செயலில் உள்ள LOTO தனிமைப்படுத்தல் புள்ளிகள்',
    equipment_loto_desc: 'அனுமதி வழங்கப்படுவதற்கு முன், இயற்பியல் தனிமைப்படுத்தல் வால்வுகள் பாதுகாப்பான நிலையில் இருப்பது உறுதி செய்யப்படுகிறது.',
    equipment_search_placeholder: 'குறிச்சொல், பெயர் அல்லது வகை மூலம் தேடுங்கள்...',
    equipment_th_tag: 'சொத்து குறிச்சொல்',
    equipment_th_name: 'உபகரண பெயர்',
    equipment_th_category: 'வகை',
    equipment_th_inspection: 'ஆய்வு நிலை',
    equipment_th_calibration: 'வாயு அளவீடு',
    equipment_th_readiness: 'தயார்நிலை',
    equipment_status_cleared: 'அனுமதிக்கப்பட்டது',
    equipment_status_restricted: 'கட்டுப்படுத்தப்பட்டது',
    equipment_status_indate: 'செல்லுபடியாகும்',
    equipment_status_overdue: 'காலாவதியானது',
    equipment_status_certified: 'சான்றளிக்கப்பட்டது',

    // Hazard Rules & SIMOPS Domain (Student 4)
    hazard_title: 'ஆபத்து விதிகள் மற்றும் SIMOPS பொருந்தாமை அணி',
    hazard_subtitle: 'வளிமண்டல மோதல் விதிகள், ஒரே நேரத்தில் நிகழும் செயல்பாடுகள் (SIMOPS) மற்றும் வானிலை கட்டுப்பாடுகள்.',
    hazard_simops_title: 'SIMOPS பொருந்தாமை விதி HR-07 (அதிக வெடிப்பு ஆபத்து)',
    hazard_simops_desc: 'கரைப்பான் தெளிப்பு செயல்பாடுகள் அல்லது கரைப்பான் சேமிப்பிற்கு அருகிலுள்ள எந்தவொரு மண்டலத்திலும் வெப்ப வேலை (வெல்டிங், அரைத்தல்) கண்டிப்பாக தடைசெய்யப்பட்டுள்ளது.',
    hazard_primary: 'முதன்மை ஆபத்து',
    hazard_conflicting: 'முரண்பட்ட ஆபத்து',
    hazard_weather_title: 'சுற்றுச்சூழல் செயல்பாட்டு வரம்புகள் (Open-Meteo ஒருங்கிணைப்பு)',
    hazard_zones_title: 'தொழில்துறை தள மண்டலங்கள் & இடஞ்சார்ந்த வரைபடம்',
    hazard_adjacent_transfer: 'அருகிலுள்ள இடர் பரிமாற்ற மண்டலங்கள்:',

    // Safety Analytics & Refusal Audit
    analytics_title: 'HSE ஆலை பாதுகாப்பு பகுப்பாய்வு மற்றும் மறுப்பு தணிக்கை',
    analytics_subtitle: 'வரலாற்று போக்குகள், மூல காரண மறுப்பு தரவரிசைகள் மற்றும் ஒப்புதலுக்கான சராசரி நேரம் (MTTA).',
    analytics_kpi_approved: 'அனுமதிகள் அங்கீகரிக்கப்பட்டன',
    analytics_kpi_approved_sub: 'பாதுகாப்பு சோதனைகளில் தேர்ச்சி பெற்றது',
    analytics_kpi_refused: 'பாதுகாப்பான தோல்விகள் / மறுக்கப்பட்டவை',
    analytics_kpi_refused_sub: 'தள நுழைவுக்கு முன் விபத்துக்கள் தடுக்கப்பட்டன',
    analytics_kpi_active: 'செயலில் உள்ள வெப்ப வேலை',
    analytics_kpi_active_sub: 'தற்போது தளத்தில் செயல்படுத்தப்படுகிறது',
    analytics_kpi_mtta: 'ஒப்புதலுக்கான சராசரி நேரம்',
    analytics_kpi_mtta_sub: 'AI உதவியுடன் 2 மணி நேரத்திற்குள்',
    analytics_causes_title: 'மூல காரண பாதுகாப்பான தோல்வி தரவரிசை (விபத்து தடுப்பு)',
    analytics_causes_subtitle: 'அனுமதி கையொப்பத்திற்கு முன் LangGraph முகவர்கள் மற்றும் விதிகளால் பிடிக்கப்பட்ட பாதுகாப்பற்ற நிலைமைகள்.',
    analytics_zone_dist_title: 'மண்டலம் வாரியாக அனுமதி பணிச்சுமை பகிர்வு',
    analytics_zone_dist_subtitle: 'தொழில்துறை பிரிவுகளில் செயலில் உள்ள செயல்பாட்டு தீவிரம்.',

    // Permit Dossier & Detail
    permit_list_title: 'பாதுகாப்பு அனுமதிகள் பதிவு',
    permit_list_subtitle: 'நிகழ்நேர பல முகவர் பாதுகாப்பு பணி அனுமதி அனுமதி மற்றும் இடர் சரிபார்ப்பு டாஷ்போர்டு.',
    permit_new_btn: 'புதிய அனுமதியை உருவாக்கு',
    permit_filter_all: 'அனைத்து அனுமதிகள்',
    permit_filter_pending: 'AI மதிப்பாய்வு நிலுவையில் உள்ளது',
    permit_filter_approved: 'HSE அங்கீகரிக்கப்பட்டது',
    permit_filter_refused: 'மறுக்கப்பட்டது (பாதுகாப்பான தோல்வி)',
    permit_detail_back: 'அனுமதி பதிவேட்டிற்கு திரும்பவும்',
    permit_detail_run_ai: 'முகவர் AI அனுமதியை இயக்கவும்',
    permit_detail_signoff: 'பாதுகாப்பு அனுமதியை கையொப்பமிடுங்கள்',
    permit_assigned_personnel: 'ஒதுக்கப்பட்ட பணியாளர்கள்',
    permit_assigned_equipment: 'பாதுகாப்பு உபகரணங்கள்',
    permit_no_workers: 'பணியாளர்கள் இன்னும் ஒதுக்கப்படவில்லை.',
    permit_no_equipment: 'உபகரணங்கள் ஒதுக்கப்படவில்லை.',
    permit_timeline_title: 'பல முகவர் பணிப்பாய்வு தணிக்கை தடம்',
    permit_timeline_latency: 'மொத்த மறுமொழி நேரம்:',
    permit_remediation_title: 'முகவர் பரிந்துரைத்த பாதுகாப்பான தீர்வு மற்றும் மாற்றீடு',
  }
};

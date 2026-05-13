# Pocket Coach App Plan (ADHD + Depression + Anxiety Support)

## 1) Product Goal
Build a **mobile-first companion** that helps users complete basic life tasks (hygiene, food, meds, movement, sleep, admin) using:
- tiny actions,
- compassionate nudges,
- adaptive reminders,
- dopamine rewards,
- and crisis-aware safety rails.

Design principles:
1. **No shame language**.
2. **Smallest possible next step** always available.
3. **Fail-safe, not fail-hard** (missing a streak never wipes progress).
4. **Works on very low energy days**.
5. **Privacy by default**.

---

## 2) Core User Problems to Solve
- Task paralysis (starting is hard).
- Time blindness (hours disappear).
- Interoception gaps (forgetting hunger, thirst, hygiene, meds).
- Avoidance spirals after missed routines.
- Need for novelty + reward to sustain behavior.
- Anxiety when reminders feel aggressive.

---

## 3) MVP Scope (8–12 weeks)

### A. Daily Survival Checklist (non-judgmental)
Default cards:
- Drink water
- Eat something
- Brush teeth
- Wash face / quick shower
- Take meds
- 2-minute movement
- One admin task

Each card has **3 completion levels**:
- `Minimum`: tiny version (e.g., rinse mouth)
- `Standard`: normal version
- `Bonus`: extra effort

Why: gives a win even on bad days and preserves momentum.

### B. Adaptive Reminder Engine
- Reminder windows instead of hard times.
- Nudges escalate in tone only if user opts in.
- “Snooze with intention” choices:
  - 10 min
  - after current activity
  - at next location (if enabled)
- If repeatedly ignored, reminder switches to a smaller ask.

### C. Dopamine System
- XP for any completion tier.
- Randomized micro-rewards (sound/animation/quote) for novelty.
- Weekly “stability streak” = number of days with at least one care action (not perfect completion).
- Unlockables:
  - themes,
  - badges,
  - tiny stories/collectibles.

### D. Energy/Mood-Aware Mode
Quick check-in (5 seconds):
- Energy: low/medium/high
- Anxiety: low/medium/high
- Mood: down/neutral/okay

App auto-scales expectations:
- low energy → 1-minute tasks only
- high anxiety → grounding tasks first

### E. Crisis and Safety Layer
- “I’m overwhelmed” panic button:
  - 30-second breathing coach
  - 3-step grounding
  - message trusted contact shortcut
- Regional crisis resources page.
- Strong disclaimer: not a replacement for clinical care.

---

## 4) Suggested Tech Stack

### Frontend (Mobile)
- **React Native + Expo** (fast iteration, iOS + Android from one codebase)
- TypeScript
- Expo Notifications for local push reminders

### Backend
- **Supabase** (Postgres + auth + row-level security + edge functions)
- Optional: Firebase Cloud Messaging for advanced push workflows

### Local-first data
- SQLite / MMKV cache for offline mode
- Sync when online

### Analytics (privacy-safe)
- PostHog (self-host or cloud) with strict event minimization
- No selling data; no ad SDKs

---

## 5) Data Model (minimal)

### tables
- `users`
- `tasks` (name, category, default_tier_rules)
- `task_completions` (task_id, tier, completed_at, energy_snapshot)
- `reminder_rules` (task_id, window_start, window_end, escalation_style)
- `checkins` (mood, anxiety, energy, timestamp)
- `rewards` (type, seed, granted_at)
- `trusted_contacts` (name, method)

---

## 6) Behavioral Design Patterns

1. **Implementation intention prompts**
   - “When I finish coffee, I will brush teeth for 30 seconds.”
2. **Temptation bundling**
   - Pair boring task + preferred audio.
3. **Action shrinking**
   - If skipped twice, auto-shrink task.
4. **Gentle restart protocol**
   - After inactivity, app says: “No reset needed. Pick one 30-second win.”
5. **Choice architecture**
   - Always show only 1–3 next actions to avoid overwhelm.

---

## 7) Notification Tone Library

User chooses tone profile:
- **Soft**: “Hey, tiny check-in?”
- **Direct**: “Time to do one survival task now.”
- **Coach**: “Two minutes now saves pain later. Let’s go.”
- **Chaos Goblin** (humorous): opt-in spicy reminders.

Support for “gentle or otherwise” without shame.

---

## 8) Accessibility + UX Requirements
- Big tap targets.
- High-contrast themes + dyslexia-friendly font option.
- Minimal typing; mostly tap flows.
- Voice note task capture.
- Widget + lock-screen quick actions.
- Apple Health / Google Fit integration optional later.

---

## 9) Privacy, Ethics, and Safety
- End-to-end encryption for sensitive notes (phase 2).
- Clear crisis boundary messaging.
- Optional passcode lock.
- Data export + delete account.
- Avoid manipulative dark patterns.

---

## 10) Build Plan

### Phase 0 (Week 1)
- Define personas, non-goals, risk register.
- Create design tokens and component system.

### Phase 1 (Weeks 2–4)
- Auth + onboarding.
- Survival checklist with 3 tiers.
- Basic reminders.

### Phase 2 (Weeks 5–7)
- XP and reward loops.
- Mood/energy check-in.
- Adaptive task shrinking.

### Phase 3 (Weeks 8–10)
- Safety flows + trusted contacts.
- Analytics dashboard for engagement metrics.

### Phase 4 (Weeks 11–12)
- Beta polish, accessibility audit, app store prep.

---

## 11) Success Metrics
Primary:
- % of active users completing at least 1 care action/day.
- 7-day retention.
- Reduction in “zero-care days”.

Secondary:
- Reminder response rate.
- Average tasks/day by energy state.
- User-reported shame score trend.

---

## 12) Example MVP Screens
1. Onboarding: choose tone + top 5 survival tasks.
2. Home: “Today’s 3 tiny wins.”
3. Task card with Minimum / Standard / Bonus.
4. Reward pop after completion.
5. Check-in modal (energy/anxiety/mood).
6. Panic support page.

---

## 13) First Engineering Tasks (concrete)
1. Scaffold Expo app with TypeScript.
2. Implement local task list + completion states.
3. Add local notifications by reminder window.
4. Build XP service and reward RNG.
5. Build check-in modal and store snapshots.
6. Add simple charts for weekly wins.

---

## 14) Prompt Pack for AI Features (optional)
- “Given energy=low and anxiety=high, suggest 3 tasks under 2 minutes.”
- “Rewrite this reminder in soft tone.”
- “Generate restart message after 10 missed days with no shame.”

---

## 15) Important Clinical Disclaimer Copy (starter)
“This app supports routines and self-management. It is not a medical device and does not replace mental health care. If you are in immediate danger or considering self-harm, contact local emergency services or a crisis line now.”

---

## 16) Next Step
If you want, we can immediately generate:
1. Expo project scaffold,
2. screen-by-screen component list,
3. Supabase schema migrations,
4. and a week-by-week implementation backlog.

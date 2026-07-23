# HospitalFinder — Multi-Framework Test Automation Capstone

A test automation capstone project built around a real-world case study on [Practo.com](https://www.practo.com), covering **Selenium**, **Playwright**, **JMeter**, **Reqnroll (BDD)**, and **Jenkins CI/CD**.

Built as part of the QEA SDET C# 2026 programme.

---

## Problem Statement

> "Finding Hospitals — Get the hospital names that are open 24×7, have parking facility, and rating more than 3.5."

The case study set three tasks against Practo.com:

1. **Bangalore Hospitals** — identify hospitals open 24/7, with parking, rated above 3.5, and display the names
2. **Diagnostics Page** — scrape the top cities' names into a `List` and display them
3. **Corporate Wellness** — fill in invalid details, submit, and capture the resulting warning message

---

## Repository Structure

```
Capstone Project/
├── HospitalFinder.Selenium/       # Task 1 + cross-cutting Selenium coverage
├── HospitalFinder.Playwright/     # Tasks 2 & 3, plus Reqnroll BDD
├── HospitalFinder.JMeter/         # Performance testing (response time, throughput, error rate)
└── Capstone Project.slnx          # Solution file — opens all three projects together
```

---

## Why Three Frameworks

| Framework | Responsibility |
|---|---|
| **Selenium + NUnit** | Hospital search & filtering, negative tests, accessibility, cross-browser/parallel execution (Selenium Grid), login validation, CI/CD (Jenkins) |
| **Playwright + Reqnroll** | Diagnostics page scraping, Corporate Wellness form validation, BDD scenarios |
| **JMeter** | Load testing — response time, throughput, error rate |

All three share the same underlying design principles:
- **Page Object Model** throughout
- **Config-driven** — `appsettings.json` (with `.grid`/`.ci` variants for Selenium) controls environment behavior without code changes
- **Externalized test data** — `testdata.json`, never hardcoded
- **Thread-safe reporting** — Extent Reports (+ Allure where functioning) across parallel test runs

---

## Selenium — `HospitalFinder.Selenium/`

- **Hospital search**: all four case-study filter criteria implemented — rating, 24×7, and **parking** (found on each hospital's detail page under "Read more info" → Amenities, not the listing page)
- **Negative tests**: impossible filter criteria, nonexistent city — both assert graceful empty results, not exceptions
- **Data-driven search**: same logic parameterized across Bangalore, Mumbai, Delhi via `[TestCase]`
- **Accessibility**: keyboard-only navigation + AxeCore WCAG 2.1/2.2 scan (found 6 real violations on the live site)
- **Cross-browser & parallel execution**: Dockerized Selenium Grid (Chrome/Firefox/Edge), driven via `[TestFixture]` + `[Parallelizable(ParallelScope.Fixtures)]`, viewable live via noVNC
- **Login validation**: investigated whether a native browser alert exists anywhere on Practo (it doesn't); test runs on Firefox/Edge only, since Chrome is blocked by Akamai on the login subdomain
- **CI/CD**: Jenkins pipeline (checkout → configure → restore → build → test → publish), self-configuring via `appsettings.ci.json`

### Running locally
```bash
cd HospitalFinder.Selenium
dotnet build
dotnet test
```

### Selenium Grid (cross-browser, live viewing)
```bash
docker compose up -d
# Set "UseGrid": true, "Headless": false in appsettings.json
dotnet test --filter "FullyQualifiedName~HospitalSearchTests"
```
View live sessions at `http://localhost:7900` (Chrome), `7901` (Firefox), `7902` (Edge) — password `secret`.

---

## Playwright — `HospitalFinder.Playwright/`

- **Diagnostics scraping**: extracts Practo's "Top Cities" modal into a `List<string>` (8 cities), config-driven assertions
- **Corporate Wellness**: 4 field-level validation tests (contact number, email, name, organization name) — Practo uses inline error styling, not a native alert or warning message; a fully valid submission surfaces a reCAPTCHA, which was not attempted to be bypassed
- **BDD (Reqnroll)**: the Corporate Wellness invalid-contact-number scenario, written in Gherkin, with a manually-bridged Playwright lifecycle (`[BeforeScenario]`/`[AfterScenario]` hooks, since Reqnroll-generated classes don't inherit `PageTest`)

### Running locally
```bash
cd HospitalFinder.Playwright
dotnet build
export HEADED=1   # optional, for a visible browser
dotnet test
```

---

## JMeter — `HospitalFinder.JMeter/`

Load tests `practo.com/bangalore/hospitals` and the homepage directly via HTTP, measuring response time, throughput, and error rate at varying concurrency (3/6/10/15 threads).

### Running
```bash
cd HospitalFinder.JMeter
jmeter -n -t PractoHospitalSearch.jmx -l results/results.jtl -e -o results/html-report
```
Dashboard report opens at `results/html-report/index.html`.

---

## Key Findings

- **Akamai bot management** — investigated across all three frameworks. Two distinct challenge tiers identified: a soft, timed countdown page (residential networks — mitigated with a wait-and-refresh retry) and a harder, sensor-gated "Challenge Validation" tier (CI/data-center networks — documented, not bypassed).
- **Parking data correction** — initially concluded parking wasn't exposed anywhere on Practo; further investigation found it on each hospital's individual detail page, leading to a two-phase filter (cheap checks first, expensive per-hospital check only for survivors).
- **6 real WCAG accessibility violations** found on Practo's live hospital listing page via AxeCore.
- **No native browser alert exists anywhere on Practo** — confirmed across search, Diagnostics, Corporate Wellness, and Login; every validation flow uses inline styling instead.
- **Cross-browser testing found a genuine bug**: a cookie-consent overlay blocked native clicks on Chrome/Edge but not Firefox.

Throughout, the deliberate boundary was: **investigate and document anti-automation behavior, never attempt to bypass it** (Akamai, `navigator.webdriver` detection, reCAPTCHA).

---

## Requirements Mapping

| Brief requirement | Delivered via |
|---|---|
| Alert / windows / search | Window handling (View Profile); alert — no native alert exists, inline validation used instead |
| Navigate back to home | `SearchResults_NavigateBack_ReturnsToHomePage` |
| Extract into collections | Hospital names/ratings (Selenium); Diagnostics city list (Playwright) |
| Filling forms (different objects) | Corporate Wellness — text, email, tel, dropdown fields |
| Capture warning message | Inline error-class + disabled-button state |
| BDD based automation | Reqnroll Gherkin scenario |
| Parallel test execution | Selenium Grid, 3 browsers, `[Parallelizable(Fixtures)]` |
| Page Object Model | Used throughout, both frameworks |
| Performance metrics | JMeter — response time, throughput, error rate |
| Accessibility testing | Keyboard-only + AxeCore WCAG scan |

---

## Author

Adam Dejen — QEA SDET C# 2026 Programme

#  PlaywrightUITests

Automated web UI testing project built with **C#**, **Playwright**, **NUnit**, and **FluentAssertions**.  
This project performs an end-to-end browser automation workflow through Google Search and the Prometheus Group website to test form validation on the “Contact Us” page.

---

##  Overview

This project automates the following workflow:

1. Navigate to [Google](https://www.google.com)
2. Search for **"Prometheus Group"**
3. Verify that results contain the expected company name
4. Click the **Prometheus Group** link if a direct **“Contact Us”** link is not visible
5. On the Prometheus Group website, locate and click **“Contact Us”**
6. Fill in **First Name** and **Last Name** fields
7. Click the **“Contact Us”** or **Submit** button
8. Validate that **4 or more required fields** are flagged as invalid

---

##  Requirements

Before running the tests, ensure you have:

- [.NET SDK 9.0+](https://dotnet.microsoft.com/en-us/download)
- Internet connection (for browser automation)
- Playwright browsers installed

Install Playwright browsers using PowerShell:
```bash
pwsh bin/Debug/net9.0/playwright.ps1 install
```

---

##  Project Structure

```
PlaywrightUITests/
│
├── Tests/
│   └── GoogleSearchTests.cs        # Main automation test suite
│
├── PlaywrightUITests.csproj        # Project definition
└── README.md                       # This file
```

---

##  Setup Instructions

### 1️ Clone or Download the Repository
```bash
git clone https://github.com/<your-username>/PlaywrightUITests.git
cd PlaywrightUITests
```

Or, download the folder directly from GitHub.

---

### 2️ Restore Dependencies
```bash
dotnet restore
pwsh bin/Debug/net9.0/playwright.ps1 install
```

---

### 3️ Build the Project
```bash
dotnet build
```
Expected output:
```
Build succeeded.
```

---

### 4️ Run the Tests
```bash
dotnet test
```
If Google shows a CAPTCHA or the layout changes, the test will **mark itself inconclusive** and save a screenshot.

---

##  Notes

- Runs with **Headless = false** and **SlowMo = 200 ms** for easy observation.
- Includes intelligent logic to:
  - Detect and skip CAPTCHA pages (with screenshots saved as `GoogleSearch_Captcha.png`)
  - Handle dynamic or changing Google layouts
  - Follow links to the Prometheus Group site when no direct “Contact Us” link is present
  - Support both `<button>` and `<input type='submit'>` form submissions
- Screenshots are automatically captured in your project root directory for debugging.

---

##  Possible Screenshots Created

| Screenshot Name | When It's Captured |
|------------------|--------------------|
| `GoogleSearch_Captcha.png` | When CAPTCHA page appears |
| `GoogleSearch_Failure.png` | Timeout waiting for results |
| `NoPrometheusResult.png` | No Prometheus link found on Google |
| `NoContactUs_OnSite.png` | No Contact Us link found on Prometheus site |
| `NoSubmitFound.png` | No form submit button found on contact page |

---

##  Author

**Michael Gentry**  
Entry-Level QA / Embedded Systems Engineer Candidate  
[GitHub Profile](https://github.com/mgentry3035)

---

##  Test Summary

| Step | Description | Expected Result |
|------|--------------|----------------|
| 1 | Go to Google | Page loads successfully |
| 2 | Search for "Prometheus Group" | Search results display company name |
| 3 | Find and click company link | Prometheus site opens |
| 4 | Click "Contact Us" | Contact form page loads |
| 5 | Fill name fields | First/Last names entered |
| 6 | Click Contact Us button | Form attempts to submit |
| 7 | Validate errors | ≥ 4 validation errors detected |
| 8 | CAPTCHA detected | Test marked inconclusive |


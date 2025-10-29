#  PlaywrightUITests

Automated web UI testing project built with **C#**, **Playwright**, **NUnit**, and **FluentAssertions**.  
This project simulates an end-to-end user interaction using Google Search and form submission to demonstrate robust UI automation and dynamic content handling.

---

##  Overview

This project automates the following workflow:
1. Navigate to [Google](https://www.google.com)
2. Search for "Prometheus Group"
3. Verify the search results contain "Prometheus Group"
4. Click the "Contact Us" link
5. Fill out the "First Name" and "Last Name" fields
6. Submit the form
7. Validate that at least four required fields remain unfilled (required-field errors)

---

##  Requirements

Before running the tests, ensure you have:
- [.NET SDK 9.0+](https://dotnet.microsoft.com/en-us/download)
- Internet connection for browser automation
- Playwright browsers installed

Install Playwright browsers using:
```bash
pwsh bin/Debug/net9.0/playwright.ps1 install
```

---

##  Project Structure

```
PlaywrightUITests/
│
├── Tests/
│   └── GoogleSearchTests.cs        # Main test suite
│
├── PlaywrightUITests.csproj        # Project definition
└── README.md                       # This file
```

---

##  Setup Instructions

### 1️) Clone or Download
If using Git:
```bash
git clone https://github.com/<your-username>/PlaywrightUITests.git
cd PlaywrightUITests
```

Or, download the folder directly from your GitHub repository.

---

### 2️) Restore Dependencies
Install required NuGet packages and Playwright:
```bash
dotnet restore
pwsh bin/Debug/net9.0/playwright.ps1 install
```

---

### 3️) Build the Project
```bash
dotnet build
```
You should see:
```
Build succeeded.
```

---

### 4️) Run the Tests
```bash
dotnet test
```

Expected output:
```
Passed!  - Failed: 0, Passed: 1, Skipped: 0, Total: 1
```
If Google displays a CAPTCHA, the test will mark itself as **Inconclusive** rather than fail.

---

##  Notes

- Running in visible mode (`Headless = false`) reduces the chance of CAPTCHA detection.
- Slow motion (`SlowMo = 200`) is used to simulate more natural browsing speed.
- The test includes automatic waits and navigation handling for stable results.
- CAPTCHA detection is logged and gracefully skipped to prevent false failures.

---

##  Author

**Michael Gentry**  
[GitHub Profile](https://github.com/mgentry3035)

---

##  Test Summary

| Step | Description | Expected Result |
|------|--------------|----------------|
| 1 | Go to Google | Page loads successfully |
| 2 | Search for "Prometheus Group" | Results contain "Prometheus Group" |
| 3 | Click "Contact Us" | Navigates to contact page |
| 4 | Fill name fields | First and last names entered |
| 5 | Submit form | Validation messages appear |
| 6 | Count required errors | ≥ 4 required-field errors detected |


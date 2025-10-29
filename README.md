#  PlaywrightUITests

Automated web UI testing project built with **C#**, **Playwright**, **NUnit**, and **FluentAssertions**.  
This project simulates an end-to-end web interaction using **Google Search** and a **Contact Us** form to demonstrate robust, dynamic UI automation.

---

##  Overview

This project automates the following workflow:
1. Navigate to [Google](https://www.google.com)  
2. Search for **"Prometheus Group"**  
3. Verify that results contain the term  
4. Click the **"Contact Us"** link  
5. Fill the **First Name** and **Last Name** fields  
6. Submit the form  
7. Verify that **4+ required-field validation messages** appear  

---

##  Requirements

Before running the tests, ensure you have:
- [.NET SDK 9.0+](https://dotnet.microsoft.com/en-us/download)
- Internet connection for browser automation
- Playwright browsers installed

Install browsers using:
```bash
pwsh bin/Debug/net9.0/playwright.ps1 install
```

---

## ⚙️ Project Structure

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

### 1️ Clone or Download
If using Git:
```bash
git clone https://github.com/<your-username>/PlaywrightUITests.git
cd PlaywrightUITests
```

Or download the folder directly from your GitHub repository.

---

### 2️ Restore Dependencies
Install NuGet packages and Playwright browsers:
```bash
dotnet restore
pwsh bin/Debug/net9.0/playwright.ps1 install
```

---

### 3️ Build the Project
```bash
dotnet build
```
You should see:
```
Build succeeded.
```

---

### 4️ Run the Tests
```bash
dotnet test
```

Expected output:
```
Passed!  - Failed: 0, Passed: 1, Skipped: 0, Total: 1
```
If Google triggers a CAPTCHA, the test will **skip** itself automatically and mark as *Inconclusive* to avoid false failures.

---

##  Notes

- Test runs with **Headless = false** and **SlowMo = 200 ms** for more natural browsing and visibility.
- Test includes a **60-second timeout** for dynamic Google content.
- If CAPTCHA or network delays occur, the test:
  - Detects and logs it  
  - Saves a **screenshot (GoogleSearch_Failure.png)** in the project root directory  
  - Marks the test as *Inconclusive* instead of failing  
- You can optionally store screenshots in a subfolder:
  ```csharp
  await page.ScreenshotAsync(new PageScreenshotOptions { Path = "Screenshots/GoogleSearch_Failure.png" });
  ```
  (Create a `Screenshots` folder manually if you prefer this structure.)
- Clean teardown ensures browser and Playwright processes close safely.

---

##  Author

**Michael Gentry**  
[GitHub Profile](https://github.com/mgentry3035)

---

## 🏁 Test Summary

| Step | Description | Expected Result |
|------|--------------|----------------|
| 1 | Go to Google | Page loads successfully |
| 2 | Search for "Prometheus Group" | Results appear with matching text |
| 3 | Click "Contact Us" | Navigates to contact page |
| 4 | Fill name fields | First/Last names entered |
| 5 | Submit form | Page shows validation messages |
| 6 | Count required errors | ≥ 4 validation errors detected |
| 7 | CAPTCHA detected (optional) | Test skipped as Inconclusive |
| 4 | Fill name fields | First and last names entered |
| 5 | Submit form | Validation messages appear |
| 6 | Count required errors | ≥ 4 required-field errors detected |


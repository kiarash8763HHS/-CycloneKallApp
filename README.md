# Cyclone Kall App — PRO

نسخه بازطراحی‌شده‌ی Cyclone Kall App برای Windows/WPF و Visual Studio.

## امکانات
- انتخاب میکروفون واقعی از بین Capture Deviceهای فعال ویندوز در اولین ورود.
- بوست قابل انتخاب از **0 تا +100 dB** در رابط کاربری.
- پریست پیش‌فرض **دعوا / VOICE** برای صدای پرقدرت و واضح.
- Noise Gate، Compression و Presence برای کنترل ساده‌ی گفتار.
- Limiter/Soft-Limit برای کاهش کلیپ دیجیتال؛ +100 dB یک محدوده‌ی کاربری است و بوست خامِ بدون محدودکننده در این سطح عمداً اعمال نمی‌شود چون باعث انفجار نویز و کلیپ شدید می‌شود.
- طراحی فارسی، تیره و مدرن با نام Cyclone Kall App در هدر و بخش‌های اصلی.
- بدون هیچ عکس یا Asset خارجی.
- آماده‌ی Build/Publish با Visual Studio 2022 و .NET 8.
- برای ارسال صدای پردازش‌شده به Discord/FiveM و برنامه‌های دیگر، از Virtual Audio Cable استفاده می‌شود.

## نکته مهم درباره Equalizer APO
Cyclone Kall App از نظر تجربه‌ی کاربری، ایده‌ی «پردازش زنده‌ی ورودی + کنترل ساده» را دنبال می‌کند؛ اما Equalizer APO در سطح Audio Processing Object ویندوز کار می‌کند. این پروژه برای اینکه بدون درایور اختصاصی و با Visual Studio ساده Build شود، مسیر Capture → Process → Output را استفاده می‌کند.

اگر خروجی را به VB-CABLE بدهید:
- داخل Cyclone Kall App: `CABLE Input`
- داخل Discord/FiveM/برنامه مقصد: `CABLE Output`

## ساخت EXE در Visual Studio
1. Visual Studio 2022 را نصب کنید و workload **.NET Desktop Development** را فعال کنید.
2. `CycloneKallApp.sln` را باز کنید.
3. NuGet Restore را انجام دهید.
4. Configuration را روی `Release` و Platform را روی `x64` بگذارید.
5. Build → Build Solution.
6. برای EXE مستقل تک‌فایلی، در Developer PowerShell پروژه اجرا کنید:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

خروجی:
`CycloneKallApp\bin\Release\net8.0-windows\win-x64\publish\CycloneKallApp.exe`

## هشدار صدا
+100 dB را برای تست مداوم پیشنهاد نمی‌کنیم. این مقدار می‌تواند نویز را شدیداً تقویت کند و در صورت خروجی به هدفون/اسپیکر، خطر آسیب به شنوایی داشته باشد. برای Voice معمولاً پریست پیش‌فرض بسیار پایین‌تر از سقف 100 dB کار می‌کند.

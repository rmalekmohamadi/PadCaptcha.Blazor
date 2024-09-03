<h1>
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="content/Nuget.png">
    <source media="(prefers-color-scheme: light)" srcset="content/Nuget.png">
    <img alt="PadCaptcha.Blazor" src="content/Nuget.png">
  </picture>
</h1>

# PadCaptcha.Blazor
Captcha for Blazor



## Installation

Install Package
```
dotnet add package PadCaptcha.Blazor
```
## Features

- Work On Linux
- Refresh On Failed
- Customize Size
- Customize Color
- Customize Noise
- Customize Style

## Usage
```
<PadCaptcha.Blazor.PadCaptcha @ref="_captcha" />
<input @bind-value="_captchaValue" />
<button @onclick="ValidateCaptcha">Validate</button>

@code
{
  private PadCaptcha.Blazor.PadCaptcha _captcha;
  private string _captchaValue;
  private bool _captchaValidateResult;

  private void ValidateCaptcha()
  {
      _captchaValidateResult = _captcha.IsValid(_captchaValue);
  }
}
```

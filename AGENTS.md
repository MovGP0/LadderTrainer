# Repository Guidelines

## Project Structure & Module Organization
- Root solution file `LadderTrainer.slnx` with MAUI app under `LadderTrainer/`.
- UI: `LadderTrainer/Views` (pages) and XAML roots `App.xaml`, `AppShell.xaml`, `MainPage.xaml`.
- App setup and DI in `LadderTrainer/MauiProgram.cs`; core logic currently in `LadderTrainer/LadderSessionViewModel.cs` and `SessionPhase.cs`.
- Platform assets live in `LadderTrainer/Platforms/*`; images/fonts under `LadderTrainer/Resources`.

## Build, Test, and Development Commands
- Restore/build Windows: `pwsh -NoProfile -NoLogo -File build.ps1` (psake default runs `dotnet restore` then `dotnet build -f net10.0-windows10.0.19041.0`).
- Run Windows app: `pwsh -NoProfile -NoLogo -File build.ps1 -TaskList Run-Windows`.
- Build Android (requires workloads/JDK/SDK): `pwsh -NoProfile -NoLogo -File build.ps1 -TaskList Build-Android`.
- Direct dotnet examples (Windows): `dotnet run LadderTrainer.slnx -f net10.0-windows10.0.19041.0`.
- Tests: add xUnit projects and run `dotnet test` from repo root.

## Coding Style & Naming Conventions
- C#: Allman braces; 4-space indent; file-scoped namespaces; `System.*` usings first; prefer `var` when obvious; use object/collection initializers and null-propagation.
- Fields: `_camelCase` instance, `s_camelCase` static; public members/constants PascalCase.
- Avoid nested classes (except private test helpers); use ReactiveUI (`ReactiveObject` + `[Reactive]`) and `ReactiveMarbles.ObservableEvents` for event subscriptions.
- Line endings: CRLF; C# files UTF-8 BOM. Run encoding/line-ending fix script if editors differ.

## Testing Guidelines
- Prefer xUnit + Shouldy assertions. Test class names: `<Subject>Tests`; method names start with `Should...` (set `DisplayName`). Use `[TestOf(typeof(Target), nameof(Target.Method))]` where helpful.
- Structure tests with `// Arrange`, `// Act`, `// Assert`; name the main instance `subject` and the outcome `result`.

## Commit & Pull Request Guidelines
- Commits: short, imperative subjects (≈50 chars); group logical changes; include context in body when needed. Current history is minimal—keep it tidy.
- PRs: describe motivation and behavior changes; link issues/work items; include screenshots or recordings for UI changes (Windows/Android). Note required workloads or tooling changes in the description.

## Security & Configuration Tips
- Do not commit secrets or platform keystores. Use user-level env vars for API keys.
- Android: ensure `JAVA_HOME` points to `C:\Program Files\Microsoft\jdk-21.0.x` and `ANDROID_HOME`/SDK path is set; accept SDK licenses when prompted.
- Keep workloads current: `dotnet workload update` before major builds.

## Source generation
- Some code is source-generated (e.g., ReactiveUI `[Reactive]` properties). See https://github.com/reactiveui/ReactiveUI.SourceGenerators for the documentation.
- When available, prefer tools (ie. Rider) for inspecting and editing files.

## Implicit usings
- Note that implicit usings are enabled. Do not import the following namespaces:
  - `System`
  - `System.Collections.Generic`
  - `System.IO`
  - `System.Linq`
  - `System.Net.Http`
  - `System.Threading`
  - `System.Threading.Tasks`
  - `Microsoft.Maui`
  - `Microsoft.Maui.Controls`

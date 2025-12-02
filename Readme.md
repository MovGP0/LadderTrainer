## Workloads
```csharp
dotnet workload install android
```

## Windows

```csharp
dotnet build LadderTrainer.slnx -f net10.0-windows10.0.19041.0
dotnet run LadderTrainer.slnx -f net10.0-windows10.0.19041.0
```

## Android

Install the Android Workload
```csharp
dotnet workload install android
```

Install current JDK
```csharp
winget install -e --id Microsoft.OpenJDK.21
```

```csharp
dotnet build LadderTrainer.slnx `
    -t:InstallAndroidDependencies `
    -f net10.0-android `
    -p:JavaSdkDirectory="C:\Program Files\Microsoft\jdk-21.0.x" `
    -p:AndroidSdkDirectory="$env:LOCALAPPDATA\Local\Android\Sdk `
    -p:AcceptAndroidSdkLicenses=True
```

Properties
{
    $Solution = 'LadderTrainer.slnx'
    $WindowsFramework = 'net10.0-windows10.0.19041.0'
    $AndroidFramework = 'net10.0-android'
    $JavaSdkDirectory = 'C:\Program Files\Microsoft\jdk-21.0.x'
    $AndroidSdkDirectory = Join-Path -Path $env:LOCALAPPDATA -ChildPath 'Local\Android\Sdk'
}

Task Default -Depends Build-Windows

Task Restore
{
    Exec {
        dotnet restore $Solution
    }
}

Task Build-Windows -Depends Restore
{
    Exec {
        dotnet build $Solution `
            -f $WindowsFramework
    }
}

Task Run-Windows -Depends Build-Windows
{
    Exec {
        dotnet run $Solution `
            -f $WindowsFramework
    }
}

Task Ensure-Android-Workload
{
    Exec {
        dotnet workload install android
    }
}

Task Build-Android -Depends Restore, Ensure-Android-Workload
{
    Exec {
        dotnet build $Solution `
            -t:InstallAndroidDependencies `
            -f $AndroidFramework `
            -p:JavaSdkDirectory="$JavaSdkDirectory" `
            -p:AndroidSdkDirectory="$AndroidSdkDirectory" `
            -p:AcceptAndroidSdkLicenses=True
    }
}

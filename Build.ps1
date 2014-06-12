Write-Host "Restoring packages ..."
.\.nuget\NuGet.exe restore
Write-Host "Restored packages."

Write-Host "Getting path to msbuild.exe ..."
[void][System.Reflection.Assembly]::Load('Microsoft.Build.Utilities.v12.0, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a')
$msbuild = [Microsoft.Build.Utilities.ToolLocationHelper]::GetPathToBuildToolsFile("msbuild.exe", "12.0", "Bitness64")
Write-Host "MSBuild: $msbuild"

Write-Host "Compiling solution ..."
&$msbuild NAppTracking.sln /nologo /verbosity:minimal /m /p:Configuration=Release
Write-Host "Compiled solution."
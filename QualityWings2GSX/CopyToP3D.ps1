# powershell -ExecutionPolicy Unrestricted -file "$(ProjectDir)CopyToP3D.ps1" $(ConfigurationName)
$buildConfiguration = $args[0]
$baseDir = "C:\Users\Fragtality\source\repos\QualityWings2GSX\QualityWings2GSX"
$bindir = "$baseDir\bin\$buildConfiguration"
$destDir = "F:\Prepar3D\QualityWings2GSX"

if ($buildConfiguration -eq "Release") {
	Write-Host "Copy new Binaries ..."
	Copy-Item -Path ($bindir + "\QualityWings2GSX.exe") -Destination $destDir -Recurse -Force
	Copy-Item -Path ($bindir + "\QualityWings2GSX.exe.config") -Destination $destDir -Recurse -Force
	#Copy-Item -Path ($baseDir + "\ofp.xml") -Destination $destDir -Recurse -Force
	Copy-Item -Path ($bindir + "\*.dll") -Destination $destDir -Recurse -Force
}

exit 0

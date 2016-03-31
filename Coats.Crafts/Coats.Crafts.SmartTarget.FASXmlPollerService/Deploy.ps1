# These variables should be set via the Octopus web portal:
#
#   ServiceName         - Name of the Windows service
#   ServiceExecutable   - Path to the .exe containing the service
#	IncomingDir			- Path whre XML is dropped
#	ProcessedDir		- Path were XML is moved to after processing
#	RejectedDirectory	- Path for any problem XML
# 
# sc.exe is the Service Control utility in Windows
  
$service = Get-Service $ServiceName -ErrorAction SilentlyContinue



if (! $service)
{
    Write-Host "The service will be installed"
    
    New-Service -Name $ServiceName -BinaryPathName $ServiceExecutable -StartupType Automatic	
}
else
{
	$fullPath = Resolve-Path $ServiceExecutable -ErrorAction SilentlyContinue

    Write-Host "The service will be stopped and reconfigured"

    Stop-Service $ServiceName -Force
     
    & "sc.exe" config $service.Name binPath= $fullPath start= auto | Write-Host
}

if (-Not (Test-Path $IncomingDir))
{
	Write-Host "Creating $IncomingDir directory"
	New-Item -path $IncomingDir -itemtype directory
}
else
{
	Write-Host "$IncomingDir directory already exist"
}

if (-Not (Test-Path $ProcessedDir))      
{ 
	Write-Host "Creating $ProcessedDir directory"
	New-Item -path $ProcessedDir -itemtype directory
}
else
{
	Write-Host "$ProcessedDir directory already exist"
}

if (-Not (Test-Path $RejectedDir))
{
	Write-Host "Creating $RejectedDir directory"
	New-Item -path $RejectedDir -itemtype directory
}
else
{
	Write-Host "$RejectedDir directory already exist"
}

Write-Host "Starting the service"
Start-Service $ServiceName
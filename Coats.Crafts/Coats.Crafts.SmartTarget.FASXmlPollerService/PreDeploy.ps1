# These variables should be set via the Octopus web portal:
#
#   ServiceName         - Name of the Windows service
#   ServiceExecutable   - Path to the .exe containing the service
#	Config				- Path to main PROD file (on PROD only)
#	Subject				- String to replace (PROD only)
#	Endpoint			- Strig to replace (PROD only)
# 

# Replace settings (if passed)
if(Test-Path variable:global:Config)
{
	(Get-Content $Config) | Foreach-Object {
	    $_ -replace "subject value=`"SmtpAppender`"", "subject value=`"$Subject`"" `
	       -replace "endpoint address=`"http://172.18.5.69", "endpoint address=`"http://$Endpoint" 
	    } | Set-Content $Config	
}

# Service stuff
$service = Get-Service $ServiceName -ErrorAction SilentlyContinue

if ($service)
{
	Write-Host "The service will be stopped."

    Stop-Service $ServiceName -Force
}
else
{
    Write-Host "Service not found"
}

# These variables should be set via the Octopus web portal:
#
#   OctopusEnvironmentName	- Environemtn name
#   Files   				- Path application files (used in purge step)
#	Endpoint				- String to replace (Staging only)
# 	Config 					- Web.config
# 

#$OctopusEnvironmentName = "Coats PROD"
#$Files = "E:\Work\Coats\Build\Code\Coats.Crafts\Coats.Crafts\*"

$SiteFiles = (get-item $Files.Replace("*","")).parent.Fullname
$Configs = Join-Path $Files.Replace("*","") "site configs"

Write-host $SiteFiles
Write-host $Configs

if ($OctopusEnvironmentName -eq "Coats PROD")
{ 
    Get-ChildItem $Configs -Filter "*.$OctopusEnvironmentName.config" | `
        Foreach-Object{
            $ConfigFile = $_.Name
            $Folder = $ConfigFile.Split(".")[0]            
            $SitePath = Join-Path $SiteFiles $Folder
            
            Write-host $ConfigFile
            Write-host $Folder
            Write-host $SitePath
            Test-Path $SitePath | Write-Host
            
            If (Test-Path $SitePath)
            {
               Copy-Item $_.FullName (Join-Path $SitePath "Web.config") -Force
            }
        }    

    # On staging we run WCfs vias netTcp, the Endpoint and Config variables are only provide if running deployment on STAGING
	if(Test-Path variable:global:Endpoint)
	{
		(Get-Content $Config) | Foreach-Object {		    	
		    $_ -replace "endpoint address=`"http://tridion.page.service/CoatsTridionpageService.svc`" binding=`"basicHttpBinding`" bindingConfiguration=`"BasicHttpBinding_CoatsServices`"", "endpoint address=`"net.tcp://$Endpoint`:810/CoatsTridionPageService.svc`" binding=`"netTcpBinding`" bindingConfiguration=`"ServiceEndpoint`"" `
		    	-replace "endpoint address=`"http://tridion.component.service/CoatsTridionComponentService.svc`" binding=`"basicHttpBinding`" bindingConfiguration=`"BasicHttpBinding_CoatsServices`"", "endpoint address=`"net.tcp://$Endpoint`:811/CoatsTridionComponentService.svc`" binding=`"netTcpBinding`" bindingConfiguration=`"ServiceEndpoint`"" `
		    	-replace "endpoint address=`"http://tridion.link.service/CoatsTridionLinkService.svc`" binding=`"basicHttpBinding`" bindingConfiguration=`"BasicHttpBinding_CoatsServices`"", "endpoint address=`"net.tcp://$Endpoint`:812/CoatsTridionLinkService.svc`" binding=`"netTcpBinding`" bindingConfiguration=`"ServiceEndpoint`"" `
		    	-replace "endpoint address=`"http://coats.integration.service/CoatsIntegrationService.svc`" binding=`"basicHttpBinding`" bindingConfiguration=`"BasicHttpBinding_CoatsServices`"", "endpoint address=`"net.tcp://$Endpoint`:814/CoatsIntegrationService.svc`" binding=`"netTcpBinding`" bindingConfiguration=`"ServiceEndpoint`"" `
				-replace "endpoint address=`"http://172.18.5.69", "endpoint address=`"http://$Endpoint" 
		    } | Set-Content $Config	-Encoding Ascii
	}       
}
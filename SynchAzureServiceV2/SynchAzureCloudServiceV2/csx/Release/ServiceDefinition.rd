<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="SynchAzureCloudServiceV2" generation="1" functional="0" release="0" Id="3acebb88-30bd-4703-8942-1a0990d69189" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="SynchAzureCloudServiceV2Group" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="SynchRestWebApi:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/LB:SynchRestWebApi:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="QuickBooksIntegrationWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQuickBooksIntegrationWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="QuickBooksIntegrationWorker:SynchStorageConnection" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQuickBooksIntegrationWorker:SynchStorageConnection" />
          </maps>
        </aCS>
        <aCS name="QuickBooksIntegrationWorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQuickBooksIntegrationWorkerInstances" />
          </maps>
        </aCS>
        <aCS name="SynchRestWebApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="SynchRestWebApi:SynchStorageConnection" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapSynchRestWebApi:SynchStorageConnection" />
          </maps>
        </aCS>
        <aCS name="SynchRestWebApiInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapSynchRestWebApiInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:SynchRestWebApi:Endpoint1">
          <toPorts>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapQuickBooksIntegrationWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QuickBooksIntegrationWorker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapQuickBooksIntegrationWorker:SynchStorageConnection" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QuickBooksIntegrationWorker/SynchStorageConnection" />
          </setting>
        </map>
        <map name="MapQuickBooksIntegrationWorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QuickBooksIntegrationWorkerInstances" />
          </setting>
        </map>
        <map name="MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapSynchRestWebApi:SynchStorageConnection" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/SynchStorageConnection" />
          </setting>
        </map>
        <map name="MapSynchRestWebApiInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApiInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="QuickBooksIntegrationWorker" generation="1" functional="0" release="0" software="C:\Users\chhan\Documents\GitHub\Synch-AzureCloudService\SynchAzureServiceV2\SynchAzureCloudServiceV2\csx\Release\roles\QuickBooksIntegrationWorker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="3584" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="SynchStorageConnection" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;QuickBooksIntegrationWorker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;QuickBooksIntegrationWorker&quot; /&gt;&lt;r name=&quot;SynchRestWebApi&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QuickBooksIntegrationWorkerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QuickBooksIntegrationWorkerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QuickBooksIntegrationWorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="SynchRestWebApi" generation="1" functional="0" release="0" software="C:\Users\chhan\Documents\GitHub\Synch-AzureCloudService\SynchAzureServiceV2\SynchAzureCloudServiceV2\csx\Release\roles\SynchRestWebApi" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="3584" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="SynchStorageConnection" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;SynchRestWebApi&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;QuickBooksIntegrationWorker&quot; /&gt;&lt;r name=&quot;SynchRestWebApi&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApiInstances" />
            <sCSPolicyUpdateDomainMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApiUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApiFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="SynchRestWebApiUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="QuickBooksIntegrationWorkerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="QuickBooksIntegrationWorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="SynchRestWebApiFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="QuickBooksIntegrationWorkerInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="SynchRestWebApiInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="3cb0de49-a7bb-430f-a8aa-df70daa3c1ba" ref="Microsoft.RedDog.Contract\ServiceContract\SynchAzureCloudServiceV2Contract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="1ab0bf52-32bb-48bd-a829-9f2baeac9942" ref="Microsoft.RedDog.Contract\Interface\SynchRestWebApi:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>
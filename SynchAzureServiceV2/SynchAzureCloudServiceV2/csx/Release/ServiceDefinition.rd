﻿<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="SynchAzureCloudServiceV2" generation="1" functional="0" release="0" Id="25d2cbe3-fb0c-4bf6-89f8-b852f02a12e4" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
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
        <aCS name="SynchRestWebApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
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
        <map name="MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
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
          <role name="SynchRestWebApi" generation="1" functional="0" release="0" software="C:\Users\chhan\Documents\GitHub\Synch-AzureCloudService\SynchAzureServiceV2\SynchAzureCloudServiceV2\csx\Release\roles\SynchRestWebApi" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="3584" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;SynchRestWebApi&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;SynchRestWebApi&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
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
        <sCSPolicyFaultDomain name="SynchRestWebApiFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="SynchRestWebApiInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="702c0a30-87a8-4353-888d-75eec29fbc73" ref="Microsoft.RedDog.Contract\ServiceContract\SynchAzureCloudServiceV2Contract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="8ef64d55-2cb5-409c-8dd9-7697a2166321" ref="Microsoft.RedDog.Contract\Interface\SynchRestWebApi:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>
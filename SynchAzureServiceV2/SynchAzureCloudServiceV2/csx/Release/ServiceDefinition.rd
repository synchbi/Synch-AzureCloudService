<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="SynchAzureCloudServiceV2" generation="1" functional="0" release="0" Id="ec2487c7-4605-4e50-a485-d83a833ad5ab" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="SynchAzureCloudServiceV2Group" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/LB:QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
        <inPort name="SynchRestWebApi:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/LB:SynchRestWebApi:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="SynchRestWebApi:HttpsEndpoint" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/LB:SynchRestWebApi:HttpsEndpoint" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapCertificate|QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="Certificate|SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapCertificate|SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="Certificate|SynchRestWebApi:SynchEntrustWildcard" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapCertificate|SynchRestWebApi:SynchEntrustWildcard" />
          </maps>
        </aCS>
        <aCS name="QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </maps>
        </aCS>
        <aCS name="QBDIntegrationWorker:SynchStorageConnection" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQBDIntegrationWorker:SynchStorageConnection" />
          </maps>
        </aCS>
        <aCS name="QBDIntegrationWorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapQBDIntegrationWorkerInstances" />
          </maps>
        </aCS>
        <aCS name="SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
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
        <lBChannel name="LB:QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:SynchRestWebApi:Endpoint1">
          <toPorts>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:SynchRestWebApi:HttpsEndpoint">
          <toPorts>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/HttpsEndpoint" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
        <sFSwitchChannel name="SW:SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapCertificate|SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapCertificate|SynchRestWebApi:SynchEntrustWildcard" kind="Identity">
          <certificate>
            <certificateMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/SynchEntrustWildcard" />
          </certificate>
        </map>
        <map name="MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapQBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </setting>
        </map>
        <map name="MapQBDIntegrationWorker:SynchStorageConnection" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/SynchStorageConnection" />
          </setting>
        </map>
        <map name="MapQBDIntegrationWorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorkerInstances" />
          </setting>
        </map>
        <map name="MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapSynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
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
          <role name="QBDIntegrationWorker" generation="1" functional="0" release="0" software="C:\Users\Changhao\Documents\GitHub\Synch-AzureCloudService\SynchAzureServiceV2\SynchAzureCloudServiceV2\csx\Release\roles\QBDIntegrationWorker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SW:QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
              <outPort name="SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SW:SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="SynchStorageConnection" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;QBDIntegrationWorker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;QBDIntegrationWorker&quot;&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;SynchRestWebApi&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;HttpsEndpoint&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorkerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorkerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="SynchRestWebApi" generation="1" functional="0" release="0" software="C:\Users\Changhao\Documents\GitHub\Synch-AzureCloudService\SynchAzureServiceV2\SynchAzureCloudServiceV2\csx\Release\roles\SynchRestWebApi" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="3584" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
              <inPort name="HttpsEndpoint" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/SynchEntrustWildcard" />
                </certificate>
              </inPort>
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SW:QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
              <outPort name="SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SW:SynchRestWebApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="SynchStorageConnection" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;SynchRestWebApi&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;QBDIntegrationWorker&quot;&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;SynchRestWebApi&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;HttpsEndpoint&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EmailAttachmentStorage" defaultAmount="[64,64,64]" defaultSticky="false" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
              <storedCertificate name="Stored1SynchEntrustWildcard" certificateStore="CA" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi/SynchEntrustWildcard" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
              <certificate name="SynchEntrustWildcard" />
            </certificates>
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
        <sCSPolicyUpdateDomain name="QBDIntegrationWorkerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="QBDIntegrationWorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="SynchRestWebApiFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="QBDIntegrationWorkerInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="SynchRestWebApiInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="65e7c791-2694-4a47-a8de-f2b04127aafe" ref="Microsoft.RedDog.Contract\ServiceContract\SynchAzureCloudServiceV2Contract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="43a1e4b8-322d-4fc5-94d5-e9a34de9674d" ref="Microsoft.RedDog.Contract\Interface\QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/QBDIntegrationWorker:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="8dd58d29-a683-4756-8ae3-ba498d7d4b0a" ref="Microsoft.RedDog.Contract\Interface\SynchRestWebApi:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="721864de-99c3-4b40-b4c6-66e0e36fe6be" ref="Microsoft.RedDog.Contract\Interface\SynchRestWebApi:HttpsEndpoint@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/SynchAzureCloudServiceV2/SynchAzureCloudServiceV2Group/SynchRestWebApi:HttpsEndpoint" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>
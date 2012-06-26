<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="CloudNotes.Web.MVC" generation="1" functional="0" release="0" Id="299ac941-48fd-4e11-aef3-be91b0bdd6dd" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="CloudNotes.Web.MVCGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="CloudNotes.WebRole:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/LB:CloudNotes.WebRole:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="CloudNotes.WebRole:CloudNotesSetting" defaultValue="">
          <maps>
            <mapMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/MapCloudNotes.WebRole:CloudNotesSetting" />
          </maps>
        </aCS>
        <aCS name="CloudNotes.WebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/MapCloudNotes.WebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="CloudNotes.WebRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/MapCloudNotes.WebRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:CloudNotes.WebRole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/CloudNotes.WebRole/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapCloudNotes.WebRole:CloudNotesSetting" kind="Identity">
          <setting>
            <aCSMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/CloudNotes.WebRole/CloudNotesSetting" />
          </setting>
        </map>
        <map name="MapCloudNotes.WebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/CloudNotes.WebRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapCloudNotes.WebRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/CloudNotes.WebRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="CloudNotes.WebRole" generation="1" functional="0" release="0" software="D:\CloudNotes\CloudNotes\CloudNotes.Web.MVC\csx\Debug\roles\CloudNotes.WebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="CloudNotesSetting" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;CloudNotes.WebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;CloudNotes.WebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/CloudNotes.WebRoleInstances" />
            <sCSPolicyFaultDomainMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/CloudNotes.WebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="CloudNotes.WebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="CloudNotes.WebRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="9b27c876-682e-4e97-9a96-a1f1d14d30ec" ref="Microsoft.RedDog.Contract\ServiceContract\CloudNotes.Web.MVCContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="8807d4ea-99ca-4ef0-95f7-94560ec2aff1" ref="Microsoft.RedDog.Contract\Interface\CloudNotes.WebRole:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/CloudNotes.Web.MVC/CloudNotes.Web.MVCGroup/CloudNotes.WebRole:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>
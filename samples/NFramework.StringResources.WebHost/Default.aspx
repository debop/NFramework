<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NSoft.NFramework.StringResources.WebHost.Default" meta:resourcekey="PageResource1" %>

<asp:Content ID="contentMainImpl" ContentPlaceHolderID="contentMain" runat="server">
    <h1>
        <asp:Localize ID="locHeading" runat="server" meta:resourcekey="locHeadingResource1" Text="예제 설명"></asp:Localize>
    </h1>
    <p>
        <asp:Localize ID="locContent" 
                      runat="server" 
                      meta:resourcekey="locContentResource1" Text="로칼라이제이션 코드 샘플입니다. 여기서는 데이타베이스를 이용한 사용자 정의 리소스 제공자에 대한 사용에 대해 설명합니다. 네비게이션 메뉴를 선택하면 예제를 수행할 수 있습니다."></asp:Localize>
        <br />
        <br />
        Castle.Windsor IoC 를 이용하므로, Windsor.ResourceProviders.config에서 다음과 같은 값을 변경해 가면서 작업할 수 있다.
        <br />
        <textarea readonly cols="80" rows="15">
<component id="ResourceProviderFactory"
   service="NSoft.NFramework.StringResources.IResourceProviderFactory, NSoft.NFramework.StringResources"
   type="NSoft.NFramework.StringResources.DefaultResourceProviderFactory, NSoft.NFramework.StringResources">
	<parameters>
		<culture>#{culture}</culture>

	<GlobalResourceProviderName>FileResourceProvider</GlobalResourceProviderName>
	<?if DEBUG?>
	<LocalResourceProviderName>ExternalResourceProvider</LocalResourceProviderName>
	<?else?>
	<LocalResourceProviderName>NHResourceProvider</LocalResourceProviderName>
	<?end?>

  </parameters>
</component>
		</textarea>
        에서 GlobalResourceProviderName 값을 원하는 Provider로 변경하면 된다.
    </p>
    <p>
        <asp:Localize ID="locContent2" runat="server" meta:resourcekey="locContent2Resource1" Text="
      다른 컬처에 대한 테스트를 하려면 web.config 파일의 &amp;lt;globalization&amp;gt; 섹션의 
      uiCulture 를 'auto'에서 'en', 'en-US', 'en-CA', 'ko-KR' 등으로 변경하고, 
      각 페이지들의 결과를 확인해 보십시요.
		"></asp:Localize>
    </p>
    <p>
        <i>
            <asp:Localize ID="locNotes" runat="server" meta:resourcekey="locNotesResource1" Text="
      주의 : 만약 사용자 리소스 제공자 설정 정보를 제거하면, 기본적으로 App_GlobalResources, App_LocalResources에 있는 .resx 파일에 저장된 정보로 대체한다.
			"></asp:Localize>
        </i>
    </p>
</asp:Content>
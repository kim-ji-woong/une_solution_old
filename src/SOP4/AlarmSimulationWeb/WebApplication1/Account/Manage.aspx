<%@ Page Title="계정 관리" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="WebApplication1.Account.Manage" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>

    <div>
        <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
            <p class="text-success"><%: SuccessMessage %></p>
        </asp:PlaceHolder>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="form-horizontal">
                <h4>계정 설정 변경</h4>
                <hr />
                <dl class="dl-horizontal">
                    <dt>암호:</dt>
                    <dd>
                        <asp:HyperLink NavigateUrl="/Account/ManagePassword" Text="[변경]" Visible="false" ID="ChangePassword" runat="server" />
                        <asp:HyperLink NavigateUrl="/Account/ManagePassword" Text="[만들기]" Visible="false" ID="CreatePassword" runat="server" />
                    </dd>
                    <dt>외부 로그인:</dt>
                    <dd><%: LoginsCount %>
                        <asp:HyperLink NavigateUrl="/Account/ManageLogins" Text="[관리]" runat="server" />

                    </dd>
                    <%--
                        2단계 인증 시스템에서는 두 번째 확인 단계로 전화 번호를 사용할 수 있습니다.
                        SMS를 사용하여 2단계 인증을 지원하도록 이 ASP.NET 응용 프로그램을 설정하는 방법에 대한
                        자세한 내용은 <a href="http://go.microsoft.com/fwlink/?LinkId=403804">이 문서</a>를 참조하십시오.
                        2단계 인증을 설정한 후에는 다음 블록의 주석 처리를 제거하십시오.
                    --%>
                    <%--
                    <dt>전화 번호:</dt>
                    <% if (HasPhoneNumber)
                       { %>
                    <dd>
                        <asp:HyperLink NavigateUrl="/Account/AddPhoneNumber" runat="server" Text="[추가]" />
                    </dd>
                    <% }
                       else
                       { %>
                    <dd>
                        <asp:Label Text="" ID="PhoneNumber" runat="server" />
                        <asp:HyperLink NavigateUrl="/Account/AddPhoneNumber" runat="server" Text="[변경]" /> &nbsp;|&nbsp;
                        <asp:LinkButton Text="[제거]" OnClick="RemovePhone_Click" runat="server" />
                    </dd>
                    <% } %>
                    --%>

                    <dt>2단계 인증:</dt>
                    <dd>
                        <p>
                            구성된 2단계 인증 공급자가 없습니다. 2단계 인증을 지원하도록 이 ASP.NET 응용 프로그램을 설정하는 방법에 대한
                            자세한 내용은 <a href="http://go.microsoft.com/fwlink/?LinkId=403804">이 문서</a>를 참조하십시오.
                        </p>
                        <% if (TwoFactorEnabled)
                          { %> 
                        <%--
                        사용
                        <asp:LinkButton Text="[사용 안 함]" runat="server" CommandArgument="false" OnClick="TwoFactorDisable_Click" />
                        --%>
                        <% }
                          else
                          { %> 
                        <%--
                        사용 안 함
                        <asp:LinkButton Text="[사용]" CommandArgument="true" OnClick="TwoFactorEnable_Click" runat="server" />
                        --%>
                        <% } %>
                    </dd>
                </dl>
            </div>
        </div>
    </div>

</asp:Content>

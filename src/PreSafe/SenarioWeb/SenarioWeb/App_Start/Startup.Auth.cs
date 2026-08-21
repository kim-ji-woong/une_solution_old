using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;
using Owin;
using SenarioWeb.Models;

namespace SenarioWeb
{
    public partial class Startup {

        // 인증을 구성하는 방법에 대한 자세한 내용을 보려면 http://go.microsoft.com/fwlink/?LinkId=301883을 방문하십시오.
        public void ConfigureAuth(IAppBuilder app)
        {
            // 요청당 단일 인스턴스를 사용하도록 db 컨텍스트와 사용자 관리자 구성
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // 응용 프로그램이 쿠키를 사용하여 로그인된 사용자에 대한 정보를 저장할 수 있도록 합니다.
            // 또한 쿠키를 사용하여 타사 로그인 공급자를 통한 사용자 로그인 관련 정보를 일시적으로 저장할 수 있도록 합니다.
            // 쿠키에 서명 구성
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(20),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            // 쿠키를 사용하여 타사 로그인 공급자를 통한 사용자 로그인 관련 정보를 일시적으로 저장
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // 타사 로그인 공급자로 로그인할 수 있으려면 다음 줄의 주석 처리를 제거하십시오.
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}

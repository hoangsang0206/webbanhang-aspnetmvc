using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.DynamicData;
using STech_Web.Identity;
using System.Web.Services.Description;

[assembly: OwinStartup(typeof(STech_Web.Startup))]

namespace STech_Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/")
            });

            this.CreateRolesAndUsers();
        }

        //Crate user role
        public void CreateRolesAndUsers()
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(
                    new AppDBContext()
                    )
                );
            var appDpContext = new AppDBContext();
            var appUserStore = new AppUserStore(appDpContext);
            var userManager = new AppUserManager(appUserStore);

            //Kiểm tra nếu chưa tồn tại role Admin thì tạo role Admin
            //và 1 tài khoản admin
            if(!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            if(userManager.FindByName("admin") == null)
            {
                var user = new AppUser();
                user.UserName = "admin";
                user.Email = "admin@lehoangsang.com";
                string password = "admin@2001210561";

                var checkUser = userManager.Create(user, password);
                if(checkUser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }

            //Kiểm tra nếu chưa tồn tại role Manager thì tạo role Manager
            //và 1 tài khoản manager
            if (!roleManager.RoleExists("Manager"))
            {
                var role = new IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);
            }

            if (userManager.FindByName("manager") == null)
            {
                var user = new AppUser();
                user.UserName = "manager";
                user.Email = "manager@lehoangsang.com";
                string password = "manager@2001210561";

                var checkUser = userManager.Create(user, password);
                if (checkUser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Manager");
                }
            }

            //Kiểm tra nếu chưa tồn tại role Customer thì tạo role Customer
            if (!roleManager.RoleExists("Customer"))
            {
                var role = new IdentityRole();
                role.Name = "Customer";
                roleManager.Create(role);
            }
        }
    }
}

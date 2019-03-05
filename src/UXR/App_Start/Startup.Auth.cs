using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Microsoft.Owin;
using Ninject.Web.Common.OwinHost;
using Ninject;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject.Web.Common;
using UXI.CQRS;
using UXI.CQRS.Commands;
using UXR.Studies.Models;
using UXR.Common;
using UXR.Models;
using UXR.Models.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Data.Entity;
using Microsoft.Owin.Security.DataProtection;

[assembly: OwinStartup(typeof(UXR.Startup))]

namespace UXR
{
    public partial class Startup
    {
        /// <summary>  
        /// Ninject kernel for dependency injection.  
        /// </summary>  
        private IKernel kernel = null;

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            kernel = CreateKernel();
            app.UseNinjectMiddleware(() => kernel);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                var resolver = new Common.NinjectDependencyResolver(kernel);
                System.Web.Mvc.DependencyResolver.SetResolver(resolver); // MVC
                // Set the Ninject-based IDependencyResolver into the Web API config
                GlobalConfiguration.Configuration.DependencyResolver = resolver;


                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Loads modules and registers services for the application. If a new module is added, this method should be updated.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ITimeProvider>().To<TimeProvider>().InRequestScope();

            kernel.Bind<IStudiesDbContext, IIdentityDbContext<ApplicationUser>, DbContext>()
                  .To<UXRDbContext>()
                  .InRequestScope();

            kernel.Bind<ICommandHandlerResolver>()
                  .To<NinjectCommandHandlerResolver>()
                  .InSingletonScope();

            kernel.Bind<IUserStore<ApplicationUser>>().ToMethod((context) =>
            {
                return new UserStore<ApplicationUser>(kernel.Get<DbContext>());
            }).InRequestScope();

            kernel.Bind<ApplicationUserManager>().ToMethod((context) =>
            {
                var options = context.Kernel.Get<IdentityFactoryOptions<ApplicationUserManager>>(); 
                options.DataProtectionProvider = options.DataProtectionProvider 
                                                 ?? new DpapiDataProtectionProvider("UXR");
                var userStore = context.Kernel.Get<IUserStore<ApplicationUser>>();
                return ApplicationUserManager.Create(options, userStore);
            }).InRequestScope();

            kernel.Bind<IAuthenticationManager>().ToMethod(context =>
            {
                var contextBase = new HttpContextWrapper(HttpContext.Current);
                return contextBase.GetOwinContext().Authentication;
            }).InRequestScope();

            kernel.Bind<ApplicationSignInManager>().ToMethod((context) =>
            {
                var options = context.Kernel.Get<IdentityFactoryOptions<ApplicationSignInManager>>();
                var userManager = context.Kernel.Get<ApplicationUserManager>();
                var authenticatioManager = context.Kernel.Get<IAuthenticationManager>();
                return ApplicationSignInManager.Create(options, userManager, authenticatioManager);
            }).InRequestScope();

            kernel.Bind<CommandDispatcher>().ToSelf().InSingletonScope();

            kernel.Load
            (
                new UXR.Modules.IdentityDbModule(),

                new UXR.Studies.Modules.StudiesDbModule(),
                new UXR.Studies.Modules.CommandHandlersModule(),
                new UXR.Studies.Modules.FilesModule()
            );
        }
    }
}

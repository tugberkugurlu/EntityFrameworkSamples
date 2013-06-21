using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.WebApi;
using EFConcurrentAsyncSample.Api.Models;
using EFConcurrentAsyncSample.Data.Core;
using System;
using System.Reflection;
using System.Web.Http;
using EFConcurrentAsyncSample.Data.Entities;

namespace EFConcurrentAsyncSample.Api
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.MessageHandlers.Add(new MyDummyHandler());
            config.DependencyResolver = new AutofacWebApiDependencyResolver(RegisterServices(new ContainerBuilder()));
            config.Routes.MapHttpRoute("DefaultRoute", "api/{controller}");

            Mapper.CreateMap<Room, RoomDto>();
            Mapper.CreateMap<Session, SessionDto>();
            Mapper.CreateMap<Person, PersonDto>();
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<ConfContext>().InstancePerApiRequestOrOwned();

            return builder.Build();
        }
    }

    public class MyDummyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ConfContext ctx = (ConfContext)request.GetDependencyScope().GetService(typeof(ConfContext));
            return base.SendAsync(request, cancellationToken);
        }
    }

    public static class RegistrationExtensions
    {
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle>
            InstancePerApiRequestOrOwned<TLimit, TActivatorData, TStyle>(
                this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration)
        {
            if (registration == null) throw new ArgumentNullException("registration");

            var tags = new object[] {AutofacWebApiDependencyResolver.ApiRequestTag, new TypedService(typeof(TLimit))};

            return registration.InstancePerMatchingLifetimeScope(tags);
        }
    }
}
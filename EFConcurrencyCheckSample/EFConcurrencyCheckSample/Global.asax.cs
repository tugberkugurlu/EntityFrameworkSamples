using System;
using System.Web.Http;
using System.Web.Http.Validation;
using System.Web.Http.Validation.Providers;
using AutoMapper;
using EFConcurrencyCheckSample.Entities;
using EFConcurrencyCheckSample.Models;
using WebApiDoodle.Web.Filters;

namespace EFConcurrencyCheckSample
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Configure AutoMapper
            Mapper.CreateMap<Person, PersonDto>();
            Mapper.CreateMap<PersonRequestModel, Person>();

            // Configure Web API
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Filters.Add(new InvalidModelStateFilterAttribute());
            config.Routes.MapHttpRoute("DefaultRoute", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.EnableSystemDiagnosticsTracing();

            // Remove all the validation providers 
            // except for DataAnnotationsModelValidatorProvider
            config.Services.RemoveAll(typeof(ModelValidatorProvider), validator => !(validator is DataAnnotationsModelValidatorProvider));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Owin;

namespace EFConcurrencyCheckSample.Controllers
{
    public class PeopleController : ApiController
    {
        public string[] GetPeople()
        {
            return new[]
            {
                "Person 1",
                "Person 2",
                "Person 3"
            };
        }
    }
}
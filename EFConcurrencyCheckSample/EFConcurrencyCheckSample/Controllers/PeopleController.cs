using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using AutoMapper;
using EFConcurrencyCheckSample.Entities;
using EFConcurrencyCheckSample.Models;

namespace EFConcurrencyCheckSample.Controllers
{
    public class PeopleController : ApiController
    {
        public IEnumerable<PersonDto> GetPeople()
        {
            using (ConfContext _confContext = new ConfContext())
            {
                Person[] people = _confContext.People.ToArray();
                return Mapper.Map<IEnumerable<Person>, IEnumerable<PersonDto>>(people);
            }
        }

        public HttpResponseMessage GetPerson(int id)
        {
            using (ConfContext _confContext = new ConfContext())
            {
                Person person = _confContext.People.FirstOrDefault(personEntity => personEntity.Id == id);
                if (person == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                PersonDto personDto = Mapper.Map<Person, PersonDto>(person);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, personDto);
                response.Headers.ETag = new EntityTagHeaderValue(string.Concat("\"", Convert.ToBase64String(person.Timestamp, Base64FormattingOptions.None), "\""));

                return response;
            }
        }

        public HttpResponseMessage PostPerson(PersonRequestModel requestModel)
        {
            using (ConfContext _confContext = new ConfContext())
            {
                Person person = Mapper.Map<PersonRequestModel, Person>(requestModel);
                _confContext.People.Add(person);
                int result = _confContext.SaveChanges();
                if(result > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, Mapper.Map<Person, PersonDto>(person));
                }

                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }

        public HttpResponseMessage PutPerson(int id, PersonRequestModel requestModel)
        {
            // Change tracker has a deep knowladge on the Timestamp property and
            // it sends the first retrieved Timestamp no matter what the latter value is.
            // That's why we are doing the operations with two seperate context objects.
            // ref: http://stackoverflow.com/questions/4402586/optimisticconcurrencyexception-does-not-work-in-entity-framework-in-certain-situ
            // http://stackoverflow.com/questions/13581473/why-does-the-objectstatemanager-property-not-exist-in-my-db-context

            using (ConfContext _confContext = new ConfContext())
            {
                Person existingPerson = _confContext.People.FirstOrDefault(personEntity => personEntity.Id == id);
                if (existingPerson == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                string ifMatchHeader;
                if (Request.Headers.IfMatch.TryGetETag(out ifMatchHeader))
                {
                    //ObjectStateManager objectStateManager = ((IObjectContextAdapter)_confContext).ObjectContext.ObjectStateManager;
                    //objectStateManager.ChangeObjectState(existingPerson, EntityState.Detached);
                    _confContext.Entry(existingPerson).State = EntityState.Detached; 

                    ifMatchHeader = ifMatchHeader.Trim('"');
                    byte[] timestamp = Convert.FromBase64String(ifMatchHeader);
                    existingPerson.Timestamp = timestamp;
                    Person person = Mapper.Map<PersonRequestModel, Person>(requestModel, existingPerson);

                    try
                    {
                        _confContext.Entry(person).State = EntityState.Modified;
                        _confContext.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        const string message = "The record you attempted to edit was modified by another user after you got the original value.";
                        return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, message);
                    }
                    catch (DataException)
                    {
                        //Log the error (add a variable name after Exception)
                        const string message = "Unable to save changes. Try again, and if the problem persists contact your system administrator.";
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, message);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<Person, PersonDto>(person));
                }

                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "If-Match header needs to be provided in order to update the entity.");
            }
        }
    }

    internal static class Extensions
    {
        internal static bool TryGetETag(this IEnumerable<EntityTagHeaderValue> headers, out string eTag)
        {
            eTag = null;
            EntityTagHeaderValue eTagHeader = headers.FirstOrDefault();
            if(eTagHeader == null)
            {
                return false;
            }

            eTag = eTagHeader.Tag;
            return true;
        }
    }
}
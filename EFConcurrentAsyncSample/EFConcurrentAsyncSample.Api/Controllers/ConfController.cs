using Autofac.Features.OwnedInstances;
using AutoMapper;
using EFConcurrentAsyncSample.Api.Models;
using EFConcurrentAsyncSample.Data.Core;
using EFConcurrentAsyncSample.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace EFConcurrentAsyncSample.Api.Controllers
{
    // ref: https://code.google.com/p/autofac/wiki/OwnedInstances#Combining_Owned_with_Func
    public class ConfController : ApiController
    {
        private readonly ConfContext _confContext;
        private readonly Func<Owned<ConfContext>> _confContextFactory;

        public ConfController(ConfContext confContext, Func<Owned<ConfContext>> confContextFactory)
        {
            _confContext = confContext;
            _confContextFactory = confContextFactory;
        }

        public async Task<ConfDto> GetConfDto()
        {
            List<Owned<ConfContext>> confContexts = Enumerable.Range(1, 3).Select(_ => _confContextFactory()).ToList();

            var foo = object.ReferenceEquals(confContexts[0].Value, confContexts[1].Value);

            Task<Person[]> peopleTask = confContexts[0].Value.People.ToArrayAsync();
            Task<Room[]> roomsTask = confContexts[1].Value.Rooms.ToArrayAsync();
            Task<Session[]> sessionsTask = confContexts[2].Value.Sessions.ToArrayAsync();

            try
            {
                await Task.WhenAll(peopleTask, roomsTask, sessionsTask);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                confContexts.ForEach(ownedCtx => ownedCtx.Dispose());
            }

            return new ConfDto
            {
                People = Mapper.Map<IEnumerable<Person>, IEnumerable<PersonDto>>(peopleTask.Result),
                Rooms = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomDto>>(roomsTask.Result),
                Sessions = Mapper.Map<IEnumerable<Session>, IEnumerable<SessionDto>>(sessionsTask.Result)
            };
        }
    }
}
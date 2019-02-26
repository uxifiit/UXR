using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.Api.Entities;
using UXR.Studies.Api.Entities.Sessions;
using UXR.Studies.Models;
using Newtonsoft.Json.Linq;

namespace UXR.Studies.Api.MapperProfiles
{
    public class SessionApiProfile : Profile
    {
        public SessionApiProfile()
        {
            CreateMap<Session, SessionInfo>()
                  .ForMember(s => s.Project,
                             e => e.MapFrom(session => session.Project.Name))
                  .ForMember(s => s.Owner,
                             e => e.MapFrom(session => session.Project.Owner.UserName))
                  .ForMember(s => s.Description,
                             e => e.MapFrom(session => session.Project.Description))
                  .ForMember(s => s.CreatedAt,
                             e => e.ResolveUsing(s => s.UpdatedAt?.ToLocalTime() ?? s.CreatedAt?.ToLocalTime() ?? DateTime.Now))
                  .ForMember(s => s.Definition,
                             e => e.ResolveUsing(s => new JRaw(JToken.Parse(s.Definition ?? s.Project.SessionDefinitionTemplate ?? String.Empty))));
        }
    }
}

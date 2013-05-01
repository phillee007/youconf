using System;
using System.Collections.Generic;
using YouConf.Data.Entities;
namespace YouConf.Data
{
    public interface IYouConfDataContext
    {
        IEnumerable<Conference> GetAllConferences();
        void UpsertConference(Conference conference);
        Conference GetConference(string hashtag);
    }
}

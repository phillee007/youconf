using System;
using System.Collections.Generic;
using YouConf.Data.Entities;
namespace YouConf.Data
{
    public interface IYouConfDataContext
    {
        Conference GetConference(string hashtag);
        IEnumerable<Conference> GetAllConferences();
        void UpsertConference(string hashTag, Conference conference);
        void DeleteConference(string hashTag);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace YouConf.SignalRHubs
{
    public class YouConfHub : Hub
    {
        public Task UpdateConferenceVideoUrl(string conferenceHashTag, string url)
        {
            //Only update the clients for the specific conference 
            return Clients.All.updateConferenceVideoUrl(url);
        }

        public Task Join(string conferenceHashTag)
        {
            return Groups.Add(Context.ConnectionId, conferenceHashTag);
        }

    }
}

using SolrNet;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouConf.Common.Data;
using YouConf.Common.Messaging;
using YouConfWorker.Data.SolrEntities;

namespace YouConfWorker.MessageHandlers
{
    public class UpdateSolrIndexMessageHandler : IMessageHandler<UpdateSolrIndexMessage>
    {
        public IYouConfDbContext Db { get; set; }
        public ISolrOperations<ConferenceDto> Solr { get; set; }

        public UpdateSolrIndexMessageHandler(IYouConfDbContext db, ISolrOperations<ConferenceDto> solr)
        {
            Db = db;
            Solr = solr;
        }

        public void Handle(UpdateSolrIndexMessage message)
        {
            if (message.Action == SolrIndexAction.Delete)
            {
                Trace.WriteLine(String.Format("Deleting conference: {0}", message.ConferenceId.ToString()));
                Solr.Delete(message.ConferenceId.ToString());
            }
            else
            {
                Trace.WriteLine(String.Format("Updating conference: {0}", message.ConferenceId.ToString()));
                var conference = Db.Conferences.First(x => x.Id == message.ConferenceId);
                if (conference.AvailableToPublic)
                {
                    Solr.Add(new ConferenceDto()
                    {
                        ID = conference.Id,
                        HashTag = conference.HashTag,
                        Title = conference.Name,
                        Content = conference.Abstract + " " + conference.Description,
                        Speakers = conference.Speakers
                            .Select(x => x.Name)
                            .ToList()
                    });
                }
            }
            Solr.Commit();
        }
    }
}

using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YouConfWorker.Data.SolrEntities
{
    public class ConferenceDto
    {
        [SolrUniqueKey("id")]
        public int ID { get; set; }

        [SolrUniqueKey("hashtag")]
        public string HashTag { get; set; }

        [SolrField("title")]
        public string Title { get; set; }

        [SolrField("content")]
        public string Content { get; set; }

        [SolrField("cat")]
        public ICollection<string> Speakers { get; set; }
    }
}
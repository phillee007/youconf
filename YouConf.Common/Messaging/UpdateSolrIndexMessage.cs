using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouConf.Common.Messaging
{
    public enum SolrIndexAction
    {
        Delete = 0,
        Update = 1
    }

    public class UpdateSolrIndexMessage
    {
        public int ConferenceId { get; set; }
        public SolrIndexAction Action { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouConf.Common.Messaging
{
    public interface IMessageHandler<T> where T : class
    {
        void Handle(T message);
    }
}

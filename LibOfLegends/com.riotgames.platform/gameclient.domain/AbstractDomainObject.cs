using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.gameclient.domain
{
    class AbstractDomainObject
    {
        public AbstractDomainObject()
        {
        }

        public object futureData;
        public object dataVersion;
        public object count;
    }
}


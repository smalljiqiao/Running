using System;
using System.Collections.Generic;
using System.Text;

namespace HB.Common.Core.Framework.Helpers.SnowFlake
{
    public class InvalidSystemClock : Exception
    {
        public InvalidSystemClock(string message) : base(message) { }
    }
}

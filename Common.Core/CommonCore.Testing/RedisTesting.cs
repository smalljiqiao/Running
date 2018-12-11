using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedisDataCoreDomain;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonCore.Testing
{
    [TestClass]
   public class RedisTesting
    {
        [TestMethod]
        public void addData() {
            RedisHelper redisHelper = new RedisHelper();
            redisHelper.Add("testC:nothing","timeYound");


        }


    }
}

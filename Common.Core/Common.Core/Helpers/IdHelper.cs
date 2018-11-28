using System;
using System.Collections.Generic;
using System.Text;
using HB.Common.Core.Framework.Helpers.SnowFlake;

namespace HB.Common.Core.Framework.Helpers
{
    /// <summary>
    /// Id帮助类
    /// </summary>
    public class IdHelper
    {
        private const long WorkerMask = 0x000000000001F000L;
        private const long DatacenterMask = 0x00000000003E0000L;
        private const ulong TimestampMask = 0xFFFFFFFFFFC00000UL;
        private static IdHelper _instance;
        private static readonly object obj_lock = new object();
        IdWorker worker = null;
        private IdHelper()
        {
            worker = new IdWorker(1, 1);
        }

        public static IdHelper Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (obj_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new IdHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取分布式Id(雪花算法)
        /// </summary>
        /// <returns></returns>
        public long GetDFSId()
        {

            var v = worker.NextId();
            return v;
        }

        /// <summary>
        /// 获取图片Id
        /// </summary>
        /// <returns></returns>
        public string GetImgId()
        {
            String imgId = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            var dfsId = this.GetDFSId().ToString();
            return string.Format("{0}_{1}", imgId, dfsId.Substring(dfsId.Length - 4));
        }


        /// <summary>
        /// 获取去掉横岗的Guid
        /// </summary>
        /// <returns></returns>
        public static string GetGuid() {
          return  Guid.NewGuid().ToString().Replace("-", "");
        }
        /// <summary>
        /// 创建用户账号（随机数）
        /// </summary>
        /// <returns></returns>
        public static string CreateB2CUserAccount()
        {
            Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            string result = r.Next(1000000, 9999999).ToString();
            return result;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrlToShort.Models
{
    public class Route
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 短地址
        /// </summary>
        public string Short { get; set; }

        /// <summary>
        /// 源地址
        /// </summary>
        public string Long { get; set; }

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 请求IP
        /// </summary>
        public string IP { get; set; }
    }
}
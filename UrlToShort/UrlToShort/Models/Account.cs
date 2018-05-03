using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrlToShort.Models
{
    public class Account
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pass { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPass { get; set; }
    }
}
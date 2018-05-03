using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text.RegularExpressions;
using System.Net;

namespace UrlToShort.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            object obj = RouteData.Values["id"];
            string key=obj==null?"":obj.ToString();
            if (key == "")
            {
                return View();
            }
            else
            {
                string sql = "SELECT Long FROM Route WHERE Short = '" + key + "'";
                string url= (new DBHelper()).GetFirst(sql).ToString();
                if (url == "")
                {
                    return View();
                }
                else
                {
                    return Redirect(url);
                }
            }
        }

        /// <summary>
        /// 缩短操作
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public JsonResult ToShort(Models.Route route)
        {
            bool res = false;
            string msg = "";

            //验证
            if (string.IsNullOrEmpty(route.Long))
            {
                msg = "请输入需要转换的源地址！";
                goto Last;
            }
            if (!UrlIsOK(route.Long))
            {
                msg = "您输入的地址无法访问！";
                goto Last;
            }

            DBHelper db = new DBHelper();

            //检测存在
            string sqlURL = "SELECT Short FROM Route WHERE Long = '" + route.Long + "'";
            string url = db.GetFirst(sqlURL).ToString();
            if (url != "")
            {
                res = true;
                msg = url;
                goto Last;
            }

            //是否自动生成短网址
            route.IP = Request.UserHostAddress.ToString();
            if (string.IsNullOrEmpty(route.Short))
            {
                string name = MakeName(route.Long, route.IP);
                res = name == "False" ? false : true;
                if (res)
                {
                    msg = name;
                }
                else
                {
                    msg = "系统发生异常，转换失败！";
                }
            }
            else
            {
                if (route.Short.ToLower() == "admin" || route.Short.ToLower() == "manager")
                {
                    msg = "您输入的域名已被占用！";
                    goto Last;
                }
                //执行增加
                string sql = "SELECT Short FROM Route WHERE Short ='" + route.Short + "'";
                if (db.GetFirst(sql) != "")
                {
                    msg = "您输入的域名已被占用！";
                    goto Last;
                }
                sql = "INSERT INTO Route VALUES(NULL,'" + route.Long + "','" + route.Short + "',DATETIME('now'),'" + route.IP + "')";
                if (db.GetLine(sql) == 1)
                {
                    res = true;
                    msg = route.Short; ;
                }
                else
                {
                    res = false;
                    msg = "系统发生异常，转换失败！";
                }

            }

        Last: return Json(
            new
            {
                result = res,
                message = msg
            });
        }

        /// <summary>
        /// 获取短网址
        /// </summary>
        bool NameUsed = false; //重名状态
        private string MakeName(string url, string ip)
        {
            string name = string.Empty; //名称缓存
            //名称基单元
            char[] NBase = new char[] { '0','1','2','3','4','5','6','7','8','9',
                '_','A','a','B','b','C','c','D','d','E','e','F','f','G','g','H',
                'h','I','i','J','j','K','k','L','l','M','m','N','n','O','o','P',
                'p','Q','q','R','r','S','s','T','t','U','u','V','v','W','w','X',
                'x','Y','y','Z','z'};
            //依据当前日期时间计算名称
            DateTime now = DateTime.Now;
            int year = int.Parse(now.Year.ToString().Substring(2)); //年
            name = NBase[year - 18].ToString(); //以2018年为基础
            int month = now.Month; //月
            if (NameUsed)
            {
                Random ran = new Random(); //拓展资源使用
                month += ran.Next(1, 49);
            }
            name += NBase[month].ToString();
            int day = now.Day; //日
            if (NameUsed)
            {
                Random ran = new Random(); //拓展资源使用
                day += ran.Next(1, 31);
            }
            name += NBase[day].ToString();
            int hour = now.Hour; //时
            if (NameUsed)
            {
                Random ran = new Random(); //拓展资源使用
                hour += ran.Next(1, 37);
            }
            name += NBase[hour].ToString();
            int minute = now.Minute; //分
            name += NBase[minute].ToString();
            int second = now.Second; //秒
            name += NBase[second].ToString();
            //重名检测
            string sql = "SELECT Short FROM Route WHERE Short ='" + name + "'";
            sql = (new DBHelper()).GetFirst(sql);
            NameUsed = sql == "" ? false : true;
            //name更新
            if (NameUsed)
            {
                name = MakeName(url, ip);
            }
            //执行增加
            sql = "SELECT Short FROM Route WHERE Short ='" + name + "'";
            if ((new DBHelper()).GetFirst(sql) != "")
                goto Last; //确保迭代中SQL只被执行一次
            sql = "INSERT INTO Route VALUES(NULL,'" + url + "','" + name + "',DATETIME('now'),'" + ip + "')";
            if ((new DBHelper()).GetLine(sql) != 1)
                name = "False";
        //返回名称
        Last: return name;
        }

        /// <summary>
        /// 网址有效性检查
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool UrlIsOK(string ip)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(ip);
                myRequest.Method = "HEAD";
                myRequest.Timeout = 5000;  //超时时间5秒
                HttpWebResponse res = (HttpWebResponse)myRequest.GetResponse();
                return (res.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 还原网址
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public JsonResult ToLong(Models.Route route)
        {
            bool res = false;
            string msg = "";

            if (string.IsNullOrEmpty(route.Short))
            {
                msg = "请输入您要转换的短网址！";
                goto Last;
            }
            int index = route.Short.LastIndexOf('/');
            route.Short = route.Short.Substring(index+1);
            string sql = "SELECT Long FROM Route WHERE Short = '"+route.Short+"'";
            sql = (new DBHelper()).GetFirst(sql).ToString();
            if (sql == "")
            {
                msg = "您输入的短网址不存在！";
            }
            else
            {
                res = true;
                msg = sql;
            }

        Last: return Json(new
        {
            result = res,
            message = msg
        });
        }

    }
}

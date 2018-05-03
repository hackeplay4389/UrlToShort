using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UrlToShort.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/
        /// <summary>
        /// 登录界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Login(Models.Account a)
        {
            bool res = false;
            string msg = "";
            if (string.IsNullOrEmpty(a.Name) || string.IsNullOrEmpty(a.Pass))
            {
                msg = "请输入登录信息！";
                goto last;
            }
            DBHelper db = new DBHelper();
            string pass = db.GetFirst("select Pass from Account where Name = '" + a.Name + "'").ToString();
            if (pass != a.Pass)
            {
                msg = "账号或密码错误！";
            }
            else
            {
                Session["login_name"] = a.Name;
                res = true;
                msg = "正在登录...";
                Session["login"] = "_login_true";
            }
        last: return Json(new
        {
            result = res,
            message = msg
        });
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public RedirectResult OutLogin()
        {
            Authorize();
            Session["login_name"] = null;
            Session["login"] = null;
            return Redirect("/admin");
        }

        /// <summary>
        /// 管理界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Manager()
        {
            Authorize();
            //搜索判断
            object key = Request.QueryString["key"];
            string sql = ""; ;
            if (key == null)
            {
                sql = "SELECT * FROM Route";
            }
            else
            {
                key = key.ToString();
                sql = "SELECT * FROM Route WHERE (ID LIKE '%" + key + "%') or (Long LIKE '%" + key + "%') or (Short LIKE '%" + key + "%') or (Time LIKE '%" + key + "%') or (IP LIKE '%" + key + "%')";
            }
            System.Data.DataTable data = (new DBHelper()).GetDT(sql);
            return View(data);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Del()
        {
            Authorize();
            //操作状态
            bool res = false;
            string id = Request.QueryString["id"] == null ? "0" : Request.QueryString["id"].ToString();
            string sql = "DELETE FROM Route WHERE ID=" + id;
            if ((new DBHelper()).GetLine(sql) == 1)
                res = true;
            return Json(new
            {
                result = res
            });
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Change(Models.Account user)
        {
            Authorize();
            bool res = false;
            string msg = "";

            if (string.IsNullOrEmpty(user.Pass)||string.IsNullOrEmpty(user.NewPass))
            {
                msg = "您输入的密码不能为空!";
                goto Last;
            }
            if (user.Name != user.NewPass) //借助Name传递密码1
            {
                msg = "两次输入的密码不一致!";
                goto Last;
            }
            if (user.NewPass.Length < 6)
            {
                msg = "新密码过于简单!";
                goto Last;
            }
            DBHelper db=new DBHelper();
            string sql = "SELECT Pass FROM Account WHERE Name='" + Session["login_name"].ToString()+ "'";
            string pass = db.GetFirst(sql);
            if (user.Pass != pass)
            {
                msg = "您输入的登录密码有误！";
                goto Last;
            }
            string upPass = "UPDATE Account SET Pass='"+user.NewPass+"' WHERE Name='" + Session["login_name"].ToString() + "'";
            if (db.GetLine(upPass) == 1)
            {
                res = true;
                msg = "恭喜您，密码修改成功！";
            }
            else
            {
                res = false;
                msg = "系统异常，修改失败！";
            }

            Last: return Json(new { 
                result=res,
                message=msg
            });
        }

        /// <summary>
        /// 身份安全验证
        /// </summary>
        private void Authorize()
        {
            if ((Session["login"] == null) || (Session["login"].ToString() != "_login_true"))
                Response.Redirect("/manager");
        }

    }
}

using QuanLyGaraOto.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyGaraOto.Controllers
{
    public class DangNhapController : Controller
    {
        private AutoGaraEntities db = new AutoGaraEntities();
        // GET: DangNhap
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult frmLogin(TaiKhoanNhanVien model)
        {
            if (db.TaiKhoanNhanViens.Count(x => x.TenDN == model.TenDN && x.MatKhau == model.MatKhau) > 0)
            {
                Session["nhanvien"] = db.TaiKhoanNhanViens.SingleOrDefault(x => x.TenDN == model.TenDN && x.MatKhau == model.MatKhau);
                return Redirect("/");
            }
            else
            {
                TempData["error"] = "Tài khoản hoặc mật khẩu không chính xác";
                return Redirect("/dangnhap/index");
            }
        }

        public ActionResult DoiMK()
        {
            return View();
        }


        [HttpPost]
        public ActionResult frmChangePass(string Old_Pass, string New_Pass)
        {
            var nv = Session["nhanvien"] as TaiKhoanNhanVien;
            if(nv.MatKhau != Old_Pass)
            {
                TempData["error"] = "Mật khẩu cũ không chính xác, vui lòng nhập lại";
                return Redirect("/dangnhap/doimk");
            }
            else
            {
                var tk = db.TaiKhoanNhanViens.Find(nv.IDTK);
                tk.MatKhau = New_Pass;
                db.SaveChanges();
                Session["nhanvien"] = null;
                TempData["error"] = "Đổi mật khẩu thành công, vui lòng đăng nhập lại để tiếp tục.";
                return Redirect("/dangnhap/index");
            }
        }


        public ActionResult Logout()
        {
            Session["nhanvien"] = null;
            return Redirect("/dangnhap/index");
        }
    }
}
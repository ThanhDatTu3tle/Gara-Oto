using QuanLyGaraOto.Models.Business;
using QuanLyGaraOto.Models.DTO;
using QuanLyGaraOto.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyGaraOto.Controllers
{
    public class NhanVienController : Controller
    {
        // GET: NhanVien
        private AutoGaraEntities db = new AutoGaraEntities();
        [CustomRoleProvider(RoleName = "Admin")]
        public ActionResult Index()
        {
            var model = db.TaiKhoanNhanViens.Where(x => x.TenDN != "admin").OrderByDescending(x => x.IDTK);
            ViewBag.lstPhanQuyen = db.PhanQuyens.ToList();
            return View(model.ToList());
        }

        public JsonResult Delete(int ID)
        {

            try
            {
                var model = db.TaiKhoanNhanViens.Find(ID);
                var nv = db.ThongTinNVs.Find(model.IDNV);
                db.TaiKhoanNhanViens.Remove(model);
                db.ThongTinNVs.Remove(nv);
                db.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }

        }


        [HttpPost]
        public ActionResult frmAdd(NhanVienDTO entity)
        {
            try
            {
                var nv = new ThongTinNV();
                nv.Ten = entity.Ten;
                nv.CMND = entity.CMND;
                nv.DiaChi = entity.DiaChi;
                nv.SDT = entity.SDT;

                db.ThongTinNVs.Add(nv);
                db.SaveChanges();

                var tk = new TaiKhoanNhanVien();
                tk.TenDN = entity.TenDN;
                tk.MatKhau = entity.MatKhau;
                tk.IDNV = db.ThongTinNVs.Max(x => x.IDNV);
                tk.IDPhanQuyen = entity.IDPhanQuyen;

                db.TaiKhoanNhanViens.Add(tk);
                db.SaveChanges();

                
                TempData["add_success"] = "Thêm tài khoản thành công";
                return Redirect("/nhanvien/index");

            }
            catch
            {
                TempData["add_success"] = "Thêm tài khoản KHÔNG thành công";
                return Redirect("/nhanvien/index");
            }
        }


        [HttpPost]
        public ActionResult frmEdit(NhanVienDTO entity)
        {
            try
            {
                var nv = db.ThongTinNVs.Find(entity.ID_NhanVien);
                nv.Ten = entity.Ten;
                nv.CMND = entity.CMND;
                nv.DiaChi = entity.DiaChi;
                nv.SDT = entity.SDT;

                var tk = db.TaiKhoanNhanViens.Find(entity.IDTK);
                tk.IDPhanQuyen = entity.IDPhanQuyen;

                db.SaveChanges();
                TempData["add_success"] = "Cập nhật tài khoản thành công";
                return Redirect("/nhanvien/index");
            }
            catch
            {
                TempData["add_success"] = "Cập nhật tài khoản KHÔNG thành công";
                return Redirect("/nhanvien/index");
            }
        }

        public JsonResult GetByID(int ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var model = from tk in db.TaiKhoanNhanViens
                        join nv in db.ThongTinNVs on tk.IDNV equals nv.IDNV
                        select new NhanVienDTO()
                        {
                            IDTK = tk.IDTK,
                            IDPhanQuyen = tk.IDPhanQuyen,
                            ID_NhanVien = tk.IDNV,
                            Ten = nv.Ten,
                            CMND = nv.CMND,
                            DiaChi = nv.DiaChi,
                            SDT = nv.SDT
                        };
            
            return Json(model.FirstOrDefault(x => x.IDTK == ID), JsonRequestBehavior.AllowGet);
        }
    }
}
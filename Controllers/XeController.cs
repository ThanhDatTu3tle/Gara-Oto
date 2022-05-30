using QuanLyGaraOto.Models.Business;
using QuanLyGaraOto.Models.DTO;
using QuanLyGaraOto.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace QuanLyGaraOto.Controllers
{
    public class XeController : Controller
    {
        private AutoGaraEntities db = new AutoGaraEntities();
        // GET: Xe
        public ActionResult Index()
        {
            var thamso = db.ThamSoes.Find(1).GiaTri;
            var soxe = db.TiepNhanXes.Where(x => DbFunctions.TruncateTime(x.NgayTiepNhan) == DbFunctions.TruncateTime(DateTime.Now)).Count();
            if(soxe > thamso)
            {
                return Redirect("/xe/cancer");
            }
            else
            {
                var model = db.TiepNhanXes.Where(x => x.TrangThai != 2).OrderByDescending(x => x.NgayTiepNhan).ToList();
                ViewBag.lstHieuXe = db.HieuXes.ToList();
                return View(model);
            }
        }

        public ActionResult History()
        {
            var model = db.TiepNhanXes.OrderByDescending(x => x.NgayTiepNhan).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult frmAdd(XeDTO entity)
        {
            try
            {
                var xe = new Xe();
                xe.BienSo = entity.BienSo;
                xe.IDHieuXe = entity.IDHieuXe;
                xe.DiaChi = entity.DiaChi;
                xe.SDT = entity.SDT;
                xe.TenChuXe = entity.TenChuXe;
                xe.TienNo = 0;

                db.Xes.Add(xe);
                db.SaveChanges();

                var tp = new TiepNhanXe();
                tp.IDTK = entity.IDTK;
                tp.NgayTiepNhan = DateTime.Now;
                tp.IDXe = db.Xes.Max(x => x.IDXe);
                tp.TrangThai = 0;

                db.TiepNhanXes.Add(tp);
                db.SaveChanges();

                TempData["add_success"] = "Tiếp nhận xe thành công";
                return Redirect("/xe/index");

            }
            catch
            {
                TempData["add_success"] = "Tiếp nhận xe KHÔNG thành công";
                return Redirect("/xe/index");
            }
        }


        [HttpPost]
        public ActionResult frmEdit(Xe entity)
        {
            try
            {
                var xe = db.Xes.Find(entity.IDXe);
                xe.BienSo = entity.BienSo;
                xe.IDHieuXe = entity.IDHieuXe;
                xe.DiaChi = entity.DiaChi;
                xe.SDT = entity.SDT;
                xe.TenChuXe = entity.TenChuXe;

                db.SaveChanges();
                TempData["add_success"] = "Cập nhật xe thành công";
                return Redirect("/xe/index");
            }
            catch
            {
                TempData["add_success"] = "Cập nhật xe KHÔNG thành công";
                return Redirect("/xe/index");
            }
        }

        public JsonResult GetByID(int ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var model = from tn in db.TiepNhanXes
                        join xe in db.Xes on tn.IDXe equals xe.IDXe
                        select new XeDTO() 
                        { 
                            IDTN = tn.IDTN,
                            BienSo = xe.BienSo,
                            IDHieuXe = xe.IDHieuXe,
                            TenChuXe = xe.TenChuXe,
                            DiaChi = xe.DiaChi,
                            SDT = xe.SDT,
                            IDXe = xe.IDXe,
                            NgayTiepNhan = tn.NgayTiepNhan,
                            TenNhanvien = tn.TaiKhoanNhanVien.ThongTinNV.Ten
                        };
            var entity = model.FirstOrDefault(x => x.IDTN == ID);
            var result = new
            {
                IDTN = entity.IDTN,
                BienSo = entity.BienSo,
                IDHieuXe = entity.IDHieuXe,
                TenChuXe = entity.TenChuXe,
                DiaChi = entity.DiaChi,
                SDT = entity.SDT,
                IDXe = entity.IDXe,
                NgayTiepNhan = entity.NgayTiepNhan.ToString("dd-MM-yyyy"),
                TenNhanvien = entity.TenNhanvien
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int ID)
        {
            var tp = db.TiepNhanXes.Find(ID);
            ViewBag.PSC = db.PhieuSuaChuas.FirstOrDefault(x => x.IDXe == tp.IDXe);
            ViewBag.CTPhieuSuaChua = db.ChiTietPSCs.ToList();
            ViewBag.TiepNhanXe = tp;

            ViewBag.lstTienCong = db.TienCongs.ToList();
            ViewBag.lstVTPT = db.VTPTs.ToList();
            return View();
        }

        public JsonResult addPSC(string json_str)
        {
            var JsonReview = new JavaScriptSerializer().Deserialize<List<PhieuSuaChuaDTO>>(json_str);

            foreach(var item in JsonReview)
            {
                if (item.IDPSC == 0)
                {
                    var psc = new PhieuSuaChua();
                    psc.IDXe = item.IDXe;
                    psc.NgaySuaChua = DateTime.Now;
                    db.PhieuSuaChuas.Add(psc);
                    db.SaveChanges();

                    var ct = new ChiTietPSC();
                    ct.IDPSC = db.PhieuSuaChuas.Max(x => x.IDPSC);
                    ct.IDVTPT = item.IDVTPT;
                    ct.IDTienCong = item.IDTienCong;
                    ct.SoLuong = item.SoLuong;
                    db.ChiTietPSCs.Add(ct);

                    //Cập nhật trạng thái đang sửa chữa
                    var tiepnhanxe = db.TiepNhanXes.Find(item.IDTN);
                    tiepnhanxe.TrangThai = 1;
                    db.SaveChanges();
                }
                else
                {
                    var result = db.ChiTietPSCs.FirstOrDefault(x => x.IDPSC == item.IDPSC && x.IDVTPT == item.IDVTPT);
                    if (result != null)
                    {
                        result.SoLuong += item.SoLuong;
                        db.SaveChanges();
                    }
                    else
                    {
                        var ct = new ChiTietPSC();
                        ct.IDPSC = item.IDPSC;
                        ct.IDVTPT = item.IDVTPT;
                        ct.IDTienCong = item.IDTienCong;
                        ct.SoLuong = item.SoLuong;
                        db.ChiTietPSCs.Add(ct);
                        db.SaveChanges();
                    }
                }

                //Trừ tồn kho
                new NhapKhoBusiness().Sub_Quantity(item.IDVTPT, item.SoLuong);
                new NhapKhoBusiness().Update_TienNo(item.IDXe);

            }
            
            return Json(new
            {
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        //Xóa Chi tiết phiếu sửa chữa
        public JsonResult Delete_CTPSC(int IDPSC)
        {
            var detail = db.ChiTietPSCs.Find(IDPSC);

            //Cộng lại tồn kho
            new NhapKhoBusiness().Add_Quantity(detail.IDVTPT, detail.SoLuong);

            db.ChiTietPSCs.Remove(detail);
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        //Sửa Chi tiết phiếu sửa chữa
        public JsonResult Edit_CTPSC(int IDPSC, int SoLuong)
        {
            var detail = db.ChiTietPSCs.Find(IDPSC);
            //Trừ tồn kho
            new NhapKhoBusiness().Sub_Quantity(detail.IDVTPT, SoLuong - detail.SoLuong);

            detail.SoLuong = SoLuong;
            db.SaveChanges();

            return Json(new
            {
                status = true
            });
        }

        [HttpPost]
        public ActionResult frmPhieuThuTien(PhieuThuTien entity)
        {
            var tiepnhan = db.TiepNhanXes.FirstOrDefault(x => x.IDXe == entity.IDXe);
            tiepnhan.TrangThai = 2;

            entity.NgayThuTien = DateTime.Now;
            db.PhieuThuTiens.Add(entity);
            db.SaveChanges();

            TempData["add_success"] = "Thanh toán xe thành công";
            return Redirect("/xe/detail/" + tiepnhan.IDTN);
        }

        public ActionResult Chitiet(int ID)
        {
            var tp = db.TiepNhanXes.Find(ID);
            ViewBag.PSC = db.PhieuSuaChuas.FirstOrDefault(x => x.IDXe == tp.IDXe);
            ViewBag.CTPhieuSuaChua = db.ChiTietPSCs.ToList();
            ViewBag.TiepNhanXe = tp;
            return View();
        }

        public ActionResult Cancer()
        {
            return View();
        }
    }
}
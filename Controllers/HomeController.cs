using QuanLyGaraOto.Models.DTO;
using QuanLyGaraOto.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyGaraOto.Controllers
{
    public class HomeController : Controller
    {
        private AutoGaraEntities db = new AutoGaraEntities();
        public ActionResult Index(int month = 0)
        {
            //Xe đang sửa chữa
            ViewBag.XeDangSC = db.TiepNhanXes.Where(x => x.TrangThai != 2).Count();

            //Tổng doanh thu
            ViewBag.TongDoanhThu = db.PhieuThuTiens.Sum(x => x.SoTienThu).ToString("N0");

            //Lượt xe đã sửa
            ViewBag.XeDaSC = db.TiepNhanXes.Where(x => x.TrangThai == 2).Count();

            //Thống kê doanh thu
            var xe = db.Xes.ToList();
            var lstDoanhSo = new List<DoanhSoDTO>();
            ViewBag.Month = month != 0 ? month : 0;
            foreach (var jtem in xe)
            {
                var doanhso = new DoanhSoDTO();
                int soluotsua = 0;
                decimal thanhtien = 0;
                decimal tyle = 0;
                if (month != 0 && db.PhieuThuTiens.Count(x => x.NgayThuTien.Month == month) > 0)
                {
                    var tongdoanhthu = db.PhieuThuTiens.Where(x => x.NgayThuTien.Month == month).Sum(x => x.SoTienThu);
                    foreach (var item in db.PhieuThuTiens.Where(x => x.IDXe == jtem.IDXe && x.NgayThuTien.Month == month).ToList())
                    {
                        soluotsua++;
                        thanhtien += item.SoTienThu;
                    }

                    if (thanhtien != 0)
                        tyle = Math.Round((1 - (thanhtien / tongdoanhthu)) * 100, 2);
                }
                else if (month == 0)
                {
                    var tongdoanhthu = db.PhieuThuTiens.Sum(x => x.SoTienThu);
                    foreach (var item in db.PhieuThuTiens.Where(x => x.IDXe == jtem.IDXe).ToList())
                    {
                        soluotsua++;
                        thanhtien += item.SoTienThu;
                    }

                    if (thanhtien != 0)
                        tyle = Math.Round((thanhtien / tongdoanhthu) * 100, 2);
                }

                if (lstDoanhSo.Exists(x => x.HieuXe == jtem.HieuXe.HieuXe1))
                {
                    var ds = lstDoanhSo.FirstOrDefault(x => x.HieuXe == jtem.HieuXe.HieuXe1);
                    ds.SoLuotSua += soluotsua;
                    ds.ThanhTien += thanhtien;
                    ds.TyLe += (float)tyle;
                }
                else
                {
                    doanhso.HieuXe = jtem.HieuXe.HieuXe1;
                    doanhso.SoLuotSua = soluotsua;
                    doanhso.ThanhTien = thanhtien;
                    doanhso.TyLe = (float)tyle;
                    lstDoanhSo.Add(doanhso);
                }

            }
            ViewBag.lstDoanhSo = lstDoanhSo;

            //Tổng doanh thu
            decimal doanhthu = 0;
            if(month != 0 && db.PhieuThuTiens.Count(x => x.NgayThuTien.Month == month) > 0)
            {
                doanhthu = db.PhieuThuTiens.Where(x => x.NgayThuTien.Month == month).Sum(x => x.SoTienThu);
            }
            else if(month == 0)
            {
                doanhthu = db.PhieuThuTiens.Sum(x => x.SoTienThu);
            }

            ViewBag.Thang_doanhthu = doanhthu.ToString("N0");

            //Xe tiếp nhận, sửa chữa hôm nay
            ViewBag.TiepNhan_today = db.TiepNhanXes.Where(x => DbFunctions.TruncateTime(x.NgayTiepNhan) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Lượt xe tối đa
            ViewBag.ThamSo = db.ThamSoes.Find(1).GiaTri;

            //Số nhân viên
            ViewBag.NhanVien = db.TaiKhoanNhanViens.Count();

            //Nhà cung cấp
            ViewBag.NhaCC = db.NhaCungCaps.Count();

            //Nhập kho hôm nay
            ViewBag.NhapKho_Today = db.PhieuNhaps.Where(x => DbFunctions.TruncateTime(x.NgayNhap) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Số vật tư, phụ tùng
            ViewBag.VTPT = db.VTPTs.Count();

            //Loại tiền công
            ViewBag.LoaiTC = db.TienCongs.Count();
            return View();
        }

    }
}
using QuanLyGaraOto.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyGaraOto.Models.Business
{
    public class NhapKhoBusiness
    {
        private AutoGaraEntities db = new AutoGaraEntities();

        public bool Add_PN(PhieuNhap entity)
        {
            try
            {
                entity.NgayNhap = DateTime.Now;
                db.PhieuNhaps.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Add_CTPN(CTPhieuNhap entity)
        {
            db.CTPhieuNhaps.Add(entity);
            db.SaveChanges();
        }

        //Cộng tồn kho
        public void Add_Quantity(int IDVTPT, int SoLuongTon)
        {
            var vt = db.VTPTs.Find(IDVTPT);
            vt.SoLuongTon += SoLuongTon;
            db.SaveChanges();
        }

        //Trừ tồn kho
        public void Sub_Quantity(int IDVTPT, int SoLuongTon)
        {
            var vt = db.VTPTs.Find(IDVTPT);
            vt.SoLuongTon -= SoLuongTon;
            db.SaveChanges();
        }

        //Tìm kiếm vtpt theo tên
        public VTPT searchVTPT(string ten)
        {
            return db.VTPTs.Single(x => x.Ten == ten);
        }

        //Cập nhật tiền nợ
        public void Update_TienNo(int IDXe)
        {
            //Cập nhật tiền nợ
            var psc = db.PhieuSuaChuas.FirstOrDefault(x => x.IDXe == IDXe);
            decimal TienNo = 0;
            foreach(var item in db.ChiTietPSCs.Where(x => x.IDPSC == psc.IDPSC).ToList())
            {
                TienNo += item.SoLuong * item.VTPT.DonGia + item.TienCong.TienCong1;
            }
            var xe = db.Xes.Find(IDXe);
            xe.TienNo = TienNo;
            db.SaveChanges();
        }
    }
}
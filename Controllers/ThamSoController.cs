using QuanLyGaraOto.Models.Business;
using QuanLyGaraOto.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyGaraOto.Controllers
{
    public class ThamSoController : Controller
    {
        // GET: ThamSo
        private AutoGaraEntities db = new AutoGaraEntities();
        [CustomRoleProvider(RoleName = "Admin")]
        public ActionResult Index()
        {
            var model = db.ThamSoes.OrderByDescending(x => x.MaThamSo);
            return View(model.ToList());
        }


        [HttpPost]
        public ActionResult frmEdit(ThamSo entity)
        {
            try
            {
                var model = db.ThamSoes.Find(entity.MaThamSo);
                model.GiaTri = entity.GiaTri;
                model.GhiChu = "Số xe sửa không vượt quá " + entity.GiaTri + " xe / ngày";
                db.SaveChanges();
                TempData["add_success"] = "Cập nhật quy định thành công";
                return Redirect("/thamso/index");
            }
            catch
            {
                TempData["add_success"] = "Cập nhật quy định KHÔNG thành công";
                return Redirect("/thamso/index");
            }
        }

        public JsonResult GetByID(int ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var model = db.ThamSoes.Find(ID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
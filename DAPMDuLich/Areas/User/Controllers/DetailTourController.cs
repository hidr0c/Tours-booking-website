using DAPMDuLich.App_Start;
using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAPMDuLich.Areas.User.Controllers
{
    public class DetailTourController : Controller
    {
        // GET: User/DetailTour
        //[CheckPermissions(ChucNang = "TaiKhoan_ChiTiet")]
        public ActionResult DetailTour(int id)
        {
            //var user = (TaiKhoan)HttpContext.Session["user"];
            //var name = user.TenDangNhap;

            using (DAPMDuLichEntities db = new DAPMDuLichEntities())
            {
                // Fetch the tour with the specified ID
                var tour = db.TourDuLiches.FirstOrDefault(t => t.ID == id);
                if (tour == null)
                {
                    return HttpNotFound();
                }
                return View(tour);
            }
        }
    }
}
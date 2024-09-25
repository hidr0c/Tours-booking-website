using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAPMDuLich.Areas.User.Controllers
{
    public class HomeController : Controller
    {
         DAPMDuLichEntities db = new DAPMDuLichEntities();
        // GET: TrangChu
        public ActionResult Home()
        {
            DAPMDuLichEntities db = new DAPMDuLichEntities();
            //1.Lấy danh sách
            var danhSachTour = db.TourDuLiches.ToList();
            return View(danhSachTour);
        }

        public ActionResult SearchTour(int? idTinh, int? idLoaiTour, int? idMucGia)
        {
            ViewBag.idTinh = idTinh;
            ViewBag.idLoaiTour = idLoaiTour;
            ViewBag.idMucGia = idMucGia;
            mapTour tour = new mapTour();
            return View(tour.DanhSach(idTinh, idLoaiTour, idMucGia));
        }
    }
}
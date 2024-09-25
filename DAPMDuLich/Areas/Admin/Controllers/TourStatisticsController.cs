using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAPMDuLich.Models.ViewModel;

namespace DAPMDuLich.Areas.Admin.Controllers
{
    public class TourStatisticsController : Controller
    {
        // GET: Admin/TourStatistics
       
        private DAPMDuLichEntities database = new DAPMDuLichEntities();


        public ActionResult Index()
        {
            var thongKe = new TourStatisticsViewModel();

            // Tính toán tổng số đặt tour
            thongKe.TongSoDatTour = database.DatTours.Count();

            // Tính toán tổng số đặt tour đã thanh toán
            thongKe.TongSoDatTourDaThanhToan = database.DatTours.Count(dt => dt.ThanhToan.HasValue && dt.ThanhToan.Value);

            // Tính toán tổng số đặt tour chưa thanh toán
            thongKe.TongSoDatTourChuaThanhToan = database.DatTours.Count(dt => !dt.ThanhToan.HasValue || !dt.ThanhToan.Value);

            // Tính toán tổng số tiền đã thanh toán
            var paidTours = from dt in database.DatTours
                            join dtct in database.DatTourChiTiets on dt.BookingID equals dtct.BookingID
                            where dt.ThanhToan.HasValue && dt.ThanhToan.Value
                            select dtct.Price ?? 0;

            thongKe.TongSoTienDaThanhToan = paidTours.Sum();

            return View(thongKe);
        }
    }
}
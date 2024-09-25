using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace DAPMDuLich.Areas.User.Controllers
{
    public class BookingDetailController : Controller
    {
        // GET: User/BookingDetail
        private DAPMDuLichEntities db = new DAPMDuLichEntities();
        // GET: DatTourChiTIet
        public ActionResult Index()
        {
            var bookingDetail = db.DatTourChiTiets.Include(b => b.DatTour);
            return View(bookingDetail.ToList());
        }
        // GET: User/BookingDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatTourChiTiet bookingDetail = db.DatTourChiTiets.SingleOrDefault(x => x.BookingID == id);
            DatTour booking = db.DatTours.SingleOrDefault(b => b.BookingID == bookingDetail.BookingID);
            TourDuLich tour = db.TourDuLiches.SingleOrDefault(t => t.ID == booking.ID);

            if (bookingDetail == null || booking == null || tour == null)
            {
                return HttpNotFound();
            }

            ViewBag.TourName = tour.TieuDe;
            ViewBag.TripStart = tour.TripStart;
            ViewBag.TripEnd = tour.TripEnd;
            ViewBag.TripType = tour.PhuongTien;
            ViewBag.TravelerCount = bookingDetail.TravelerCount;
            return View(bookingDetail);
        }
        public ActionResult ThanhToan(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DatTourChiTiet bookingDetail = db.DatTourChiTiets.SingleOrDefault(x => x.BookingDetailID == id);
            if (bookingDetail == null)
            {
                return HttpNotFound();
            }

            DatTour booking = db.DatTours.SingleOrDefault(b => b.BookingID == bookingDetail.BookingID);
            if (booking == null)
            {
                return HttpNotFound();
            }

            // lay id khach hang tu bang TaiKhoanKh neu null thi no se bao k tim thay http
            TaiKhoan customer = db.TaiKhoans.SingleOrDefault(c => c.UserID == booking.UserID);
            if (customer == null)
            {
                return HttpNotFound();
            }

            if (customer.Tien >= bookingDetail.Price)
            {
                customer.Tien -= bookingDetail.Price.Value;
                booking.ThanhToan = true; // Cập nhật trạng thái thành đã thanh toán
                booking.MaThanhToan = GeneratePaymentCode(); // Tạo và lưu mã thanh toán
                db.SaveChanges();
                ViewBag.Message = "Thanh toán thành công";
            }
            else
            {
                ViewBag.Message = "Không đủ tiền để thanh toán";
            }

            // Truyền thông tin cần thiết đến View
            ViewBag.TenDangNhap = customer.TenDangNhap;
            ViewBag.CreateAt = booking.CreateAt;
            ViewBag.ThanhToan = booking.ThanhToan;
            ViewBag.MaThanhToan = booking.MaThanhToan;

            return View("ThanhToan", bookingDetail);
        }

        private string GeneratePaymentCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(); // Tạo mã thanh toán ngẫu nhiên
        }
    }
}

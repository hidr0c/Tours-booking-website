using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DAPMDuLich.Areas.Contributors.Controllers
{
    public class BookingController : Controller
    {
        private DAPMDuLichEntities db = new DAPMDuLichEntities();

        // GET: Admin/Booking
        public ActionResult List()
        {
            // Lấy Contributor từ session
            var contributor = Session["contributor"] as Contributor;
            if (contributor == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Contributors" });
            }

            // Lấy danh sách đặt tour của Contributor hiện tại
            var bookings = db.DatTours
                             .Include(b => b.TourDuLich) // Bao gồm thông tin Tour
                             .Include(b => b.TaiKhoan) // Bao gồm thông tin người đặt
                             .Where(b => b.TourDuLich.ContributorID == contributor.ContributorID)
                             .ToList();

            // Lấy danh sách thông báo chưa đọc
            var notifications = db.Notifications
                            .Include(n => n.TourDuLich)
                            .Include(n => n.TaiKhoan)
                            .Where(n => n.ContributorID == contributor.ContributorID) // Chỉ lọc theo ContributorID
                            .ToList();

            // Cập nhật ViewBag
            ViewBag.Notifications = notifications;
            ViewBag.NotificationCount = notifications.Count(n => n.IsRead == false);
            return View(bookings);
        }

        // GET: Admin/DatTour/Create
        //public ActionResult Create()
        //{
        //    ViewBag.ID = new SelectList(db.TourDuLiches, "ID", "TieuDe");
        //    ViewBag.UserID = new SelectList(db.TaiKhoans, "UserID", "TenDangNhap");
        //    return View();
        //}

        //// POST: Admin/DatTour/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,UserID,Status,CreateAt,ThanhToan,MaThanhToan")] DatTour booking, int travelerCount)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        booking.CreateAt = DateTime.Now;

        //        if (booking.ThanhToan.HasValue && booking.ThanhToan.Value)
        //        {
        //            booking.MaThanhToan = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        //        }
        //        else
        //        {
        //            booking.MaThanhToan = null;
        //        }

        //        db.DatTours.Add(booking);
        //        db.SaveChanges();

        //        // Lấy giá tour từ bảng TourDuLiches dựa trên ID Tour
        //        var giaTour = db.TourDuLiches.FirstOrDefault(t => t.ID == booking.ID)?.GiaTour ?? 0;

        //        // Tạo một đối tượng DatTourChiTiet mới
        //        var datTourChiTiet = new DatTourChiTiet
        //        {
        //            BookingID = booking.BookingID, // Correctly reference BookingID
        //            TravelerCount = travelerCount,
        //            Price = giaTour * travelerCount - (giaTour * travelerCount) * 3 / 100,
        //            CreateAt = DateTime.Now
        //        };

        //        // Thêm DatTourChiTiet vào cơ sở dữ liệu
        //        db.DatTourChiTiets.Add(datTourChiTiet);
        //        db.SaveChanges();

        //        return RedirectToAction("List");
        //    }

        //    ViewBag.ID = new SelectList(db.TourDuLiches, "ID", "TieuDe", booking.ID);
        //    ViewBag.UserID = new SelectList(db.TaiKhoans, "UserID", "TenDangNhap", booking.UserID);
        //    return View(booking);
        //}

        //// GET: Admin/DatTour/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    DatTour booking = db.DatTours.Find(id);
        //    if (booking == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    // Kiểm tra quyền sở hữu đặt tour
        //    var user = Session["user"] as TaiKhoan;
        //    if (user == null || booking.UserID != user.UserID)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        //    }

        //    // Lấy số lượng hành khách từ DatTourChiTiet
        //    var datTourChiTiet = db.DatTourChiTiets.FirstOrDefault(d => d.BookingID == booking.BookingID);
        //    ViewBag.TravelerCount = datTourChiTiet?.TravelerCount ?? 0;

        //    ViewBag.ID = new SelectList(db.TourDuLiches, "ID", "TieuDe", booking.ID);
        //    ViewBag.UserID = new SelectList(db.TaiKhoans, "UserID", "TenDangNhap", booking.UserID);
        //    return View(booking);
        //}

        //// POST: Admin/DatTour/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "BookingID,ID,UserID,Status,CreateAt,ThanhToan,MaThanhToan")] DatTour booking, int travelerCount)
        //{
        //    // Lấy UserID từ session
        //    var user = Session["user"] as TaiKhoan;
        //    if (user == null || booking.UserID != user.UserID)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        if (booking.ThanhToan.HasValue && booking.ThanhToan.Value)
        //        {
        //            if (string.IsNullOrEmpty(booking.MaThanhToan))
        //            {
        //                booking.MaThanhToan = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        //            }
        //        }
        //        else
        //        {
        //            booking.MaThanhToan = null;
        //        }

        //        db.Entry(booking).State = EntityState.Modified;

        //        // Lấy mục DatTourChiTiet hiện có
        //        var datTourChiTiet = db.DatTourChiTiets.FirstOrDefault(d => d.BookingID == booking.BookingID);
        //        if (datTourChiTiet != null)
        //        {
        //            var giaTour = db.TourDuLiches.FirstOrDefault(t => t.ID == booking.ID)?.GiaTour ?? 0;
        //            datTourChiTiet.TravelerCount = travelerCount;
        //            datTourChiTiet.Price = giaTour * travelerCount - (giaTour * travelerCount) * 3 / 100;
        //            db.Entry(datTourChiTiet).State = EntityState.Modified;
        //        }

        //        db.SaveChanges();
        //        return RedirectToAction("List");
        //    }

        //    ViewBag.ID = new SelectList(db.TourDuLiches, "ID", "TieuDe", booking.ID);
        //    ViewBag.UserID = new SelectList(db.TaiKhoans, "UserID", "TenDangNhap", booking.UserID);
        //    ViewBag.TravelerCount = travelerCount; // cung cấp giá trị để hiển thị lên view thôi
        //    return View(booking);
        //}

        //// GET: Admin/DatTour/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    DatTour booking = db.DatTours.Find(id);
        //    if (booking == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    // Kiểm tra quyền sở hữu đặt tour
        //    var user = Session["user"] as TaiKhoan;
        //    if (user == null || booking.UserID != user.UserID)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        //    }

        //    return View(booking);
        //}

        //// POST: Admin/DatTour/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    // Tìm đặt tour theo id
        //    DatTour booking = db.DatTours.Include(b => b.DatTourChiTiets).FirstOrDefault(b => b.BookingID == id);

        //    if (booking != null)
        //    {
        //        // Kiểm tra quyền sở hữu đặt tour
        //        var user = Session["user"] as TaiKhoan;
        //        if (user == null || booking.UserID != user.UserID)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        //        }

        //        // Xóa tất cả chi tiết đặt tour trước
        //        foreach (var detail in booking.DatTourChiTiets.ToList())
        //        {
        //            db.DatTourChiTiets.Remove(detail);
        //        }

        //        // Xóa đặt tour
        //        db.DatTours.Remove(booking);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("List");
        //}
        public ActionResult Search(string searchQuery, bool? status)
        {
            // Lấy UserID từ session
            var contributor = Session["contributor"] as Contributor;
            if (contributor == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Contributors" });
            }

            var tours = db.DatTours.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                // Tạo biến để lưu BookingID và UserID
                int bookingId;
                int userId;

                bool isBookingId = int.TryParse(searchQuery, out bookingId);
                bool isUserId = int.TryParse(searchQuery, out userId);

                // Lọc tour theo ContributorID và theo từ khóa tìm kiếm
                tours = tours.Where(t => t.ContributorID == contributor.ContributorID &&
                                         (isBookingId && t.BookingID == bookingId ||
                                          isUserId && t.UserID == userId ||
                                          t.TaiKhoan.TenHienThi.Contains(searchQuery)));
            }
            else
            {
                // Nếu không có từ khóa tìm kiếm, chỉ lấy tour của contributor
                tours = tours.Where(t => t.ContributorID == contributor.ContributorID);
            }

            // Lọc theo trạng thái thanh toán
            if (status.HasValue)
            {
                tours = tours.Where(t => t.Status == status.Value);
            }

            return View("List", tours.ToList());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult MarkAsRead(int id)
        {
            // Tìm thông báo theo ID
            var notification = db.Notifications.Find(id);
            if (notification != null)
            {
                notification.IsRead = true; // Đánh dấu là đã đọc
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            return RedirectToAction("List", "Booking"); // Quay lại danh sách thông báo
        }
    }

}
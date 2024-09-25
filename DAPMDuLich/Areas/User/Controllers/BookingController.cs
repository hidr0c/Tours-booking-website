﻿using DAPMDuLich.Models.ViewModel;
using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using DAPMDuLich.App_Start;
using System.Data.Entity.Validation;

namespace DAPMDuLich.Areas.User.Controllers
{
    public class BookingController : Controller
    {
        // GET: User/Booking
        // GET: DatTour
        DAPMDuLichEntities database = new DAPMDuLichEntities();
      

        public async Task<ActionResult> Create(int? id)
        {
            if (id == null)
            {
                // Handle the case when id is not provided
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID is required.");
            }

            var user = (TaiKhoan)HttpContext.Session["user"];
            var name = user.TenDangNhap;

            var taiKhoanKH = await database.TaiKhoans.SingleOrDefaultAsync(x => x.TenDangNhap == name);
            var tourDuLich = await database.TourDuLiches.SingleOrDefaultAsync(t => t.ID == id);
            var datTourChiTiet = database.DatTourChiTiets;

            var bookingViewModel = new BookingViewModel()
            {
                HoTen = taiKhoanKH.TenHienThi,
                Email = taiKhoanKH.Email,
                ID = tourDuLich.ID,
                TieuDe = tourDuLich.TieuDe,
                TripStart = tourDuLich.TripStart,
                TripEnd = tourDuLich.TripEnd,
                BaiViet = tourDuLich.BaiViet,
                PhuongTien = tourDuLich.PhuongTien,
                SoNguoiToiDa = tourDuLich.SoNguoiToiDa,
                GiaTour = tourDuLich.GiaTour,
            };

            return View(bookingViewModel);
        }

        [CheckPermissions(ChucNang = "TaiKhoan_ChiTiet")]
        [HttpPost]
        public async Task<ActionResult> Create(BookingViewModel model)
        {
            try
            {
                var useriId = (TaiKhoan)HttpContext.Session["user"];
                if (useriId == null)
                {
                    return Content("Tài khoản KH session không tìm thấy");
                }

                var user = await database.TaiKhoans.SingleOrDefaultAsync(x => x.TenDangNhap == useriId.TenDangNhap);
                if (user == null)
                {
                    return Content("Tài khoản KH không tìm thấy trong database");
                }

                // Kiểm tra tổng số người đã đặt tour hiện tại
                var totalCurrentBookings = SumTraveler(model.ID);

                // Kiểm tra nếu tổng số người đã đặt tour cộng với số người trong đơn đặt mới vượt quá số lượng người tối đa
                if (totalCurrentBookings + model.TravelerCount > model.SoNguoiToiDa)
                {
                    ModelState.AddModelError("TravelerCount", "Tour đã đầy chỗ.");
                    return View(model);
                }

                int bookingId;
                Random random = new Random();
                do
                {
                    bookingId = random.Next(1, int.MaxValue);
                }
                while (await database.DatTourChiTiets.AnyAsync(dt => dt.BookingID == bookingId));

                var datTour = new DatTour()
                {
                    BookingID = bookingId,
                    UserID = user.UserID,
                    Status = true,
                    ThanhToan = false,
                    ID = model.ID,
                    CreateAt = DateTime.Now,
          

                };

                database.DatTours.Add(datTour);
                await database.SaveChangesAsync();

                var datTourChiTiet = new DatTourChiTiet()
                {
                    BookingID = datTour.BookingID,
                    TravelerCount = model.TravelerCount,
                    CreateAt = DateTime.Now,
                    Price = model.GiaTour * model.TravelerCount - (model.GiaTour * model.TravelerCount) * 3 / 100,
                };
                database.DatTourChiTiets.Add(datTourChiTiet);
                await database.SaveChangesAsync();

                return RedirectToAction("History");
            }
            catch (DbEntityValidationException ex)
            {
                // Lấy danh sách các lỗi xác thực
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Nối các thông báo lỗi lại thành chuỗi
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " Các lỗi xác thực: ", fullErrorMessage);

                // Trả về thông báo lỗi chi tiết
                return Content(exceptionMessage);
            }
        }
        private string GetInnermostExceptionMessage(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex.Message;
        }
        // Phương thức để tính tổng số người đã đặt tour
        private int SumTraveler(int tourID)
        {
            var query = database.Database.SqlQuery<int>(
                "SELECT SUM(TravelerCount) " +
                "FROM DatTourChiTiet as BD " +
                "WHERE BD.BookingID IN (" +
                    "SELECT B.BookingID " +
                    "FROM DatTour as B " +
                    "WHERE B.ID = @p0 AND B.Status = 1)", tourID);

            try
            {
                var result = query.FirstOrDefault();
                return result;
            }
            catch
            {
                return 0;
            }
        }

        [CheckPermissions(ChucNang = "TaiKhoan_ChiTiet")]
        public ActionResult History()
        {
            var usermodel = (TaiKhoan)HttpContext.Session["user"];
            TaiKhoan user = database.TaiKhoans.SingleOrDefault(u => u.TenDangNhap == usermodel.TenDangNhap);

            var bookings = database.DatTours.Where(b => b.UserID == user.UserID).ToList();// lấy CustomerID của TaiKhoanKH và DatTour do khóa ngoại
            return View(bookings);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatTour booking = database.DatTours.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: User/Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            // Find the booking by ID từ DatTour và DatTourChiTiet
            DatTour booking = database.DatTours.Include(b => b.DatTourChiTiets).FirstOrDefault(b => b.BookingID == id);

            if (booking != null)
            {
                // Phải xóa cái bookingid từ bảng DatTourChiTiet trc 
                foreach (var detail in booking.DatTourChiTiets.ToList())
                {
                    database.DatTourChiTiets.Remove(detail);
                }

                // Remove the booking
                database.DatTours.Remove(booking);

                // Save changes
                database.SaveChanges();
            }


            return RedirectToAction("History", "Booking", new { id = booking.ID });
        }
    }
}
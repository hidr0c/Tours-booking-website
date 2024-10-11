using DAPMDuLich.App_Start;
using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DAPMDuLich.Areas.Contributors.Controllers
{
    public class TourController : Controller
    {
        private DAPMDuLichEntities db = new DAPMDuLichEntities(); // Đảm bảo DbContext được khởi tạo

        // GET: Contributors/Tour/List
        public ActionResult List()
        {
            // Lấy UserID từ session
            var contributor = Session["contributor"] as Contributor;
            if (contributor == null)
            {
                return RedirectToAction("Login", "Auth", new { Areas = "Contributors" });
            }

            // Lấy danh sách tour của người dùng
            var tours = db.TourDuLiches.Where(t => t.ContributorID == contributor.ContributorID).ToList();
            // Lấy danh sách thông báo chưa đọc
            var notifications = db.Notifications
                            .Include(n => n.TourDuLich)
                            .Include(n => n.TaiKhoan)
                            .Where(n => n.ContributorID == contributor.ContributorID) // Chỉ lọc theo ContributorID
                            .ToList();

            ViewBag.Notifications = notifications;
            ViewBag.NotificationCount = notifications.Count(n => n.IsRead==false);
            return View(tours); // Trả về View với danh sách tour
        }
        // GET: Contributors/Tour/Create
        public ActionResult Create()
        {
            return View(new TourDuLich());
        }

        // POST: Contributors/Tour/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TourDuLich model)
        {
            var contributor = Session["contributor"] as Contributor;
            if (contributor == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Contributors" });
            }

            model.ContributorID = contributor.ContributorID; // Gán UserID cho tour
            model.Status = null; // Trạng thái chờ duyệt

            if (ModelState.IsValid)
            {
                try
                {
                    db.TourDuLiches.Add(model);
                    db.SaveChanges();

                    // Thông báo thành công hoặc thêm logic gửi thông báo cho Admin nếu cần

                    return RedirectToAction("List");
                }
                catch (DbUpdateException ex)
                {
                    string errorMessage = "Có lỗi xảy ra khi lưu dữ liệu.";
                    Exception inner = ex;
                    while (inner != null)
                    {
                        errorMessage += " --> " + inner.Message;
                        inner = inner.InnerException;
                    }
                    ModelState.AddModelError("", errorMessage);
                    System.Diagnostics.Debug.WriteLine(errorMessage);
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                    ModelState.AddModelError("", errorMessage);
                    System.Diagnostics.Debug.WriteLine($"Exception: {errorMessage}");
                }
            }

            return View(model);
        }


        // GET: Contributors/Tour/Update/5
        public ActionResult Update(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TourDuLich tour = db.TourDuLiches.Find(id);
            if (tour == null)
            {
                return HttpNotFound();
            }

            return View(tour);
        }

        // POST: Contributors/Tour/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(TourDuLich tour)
        {
            // Lấy UserID từ session
            var contributor = Session["contributor"] as Contributor;
            if (contributor == null)
            {
                return RedirectToAction("Login", "Auth", new { Areas = "Contributors" });
            }

            if (ModelState.IsValid)
            {
                // Tìm tour hiện tại trong cơ sở dữ liệu
                var existingTour = db.TourDuLiches.Find(tour.ID);
                if (existingTour != null)
                {
                    // Cập nhật các trường thông tin
                    existingTour.TieuDe = tour.TieuDe;
                    existingTour.BaiViet = tour.BaiViet;
                    existingTour.DiaDiem = tour.DiaDiem;
                    existingTour.GiaTour = tour.GiaTour;
                    existingTour.HinhAnh = tour.HinhAnh;
                    existingTour.idLoaiTour = tour.idLoaiTour;
                    existingTour.idTinh = tour.idTinh;
                    existingTour.PhuongTien = tour.PhuongTien;
                    existingTour.idMucGia = tour.idMucGia;
                    existingTour.LichTrinh = tour.LichTrinh;
                    existingTour.TripStart = tour.TripStart;
                    existingTour.TripEnd = tour.TripEnd;
                    existingTour.SoNguoiToiDa = tour.SoNguoiToiDa;
                    
                    // Gán UserID cho tour
                    existingTour.ContributorID = contributor.ContributorID;
                  
                    // Đảm bảo trạng thái là null
                    existingTour.Status = null;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();
                    return RedirectToAction("List", "Tour", new { area = "Contributors" });
                }
            }

            return View(tour);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TourDuLich booking = db.TourDuLiches.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);

        }
        // POST: Contributors/Tour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TourDuLich tour = db.TourDuLiches.Find(id);
            if (tour != null)
            {
                db.TourDuLiches.Remove(tour);
                db.SaveChanges();
            }
            return RedirectToAction("List", "Tour", new { area = "Contributors" });
        }
        public ActionResult Search(string searchQuery)
        {
            // Lấy UserID từ session
            var contributor = Session["contributor"] as Contributor;
            

            var tours = db.TourDuLiches.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                // Lọc tour theo ContributorID và theo từ khóa tìm kiếm
                tours = tours.Where(t => t.ContributorID == contributor.ContributorID &&
                                         (t.TieuDe.Contains(searchQuery) ||
                                          t.DiaDiem.Contains(searchQuery) ||
                                          t.PhuongTien.Contains(searchQuery)));
            }
            else
            {
                // Nếu không có từ khóa tìm kiếm, chỉ lấy tour của contributor
                tours = tours.Where(t => t.ContributorID == contributor.ContributorID);
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

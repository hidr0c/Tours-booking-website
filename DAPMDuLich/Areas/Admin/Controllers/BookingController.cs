using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DAPMDuLich.Areas.Admin.Controllers
{
    public class BookingController : Controller
    {
        // GET: Admin/Booking
        private DAPMDuLichEntities db = new DAPMDuLichEntities();

        // GET: Admin/DatTour
        public ActionResult List()
        {
            return View(db.DatTours.ToList());
        }

        
        // GET: Admin/DatTourr/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.TourDuLiches, "ID", "TieuDe");
            ViewBag.UserID = new SelectList(db.TaiKhoans, "UserID", "TenDangNhap");
            ViewBag.ContributorID=new SelectList(db.Contributors,"ContributorID","ContributorName");
            return View();
        }

        // POST: Admin/DatTourr/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,ContributorID,Status,CreateAt,ThanhToan,MaThanhToan")] DatTour booking, int travelerCount)
        {
            if (ModelState.IsValid)
            {
                booking.CreateAt = DateTime.Now;

                if (booking.ThanhToan.HasValue && booking.ThanhToan.Value)
                {
                    booking.MaThanhToan = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                }
                else
                {
                    booking.MaThanhToan = null;
                }

                db.DatTours.Add(booking);
                db.SaveChanges();

                // lấy giá tour từ bảng TourDuLiches dựa trên ID Tour, nếu k tìm thấy tour hoặc giá k có giá trị, nó sẽ mặc định là 0
                var giaTour = db.TourDuLiches.FirstOrDefault(t => t.ID == booking.ID)?.GiaTour ?? 0;

                // Create a new DatTourChiTiet object
                var datTourChiTiet = new DatTourChiTiet
                {
                    BookingID = booking.BookingID, // Correctly reference BookingID
                    TravelerCount = travelerCount,
                    Price = giaTour * travelerCount - (giaTour * travelerCount) * 3 / 100,
                    CreateAt = DateTime.Now
                };

                // Add the new DatTourChiTiet to the database
                db.DatTourChiTiets.Add(datTourChiTiet);
                db.SaveChanges();

                return RedirectToAction("List");
            }

            ViewBag.ID = new SelectList(db.TourDuLiches, "ID", "TieuDe", booking.ID);
            ViewBag.UserID = new SelectList(db.TaiKhoans, "UserID", "TenDangNhap", booking.UserID);
            ViewBag.ContributorID = new SelectList(db.Contributors, "ContributorID", "ContributorName",booking.ContributorID);
            return View(booking);
        }


        // GET: Admin/DatTourr/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatTour booking = db.DatTours.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }

            // Retrieve the TravelerCount from DatTourChiTiet
            var datTourChiTiet = db.DatTourChiTiets.FirstOrDefault(d => d.BookingID == booking.BookingID);
            ViewBag.TravelerCount = datTourChiTiet?.TravelerCount ?? 0;

            ViewBag.ID = new SelectList(db.TourDuLiches, "ID", "TieuDe", booking.ID);
            ViewBag.UserID = new SelectList(db.TaiKhoans, "UserID", "TenDangNhap", booking.UserID);
            return View(booking);
        }

        // POST: Admin/DatTourr/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,ID,UserID,ContributorID,Status,CreateAt,ThanhToan,MaThanhToan")] DatTour booking, int travelerCount)
        {
            if (ModelState.IsValid)
            {
                if (booking.ThanhToan.HasValue && booking.ThanhToan.Value)
                {
                    if (string.IsNullOrEmpty(booking.MaThanhToan))
                    {
                        booking.MaThanhToan = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                    }
                }
                else
                {
                    booking.MaThanhToan = null;
                }

                db.Entry(booking).State = EntityState.Modified;//Danh dau booking da duoc sua doi

                // Retrieve the existing DatTourChiTiet entry
                var datTourChiTiet = db.DatTourChiTiets.FirstOrDefault(d => d.BookingID == booking.BookingID);
                if (datTourChiTiet != null)
                {
                    // Assuming GiaTour is a property in the TourDuLich model or needs to be retrieved from the database
                    var giaTour = db.TourDuLiches.FirstOrDefault(t => t.ID == booking.ID)?.GiaTour ?? 0;
                    datTourChiTiet.TravelerCount = travelerCount;
                    datTourChiTiet.Price = giaTour * travelerCount - (giaTour * travelerCount) * 3 / 100;
                    db.Entry(datTourChiTiet).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("List");
            }

            ViewBag.ID = new SelectList(db.TourDuLiches, "ID", "TieuDe", booking.ID);
            ViewBag.UserID = new SelectList(db.TaiKhoans, "UserID", "TenDangNhap", booking.UserID);
            ViewBag.ContributorID = new SelectList(db.Contributors, "ContributorID", "ContributorName", booking.ContributorID);
            ViewBag.TravelerCount = travelerCount; //cung cấp giá trị để hiển thị lên view thôi
            return View(booking);
        }


        // GET: Admin/DatTourr/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatTour booking = db.DatTours.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);

        }

        // POST: Admin/DatTourr/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // tim booking tu id
            DatTour booking = db.DatTours.Include(b => b.DatTourChiTiets).FirstOrDefault(b => b.BookingID == id);

            if (booking != null)
            {
                // Remove all related booking details first
                foreach (var detail in booking.DatTourChiTiets.ToList())
                {
                    db.DatTourChiTiets.Remove(detail);
                }

                // Remove the booking
                db.DatTours.Remove(booking);

                db.SaveChanges();
            }

            return RedirectToAction("List");
        }
        // GET: Admin/Account/Search
        public ActionResult Search(string searchQuery)
        {
            var accounts = db.DatTours.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                accounts = accounts.Where(a => a.TaiKhoan.TenHienThi.Contains(searchQuery)
                                             || a.Contributor.ContributorName.Contains(searchQuery));
                                             
            }

            return View("List", accounts.ToList());

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAPMDuLich.Areas.Admin.Controllers
{
    public class AwaitingApprovalTourController : Controller
    {
        // GET: Admin/AwaitingApprovalTour
        private DAPMDuLichEntities db = new DAPMDuLichEntities();
        public ActionResult ListPendingTours()
        {
            // Lấy tất cả các tour mà không cần lọc theo trạng thái
            var allTours = db.TourDuLiches.ToList();

            // Trả về View với danh sách tất cả các tour
            return View(allTours);
        }
        // GET: Admin/AwaitingApprovalTour/Details
        public ActionResult Details(int id)
        {
            var tour = db.TourDuLiches.Find(id);
            if (tour == null)
            {
                return HttpNotFound();
            }
            return View(tour);
        }
        [HttpPost]
        public ActionResult UpdateStatusTour(int id, bool? status)
        {
            // Tìm tour cần cập nhật
            var tour = db.TourDuLiches.Find(id);
            if (tour != null)
            {
                // Cập nhật trạng thái dựa trên giá trị truyền vào
                tour.Status = status; // Status có thể là true (đã duyệt), false (bị từ chối), hoặc null (chờ duyệt)
                db.SaveChanges();
            }
            return RedirectToAction("Details", "AwaitingApprovalTour", new { id = id }); // Quay lại trang chi tiết của tour
        }


    }
}
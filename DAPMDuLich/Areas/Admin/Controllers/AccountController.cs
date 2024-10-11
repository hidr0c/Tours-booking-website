using DAPMDuLich.App_Start;
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
    public class AccountController : Controller
    {
        private DAPMDuLichEntities db = new DAPMDuLichEntities();
        // GET: Admin/Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string tenDangNhap, string matKhau)
        {
            //1. Kiểm tra tên đăng nhập hoặc mật khẩu có trống => Trở về trang đăng nhập: Thông báo thiếu thông tin
            if (string.IsNullOrEmpty(tenDangNhap) == true | string.IsNullOrEmpty(matKhau) == true)
            {
                ViewBag.thongbao = "Thông báo thiếu thông tin";
                return View();
            }
            //2. Tìm tài khoản theo tên đăng nhập trong Database
            var taiKhoan = new mapTaiKhoan().ChiTiet(tenDangNhap);
            //3. Kiểm tra tồn tại tài khoản => nếu ko tồn tại => Trở về trang đăng nhập: Tài khoản hoặc mật khẩu không đúng
            if (taiKhoan == null)
            {
                ViewBag.thongbao = "Tài khoản hoặc mật khẩu không đúng";
                ViewBag.tenDangNhap = tenDangNhap;
                return View();
            }
            //4. Kiểm tra mật khẩu => Nếu sai => Trở về trang đăng nhập: Tài khoản hoặc mật khẩu không đúng
            //string matKhauMaHoa = new Common.MaHoa.MaHoaDuLieu().CreateMd5(matKhau);
            if (taiKhoan.MatKhau != matKhau)
            {
                ViewBag.thongbao = "Tài khoản hoặc mật khẩu không đúng";
                ViewBag.tenDangNhap = tenDangNhap;
                return View();
            }
            //5. Kiểm tra active (hoat dong)
            if (taiKhoan.Active != true)
            {
                ViewBag.thongbao = "Tài khoản đang tạm khóa";
                ViewBag.tenDangNhap = tenDangNhap;
                return View();
            }
            // 7. Kiểm tra Role của tài khoản => Nếu không phải là admin thì không được đăng nhập
            if (taiKhoan.Role != "admin")
            {
                ViewBag.thongbao = "Tài khoản không có quyền truy cập";
                ViewBag.tenDangNhap = tenDangNhap;
                return View();
            }

            //6. Tài khoản đăng nhập ok: Lưu lại session server
            Session["user"] = taiKhoan;



            //8. Chuyển hướng sang trang chủ Admin 
            return Redirect("/Admin/HomeAdmin/Index");
        }

        //[CheckPermissions(ChucNang = "TaiKhoan_ChiTiet")]
        //Danh sách tài khoản
        public ActionResult List()
        {
            return View(new mapTaiKhoan().DanhSach());
        }


        //public ActionResult Detail(string tenDangNhap)
        //{
        //    var taikhoan = new mapTaiKhoan().ChiTiet(tenDangNhap);
        //    return View(taikhoan);
        //}

        // GET: Admin/TaiKhoan/Create
        //Thêm tài khoản
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,TenDangNhap,MatKhau,TenHienThi,Email,SoDienThoai,DiaChi,CreateAt,Role,Active,Tien")] TaiKhoan taiKhoanKH)
        {
            if (ModelState.IsValid)
            {
                taiKhoanKH.CreateAt = DateTime.Now;
                db.TaiKhoans.Add(taiKhoanKH);
                db.SaveChanges();
                return RedirectToAction("List");
            }

            return View(taiKhoanKH);
        }
        // GET: Admin/TaiKhoan/Edit/5
        //Sửa tài khoản
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiKhoan taiKhoanKH = db.TaiKhoans.Find(id);
            if (taiKhoanKH == null)
            {
                return HttpNotFound();
            }
            return View(taiKhoanKH);
        }
        // POST: Admin/TaiKhoanKH/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,TenDangNhap,MatKhau,TenHienThi,Email,SoDienThoai,DiaChi,CreateAt,Role,Active,Tien")] TaiKhoan taiKhoanKH)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = db.TaiKhoans.Find(taiKhoanKH.UserID);
                if (existingAccount == null)
                {
                    return HttpNotFound();
                }

                // Giữ giá trị TenDangNhap từ cơ sở dữ liệu
                taiKhoanKH.TenDangNhap = existingAccount.TenDangNhap;
                taiKhoanKH.CreateAt = existingAccount.CreateAt;

                existingAccount.MatKhau = taiKhoanKH.MatKhau;
                existingAccount.TenHienThi = taiKhoanKH.TenHienThi;
                existingAccount.Email = taiKhoanKH.Email;
                existingAccount.SoDienThoai = taiKhoanKH.SoDienThoai;
                existingAccount.DiaChi = taiKhoanKH.DiaChi;
                existingAccount.Role = taiKhoanKH.Role;
                existingAccount.Active = taiKhoanKH.Active;
                existingAccount.Tien = taiKhoanKH.Tien;

                db.Entry(existingAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List");
            }
            return View(taiKhoanKH);
        }
        // GET: Admin/Account/Search
        public ActionResult Search(string searchQuery)
        {
            var accounts = db.TaiKhoans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                accounts = accounts.Where(a => a.TenDangNhap.Contains(searchQuery)
                                             || a.TenHienThi.Contains(searchQuery)
                                             || a.Email.Contains(searchQuery));
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
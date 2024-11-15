using BookStoreWebMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Web.UI.WebControls;
using Ganss.Xss;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Data;

namespace BookStoreWebMvc.Controllers
{
    public class AdminController : Controller
    {
        private QLBansachEntities qLBansachEntities;

        public AdminController()
        {
            qLBansachEntities = new QLBansachEntities();
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sach(int ?page)
        {
            int pageSize = 7; // Số mục mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là 1

            var saches = qLBansachEntities.SACHes.OrderBy(s => s.Masach).ToPagedList(pageNumber, pageSize);
            return View(saches);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            // Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["username"];
            var matkhau = collection["password"];

            // Kiểm tra nếu thiếu tên đăng nhập
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }

            // Kiểm tra nếu thiếu mật khẩu
            if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }

            // Nếu cả hai trường đều có giá trị, kiểm tra tính hợp lệ của tài khoản
            if (!String.IsNullOrEmpty(tendn) && !String.IsNullOrEmpty(matkhau))
            {
                // Gán giá trị cho đối tượng được tạo mới (ad)
                var ad = qLBansachEntities.Admins
                    .SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult ThemmoiSach()
        {
            // Lay ds tu table chu de, sap xep tang dan theo ten chu de, chon lay gia tri MaCD, hien thi thi TenChude
            ViewBag.MaCD = new SelectList(qLBansachEntities.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(qLBansachEntities.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }

        [HttpPost]
        public ActionResult ThemmoiSach(SACH sACH, HttpPostedFileBase fileupload)
        {
            // Làm sạch HTML trong Mota để loại bỏ các thẻ nguy hiểm
            var sanitizer = new HtmlSanitizer();
            sACH.Mota = sanitizer.Sanitize(sACH.Mota);

            // Chuyển Mota thành kiểu text thuần (plain text) sau khi làm sạch HTML
            sACH.Mota = HttpUtility.HtmlDecode(sACH.Mota);

            // Kiểm tra xem fileupload có null không (người dùng có chọn ảnh hay không)
            if (fileupload != null && fileupload.ContentLength > 0)
            {
                // Lưu tên file, lưu ý bổ sung thư viện using System.IO;
                var fileName = Path.GetFileName(fileupload.FileName);

                // Lưu đường dẫn của file
                var path = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName);

                // Kiểm tra hình ảnh tồn tại chưa
                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình ảnh đã tồn tại. Vui lòng chọn một tên khác.";
                    // Trả lại danh sách chọn MaCD và MaNXB
                    ViewBag.MaCD = new SelectList(qLBansachEntities.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
                    ViewBag.MaNXB = new SelectList(qLBansachEntities.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
                    return View(sACH);
                }
                else
                {
                    try
                    {
                        // Lưu hình ảnh vào thư mục Hinhsanpham
                        fileupload.SaveAs(path);
                        sACH.Anhbia = fileName; // Gán tên ảnh cho thuộc tính Anhbia của đối tượng SACH
                        ViewBag.Thongbao = "Lưu hình ảnh thành công.";
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi nếu có
                        ViewBag.Thongbao = "Có lỗi khi lưu hình ảnh: " + ex.Message;
                        // Trả lại danh sách chọn MaCD và MaNXB
                        ViewBag.MaCD = new SelectList(qLBansachEntities.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
                        ViewBag.MaNXB = new SelectList(qLBansachEntities.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
                        return View(sACH);
                    }
                }
            }
            else
            {
                // Thông báo khi không chọn file
                ViewBag.Thongbao = "Vui lòng chọn một tệp để tải lên.";
                // Trả lại danh sách chọn MaCD và MaNXB
                ViewBag.MaCD = new SelectList(qLBansachEntities.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
                ViewBag.MaNXB = new SelectList(qLBansachEntities.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
                return View(sACH);
            }

            // Thêm đối tượng SACH vào database
            qLBansachEntities.SACHes.Add(sACH);
            qLBansachEntities.SaveChanges(); // Lưu thay đổi vào database

            // Thiết lập danh sách chọn cho các trường MaCD và MaNXB
            ViewBag.MaCD = new SelectList(qLBansachEntities.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(qLBansachEntities.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            // Trả về View sau khi hoàn tất
            ViewBag.Thongbao = "Thêm sách mới thành công.";
            return RedirectToAction("Sach", "Admin");
        }

        [HttpGet]
        public ActionResult Suasach(int id)
        {
            var sACH = qLBansachEntities.SACHes.Find(id);

            if(sACH == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.Ngaycapnhat = DateTime.Now.ToString("dd-mm-yyyy");
            ViewBag.MaCD = new SelectList(qLBansachEntities.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sACH.MaCD);
            ViewBag.MaNXB = new SelectList(qLBansachEntities.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sACH.MaNXB);

            return View(sACH);
        }

        [HttpPost]
        public ActionResult Suasach(SACH sach, HttpPostedFileBase fileUpload)
        {
            // Làm sạch HTML trong Mota để loại bỏ các thẻ nguy hiểm
            var sanitizer = new HtmlSanitizer();
            sach.Mota = sanitizer.Sanitize(sach.Mota);

            // Chuyển Mota thành kiểu text thuần (plain text) sau khi làm sạch HTML
            sach.Mota = HttpUtility.HtmlDecode(sach.Mota);

            // Lấy thông tin sách hiện tại từ cơ sở dữ liệu
            var sachHienTai = qLBansachEntities.SACHes.Find(sach.Masach);
            if (sachHienTai == null)
            {
                ViewBag.Thongbao = "Sách không tồn tại.";
                return View(sach);
            }

            // Kiểm tra xem người dùng có chọn file mới không
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                // Lưu tên file mới
                var fileName = Path.GetFileName(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName);

                // Kiểm tra trùng tên ảnh nếu ảnh mới khác ảnh hiện tại
                if (fileName != sachHienTai.Anhbia && System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình ảnh đã tồn tại. Vui lòng chọn một tên khác.";
                    return View(sach);
                }
                else
                {
                    try
                    {
                        // Lưu hình ảnh mới vào thư mục Hinhsanpham
                        fileUpload.SaveAs(path);
                        sach.Anhbia = fileName; // Gán tên ảnh mới cho thuộc tính Anhbia của đối tượng SACH
                        ViewBag.Thongbao = "Lưu hình ảnh thành công.";
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi nếu có
                        ViewBag.Thongbao = "Có lỗi khi lưu hình ảnh: " + ex.Message;
                        return View(sach);
                    }
                }
            }
            else
            {
                // Nếu người dùng không chọn ảnh mới, giữ lại ảnh hiện tại
                sach.Anhbia = sachHienTai.Anhbia;
            }

            // Cập nhật đối tượng SACH trong database
            if (ModelState.IsValid)
            {
                // Tách đối tượng sachHienTai ra khỏi Entity Framework để tránh theo dõi hai lần
                qLBansachEntities.Entry(sachHienTai).State = EntityState.Detached;

                // Cập nhật lại đối tượng với các thay đổi mới
                qLBansachEntities.Entry(sach).State = EntityState.Modified;
                qLBansachEntities.SaveChanges(); // Lưu thay đổi vào database
            }

            // Thiết lập lại danh sách chọn cho các trường MaCD và MaNXB với giá trị đã chọn
            ViewBag.MaCD = new SelectList(qLBansachEntities.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(qLBansachEntities.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);

            ViewBag.Thongbao = "Cập nhật sách thành công.";
            return RedirectToAction("Sach", "Admin");
        }


        public ActionResult Chitietsach(int id)
        {
            SACH sACH = qLBansachEntities.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sACH.Masach;
            if(sACH == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(sACH);
        }

        [HttpGet]
        public ActionResult Xoasach(int id)
        {
            SACH sACH = qLBansachEntities.SACHes.SingleOrDefault(n => n.Masach == id);
            if (sACH == null)
            {
                return HttpNotFound("Sách không tồn tại.");
            }

            return View(sACH);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Xacnhanxoa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID không hợp lệ.");
            }

            SACH sACH = qLBansachEntities.SACHes.SingleOrDefault(n => n.Masach == id);

            if (sACH == null)
            {
                return HttpNotFound("Sách không tồn tại.");
            }

            try
            {
                // Đường dẫn thư mục chứa hình ảnh
                string imageFolderPath = Server.MapPath("~/Hinhsanpham/");
                string imagePath = Path.Combine(imageFolderPath, sACH.Anhbia); // `HinhAnh` là trường lưu tên file ảnh

                // Xóa hình ảnh nếu tồn tại
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Xóa các bản ghi liên quan trong bảng VIETSACH
                var vietsach = qLBansachEntities.VIETSACHes.Where(v => v.Masach == id).ToList();
                foreach (var item in vietsach)
                {
                    qLBansachEntities.VIETSACHes.Remove(item);
                }

                // Xóa các bản ghi liên quan trong bảng CHITIETDONHANG
                var chitietdonhang = qLBansachEntities.CHITIETDONTHANGs.Where(c => c.Masach == id).ToList();
                foreach (var item in chitietdonhang)
                {
                    qLBansachEntities.CHITIETDONTHANGs.Remove(item);
                }

                // Xóa bản ghi sách
                qLBansachEntities.SACHes.Remove(sACH);
                qLBansachEntities.SaveChanges();
                return RedirectToAction("Sach", "Admin"); // Chuyển hướng về trang Sach.cshtml
            }
            catch (DbUpdateException)
            {
                ViewBag.Thongbao = "Không thể xóa sách này vì có dữ liệu liên quan.";
                return View("Xoasach", sACH);
            }
        }


        public ActionResult Thongkesach()
        {
            // Lấy dữ liệu số lượng sách theo từng chủ đề
            var data = qLBansachEntities.CHUDEs
                .Select(cd => new
                {
                    TenChuDe = cd.TenChuDe,
                    SoLuong = cd.SACHes.Count() // Đếm số sách trong từng chủ đề
                })
                .ToList();

            // Truyền dữ liệu qua ViewBag để sử dụng trong view
            ViewBag.ChartData = data;

            return View();
        }


    }
}
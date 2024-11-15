using BookStoreWebMvc.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BookStoreWebMvc.Controllers
{
    public class NguoidungController : Controller
    {
        QLBansachEntities qLBansachEntities = new QLBansachEntities();
        // GET: Nguoidung
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KHACHHANG kh)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            var nhaplaimatkhau = collection["Nhaplaimatkhau"];
            var email = collection["email"];
            var diachi = collection["Diachi"];
            var dienthoai = collection["Dienthoai"];
            var ngaysinh = collection["Ngaysinh"];

            // Khởi tạo thông báo lỗi
            bool hasError = false;

            // Kiểm tra thông tin bắt buộc
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống.";
                hasError = true;
            }
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập.";
                hasError = true;
            }
            else if (qLBansachEntities.KHACHHANGs.Any(k => k.Taikhoan == tendn))
            {
                ViewData["Loi2"] = "Tên đăng nhập đã tồn tại.";
                hasError = true;
            }

            if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu.";
                hasError = true;
            }
            else if (matkhau != nhaplaimatkhau)
            {
                ViewData["Loi4"] = "Mật khẩu và nhập lại mật khẩu không khớp.";
                hasError = true;
            }

            if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Email không được bỏ trống.";
                hasError = true;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ViewData["Loi5"] = "Email không đúng định dạng.";
                hasError = true;
            }
            else if (qLBansachEntities.KHACHHANGs.Any(k => k.Email == email))
            {
                ViewData["Loi5"] = "Email đã được sử dụng.";
                hasError = true;
            }

            if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi6"] = "Phải nhập số điện thoại.";
                hasError = true;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(dienthoai, @"^\d{10,11}$"))
            {
                ViewData["Loi6"] = "Số điện thoại không đúng định dạng (phải từ 10-11 chữ số).";
                hasError = true;
            }
            else if (qLBansachEntities.KHACHHANGs.Any(k => k.DienthoaiKH == dienthoai))
            {
                ViewData["Loi6"] = "Số điện thoại đã được sử dụng.";
                hasError = true;
            }

            // Nếu có lỗi, trả về view đăng ký
            if (hasError)
            {
                return View();
            }

            // Thêm khách hàng mới
            kh.HoTen = hoten;
            kh.Taikhoan = tendn;
            kh.Matkhau = matkhau;
            kh.Email = email;
            kh.DiachiKH = diachi;
            kh.DienthoaiKH = dienthoai;

            if (!String.IsNullOrEmpty(ngaysinh))
            {
                if (DateTime.TryParse(ngaysinh, out DateTime parsedDate))
                {
                    kh.Ngaysinh = parsedDate;
                }
                else
                {
                    ViewData["Loi7"] = "Ngày sinh không hợp lệ.";
                    return View();
                }
            }

            qLBansachEntities.KHACHHANGs.Add(kh);
            qLBansachEntities.SaveChanges();

            return RedirectToAction("Dangnhap");
        }

        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];

            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập.";
            }

            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu.";
            }
            else
            {
                KHACHHANG kh = qLBansachEntities.KHACHHANGs
                                .FirstOrDefault(n => n.Taikhoan == tendn && n.Matkhau == matkhau);

                if (kh != null)
                {
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công.";
                    Session["Taikhoan"] = kh;
                    return RedirectToAction("Index", "BookStore");
                }
                else ViewBag.Thongbao = "Tên đăng nhập thông tồn tại hoặc mật khẩu không chính xác.";
            }

            return View();
        }

        public ActionResult Dangxuat()
        {
            // Xóa session để đăng xuất
            Session.Remove("Taikhoan");
            return RedirectToAction("Index", "BookStore");
        }

        public ActionResult ThongtinTaikhoan()
        {
            if (Session["Taikhoan"] == null) { return RedirectToAction("Dangnhap", "Nguoidung"); }
            var kh = (KHACHHANG)Session["Taikhoan"];
            return View(kh);
        }
    }
}
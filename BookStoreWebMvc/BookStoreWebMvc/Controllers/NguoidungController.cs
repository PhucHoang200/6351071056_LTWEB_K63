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
            var ngaysinh = String.Format("{0: MM/dd/yyyy}", collection["Ngaysinh"]);

            if (String.IsNullOrEmpty(hoten)) { ViewData["Loi 1"] = "Họ tên khách hàng không được để trống."; }
            else if (String.IsNullOrEmpty(tendn)) { ViewData["Loi 2"] = "Phải nhập tên đăng nhập."; }
            else if(String.IsNullOrEmpty(matkhau)) { ViewData["Loi 3"] = "Phải nhập mật khẩu."; }
            else if(String.IsNullOrEmpty(nhaplaimatkhau)) { ViewData["Loi 4"] = "Phải nhập lại mật khẩu"; }
            if (String.IsNullOrEmpty(email)) { ViewData["Loi 5"] = "Email không được bỏ trống."; }
            if(String.IsNullOrEmpty(dienthoai)) { ViewData["Loi 7"] = "Phải nhập điện thoại."; }
            else
            {
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = matkhau;
                kh.DiachiKH = diachi;
                kh.DienthoaiKH = dienthoai;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                qLBansachEntities.KHACHHANGs.Add(kh);
                qLBansachEntities.SaveChanges();
                return RedirectToAction("Dangnhap");

            }
            return this.Dangky();
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
                else ViewBag.Thongbao = "Tên đăng nhập thông tồn tại.";
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
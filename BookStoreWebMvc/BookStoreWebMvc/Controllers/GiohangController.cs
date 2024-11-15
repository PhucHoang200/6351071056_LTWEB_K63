using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStoreWebMvc.Models;

namespace BookStoreWebMvc.Controllers
{
    public class GiohangController : Controller
    {
        QLBansachEntities qLBansachEntities = new QLBansachEntities();
        // GET: Giohang
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if(lstGiohang == null)
            {
                lstGiohang=new List<Giohang>();
                Session["Giohang"]=lstGiohang;
            }       
            return lstGiohang;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThemGioHang(int iMaSach, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.Find(n => n.iMasach == iMaSach);
            if(sanpham == null)
            {
                sanpham=new Giohang(iMaSach);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }

            else
            {
                sanpham.iSoluong++;
            }

            // Kiểm tra nếu strURL không null và không trống
            if (string.IsNullOrEmpty(strURL))
            {
                strURL = "/";  // Điều hướng đến trang mặc định nếu không có URL
            }

            return Redirect(strURL);
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if( lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }

        private Double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if( lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n=>n.dThanhtien);
            }
            return iTongTien;
        }

        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)  // Nếu giỏ hàng rỗng
            {
                ViewBag.Tongsoluong = 0;
                ViewBag.Tongtien = 0;
            }
            else
            {
                ViewBag.Tongsoluong = TongSoLuong();
                ViewBag.Tongtien = TongTien();
            }
            return View(lstGiohang);
        }

        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGiohang(int iMaSP)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.FirstOrDefault(n => n.iMasach==iMaSP);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMasach == iMaSP);
                return RedirectToAction("Giohang");
            }

            if(lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            return RedirectToAction("Giohang");

        }

        public ActionResult CapnhatGiohang(int iMaSP, int txtSoluong)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.FirstOrDefault(n => n.iMasach == iMaSP);
            if (sanpham != null) 
            {
                sanpham.iSoluong = txtSoluong;
            }
            return RedirectToAction("Giohang");
        }

        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Bookstore");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "BookStore");
            }

            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            // Kiểm tra giỏ hàng có sản phẩm chưa
            List<Giohang> gh = Laygiohang();
            if (gh == null || gh.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }

            // Lấy thông tin khách hàng từ session
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            DONDATHANG ddh = new DONDATHANG
            {
                MaKH = kh.MaKH,
                Ngaydat = DateTime.Now,
                Tinhtranggiaohang = false,
                Dathanhtoan = false
            };

            // Lấy và kiểm tra ngày giao hàng từ form
            var ngaygiaoStr = collection["Ngaygiao"];
            DateTime ngaygiao;
            bool isValidDate = DateTime.TryParseExact(ngaygiaoStr, "yyyy-MM-dd", // Định dạng yyyy-MM-dd cho input type="date"
                                                      System.Globalization.CultureInfo.InvariantCulture,
                                                      System.Globalization.DateTimeStyles.None,
                                                      out ngaygiao);
            if (isValidDate)
            {
                ddh.Ngaygiao = ngaygiao;
            }
            else
            {
                ModelState.AddModelError("Ngaygiao", "Ngày giao không hợp lệ.");
                ViewBag.Tongsoluong = TongSoLuong();
                ViewBag.Tongtien = TongTien();
                return View("DatHang", gh); // Trả về trang đặt hàng nếu có lỗi
            }

            // Thêm đơn đặt hàng vào cơ sở dữ liệu
            qLBansachEntities.DONDATHANGs.Add(ddh);
            qLBansachEntities.SaveChanges();

            // Thêm chi tiết đơn đặt hàng
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG
                {
                    MaDonHang = ddh.MaDonHang,
                    Masach = item.iMasach,
                    Soluong = item.iSoluong,
                    Dongia = (decimal)item.dDongia
                };
                qLBansachEntities.CHITIETDONTHANGs.Add(ctdh);
            }
            qLBansachEntities.SaveChanges();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["Giohang"] = null;

            // Chuyển đến trang xác nhận đơn hàng
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }

        public ActionResult Xacnhandonhang()
        {
            return View();
        }

    }
}
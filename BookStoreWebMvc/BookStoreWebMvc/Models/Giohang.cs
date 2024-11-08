using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookStoreWebMvc.Models;

namespace BookStoreWebMvc.Models
{
    public class Giohang
    {
        QLBansachEntities qLBansachEntities = new QLBansachEntities();

        public int iMasach {  get; set; }
        public string sTensach { get; set; }
        public string sAnhbia { get; set; }
        public Double dDongia { get; set; }
        public int iSoluong {  get; set; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }

        public Giohang(int Masach)
        {
            iMasach = Masach;
            SACH sACH = qLBansachEntities.SACHes.First(n => n.Masach == iMasach);
            sTensach = sACH.Tensach;
            sAnhbia = sACH.Anhbia;
            dDongia = Double.Parse(sACH.Giaban.ToString());
            iSoluong = 1;
        }
    }
}
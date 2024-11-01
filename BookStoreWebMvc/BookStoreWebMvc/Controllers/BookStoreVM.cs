using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookStoreWebMvc.Models;

namespace BookStoreWebMvc.Models
{
    public class BookStoreVM
    {
        public List<SACH> saches { get; set; }
        public List<CHUDE> chudes { get; set; }
        public List<NHAXUATBAN> nhaxuatbans { get; set; }
    }
}
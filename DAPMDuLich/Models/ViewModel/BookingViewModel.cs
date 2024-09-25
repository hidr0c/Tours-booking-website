using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAPMDuLich.Models.ViewModel
{
    public class BookingViewModel
    {
        public string HoTen { set; get; }
        public string Email { set; get; }
        public string Ten { get; set; }
        public int ID { set; get; }
        public string TieuDe { set; get; }
        public string BaiViet { get; set; }
        public string PhuongTien { get; set; }
        public DateTime? TripStart { set; get; }
        public DateTime? TripEnd { set; get; }

        public decimal? GiaTour { get; set; }
        public Nullable<int> SoNgayDiTour { get; set; }
        public string LichTrinh { get; set; }
        public string DiaDiem { get; set; }
        public Nullable<int> SoNguoiToiDa { get; set; }
        public int? TravelerCount { set; get; }
        public DateTime? CreateAt { set; get; }
    }
}
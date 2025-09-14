﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATN.Domain.DTO
{
    public class nguoi_dung_dto : BaseAuditableDto
    {
        public string tai_khoan { get; set; }
        public string mat_khau { get; set; }
        public string? salt_code { get; set; }
        public string ten { get; set; }
        public DateTime? ngay_sinh { get; set; }
        public string? email { get; set; }
        public string? so_dien_thoai { get; set; }
        public bool? gioi_tinh { get; set; }
        public bool? is_doi_mk { get; set; }
        public DateTime? ngay_doi_mk { get; set; }
        public string? RefreshToken { get; set; } // Lưu Refresh Token
        public DateTime? RefreshTokenExpiryTime { get; set; } // Hạn sử dụng của Refresh Token
        public bool? trang_thai { get; set; }
        public List<nhom_nguoi_dung_dto>? ds_nhom_nguoi_dung { get; set; }
        public List<danh_muc_dto>? dsPhongBan { get; set; }
        public string? token { get; set; }
        public string? errrorMessage { get; set; }

    }
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}

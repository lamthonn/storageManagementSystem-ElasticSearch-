﻿

using DATN.Application.Authentication;
using DATN.Application.DanhMucCapDo;
using DATN.Application.DanhMucPhongBan;
using DATN.Application.NhomNguoiDung;
using DATN.Application.Interfaces;
using DATN.Application.DieuHuong;
using DATN.Application.Utils;

namespace DATN.Api.ServiceManage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Đăng ký các service tại đây
            services.AddTransient<Helper>();
            services.AddTransient<IAuthen, authen>();
            services.AddTransient<IDanhMucCapDo, DanhMucCapDoService>();
            services.AddTransient<IDanhMucPhongBan, DanhMucPhongBanService>();
            services.AddTransient<INhomNguoiDung, NhomNguoiDungService>();
            services.AddTransient<IDieuHuong, DieuHuongService>();

            return services;
        }
    }
}

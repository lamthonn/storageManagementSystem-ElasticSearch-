using DATN.Application.Utils;
using DATN.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATN.Application.Interfaces
{
    public interface INhomNguoiDung
    {
        public Task<nhom_nguoi_dung_dto> GetNhomNguoiDung(Guid nguoi_dung_id);
        public Task<PaginatedList<nhom_nguoi_dung_dto>> GetPaginNhomNguoiDung(nhom_nguoi_dung_dto request); 
        public Task<List<dieu_huong_dto>> GetDsMenuByNND(Guid nguoi_dung_id);
        public Task<string> PhanQuyen(PhanQuyenDto request);
        public Task<List<dieu_huong_dto>> GetPhanQuyen(Guid nhom_nguoi_dung_id);

    }

    public class PhanQuyenDto
    {
        public Guid nhom_Nguoi_dung_id { get; set; }
        public List<dieu_huong_dto> ds_dieu_huong { get; set; }
    }
}

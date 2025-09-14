using DATN.Application.Interfaces;
using DATN.Application.Utils;
using DATN.Domain.DTO;
using DATN.Domain.Entities;
using DATN.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DATN.Application.NhomNguoiDung
{
    public class NhomNguoiDungService : INhomNguoiDung
    {
        private readonly AppDbContext _context;
        public NhomNguoiDungService(AppDbContext context)
        {
            _context = context;
        }

        public Task<nhom_nguoi_dung_dto> GetNhomNguoiDung(Guid nguoi_dung_id)
        {
            var nhomNguoiDungMacDinh = _context.nguoi_dung_2_nhom_nguoi_dung.FirstOrDefault(x => x.nguoi_dung_id == nguoi_dung_id && x.mac_dinh == true);
            if (nhomNguoiDungMacDinh != null)
            {
                var nhomNguoiDung = _context.nhom_nguoi_dung.FirstOrDefault(x => x.Id == nhomNguoiDungMacDinh.nhom_nguoi_dung_id);
                if (nhomNguoiDung != null)
                {
                    return Task.FromResult(new nhom_nguoi_dung_dto
                    {
                        Id = nhomNguoiDung.Id,
                        ma = nhomNguoiDung.ma,
                        ten = nhomNguoiDung.ten,
                        mo_ta = nhomNguoiDung.mo_ta,
                        trang_thai = nhomNguoiDung.trang_thai
                    });
                }
                else
                {
                    throw new Exception("Không tìm thấy nhóm người dùng mặc định cho người dùng này.");
                }
            }
            else
            {
                throw new Exception("Không tìm thấy nhóm người dùng mặc định cho người dùng này.");
            }
        }

        public async Task<List<dieu_huong_dto>> GetDsMenuByNND(Guid nhom_nguoi_dung_id)
        {
            // Lấy danh sách phẳng các điều hướng từ nhom_nguoi_dung_2_dieu_huong
            var lstDieuHuong = await _context.nhom_nguoi_dung_2_dieu_huong
                .Where(x => x.nhom_nguoi_dung_id == nhom_nguoi_dung_id)
                .Select(x => new dieu_huong_dto
                {
                    Id = x.dieu_huong.Id,
                    ma = x.dieu_huong.ma,
                    ten = x.dieu_huong.ten,
                    duong_dan = x.dieu_huong.duong_dan,
                    so_thu_tu = x.dieu_huong.so_thu_tu,
                    mo_ta = x.dieu_huong.mo_ta,
                    cap_dieu_huong = x.dieu_huong.cap_dieu_huong,
                    dieu_huong_cap_tren_id = x.dieu_huong.dieu_huong_cap_tren_id
                })
                .ToListAsync();

            // Xây dựng cây điều hướng
            var lstDieuHuongTree = new List<dieu_huong_dto>();

            // Lấy các điều hướng cấp cao nhất (không có điều hướng cấp trên)
            var rootMenus = lstDieuHuong
                .Where(x => !x.dieu_huong_cap_tren_id.HasValue)
                .OrderBy(x => x.so_thu_tu)
                .ToList();

            // Đệ quy để xây dựng cây
            foreach (var rootMenu in rootMenus)
            {
                BuildMenuTree(rootMenu, lstDieuHuong);
                lstDieuHuongTree.Add(rootMenu);
            }

            return lstDieuHuongTree;
        }

        // Hàm đệ quy để xây dựng cây điều hướng
        private void BuildMenuTree(dieu_huong_dto parentMenu, List<dieu_huong_dto> allMenus)
        {
            // Tìm các menu con của menu cha hiện tại
            var childMenus = allMenus
                .Where(x => x.dieu_huong_cap_tren_id == parentMenu.Id)
                .OrderBy(x => x.so_thu_tu)
                .ToList();

            // Gán danh sách menu con vào parentMenu
            parentMenu.danh_sach_dieu_huong_con = childMenus;

            // Đệ quy để xây dựng cây cho từng menu con
            foreach (var childMenu in childMenus)
            {
                BuildMenuTree(childMenu, allMenus);
            }
        }

        public async Task<PaginatedList<nhom_nguoi_dung_dto>> GetPaginNhomNguoiDung(nhom_nguoi_dung_dto request)
        {
            try
            {
                var userGroups = _context.nhom_nguoi_dung.AsNoTracking();
                if (request.keySearch != null)
                {
                    userGroups = userGroups.Where(x => x.ma.Contains(request.keySearch) || x.ten.Contains(request.keySearch));
                }

                if (request.trang_thai != null)
                {
                    userGroups = userGroups.Where(x => x.trang_thai == request.trang_thai);
                }

                var dataQueryDto = userGroups
                                    .OrderByDescending(x => x.ngay_tao)
                                    .Select(x => new nhom_nguoi_dung_dto
                                    {
                                        Id = x.Id,
                                        ma = x.ma,
                                        ten = x.ten,
                                        mo_ta = x.mo_ta,
                                        ngay_tao = x.ngay_tao,
                                        nguoi_tao = x.nguoi_tao,
                                        ngay_chinh_sua = x.ngay_chinh_sua,
                                        nguoi_chinh_sua = x.nguoi_chinh_sua,
                                        trang_thai = x.trang_thai
                                    });
                var result = await PaginatedList<nhom_nguoi_dung_dto>.Create(dataQueryDto, request.pageNumber, request.pageSize);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> PhanQuyen(PhanQuyenDto request)
        {
            try
            {
                var dsNhomNguoiDung2DieuHuong = _context.nhom_nguoi_dung_2_command.Where(x => x.nhom_nguoi_dung_id == request.nhom_Nguoi_dung_id).ToList();
                var dsDieuHuong = request.ds_dieu_huong;

                //nếu đã có quyền
                if (dsNhomNguoiDung2DieuHuong != null && dsNhomNguoiDung2DieuHuong.Count > 0)
                {
                    var listThemMoi = new List<nhom_nguoi_dung_2_command>();
                    var listXoa = new List<nhom_nguoi_dung_2_command>();

                    foreach (var item in dsDieuHuong)
                    {
                        if (item.permission != null && item.permission.Count > 0)
                        {
                            foreach (var item2 in item.permission)
                            {
                                // 1. Request có && DB không có => thêm mới
                                if (!dsNhomNguoiDung2DieuHuong.Any(x => x.dieu_huong_id == item.Id && x.command_id == item2.id))
                                {
                                    var newNND2DH = new nhom_nguoi_dung_2_command
                                    {
                                        id = Guid.NewGuid(),
                                        nhom_nguoi_dung_id = request.nhom_Nguoi_dung_id,
                                        dieu_huong_id = item.Id,
                                        command_id = item2.id,
                                        command = item2.command,
                                    };
                                    listThemMoi.Add(newNND2DH);
                                }
                                // 2. Request có && DB có => bỏ qua (không làm gì)
                            }
                            // 3. Request không có nhưng DB có => xóa
                            var quyenTrongDb = dsNhomNguoiDung2DieuHuong
                                .Where(x => x.dieu_huong_id == item.Id)
                                .ToList();
                            foreach (var q in quyenTrongDb)
                            {
                                if (!item.permission.Any(p => p.id == q.command_id))
                                {
                                    listXoa.Add(q);
                                }
                            }
                        }
                        else
                        {
                            // Trường hợp request không có permission nào => chỉ lưu dòng command_id = null
                            // Nếu DB đã có mà request không có => xóa
                            var quyenTrongDb = dsNhomNguoiDung2DieuHuong
                            .Where(x => x.dieu_huong_id == item.Id)
                            .ToList();

                            foreach (var q in quyenTrongDb)
                            {
                                if (q.command_id != null) // khác null thì xóa
                                {
                                    listXoa.Add(q);
                                }
                            }

                            // Nếu DB chưa có record command_id = null thì thêm mới
                            if (!quyenTrongDb.Any(x => x.command_id == null))
                            {
                                var newNND2DH = new nhom_nguoi_dung_2_command
                                {
                                    id = Guid.NewGuid(),
                                    nhom_nguoi_dung_id = request.nhom_Nguoi_dung_id,
                                    dieu_huong_id = item.Id,
                                    command_id = null,
                                };
                                listThemMoi.Add(newNND2DH);
                            }
                        }
                    }

                    // Thực hiện thêm / xóa
                    if (listThemMoi.Any())
                        await _context.nhom_nguoi_dung_2_command.AddRangeAsync(listThemMoi);

                    if (listXoa.Any())
                        _context.nhom_nguoi_dung_2_command.RemoveRange(listXoa);

                    await _context.SaveChangesAsync();
                    return "Phân quyền thành công";
                }
                // nếu nhóm người dùng chưa có quyền
                else
                {
                    var result = new List<nhom_nguoi_dung_2_command>();
                    foreach (var item in dsDieuHuong)
                    {
                        if (item.permission != null && item.permission.Count > 0)
                        {
                            foreach (var item2 in item.permission)
                            {
                                var newNND2DH = new nhom_nguoi_dung_2_command
                                {
                                    id = Guid.NewGuid(),
                                    nhom_nguoi_dung_id = request.nhom_Nguoi_dung_id,
                                    dieu_huong_id = item.Id,
                                    command_id = item2.id,
                                    command = item2.command,
                                };
                                result.Add(newNND2DH);
                            }
                        }
                        else
                        {
                            var newNND2DH = new nhom_nguoi_dung_2_command
                            {
                                id = Guid.NewGuid(),
                                nhom_nguoi_dung_id = request.nhom_Nguoi_dung_id,
                                dieu_huong_id = item.Id,
                                command_id = null,
                            };
                            result.Add(newNND2DH);
                        }
                    }

                    await _context.nhom_nguoi_dung_2_command.AddRangeAsync(result);
                    await _context.SaveChangesAsync();

                    return "Phân quyền thành công";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<List<dieu_huong_dto>> GetPhanQuyen(Guid nhom_nguoi_dung_id)
        {
            try
            {
                var nhomNguoiDung = _context.nhom_nguoi_dung_2_command.Where(x => x.nhom_nguoi_dung_id == nhom_nguoi_dung_id).AsNoTracking().ToList().GroupBy(x => x.dieu_huong_id);
                var result = new List<dieu_huong_dto>();
                if (nhomNguoiDung != null)
                {
                    foreach(var item in nhomNguoiDung)
                    {
                        result.Add(new dieu_huong_dto
                        {
                            Id = item.Key,
                            ma = _context.dieu_huong.FirstOrDefault(x => x.Id == item.Key) != null ? _context.dieu_huong.FirstOrDefault(x => x.Id == item.Key).ma : null,
                            ten = _context.dieu_huong.FirstOrDefault(x => x.Id == item.Key) != null ? _context.dieu_huong.FirstOrDefault(x => x.Id == item.Key).ten : null,
                            permission = item.Any(x=> x.command_id != null) ? item.Select(x => new command_dto
                            {
                                id = x.command_id,
                                command = x.command,
                            }).ToList() : new List<command_dto>()
                        });
                       
                    }
                }
                if(result != null)
                {
                    return Task.FromResult(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

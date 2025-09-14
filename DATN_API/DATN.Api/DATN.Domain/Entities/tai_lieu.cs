using DATN.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATN.Domain.Entities
{
    public class tai_lieu : BaseAuditableEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string duong_dan { get; set; }
        public int cap_do { get; set; }
        public string phong_ban { get; set; }
        public bool is_share { get; set; }
        public int phien_ban { get; set; }
        public virtual ICollection<tai_lieu_2_nguoi_dung> ds_nguoi_dung { get; set; }

    }
}

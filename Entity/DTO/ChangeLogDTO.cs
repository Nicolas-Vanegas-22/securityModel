using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO
{
    public class ChangeLogDTO
    {
        public int ChangeLogId { get; set; }
        public string Description { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}


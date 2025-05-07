using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class DestinationActivity
    {
        public int DestinationActivityId { get; set; }
        public int DestinationId { get; set; }
        public int ActivityId { get; set; }
    }
}

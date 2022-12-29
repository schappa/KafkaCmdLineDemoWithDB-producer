using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consumer.Models
{
    public class Shipment
    {
        public DateTime PacketRcvd { get; set; }
        public int PacketQty { get; set; }
        public Guid PacketId { get; set; }
        public List<Item> Items { get; set; }
    }
}

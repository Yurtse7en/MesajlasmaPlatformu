using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesajlasmaPlatformu.ViewModel
{
    public class MesajModel
    {
        public int messageId { get; set; }
        public int senderId { get; set; }
        public int recipientId { get; set; }
        public string content { get; set; }
        public string mesajtarih { get; set; }

    }
}
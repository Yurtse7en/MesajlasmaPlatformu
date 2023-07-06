using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesajlasmaPlatformu.ViewModel
{
    public class GroupMemberModel
    {
        public int gmId { get; set; }
        public int gmgrupId { get; set; }
        public int gmuserId { get; set; }
        public string grupAdi { get; set; }
        public string userName { get; set; }

    }
}
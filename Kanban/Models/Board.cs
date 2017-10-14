using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kanban.Models
{
    public class Board
    {
        public int ID { get; set; }
        public string OwnerID { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
    }
}
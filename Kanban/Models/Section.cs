using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kanban.Models
{
    public class Section
    {
        public int ID { get; set; }
        public int BoardID { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public int ParentID { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
        public virtual Board Board { get; set; }
    }
}
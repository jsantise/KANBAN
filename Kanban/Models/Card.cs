using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kanban.Models
{
    public class Card
    {
        public enum Color
        {
            blue, purple, red, pink, deeppurple, indigo, lightblue, cyan,
            teal, green, lightgreen, lime, yellow, amber, orange, deeporange,
            brown, bluegrey
        }

        public int ID { get; set; }
        public int SectionID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Color? CardColor { get; set; }
        public int Order { get; set; }

        public virtual Section Section { get; set; }
    }
}
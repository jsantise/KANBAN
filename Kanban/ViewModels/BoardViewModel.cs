using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kanban.Models;

namespace Kanban.ViewModels
{
    public class BoardViewModel
    {
        public IEnumerable<MiniBoard> Boards { get; set; }
        public Board SelectedBoard { get; set; }
        public IEnumerable<SectionRow> SectionTable { get; set; }
        public IEnumerable<Section> ActiveSections { get; set; }
    }

    // Contains only the info needed for the view.
    public class MiniBoard
    {
        public MiniBoard(int id, string title)
        {
            ID = id;
            Title = title;
        }
        public int ID { get; set; }
        public string Title { get; set; }
    }

    // Gets the Section (row) of the Kanban board
    public class SectionRow
    {
        public IEnumerable<Section> Row { get; set; }
        public int Order { get; set; }

        // Gets the "table" of that section
        public IEnumerable<SectionRow> getTable(List<Section> sections)
        {
            List<SectionRow> sectionTable = new List<SectionRow>();
            List<int> visitedSections = sections.Select(s => s.ID).ToList();
            int i = 1;

            // Visit each section of the row
            while (visitedSections.Count > 0)
            {
                SectionRow row = new SectionRow();
                row.Order = i;
                List<Section> columns = new List<Section>();
                List<Section> Parents = sections.Where(s => s.ParentID == 0).ToList();
                // Get the parent if any exist for each column
                if (i != 1)
                {
                    Parents = new List<Section>();
                    SectionRow parentRow = sectionTable.Where(t => t.Order == i - 1).FirstOrDefault();
                    foreach (var p in parentRow.Row)
                    {
                        foreach (var s in sections.Where(s => s.ParentID == p.ID))
                        {
                            Parents.Add(s);
                        }
                    }
                }
                // Add the parent column and remove it from the visitedSections
                foreach (var s in Parents)
                {
                    columns.Add(s);
                    visitedSections.Remove(s.ID);
                }
                row.Row = columns;
                sectionTable.Add(row);
                i++;
            }

            return sectionTable;
        }

        public IEnumerable<Section> getActiveSections(List<Section> sections, IEnumerable<SectionRow> table)
        {
            List<Section> active = new List<Section>();
            List<Section> start = new List<Section>();
            List<Section> end = new List<Section>();
            foreach (var row in table)
            {
                foreach (var item in row.Row)
                {
                    List<Section> children = sections.Where(s => s.ParentID == item.ID).OrderBy(s => s.Order).ToList();
                    if (active.Contains(item))
                    {
                        active.InsertRange(active.IndexOf(item) + 1, children);
                    }
                    else
                    {
                        active.Add(item);
                        active.AddRange(children);
                    }

                    if (children.Count() > 0)
                    {
                        //a parent
                        active.Remove(item);
                    }
                }
            }

            return active;
        }
    }
}
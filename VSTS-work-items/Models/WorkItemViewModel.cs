using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VSTS_work_items.Models
{
    public class WorkItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string State { get; set; }
        public string Project { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string AssignedTo { get; set; }
        public string IterationPath { get; set; }
        public string RelatedLinks { get; set; }
    }

    public class DetailsViewModel
    {
        public string Name { get; set; }
        public string ReferenceName { get; set; }
    }
}
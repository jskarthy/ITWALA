using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITWALA.Models
{
    public class GpaViewModel
    {
        [Required]
        public short RegulationYear { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public byte Semester { get; set; }

        public virtual Department Department { get; set; }

        public List<SelectListItem> Regulations
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                MyDatabaseEntities db = new MyDatabaseEntities();
                var years = db.SubjectCategories.Select(s => s.RegulationYear).Distinct().ToList();
                foreach (var year in years)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = year.ToString(),
                        Value = year.ToString()
                    });
                }
                return items;
            }
        }

        public List<SelectListItem> Semesters
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                for (var i = 1; i <= 8; i++)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = "Semester " + i,
                        Value = i.ToString()
                    });
                }
                return items;
            }
        }

    }

    public class ElectiveSubjects
    {
        public List<SelectListItem> ElectiveSelectListItems { get;set; }
    }

    //public class Electives
    //{
    //    public List<ElectiveSubjects> ElectiveSubjects { get; set; }
    //}
}
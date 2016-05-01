using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITWALA.Models
{
    public class SubjectViewModel
    {
        public string SubjectId { get; set; }

        public string SubjectName { get; set; }

        public byte Credits { get; set; }
        public String Grade { get; set; }

        public int Marks { get; set; }

        public List<SubjectViewModel> ConvertToSubjectViewModels(List<Subject> model)
        {
            List<SubjectViewModel> subjects = new List<SubjectViewModel>();
            foreach (var item in model)
            {
                SubjectViewModel subject = new SubjectViewModel();
                subject.SubjectId = item.SubjectId;
                subject.SubjectName = item.SubjectName;
                subject.Credits = item.Credits;
                subject.Grade = String.Empty;
                subject.Marks = 0;
                subjects.Add(subject);
            }

            return subjects;
        }

        public List<SubjectViewModel> ConvertElectiveListToSubjectViewModels(List<ElectiveSubject> model)
        {
            List<SubjectViewModel> subjects = new List<SubjectViewModel>();
            foreach (var item in model)
            {
                SubjectViewModel subject = new SubjectViewModel();
                subject.SubjectId = item.SubjectId;
                subject.SubjectName = item.SubjectName;
                subject.Credits = item.Credits;
                subject.Grade = String.Empty;
                subject.Marks = 0;
                subjects.Add(subject);
            }

            return subjects;
        } 
    }
}
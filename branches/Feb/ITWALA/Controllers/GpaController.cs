using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ITWALA.Models;

namespace ITWALA.Controllers
{
    public class GpaController : Controller
    {
        private MyDatabaseEntities _myDatabaseEntities = new MyDatabaseEntities();
        private GpaViewModel GpaVM = new GpaViewModel();
        // GET: Gpa
        [AllowAnonymous]
        public ActionResult ShowCategories()
        {
            try
            {
                ViewBag.Department = _myDatabaseEntities.Departments;
                ViewBag.Semester = GpaVM.Semesters;
                ViewBag.Regulation = GpaVM.Regulations;
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Gpa", "ShowCategories");
            }
            
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult ShowCategories_POST(GpaViewModel gpaViewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("CalcGpa", "Gpa", gpaViewModel);
            }
            return PartialView("ShowCategories");
        }

        public ActionResult GetGrades()
        {
            List<SelectListItem> grades = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "S", Value = "10" }, new SelectListItem() { Text = "A", Value = "9" },
                new SelectListItem() { Text = "B", Value = "8" }, new SelectListItem() { Text = "C", Value = "7" },
                new SelectListItem() { Text = "D", Value = "6" }, new SelectListItem() { Text = "E", Value = "5" },
                new SelectListItem() { Text = "U", Value = "0" }
            };
            return this.Json(grades, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ViewResultBase CalcGpa(GpaViewModel sc)
        {
            var subjectCategory = _myDatabaseEntities.SubjectCategories.Where(c => c.DepartmentId == sc.DepartmentId && c.RegulationYear == sc.RegulationYear && c.Semester == sc.Semester).Select(category => category.SubjectCategoryId).FirstOrDefault();
            //List<Subject> model = _myDatabaseEntities.Subjects.Where(s => s.SubjectCategory1.DepartmentId == sc.DepartmentId && s.SubjectCategory1.RegulationYear == sc.RegulationYear && s.SubjectCategory1.Semester == sc.Semester).ToList();
            List<Subject> model = _myDatabaseEntities.Subjects.Where(s => s.SubjectCategory == subjectCategory).ToList();
            List<SubjectViewModel> subjectsList = new SubjectViewModel().ConvertToSubjectViewModels(model);
            ViewBag.SubjectCategory = subjectCategory;
            ViewBag.Grade = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "S", Value = "10" }, new SelectListItem() { Text = "A", Value = "9" },
                new SelectListItem() { Text = "B", Value = "8" }, new SelectListItem() { Text = "C", Value = "7" },
                new SelectListItem() { Text = "D", Value = "6" }, new SelectListItem() { Text = "E", Value = "5" },
                new SelectListItem() { Text = "U", Value = "0" }
            };
            //return View("CalcGpa", new List<SubjectsMetadata>(subjects));
            return View(subjectsList);
        }


        public ActionResult GetElectiveSubjectDetails(string subjectId,string subjectCategoryId)
        {
            //string subjectId = "CS2051";
            var model =
                _myDatabaseEntities.ElectiveSubjects.Where(es => es.SubjectId == subjectId && es.SubjectCategory == subjectCategoryId).ToList();
            List<SubjectViewModel> electives = new SubjectViewModel().ConvertElectiveListToSubjectViewModels(model);
            string json = new JavaScriptSerializer().Serialize(electives[0]);
            return Json(json,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetElectiveJson(string subjectCategoryId)
        {
            //string subjectCategoryId = "CSE-2008-08";
            List<String> electives = _myDatabaseEntities.ElectiveSubjects.Where(
                es => es.SubjectCategory == subjectCategoryId).Select(e=>e.ElectiveId).Distinct().ToList();
            List<ElectiveSubjects> electiveSubjectsList = new List<ElectiveSubjects>();
            foreach (var electiveId in electives)
            {
                ElectiveSubjects electiveSubject = new ElectiveSubjects();
                electiveSubject.ElectiveSelectListItems = new List<SelectListItem>();
                var model =
                _myDatabaseEntities.ElectiveSubjects.Where(
                    es => es.SubjectCategory == subjectCategoryId && es.ElectiveId == electiveId).Select(e => e.SubjectId).ToList();
                
                foreach (string subject in model)
                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = subject,
                        Value = subject
                    };
                    electiveSubject.ElectiveSelectListItems.Add(item);
                }
                electiveSubjectsList.Add(electiveSubject); 
            }
            return Json(electiveSubjectsList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetElective2Json()
        {
            //string subjectCategoryId = ViewBag.SubjectCategory;
            string subjectCategoryId = "CSE-2008-08";
            string electiveId = "Elective-6";
            var model =
                _myDatabaseEntities.ElectiveSubjects.Where(
                    es => es.SubjectCategory == subjectCategoryId && es.ElectiveId == electiveId).Select(e => e.SubjectId).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (string subject in model)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = subject,
                    Value = subject
                };
                list.Add(item);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private ViewResult HandleError(Exception ex, string controllerName, string actionName)
        {
            return View("Error", new HandleErrorInfo(ex, controllerName, actionName));
        }
    }
}
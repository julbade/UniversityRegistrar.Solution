using Microsoft.AspNetCore.Mvc;
using UniversityRegistrar.Models;
using System.Collections.Generic;
using System;

namespace UniversityRegistrar.Controllers
{
    public class StudentsController : Controller
    {
        [HttpGet("/courses/{courseId}/students/{studentId}")]
        public ActionResult Details(int courseId, int studentId)
        {
          Student student = Student.Find(studentId);
          Dictionary<string, object> model = new Dictionary<string, object>();
          Course course = Course.Find(courseId);
          model.Add("student", student);
          model.Add("course", course);
          return View(student);
        }

        [HttpGet("/students")]
        public ActionResult Index()
        {
            List<Student> allStudents = Student.GetAll();
            return View(allStudents);
        }

        [HttpGet("/students/new")]
        public ActionResult CreateForm()
        {
            return View();
        }
        [HttpPost("/students")]
        public ActionResult Create()
        {
            Student newStudent = new Student(Request.Form["student-name"], DateTime.Parse(Request.Form["student-enrollment"]));
            newStudent.Save();
            return RedirectToAction("Index");
        }

        [HttpGet("/students/{id}/delete")]
        public ActionResult DeleteOne(int id)
        {
          Student thisStudent = Student.Find(id);
          thisStudent.Delete();
          return RedirectToAction("Index");
        }

        [HttpGet("/students/{id}")]
        public ActionResult Details(int id)
        {
            Dictionary<string, object> model = new Dictionary<string, object>();
            Student selectedStudent = Student.Find(id);
            List<Course> studentCourses = selectedStudent.GetCourses();
            List<Course> allCourses = Course.GetAll();
            model.Add("selectedStudent", selectedStudent);
            model.Add("studentCourses", studentCourses);
            model.Add("allCourses", allCourses);
            return View(model);

        }
        [HttpGet("/students/{id}/update")]
        public ActionResult UpdateForm(int id)
        {
            Student thisStudent = Student.Find(id);
            return View(thisStudent);
        }

        [HttpPost("/students/{id}/update")]
        public ActionResult Update(int id)
        {
          Student thisStudent = Student.Find(id);
          thisStudent.Edit(Request.Form["new-name"], DateTime.Parse(Request.Form["new-enrollment"]));
          return RedirectToAction("Details", new {id = thisStudent.GetId()});
        }

        [HttpPost("/students/{studentId}/courses/new")]
        public ActionResult AddCourse(int studentId)
        {
            Student student = Student.Find(studentId);
            Course course = Course.Find(Int32.Parse(Request.Form["course-id"]));
            student.AddCourse(course);
            return RedirectToAction("Index");
        }
    }
}

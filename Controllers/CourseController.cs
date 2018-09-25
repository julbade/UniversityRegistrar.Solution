using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Controllers
{
  public class CourseController : Controller
  {
    [HttpGet("/courses")]
    public ActionResult Index()
    {
      List<Course> allCourses = Course.GetAll();
      return View(allCourses);
    }

    [HttpGet("/courses/new")]
    public ActionResult CreateForm()
    {
      return View();
    }

    [HttpPost("/courses")]
    public ActionResult Create()
    {
      Course newCourse = new Course(Request.Form["course-name"], Request.Form["course-number"]);
      newCourse.Save();
      return RedirectToAction("Index");
    }

    [HttpGet("/courses/{id}")]
    public ActionResult Details(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Course selectedCourse = Course.Find(id);
      List<Student> courseStudents = selectedCourse.GetStudents();
      List<Student> allStudents = Student.GetAll();
      model.Add("selectedCourse", selectedCourse);
      model.Add("courseStudents", courseStudents);
      model.Add("allStudents", allStudents);
      return View(model);
    }

    [HttpGet("/courses/{id}/students/new")]
    public ActionResult CreateStudentForm()
    {
      return View("~/Views/Students/CreateForm.cshtml");
    }

    [HttpPost("/courses/{courseId}/students/new")]
    public ActionResult AddStudent(int courseId)
    {
      Course course = Course.Find(courseId);
      Student student = Student.Find(Int32.Parse(Request.Form["student-id"]));
      course.AddStudent(student);
      return RedirectToAction("Index");
    }

    [HttpGet("/courses/{courseId}/update")]
    public ActionResult UpdateForm(int courseId)
    {
       Course thisCourse = Course.Find(courseId);
       return View("update", thisCourse);
    }

    [HttpPost("/courses/{courseId}/update")]
    public ActionResult Update(int courseId)
    {
      Course thisCourse = Course.Find(courseId);
      thisCourse.Edit(Request.Form["new-course-name"], Request.Form["new-course-number"]);
      return RedirectToAction("Index");
    }

    [HttpGet("/courses/{courseid}/delete")]
    public ActionResult DeleteOne(int courseId)
    {
      Course thisCourse = Course.Find(courseId);
      thisCourse.Delete();
      return RedirectToAction("Index");
    }
  }
}

using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Controllers
{
  public class DepartmentController : Controller
  {
    [HttpGet("/departments")]
    public ActionResult Index()
    {
      List<Department> allDepartments = Department.GetAll();
      return View(allDepartments);
    }

    [HttpGet("/departments/new")]
    public ActionResult CreateForm()
    {
      return View();
    }

    [HttpPost("/departments")]
    public ActionResult Create()
    {
      Department newDepartment = new Department(Request.Form["department-name"]);
      newDepartment.Save();
      return RedirectToAction("Index");
    }

    [HttpGet("/departments/{id}")]
    public ActionResult Details(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Department selectedDepartment = Department.Find(id);
      List<Student> departmentStudents = selectedDepartment.GetStudents();
      List<Course> departmentCourses = selectedDepartment.GetCourses();
      List<Student> allStudents = Student.GetAll();
      List<Course> allCourses = Course.GetAll();
      model.Add("selectedDepartment", selectedDepartment);
      model.Add("departmentStudents", departmentStudents);
      model.Add("departmentCourses", departmentCourses);
      model.Add("allStudents", allStudents);
      model.Add("allCourses", allCourses);
      return View(model);
    }

    [HttpGet("/departments/{id}/students/new")]
    public ActionResult CreateStudentForm()
    {
      return View("~/Views/Students/CreateForm.cshtml");
    }

    [HttpPost("/departments/{departmentId}/students/new")]
    public ActionResult AddStudent(int departmentId)
    {
      Department department = Department.Find(departmentId);
      Student student = Student.Find(Int32.Parse(Request.Form["student-id"]));
      department.AddStudent(student);
      return RedirectToAction("Index");
    }

    [HttpPost("/departments/{departmentId}/courses/new")]
    public ActionResult AddCourse(int departmentId)
    {
      Department department = Department.Find(departmentId);
      Course course = Course.Find(Int32.Parse(Request.Form["course-id"]));
      department.AddCourse(course);
      return RedirectToAction("Index");
    }

    [HttpGet("/departments/{departmentId}/update")]
    public ActionResult UpdateForm(int departmentId)
    {
       Department thisDepartment = Department.Find(departmentId);
       return View("update", thisDepartment);
    }

    [HttpPost("/departments/{departmentId}/update")]
    public ActionResult Update(int departmentId)
    {
      Department thisDepartment = Department.Find(departmentId);
      thisDepartment.Edit(Request.Form["new-department-name"]);
      return RedirectToAction("Index");
    }

    [HttpGet("/departments/{courseid}/delete")]
    public ActionResult DeleteOne(int departmentId)
    {
      Department thisDepartment = Department.Find(departmentId);
      thisDepartment.Delete();
      return RedirectToAction("Index");
    }
  }
}

using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace UniversityRegistrar.Models
{
  public class Department
  {
    private string _name;
    private int _id;
    public Department(string name, int id = 0)
    {
      _name = name;
      _id = id;
    }

    public override bool Equals(System.Object otherDepartment)
    {
      if (!(otherDepartment is Department))
      {
        return false;
      }
      else
      {
        Department newDepartment = (Department) otherDepartment;
        return this.GetId().Equals(newDepartment.GetId());
      }
    }
    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }
    public string GetName()
    {
      return _name;
    }
    public int GetId()
    {
      return _id;
    }


    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO departments (name) VALUES (@name);";

      MySqlParameter newName = new MySqlParameter();
      newName.ParameterName = "@name";
      newName.Value = this.GetName();
      cmd.Parameters.Add(newName);



      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }
    public static List<Department> GetAll()
    {
      List<Department> allDepartments = new List<Department> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM departments;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int DepartmentId = rdr.GetInt32(0);
        string DepartmentName = rdr.GetString(1);

        Department newDepartment = new Department(DepartmentName, DepartmentId);
        allDepartments.Add(newDepartment);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allDepartments;
    }
    public static Department Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM departments WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int DepartmentId = 0;
      string DepartmentName = "";

      while(rdr.Read())
      {
        DepartmentId = rdr.GetInt32(0);
        DepartmentName = rdr.GetString(1);
      }
      Department newDepartment = new Department(DepartmentName, DepartmentId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newDepartment;
    }
    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM departments;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
      public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM departments WHERE id = @DepartmentId; DELETE FROM departments_students WHERE course_id = @DepartmentId;", conn);
      MySqlParameter courseIdParameter = new MySqlParameter();
      courseIdParameter.ParameterName = "@DepartmentId";
      courseIdParameter.Value = this.GetId();

      cmd.Parameters.Add(courseIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
    public void Edit(string newName)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE departments SET name = @newName WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newName";
      name.Value = newName;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      _name = newName;


      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public void UpdateDepartment(string newDepartment)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE departments SET name = @newDepartment WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newDepartment";
      name.Value = newDepartment;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      _name = newDepartment;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }

    public void AddStudent(Student newStudent)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO departments_students (department_id, student_id) VALUES (@DepartmentId, @StudentId);";

            MySqlParameter department_id = new MySqlParameter();
            department_id.ParameterName = "@DepartmentId";
            department_id.Value = _id;
            cmd.Parameters.Add(department_id);

            MySqlParameter student_id = new MySqlParameter();
            student_id.ParameterName = "@StudentId";
            student_id.Value = newStudent.GetId();
            cmd.Parameters.Add(student_id);

            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
              conn.Dispose();
            }
        }
        public List<Student> GetStudents()
        {
          MySqlConnection conn = DB.Connection();
          conn.Open();
          MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
          cmd.CommandText = @"SELECT students.* FROM departments
          JOIN departments_students ON (departments.id = departments_students.department_id)
          JOIN students ON (departments_students.student_id = students.id)
          WHERE departments.id = @DepartmentId;";

          MySqlParameter departmentIdParameter = new MySqlParameter();
          departmentIdParameter.ParameterName = "@DepartmentId";
          departmentIdParameter.Value = _id;
          cmd.Parameters.Add(departmentIdParameter);

          MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
          List<Student> students = new List<Student>{};

          while(rdr.Read())
          {
            int studentId = rdr.GetInt32(0);
            string studentName = rdr.GetString(1);
            DateTime studentEnrollent = rdr.GetDateTime(2);


            Student newStudent = new Student(studentName, studentEnrollent, studentId);
            students.Add(newStudent);
          }
          conn.Close();
          if (conn != null)
          {
            conn.Dispose();
          }
          return students;
        }
        public void AddCourse(Course newCourse)
        {
          MySqlConnection conn = DB.Connection();
          conn.Open();
          var cmd = conn.CreateCommand() as MySqlCommand;
          cmd.CommandText = @"INSERT INTO departments_courses (course_id, department_id) VALUES (@CourseId, @DepartmentId);";

          MySqlParameter course_id = new MySqlParameter();
          course_id.ParameterName = "@CourseId";
          course_id.Value = newCourse.GetId();
          cmd.Parameters.Add(course_id);

          MySqlParameter department_id = new MySqlParameter();
          department_id.ParameterName = "@DepartmentId";
          department_id.Value = _id;
          cmd.Parameters.Add(department_id);

          cmd.ExecuteNonQuery();
          conn.Close();
          if (conn != null)
          {
            conn.Dispose();
          }
        }
        public List<Course> GetCourses()
        {
          MySqlConnection conn = DB.Connection();
          conn.Open();
          var cmd = conn.CreateCommand() as MySqlCommand;
          cmd.CommandText = @"SELECT course_id FROM departments_courses WHERE department_id = @departmentId;";

          MySqlParameter departmentIdParameter = new MySqlParameter();
          departmentIdParameter.ParameterName = "@departmentId";
          departmentIdParameter.Value = _id;
          cmd.Parameters.Add(departmentIdParameter);

          var rdr = cmd.ExecuteReader() as MySqlDataReader;

          List<int> courseIds = new List<int> {};
          while(rdr.Read())
          {
            int courseId = rdr.GetInt32(0);
            courseIds.Add(courseId);
          }
          rdr.Dispose();

          List<Course> courses = new List<Course> {};
          foreach (int courseId in courseIds)
          {
            var courseQuery = conn.CreateCommand() as MySqlCommand;
            courseQuery.CommandText = @"SELECT * FROM courses WHERE id = @CourseId;";

            MySqlParameter courseIdParameter = new MySqlParameter();
            courseIdParameter.ParameterName = "@CourseId";
            courseIdParameter.Value = courseId;
            courseQuery.Parameters.Add(courseIdParameter);

            var courseQueryRdr = courseQuery.ExecuteReader() as MySqlDataReader;
            while(courseQueryRdr.Read())
            {
              int thisCourseId = courseQueryRdr.GetInt32(0);
              string courseName = courseQueryRdr.GetString(1);
              string courseNumber = courseQueryRdr.GetString(2);
              Course foundCourse = new Course(courseName, courseNumber, thisCourseId);
              courses.Add(foundCourse);
            }
            courseQueryRdr.Dispose();
          }
          conn.Close();
          if (conn != null)
          {
            conn.Dispose();
          }
          return courses;
        }

      }
  }

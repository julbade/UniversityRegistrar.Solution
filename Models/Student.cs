using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace UniversityRegistrar.Models
{
  public class Student
  {
    private string _name;
    private DateTime _enrollment;
    private int _id;

    public Student(string name, DateTime enrollment, int id = 0)
    {
      _name = name;
      _id = id;
      _enrollment = enrollment;
    }

    public override bool Equals(System.Object otherStudent)
    {
      if (!(otherStudent is Student))
      {
        return false;
      }
      else
      {
        Student newStudent = (Student) otherStudent;
        bool idEquality = this.GetId() == newStudent.GetId();
        bool nameEquality = this.GetName() == newStudent.GetName();
        return (idEquality && nameEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public string GetName()
    {
      return _name;
    }

    public int GetId()
    {
      return _id;
    }

    public DateTime GetEnrollment()
    {
      return _enrollment;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO students (name, enrollment) VALUES (@name, @enrollment);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      MySqlParameter enrollment = new MySqlParameter();
      enrollment.ParameterName = "@enrollment";
      enrollment.Value = this._enrollment;
      cmd.Parameters.Add(enrollment);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Student> GetAll()
    {
      List<Student> allStudents = new List<Student> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM students;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        string studentName = rdr.GetString(1);
        DateTime studentEnrollment = rdr.GetDateTime(2);

        Student newStudent = new Student(studentName, studentEnrollment, studentId);
        allStudents.Add(newStudent);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allStudents;
    }

    public static Student Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM students WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int studentId = 0;
      string studentName = "";

      rdr.Read();
      studentId = rdr.GetInt32(0);
      studentName = rdr.GetString(1);
      DateTime studentEnrollment = rdr.GetDateTime(2);

      Student newStudent = new Student(studentName, studentEnrollment, studentId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

      return newStudent;
    }

    public void UpdateName(string newName)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE students SET name = @newName WHERE id = @searchId;";

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

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM students WHERE id = @StudentId; DELETE FROM courses_students WHERE student_id = @StudentId;";

      MySqlParameter studentIdParameter = new MySqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();
      cmd.Parameters.Add(studentIdParameter);

      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM students;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }
    public void AddCourse(Course newCourse)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO courses_students (course_id, student_id) VALUES (@CourseId, @StudentId);";

      MySqlParameter course_id = new MySqlParameter();
      course_id.ParameterName = "@CourseId";
      course_id.Value = newCourse.GetId();
      cmd.Parameters.Add(course_id);

      MySqlParameter student_id = new MySqlParameter();
      student_id.ParameterName = "@StudentId";
      student_id.Value = _id;
      cmd.Parameters.Add(student_id);

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
      cmd.CommandText = @"SELECT course_id FROM courses_students WHERE student_id = @studentId;";

      MySqlParameter studentIdParameter = new MySqlParameter();
      studentIdParameter.ParameterName = "@studentId";
      studentIdParameter.Value = _id;
      cmd.Parameters.Add(studentIdParameter);

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
    public void Edit(string newName, DateTime newEnrollment)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE students SET name = @newName, enrollment = @newEnrollment WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newName";
      name.Value = newName;
      cmd.Parameters.Add(name);

      MySqlParameter enrollment = new MySqlParameter();
      enrollment.ParameterName = "@newEnrollment";
      enrollment.Value = newEnrollment;
      cmd.Parameters.Add(enrollment);

      cmd.ExecuteNonQuery();
      _name = newName;
      _enrollment = newEnrollment;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
  }
}

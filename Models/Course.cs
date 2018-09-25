using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace UniversityRegistrar.Models
{
  public class Course
  {
    private string _name;
    private int _id;
    private string _number;
    public Course(string name, string number, int id = 0)
    {
      _name = name;
      _id = id;
      _number = number;
    }
    public override bool Equals(System.Object otherCourse)
    {
      if (!(otherCourse is Course))
      {
        return false;
      }
      else
      {
        Course newCourse = (Course) otherCourse;
        return this.GetId().Equals(newCourse.GetId());
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
    public string GetNumber()
    {
      return _number;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO courses (name, number) VALUES (@name, @number);";

      MySqlParameter newName = new MySqlParameter();
      newName.ParameterName = "@name";
      newName.Value = this.GetName();
      cmd.Parameters.Add(newName);

      MySqlParameter newNumber = new MySqlParameter();
      newNumber.ParameterName = "@number";
      newNumber.Value = this.GetNumber();
      cmd.Parameters.Add(newNumber);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }
    public static List<Course> GetAll()
    {
      List<Course> allCourses = new List<Course> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM courses;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int CourseId = rdr.GetInt32(0);
        string CourseName = rdr.GetString(1);
        string CourseNumber = rdr.GetString(2);
        Course newCourse = new Course(CourseName, CourseNumber, CourseId);
        allCourses.Add(newCourse);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allCourses;
    }
    public static Course Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM courses WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int CourseId = 0;
      string CourseName = "";
      string CourseNumber = "";

      while(rdr.Read())
      {
        CourseId = rdr.GetInt32(0);
        CourseName = rdr.GetString(1);
        CourseNumber = rdr.GetString(2);
      }
      Course newCourse = new Course(CourseName, CourseNumber, CourseId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newCourse;
    }
    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM courses;";
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

      MySqlCommand cmd = new MySqlCommand("DELETE FROM courses WHERE id = @CourseId; DELETE FROM courses_students WHERE course_id = @CourseId;", conn);
      MySqlParameter courseIdParameter = new MySqlParameter();
      courseIdParameter.ParameterName = "@CourseId";
      courseIdParameter.Value = this.GetId();

      cmd.Parameters.Add(courseIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
    public void Edit(string newName, string newNumber)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE courses SET name = @newName, number = @newNumber WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newName";
      name.Value = newName;
      cmd.Parameters.Add(name);

      MySqlParameter number = new MySqlParameter();
      number.ParameterName = "@newNumber";
      number.Value = newNumber;
      cmd.Parameters.Add(number);

      cmd.ExecuteNonQuery();
      _name = newName;
      _number =newNumber;

      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public void UpdateCourse(string newCourse)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE courses SET name = @newCourse WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newCourse";
      name.Value = newCourse;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      _name = newCourse;
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
            cmd.CommandText = @"INSERT INTO courses_students (course_id, student_id) VALUES (@CourseId, @StudentId);";

            MySqlParameter course_id = new MySqlParameter();
            course_id.ParameterName = "@CourseId";
            course_id.Value = _id;
            cmd.Parameters.Add(course_id);

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
          cmd.CommandText = @"SELECT students.* FROM courses
          JOIN courses_students ON (courses.id = courses_students.course_id)
          JOIN students ON (courses_students.student_id = students.id)
          WHERE courses.id = @CourseId;";

          MySqlParameter courseIdParameter = new MySqlParameter();
          courseIdParameter.ParameterName = "@CourseId";
          courseIdParameter.Value = _id;
          cmd.Parameters.Add(courseIdParameter);

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
      }
  }

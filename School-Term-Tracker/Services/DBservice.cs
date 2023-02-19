using SchoolTermTracker.Models;
using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SchoolTermTracker.Services
{
    public static class DBservice
    {
        //CurrentTerm, CurrentCourse, and CurrentAssessment are used as placeholders to capture and pass data from a specific term, course, or assessment.
        public static Term CurrentTerm { get; set; }

        public static Course CurrentCourse { get; set; }

        public static Assessment CurrentAssessment { get; set; }

        private static SQLiteAsyncConnection _db;

        //Sets up the database and creates tables if the database does not currently exist.
        static async Task Init()
        {           
            if (_db != null)
            {
                return;
            }

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "Terms.db");

            _db = new SQLiteAsyncConnection(databasePath);

            //The commented code below is used to drop tables for testing purposes.
            //await _db.DropTableAsync<Term>();
            //await _db.DropTableAsync<Course>();
            //await _db.DropTableAsync<Assessment>();

            await _db.CreateTableAsync<Term>();
            await _db.CreateTableAsync<Course>();
            await _db.CreateTableAsync<Assessment>();
        }
        
        //When the Term table is empty this function fills the Term, Course, and Assessment tables with sample data       
        public static async Task DummyData(int integer)
        {
            if (integer == 0)
            {
                await Init();

                DateTime startDate = new DateTime(2022, 9, 1);
                DateTime endDate = new DateTime(2023, 2, 28);

                List<Term> termList = (List<Term>)await GetTerms();
                int count = 0;
                int termcount = termList.Count;

                if (termcount == count)
                {
                    await AddTerm("First Term", startDate, endDate);
                  
                    CurrentTerm = _db.Table<Term>().FirstOrDefaultAsync().Result;

                    DateTime courseStartDate = new DateTime(2022, 9, 1);
                    DateTime courseEndDate = new DateTime(2022, 10, 13);

                    await AddCourse("Mobile Application Development", courseStartDate, courseEndDate, "Completed", "Siefring", "1-888-888-8888", "blank@yahoo.com", "no notes",
                        CurrentTerm.TermID, false);

                    CurrentCourse = _db.Table<Course>().FirstOrDefaultAsync().Result;

                    DateTime assessmentStartDate = new DateTime(2022, 9, 1);
                    DateTime assessmentEndDate = new DateTime(2022, 9, 20);

                    DateTime assessment2StartDate = new DateTime(2022, 9, 21);
                    DateTime assessment2EndDate = new DateTime(2022, 10, 13);

                    await AddAssessment("Mobile Application Development", assessmentStartDate, assessmentEndDate, "Performance Assessment", CurrentCourse.CourseID, false);
                    await AddAssessment("Xamarin Forms", assessment2StartDate, assessment2EndDate, "Objective Assessment", CurrentCourse.CourseID, false);                  
                }
            }
        }

        //Shows notifications of course or assessment starting/ending today if any exist.
        public static async Task NotificationCheck(bool boolean)
        {
            if (boolean == true)
            {
                var courseList = await DBservice._db.Table<Course>().ToListAsync();
                var assessmentList = await _db.Table<Assessment>().ToListAsync();
               
                foreach (Course course in courseList)
                {
                    if (course.Notification && course.CourseStartDate == DateTime.Today)
                    { 
                        CrossLocalNotifications.Current.Show("Course Starting", "Course: " + course.CourseName + " starts today: " + DateTime.Now.Date.ToShortDateString() + "", 1, DateTime.Now); 
                    }

                    if (course.Notification && course.CourseEndDate == DateTime.Today) 
                    { 
                        CrossLocalNotifications.Current.Show("Course Ending", "Course: " + course.CourseName + " ends today: " + DateTime.Now.Date.ToShortDateString() + "", 2, DateTime.Now);
                    }                   
                }

                foreach (Assessment assessment in assessmentList)
                {
                   if (assessment.AssessmentNotification && assessment.AssessmentStartDate == DateTime.Today) 
                    { 
                        CrossLocalNotifications.Current.Show("Assessment Starting", "Assessment: " + assessment.AssessmentName + " starts today: " + DateTime.Now.Date.ToShortDateString() + "", 
                            3, DateTime.Now); 
                    }

                   if (assessment.AssessmentNotification && assessment.AssessmentEndDate == DateTime.Today)
                    { 
                        CrossLocalNotifications.Current.Show("Assessment Due", "Assessment: " + assessment.AssessmentName + " is due today: " + DateTime.Now.Date.ToShortDateString() + "",
                            4, DateTime.Now); 
                    }
                }                
            }
        }

        //Adds a new term to the Term table.
        public static async Task AddTerm(string termName, DateTime termStartDate, DateTime termEndDate)
        {
            await Init();

            var term = new Term
            {
                TermName = termName,
                TermStartDate = termStartDate,
                TermEndDate = termEndDate
            };
            var id = await _db.InsertAsync(term);
        }

        //Removes selected term from the Term table.
        public static async Task RemoveTerm(int termID)
        {
            await Init();

            await _db.DeleteAsync<Term>(termID);
        }

        //Gets all terms in the Term table and puts them in a list.
        public static async Task<IEnumerable<Term>> GetTerms()
        {
            await Init();

            var terms = await _db.Table<Term>().ToListAsync();
            return terms;
        }

        //Updates selected term in the Term table.
        public static async Task UpdateTerm(int termID, string termName, DateTime termStartDate, DateTime termEndDate)
        {
            await Init();

            var termQuery = await _db.Table<Term>()              
                .Where(i => i.TermID == termID)
                .FirstOrDefaultAsync();           
                               
            if (termQuery != null)
            {
                termQuery.TermName = termName;
                termQuery.TermStartDate = termStartDate;
                termQuery.TermEndDate = termEndDate;

                await _db.UpdateAsync(termQuery);
            }
        }

        //Adds a new course to the Course table.
        public static async Task AddCourse(string courseName, DateTime courseStartDate, DateTime courseEndDate, 
            string courseStatus, string instructorName, string instructorPhone, string instructorEmail, string notes, int termID, bool notification)
        {
            await Init();

            var course = new Course
            {
                CourseName = courseName,
                CourseStartDate = courseStartDate,
                CourseEndDate = courseEndDate,
                CourseStatus = courseStatus,
                InstructorName = instructorName,
                InstructorPhone = instructorPhone,
                InstructorEmail = instructorEmail,
                Notes = notes,
                TermID = termID,
                Notification = notification
            };
            var id = await _db.InsertAsync(course);
        }

        //Gets the courses in the Course table and puts them in a list.
        public static async Task<IEnumerable<Course>> GetCourses(Term term)
        {
            await Init();          
            
            var courseQuery = await _db.Table<Course>()
                .Where(i => i.TermID == term.TermID)
                .ToListAsync();
            return courseQuery;
        }
  
        //Updates the selected course in the Course table.
        public static async Task UpdateCourse(int courseID, string courseName, DateTime courseStartDate, DateTime courseEndDate, string courseStatus, string instructorName,
                                                string instructorPhone, string instructorEmail, string notes, bool notification)
        {
            await Init();

            var courseQuery = await _db.Table<Course>()
            .Where(i => i.CourseID == courseID)
            .FirstOrDefaultAsync();
          
            if (courseQuery != null)
            {
                courseQuery.CourseName = courseName;
                courseQuery.CourseStartDate = courseStartDate;
                courseQuery.CourseEndDate = courseEndDate;
                courseQuery.CourseStatus = courseStatus;
                courseQuery.InstructorName = instructorName;
                courseQuery.InstructorPhone = instructorPhone;
                courseQuery.InstructorEmail = instructorEmail;
                courseQuery.Notes = notes;
                courseQuery.Notification = notification;

                await _db.UpdateAsync(courseQuery);
            }
        }

        //Removes the selected course in the Course table.
        public static async Task RemoveCourse(int courseID)
        {
            await Init();

            await _db.DeleteAsync<Course>(courseID);
        }

        //Gets the assessments in the Assessment table and puts them in a list.
        public static async Task<IEnumerable<Assessment>> GetAssessments(Course course)
        {
            await Init();
            
            var assessmentQuery = await _db.Table<Assessment>()
                .Where(i => i.CourseID == course.CourseID)
                .ToListAsync();
            return assessmentQuery;
        }
      
        //Adds an assessment to the Assessment table.
        public static async Task AddAssessment(string assessmentName, DateTime assessmentStartDate, DateTime assessmentEndDate,
            string assessmentType, int courseID, bool assessmentNotification)
        {
            await Init();
            var assessment = new Assessment
            {
                AssessmentName = assessmentName,
                AssessmentStartDate = assessmentStartDate,
                AssessmentEndDate = assessmentEndDate,
                AssessmentType = assessmentType,
                CourseID = courseID,
                AssessmentNotification = assessmentNotification
            };
            var id = await _db.InsertAsync(assessment);
        }

        //Updates selected assessment in the Assessment table.
        public static async Task UpdateAssessment(int assessmentID, string assessmentName, DateTime assessmentStartDate, DateTime assessmentEndDate, string assessmentType, bool assessmentNotification)
        {
            await Init();

            var assessmentQuery = await _db.Table<Assessment>()
            .Where(i => i.AssessmentID == assessmentID)
            .FirstOrDefaultAsync();

            if (assessmentQuery != null)
            {
                assessmentQuery.AssessmentName = assessmentName;
                assessmentQuery.AssessmentStartDate = assessmentStartDate;
                assessmentQuery.AssessmentEndDate = assessmentEndDate;
                assessmentQuery.AssessmentType = assessmentType;
                assessmentQuery.AssessmentNotification = assessmentNotification;

                await _db.UpdateAsync(assessmentQuery);
            }
        }

        //Removes selected assessment from the Assessment table.
        public static async Task RemoveAssessment(int assessmentID)
        {
            await Init();

            await _db.DeleteAsync<Assessment>(assessmentID);
        }
    }
}

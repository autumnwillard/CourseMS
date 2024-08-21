using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace C971
{
    public static class DatabaseService
    {
        private static SQLiteAsyncConnection _db;
        private static SQLiteConnection _dbConnection;

        public static async Task Init()
        {
            if (_db != null)
                return;

            try
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "C971.db");
                var directory = Path.GetDirectoryName(databasePath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine("Database directory created.");
                }

                _db = new SQLiteAsyncConnection(databasePath);
                _dbConnection = new SQLiteConnection(databasePath);

                // Log before creating tables
                Console.WriteLine("Creating Term table...");
                await _db.CreateTableAsync<Term>();
                Console.WriteLine("Term table created.");

                Console.WriteLine("Creating Course table...");
                await _db.CreateTableAsync<Course>();
                Console.WriteLine("Course table created.");

                Console.WriteLine("Creating Note table...");
                await _db.CreateTableAsync<Note>();
                Console.WriteLine("Note table created.");

                Console.WriteLine("Creating Assessment table...");
                await _db.CreateTableAsync<Assessment>();
                Console.WriteLine("Assessment table created.");

                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
            }
        }

        // Term methods
        public static Task<List<Term>> GetTermsAsync()
        {
            return _db.Table<Term>().ToListAsync();
        }

        public static Task<int> SaveTermAsync(Term term)
        {
            if (term.Id != 0)
                return _db.UpdateAsync(term);
            else
                return _db.InsertAsync(term);
        }

        public static Task<int> DeleteTermAsync(Term term)
        {
            return _db.DeleteAsync(term);
        }

        // Course methods
        public static Task<List<Course>> GetCoursesAsync()
        {
            return _db.Table<Course>().ToListAsync();
        }

        public static Task<List<Course>> GetCoursesByTermAsync(int termId)
        {
            return _db.Table<Course>().Where(c => c.TermId == termId).ToListAsync();
        }

        public static Task<Course> GetCourseAsync(int id)
        {
            return _db.Table<Course>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public static Task<int> SaveCourseAsync(Course course)
        {
            if (course.Id != 0)
            {
                Console.WriteLine($"Updating course: {course.Title} with TermId: {course.TermId}");
                return _db.UpdateAsync(course);
            }
            else
            {
                Console.WriteLine($"Inserting course: {course.Title} with TermId: {course.TermId}");
                return _db.InsertAsync(course);
            }
        }

        public static Task<int> DeleteCourseAsync(Course course)
        {
            return _db.DeleteAsync(course);
        }

        public static Task<Term> GetTermAsync(int id)
        {
            return _db.Table<Term>().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        // Note methods
        public static Task<List<Note>> GetNotesByCourseAsync(int courseId)
        {
            return _db.Table<Note>().Where(n => n.CourseId == courseId).ToListAsync();
        }

        public static Task<int> SaveNoteAsync(Note note)
        {
            if (note.Id != 0)
                return _db.UpdateAsync(note);
            else
                return _db.InsertAsync(note);
        }

        public static Task<int> DeleteNoteAsync(Note note)
        {
            return _db.DeleteAsync(note);
        }

        // Assessment methods

        public static Task<List<Assessment>> GetAllAssessmentsAsync()
        {
            return _db.Table<Assessment>().ToListAsync();
        }
        public static Task<List<Assessment>> GetAssessmentsByCourseAsync(int courseId)
        {
            return _db.Table<Assessment>().Where(a => a.CourseId == courseId).ToListAsync();
        }

        public static Task<Assessment> GetAssessmentAsync(int id)
        {
            return _db.Table<Assessment>().Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public static Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            if (assessment.Id != 0)
                return _db.UpdateAsync(assessment);
            else
                return _db.InsertAsync(assessment);
        }

        public static Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            return _db.DeleteAsync(assessment);
        }

        public static async Task AddInitialData()
        {
            // Create a term if none exist
            var terms = await GetTermsAsync();
            if (terms.Any())
            {
                return;
            }
            var term = new Term
            {
                Title = "Term 1",
                StartDate = new DateTime(2024, 7, 1),
                EndDate = new DateTime(2024, 1, 30)
            };
            await SaveTermAsync(term);

            // Create one course
            var course = new Course
            {
                Title = "Course 1",
                StartDate = new DateTime(2024, 7, 15),
                EndDate = new DateTime(2024, 8, 15),
                Status = "In Progress",
                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                TermId = term.Id,
                NotifyStartDate = true,
                NotifyEndDate = true
            };
            await SaveCourseAsync(course);

            // Create two assessments
            var assessment1 = new Assessment
            {
                Title = "Assessment 1",
                Type = "Objective",
                StartDate = new DateTime(2024, 7, 21),
                EndDate = new DateTime(2024, 7, 23),
                CourseId = course.Id,
                NotifyStartDate = true,
                NotifyEndDate = true
            };
            await SaveAssessmentAsync(assessment1);

            var assessment2 = new Assessment
            {
                Title = "Assessment 2",
                Type = "Performance",
                StartDate = new DateTime(2023, 7, 24),
                EndDate = new DateTime(2023, 8, 10),
                CourseId = course.Id,
                NotifyStartDate = true,
                NotifyEndDate = true
            };
            await SaveAssessmentAsync(assessment2);
        }

    }
}

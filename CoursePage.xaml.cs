using System.Collections.Generic;
using System.Threading.Tasks;

namespace C971
{
    [QueryProperty(nameof(TermId), "termId")]
    public partial class CoursePage : ContentPage
    {
        private int _termId;

        public int TermId
        {
            get => _termId;
            set
            {
                _termId = value;
                Console.WriteLine($"TermId set to: {_termId}");
                LoadCourses();
            }
        }

        public CoursePage()
        {
            InitializeComponent();
        }

        private async void LoadCourses()
        {
            try
            {
                Console.WriteLine($"Loading courses for term ID: {_termId}");
                var courses = await DatabaseService.GetCoursesByTermAsync(_termId);
                CoursesCollectionView.ItemsSource = courses;
                Console.WriteLine($"Loaded {courses.Count} courses for term ID: {_termId}");

                // Log course details
                foreach (var course in courses)
                {
                    Console.WriteLine($"Loaded course: {course.Title}, TermId: {course.TermId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading courses: {ex.Message}");
            }
        }

        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddCoursePage), new Dictionary<string, object>
            {
                { "termId", _termId }
            });
        }

        private async void OnViewCourseClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var course = button?.BindingContext as Course;
            if (course != null)
            {
                await Shell.Current.GoToAsync(nameof(CourseDetailPage), new Dictionary<string, object>
                {
                    { "course", course }
                });
            }
        }

        private async void OnEditCourseClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var course = button?.BindingContext as Course;
            if (course != null)
            {
                await Shell.Current.GoToAsync($"{nameof(AddCoursePage)}?courseId={course.Id}&termId={course.TermId}");
            }
        }





        private async void OnDeleteCourseClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var course = button?.BindingContext as Course;
            if (course != null)
            {
                var result = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the course '{course.Title}'?", "Yes", "No");
                if (result)
                {
                    await DatabaseService.DeleteCourseAsync(course);
                    LoadCourses();
                }
            }
        }
    }
}

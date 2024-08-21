using System;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using C971.Services;
using System.Threading.Tasks;

namespace C971
{
    [QueryProperty(nameof(CourseId), "courseId")]
    [QueryProperty(nameof(TermId), "termId")]
    public partial class AddCoursePage : ContentPage
    {
        private int _courseId;
        private int _termId;
        private Course _course;

        public int CourseId
        {
            get => _courseId;
            set
            {
                _courseId = value;
                LoadCourse();
            }
        }

        public int TermId
        {
            get => _termId;
            set
            {
                _termId = value;
            }
        }

        public AddCoursePage()
        {
            InitializeComponent();
        }

        private async void LoadCourse()
        {
            if (_courseId != 0)
            {
                _course = await DatabaseService.GetCourseAsync(_courseId);
                if (_course != null)
                {
                    entryCourseName.Text = _course.Title;
                    StartDatePicker.Date = _course.StartDate;
                    EndDatePicker.Date = _course.EndDate;
                    statusPicker.SelectedItem = _course.Status;
                    entryInstructorName.Text = _course.InstructorName;
                    entryInstructorPhone.Text = _course.InstructorPhone;
                    entryInstructorEmail.Text = _course.InstructorEmail;
                    StartDateNotificationSwitch.IsToggled = _course.NotifyStartDate;
                    EndDateNotificationSwitch.IsToggled = _course.NotifyEndDate;
                    _termId = _course.TermId;
                }
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (_course == null)
            {
                _course = new Course();
            }

            _course.Title = entryCourseName.Text;
            _course.StartDate = StartDatePicker.Date;
            _course.EndDate = EndDatePicker.Date;
            _course.Status = statusPicker.SelectedItem?.ToString();
            _course.InstructorName = entryInstructorName.Text;
            _course.InstructorPhone = entryInstructorPhone.Text;
            _course.InstructorEmail = entryInstructorEmail.Text;
            _course.NotifyStartDate = StartDateNotificationSwitch.IsToggled;
            _course.NotifyEndDate = EndDateNotificationSwitch.IsToggled;
            _course.TermId = _termId;

            var validationResult = ValidateFields();
            if (!validationResult.IsValid)
            {
                await DisplayAlert("Invalid Input", validationResult.ErrorMessage, "OK");
                return;
            }

            validationResult = ValidateDates();
            if (!validationResult.IsValid)
            {
                await DisplayAlert("Invalid Dates", validationResult.ErrorMessage, "OK");
                return;
            }

            if (await CourseLimitReachedAsync())
            {
                await DisplayAlert("Course Limit", "You can only add up to 6 courses per term.", "OK");
                return;
            }

            try
            {
                await DatabaseService.SaveCourseAsync(_course);
                NotificationService.ScheduleCourseNotifications(_course);
                MessagingCenter.Send(this, "CourseUpdated", _course);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving course: {ex.Message}");
            }
        }

        private async Task<bool> CourseLimitReachedAsync()
        {
            var courses = await DatabaseService.GetCoursesByTermAsync(_termId);
            return courses.Count >= 6;
        }

        private (bool IsValid, string ErrorMessage) ValidateDates()
        {
            if (_course.StartDate >= _course.EndDate)
            {
                return (false, "The start date must be before the end date.");
            }

            var term = DatabaseService.GetTermAsync(_termId).Result;
            if (_course.StartDate < term.StartDate || _course.EndDate > term.EndDate)
            {
                return (false, "The course dates must be within the term dates.");
            }

            return (true, string.Empty);
        }

        private (bool IsValid, string ErrorMessage) ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(_course.Title))
            {
                return (false, "Course name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(_course.Status))
            {
                return (false, "Course status cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(_course.InstructorName))
            {
                return (false, "Instructor name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(_course.InstructorPhone))
            {
                return (false, "Instructor phone cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(_course.InstructorEmail))
            {
                return (false, "Instructor email cannot be empty.");
            }

            if (!Regex.IsMatch(_course.InstructorEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return (false, "Instructor email is not in a valid format.");
            }

            if (!Regex.IsMatch(_course.InstructorPhone, @"^[0-9\-]+$"))
            {
                return (false, "Instructor phone number can only contain numbers and hyphens.");
            }

            return (true, string.Empty);
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

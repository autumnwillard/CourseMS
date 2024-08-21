using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using C971.Services;

namespace C971
{
    [QueryProperty(nameof(AssessmentId), "assessmentId")]
    [QueryProperty(nameof(CourseId), "courseId")]
    public partial class AddAssessmentPage : ContentPage
    {
        private int _assessmentId;
        private int _courseId;
        private Assessment _assessment;
        private Course _course;

        public int AssessmentId
        {
            get => _assessmentId;
            set
            {
                _assessmentId = value;
                LoadAssessment();
            }
        }

        public int CourseId
        {
            get => _courseId;
            set
            {
                _courseId = value;
                LoadCourse();
            }
        }

        public AddAssessmentPage()
        {
            InitializeComponent();
        }

        private async void LoadAssessment()
        {
            if (_assessmentId != 0)
            {
                _assessment = await DatabaseService.GetAssessmentAsync(_assessmentId);
                if (_assessment != null)
                {
                    entryAssessmentTitle.Text = _assessment.Title;
                    if (_assessment.Type == "Objective")
                    {
                        objectiveRadioButton.IsChecked = true;
                    }
                    else
                    {
                        performanceRadioButton.IsChecked = true;
                    }
                    startDatePicker.Date = _assessment.StartDate;
                    endDatePicker.Date = _assessment.EndDate;
                    StartDateNotificationSwitch.IsToggled = _assessment.NotifyStartDate;
                    EndDateNotificationSwitch.IsToggled = _assessment.NotifyEndDate;
                }
            }
        }

        private async void LoadCourse()
        {
            _course = await DatabaseService.GetCourseAsync(_courseId);
            if (_course == null)
            {
                await DisplayAlert("Error", "Course not found.", "OK");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            string title = entryAssessmentTitle.Text;
            string type = objectiveRadioButton.IsChecked ? "Objective" : "Performance";
            DateTime startDate = startDatePicker.Date;
            DateTime endDate = endDatePicker.Date;

            if (string.IsNullOrWhiteSpace(title))
            {
                await DisplayAlert("Error", "Title cannot be empty.", "OK");
                return;
            }

            if (startDate >= endDate)
            {
                await DisplayAlert("Error", "The start date must be before the end date.", "OK");
                return;
            }

            if (_course == null)
            {
                _course = await DatabaseService.GetCourseAsync(_courseId);
                if (_course == null)
                {
                    await DisplayAlert("Error", "Course not found.", "OK");
                    return;
                }
            }

            if (startDate < _course.StartDate || endDate > _course.EndDate)
            {
                await DisplayAlert("Error", $"The assessment dates must be within the course dates ({_course.StartDate:MM/dd/yyyy} - {_course.EndDate:MM/dd/yyyy}).", "OK");
                return;
            }

            var assessments = await DatabaseService.GetAssessmentsByCourseAsync(_courseId);
            if (assessments.Any(a => a.Type == type && a.Id != _assessmentId))
            {
                await DisplayAlert("Error", $"There can only be one {type} assessment per course.", "OK");
                return;
            }

            if (_assessment != null)
            {
                _assessment.Title = title;
                _assessment.Type = type;
                _assessment.StartDate = startDate;
                _assessment.EndDate = endDate;
                _assessment.NotifyStartDate = StartDateNotificationSwitch.IsToggled;
                _assessment.NotifyEndDate = EndDateNotificationSwitch.IsToggled;

                await DatabaseService.SaveAssessmentAsync(_assessment);
                NotificationService.ScheduleAssessmentNotifications(_assessment);
            }
            else
            {
                var newAssessment = new Assessment
                {
                    Title = title,
                    Type = type,
                    StartDate = startDate,
                    EndDate = endDate,
                    CourseId = _courseId,
                    NotifyStartDate = StartDateNotificationSwitch.IsToggled,
                    NotifyEndDate = EndDateNotificationSwitch.IsToggled
                };

                await DatabaseService.SaveAssessmentAsync(newAssessment);
                NotificationService.ScheduleAssessmentNotifications(newAssessment);
            }

            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

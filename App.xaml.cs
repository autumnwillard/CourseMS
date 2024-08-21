using C971.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;

namespace C971
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            DatabaseService.Init().ContinueWith(async _ =>
            {
                await DatabaseService.AddInitialData();
                await ScheduleNotificationsForToday();
            });
        }

        private async Task ScheduleNotificationsForToday()
        {
            var courses = await DatabaseService.GetCoursesAsync();
            foreach (var course in courses)
            {
                NotificationService.ScheduleCourseNotifications(course);
            }
            var assessments = await DatabaseService.GetAllAssessmentsAsync();
            foreach (var assessment in assessments)
            {
                NotificationService.ScheduleAssessmentNotifications(assessment);
            }
        }
    }
}

using Plugin.LocalNotification;
using System;

namespace C971.Services
{
    public static class NotificationService
    {
        public static void ScheduleCourseNotifications(Course course)
        {
            var today = DateTime.Today;

            if (course.NotifyStartDate && course.StartDate.Date == today)
            {
                ScheduleNotification(course.Id, $"{course.Title} starts today!", "Course Reminder");
            }

            if (course.NotifyEndDate && course.EndDate.Date == today)
            {
                ScheduleNotification(course.Id + 1000, $"{course.Title} ends today!", "Course Reminder");
            }
        }

        public static void ScheduleAssessmentNotifications(Assessment assessment)
        {
            var today = DateTime.Today;

            if (assessment.NotifyStartDate && assessment.StartDate.Date == today)
            {
                ScheduleNotification(assessment.Id, $"{assessment.Title} starts today!", "Assessment Reminder");
            }

            if (assessment.NotifyEndDate && assessment.EndDate.Date == today)
            {
                ScheduleNotification(assessment.Id + 1000, $"{assessment.Title} ends today!", "Assessment Reminder");
            }
        }


        private static void ScheduleNotification(int id, string message, string title)
        {
            var notification = new NotificationRequest
            {
                NotificationId = id,
                Title = title,
                Description = message,
            };

            LocalNotificationCenter.Current.Show(notification);
        }


        public static void CancelCourseNotifications(Course course)
        {
            LocalNotificationCenter.Current.Cancel(course.Id);
            LocalNotificationCenter.Current.Cancel(course.Id + 1000);
        }
        public static void CancelAssessmentNotifications(Assessment assessment)
        {
            LocalNotificationCenter.Current.Cancel(assessment.Id);
            LocalNotificationCenter.Current.Cancel(assessment.Id + 1000);
        }
    }
}

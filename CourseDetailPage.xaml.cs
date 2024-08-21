using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace C971
{
    [QueryProperty(nameof(Course), "course")]
    public partial class CourseDetailPage : ContentPage
    {
        private Course _course;
        private List<Note> _notes;
        private List<Assessment> _assessments;

        public Course Course
        {
            get => _course;
            set
            {
                _course = value;
                if (_course != null)
                {
                    courseNameLabel.Text = _course.Title;
                    startDateLabel.Text = _course.StartDate.ToString("MM/dd/yyyy");
                    endDateLabel.Text = _course.EndDate.ToString("MM/dd/yyyy");
                    statusLabel.Text = _course.Status;
                    instructorNameLabel.Text = _course.InstructorName;
                    instructorPhoneLabel.Text = _course.InstructorPhone;
                    instructorEmailLabel.Text = _course.InstructorEmail;
                    LoadNotes();
                    LoadAssessments();
                }
            }
        }

        public CourseDetailPage()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<AddCoursePage, Course>(this, "CourseUpdated", (sender, updatedCourse) =>
            {
                if (updatedCourse.Id == _course.Id)
                {
                    _course = updatedCourse;
                    LoadCourseDetails();
                }
            });
        }

        private async void LoadCourseDetails()
        {
            _course = await DatabaseService.GetCourseAsync(_course.Id);
            if (_course != null)
            {
                courseNameLabel.Text = _course.Title;
                startDateLabel.Text = _course.StartDate.ToString("MM/dd/yyyy");
                endDateLabel.Text = _course.EndDate.ToString("MM/dd/yyyy");
                statusLabel.Text = _course.Status;
                instructorNameLabel.Text = _course.InstructorName;
                instructorPhoneLabel.Text = _course.InstructorPhone;
                instructorEmailLabel.Text = _course.InstructorEmail;
                LoadNotes();
                LoadAssessments();
            }
        }

        private async void LoadNotes()
        {
            try
            {
                _notes = await DatabaseService.GetNotesByCourseAsync(_course.Id);
                notesCollectionView.ItemsSource = _notes;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load notes: {ex.Message}", "OK");
            }
        }

        private async void LoadAssessments()
        {
            try
            {
                _assessments = await DatabaseService.GetAssessmentsByCourseAsync(_course.Id);
                assessmentsCollectionView.ItemsSource = _assessments;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load assessments: {ex.Message}", "OK");
            }
        }

        private async void OnSaveNotesClicked(object sender, EventArgs e)
        {
            if (_course != null)
            {
                var newNote = new Note
                {
                    Text = notesEditor.Text,
                    CourseId = _course.Id
                };

                await DatabaseService.SaveNoteAsync(newNote);
                notesEditor.Text = string.Empty;
                LoadNotes();
                await DisplayAlert("Success", "Note saved successfully.", "OK");
            }
        }

        private async void OnEditNoteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var note = button?.BindingContext as Note;
            if (note != null)
            {
                notesEditor.Text = note.Text;
                await DatabaseService.DeleteNoteAsync(note);
                LoadNotes();
            }
        }

        private async void OnShareNoteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var note = button?.BindingContext as Note;
            if (note != null)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = note.Text,
                    Title = "Share Note"
                });
            }
        }

        private async void OnDeleteNoteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var note = button?.BindingContext as Note;
            if (note != null)
            {
                var result = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete this note?", "Yes", "No");
                if (result)
                {
                    await DatabaseService.DeleteNoteAsync(note);
                    LoadNotes();
                }
            }
        }

        private async void OnEditCourseClicked(object sender, EventArgs e)
        {
            if (_course != null)
            {
                await Shell.Current.GoToAsync($"{nameof(AddCoursePage)}?courseId={_course.Id}&termId={_course.TermId}");
            }
        }

        private async void OnAddAssessmentClicked(object sender, EventArgs e)
        {
            if (_course != null)
            {
                await Shell.Current.GoToAsync($"{nameof(AddAssessmentPage)}?courseId={_course.Id}");
            }
        }

        private async void OnEditAssessmentClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var assessment = button?.BindingContext as Assessment;
            if (assessment != null)
            {
                await Shell.Current.GoToAsync($"{nameof(AddAssessmentPage)}?assessmentId={assessment.Id}&courseId={_course.Id}");
            }
        }

        private async void OnDeleteAssessmentClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var assessment = button?.BindingContext as Assessment;
            if (assessment != null)
            {
                var result = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete this assessment?", "Yes", "No");
                if (result)
                {
                    await DatabaseService.DeleteAssessmentAsync(assessment);
                    LoadAssessments();
                }
            }
        }
    }
}

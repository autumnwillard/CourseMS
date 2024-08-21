using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace C971
{
    [QueryProperty(nameof(Term), "term")]
    public partial class AddTermPage : ContentPage
    {
        private Term _term;

        public Term Term
        {
            get => _term;
            set
            {
                _term = value;
                if (_term != null)
                {
                    entryTermName.Text = _term.Title;
                    StartDatePicker.Date = _term.StartDate;
                    EndDatePicker.Date = _term.EndDate;
                }
            }
        }

        public AddTermPage()
        {
            InitializeComponent();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (_term == null)
            {
                _term = new Term();
            }

            _term.Title = entryTermName.Text;
            _term.StartDate = StartDatePicker.Date;
            _term.EndDate = EndDatePicker.Date;

            if (!ValidateDates())
            {
                await DisplayAlert("Invalid Dates", "The start date must be before the end date.", "OK");
                return;
            }

            if (await DatesOverlapAsync())
            {
                await DisplayAlert("Date Conflict", "The selected dates overlap with another term.", "OK");
                return;
            }

            try
            {
                await DatabaseService.SaveTermAsync(_term);

                MessagingCenter.Send(this, "TermSaved");

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving term: {ex.Message}");
            }
        }

        private async Task<bool> DatesOverlapAsync()
        {
            var terms = await DatabaseService.GetTermsAsync();
            foreach (var term in terms)
            {
                if (term.Id != _term.Id &&
                    (_term.StartDate < term.EndDate && _term.EndDate > term.StartDate))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ValidateDates()
        {
            return _term.StartDate < _term.EndDate;
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

using System.Diagnostics;
using SQLite;

namespace C971
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitializeDatabase();

            MessagingCenter.Subscribe<AddTermPage>(this, "TermSaved", async (sender) => {
                await LoadTerms();
            });
        }

        private async void InitializeDatabase()
        {
            await DatabaseService.Init();
            await LoadTerms();
        }


        private async Task LoadTerms()
        {
            try
            {
                var terms = await DatabaseService.GetTermsAsync();
                if (terms != null && terms.Count > 0)
                {
                    Console.WriteLine($"Loaded {terms.Count} terms.");
                    TermsCollectionView.ItemsSource = terms;
                }
                else
                {
                    Console.WriteLine("No terms found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading terms: {ex.Message}");
            }
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(AddTermPage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to add term page: {ex.Message}");
            }
        }
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                var term = button?.BindingContext as Term;
                if (term != null)
                {
                    var result = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the term '{term.Title}'?", "Yes", "No");
                    if (result)
                    {
                        await DatabaseService.DeleteTermAsync(term);
                        await LoadTerms();
                    }
                }
                else
                {
                    Console.WriteLine("Term is null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting term: {ex.Message}");
            }
        }
        private async void OnEditClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                var term = button?.BindingContext as Term;
                if (term != null)
                {
                    await Shell.Current.GoToAsync(nameof(AddTermPage), new Dictionary<string, object>
                    {
                        { "term", term }
                    });
                }
                else
                {
                    Console.WriteLine("Term is null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to edit page: {ex.Message}");
            }
        }
        private async void OnViewClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                var term = button?.BindingContext as Term;
                if (term != null)
                {
                    await Shell.Current.GoToAsync(nameof(CoursePage), new Dictionary<string, object>
            {
                { "termId", term.Id }
            });
                }
                else
                {
                    Console.WriteLine("Term is null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to view page: {ex.Message}");
            }
        }


    }
}

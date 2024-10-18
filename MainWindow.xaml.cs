using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using IOPath = System.IO.Path; 
using System.Security.Claims;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace CMCS
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string selectedFilePath;
        public ObservableCollection<Claim> PendingClaims { get; set; }
        public ObservableCollection<Claim> MyClaims { get; set; }
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB as the max file size
        private readonly string[] AllowedFileExtensions = { ".pdf", ".docx", ".xlsx" };

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            PendingClaims = new ObservableCollection<Claim>();
            MyClaims = new ObservableCollection<Claim>();
            LoadSampleClaims();
            DataContext = this;
        }

        private void LoadSampleClaims()
        {
            // Pending claims samples
            PendingClaims.Add(new Claim { ClaimId = 1, Lecturer = "John Doe", Course = "Web Development", Hours = 20, Rate = 50, Status = ClaimStatus.Pending });
            PendingClaims.Add(new Claim { ClaimId = 2, Lecturer = "Jane Smith", Course = "Database Design", Hours = 15, Rate = 55, Status = ClaimStatus.Pending });

            // Claims sampled
            MyClaims.Add(new Claim { ClaimId = 3, Course = "Software Engineering", Hours = 25, Rate = 60, Status = ClaimStatus.Pending });
            MyClaims.Add(new Claim { ClaimId = 4, Course = "Mobile App Development", Hours = 18, Rate = 58, Status = ClaimStatus.Approved });
        }

        private void SubmitClaimButton_Click(object sender, RoutedEventArgs e)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(CourseComboBox.Text))
            {
                ShowError("Please select a course.");
                return;
            }

            if (!decimal.TryParse(HoursWorkedTextBox.Text, out decimal hoursWorked) || hoursWorked <= 0)
            {
                ShowError("Please enter a valid number of hours worked.");
                return;
            }

            if (!decimal.TryParse(HourlyRateTextBox.Text, out decimal hourlyRate) || hourlyRate <= 0)
            {
                ShowError("Please enter a valid hourly rate.");
                return;
            }

            Claim newClaim = new Claim
            {
                ClaimId = MyClaims.Count + 1,
                Course = CourseComboBox.Text,
                Hours = int.Parse(HoursWorkedTextBox.Text),
                Rate = decimal.Parse(HourlyRateTextBox.Text),
                Status = ClaimStatus.Pending
            };

            // Adding new claim to MyClaims
            MyClaims.Add(newClaim);

            // Handles file storage
            string storedFilePath = null;
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                try
                {
                    string uploadDirectory = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "UploadedFiles");
                    Directory.CreateDirectory(uploadDirectory);

                    string fileName = IOPath.GetFileName(selectedFilePath);
                    string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                    storedFilePath = IOPath.Combine(uploadDirectory, uniqueFileName);

                    File.Copy(selectedFilePath, storedFilePath);
                }
                catch (Exception ex)
                {
                    ShowError($"Failed to store the supporting document: {ex.Message}");
                    MyClaims.Remove(newClaim);
                    return;
                }
            }

            // Displays success message
            string message = $"Claim submitted successfully!\n\n" +
                             $"Course: {CourseComboBox.Text}\n" +
                             $"Hours Worked: {hoursWorked}\n" +
                             $"Hourly Rate: {hourlyRate:C}\n" +
                             $"Additional Notes: {AdditionalNotesTextBox.Text}\n" +
                             $"Supporting Document: {(string.IsNullOrEmpty(storedFilePath) ? "None" : IOPath.GetFileName(storedFilePath))}";

            MessageBox.Show(message, "Claim Submitted", MessageBoxButton.OK, MessageBoxImage.Information);

            // After submission the form is cleared
            ClearForm();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                FileNameTextBox.Text = System.IO.Path.GetFileName(selectedFilePath);
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ClearForm()
        {
            CourseComboBox.SelectedIndex = -1;
            HoursWorkedTextBox.Clear();
            HourlyRateTextBox.Clear();
            AdditionalNotesTextBox.Clear();
            FileNameTextBox.Clear();
            selectedFilePath = null;
        }

        // ComboBox selection changed event is handled by this method
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Allows you to add any logic here that you want to occur when the course selection changes
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedCourse = selectedItem.Content.ToString();
                // Updates hourly rate based on selected course
                Console.WriteLine($"Selected course: {selectedCourse}");
            }
        }

        private void HoursWorkedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(HoursWorkedTextBox.Text, out decimal hours))
            {
                // Updates other parts of your UI based on the hours worked
                if (decimal.TryParse(HourlyRateTextBox.Text, out decimal rate))
                {
                    decimal totalAmount = hours * rate;
                    // Updates a label or textbox to display the total amount
                    Console.WriteLine($"Total amount: {totalAmount:C}");
                }
            }
        }

        private void HourlyRateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(HourlyRateTextBox.Text, out decimal rate))
            {
                // Updates UI based on rate changes
                if (decimal.TryParse(HoursWorkedTextBox.Text, out decimal hours))
                {
                    decimal totalAmount = hours * rate;
                    // Updates a label or textbox to display the total amount
                    Console.WriteLine($"Total amount: {totalAmount:C}");
                }
            }
        }

        private void LoadSamplePendingClaims()
        {
            PendingClaims.Add(new Claim { ClaimId = 1, Lecturer = "John Doe", Course = "Web Development", Hours = 20, Rate = 50 });
            PendingClaims.Add(new Claim { ClaimId = 2, Lecturer = "Jane Smith", Course = "Database Design", Hours = 15, Rate = 55 });
            PendingClaims.Add(new Claim { ClaimId = 3, Lecturer = "Bob Johnson", Course = "Software Engineering", Hours = 25, Rate = 60 });
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Claim claim)
            {
                claim.Status = ClaimStatus.Approved;
                PendingClaims.Remove(claim);
                MessageBox.Show($"Claim {claim.ClaimId} approved for {claim.Lecturer}.", "Claim Approved", MessageBoxButton.OK, MessageBoxImage.Information);

                // Updates the corresponding claim in MyClaims
                var myClaimToUpdate = MyClaims.FirstOrDefault(c => c.ClaimId == claim.ClaimId);
                if (myClaimToUpdate != null)
                {
                    myClaimToUpdate.Status = ClaimStatus.Approved;
                }

                OnPropertyChanged(nameof(PendingClaims));
                OnPropertyChanged(nameof(MyClaims));
            }
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Claim claim)
            {
                claim.Status = ClaimStatus.Rejected;
                PendingClaims.Remove(claim);
                MessageBox.Show($"Claim {claim.ClaimId} rejected for {claim.Lecturer}.", "Claim Rejected", MessageBoxButton.OK, MessageBoxImage.Information);

                // Updates the corresponding claim in MyClaims
                var myClaimToUpdate = MyClaims.FirstOrDefault(c => c.ClaimId == claim.ClaimId);
                if (myClaimToUpdate != null)
                {
                    myClaimToUpdate.Status = ClaimStatus.Rejected;
                }

                OnPropertyChanged(nameof(PendingClaims));
                OnPropertyChanged(nameof(MyClaims));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Allowed Files (*.pdf;*.docx;*.xlsx)|*.pdf;*.docx;*.xlsx",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                if (!AllowedFileExtensions.Contains(fileExtension))
                {
                    ShowError("Invalid file type. Please upload a PDF, DOCX, or XLSX file.");
                    return;
                }

                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > MaxFileSize)
                {
                    ShowError("File size exceeds the limit of 10 MB. Please upload a smaller file.");
                    return;
                }

                selectedFilePath = filePath;
                FileNameTextBox.Text = System.IO.Path.GetFileName(selectedFilePath);
                FileInfoTextBlock.Text = $"File size: {fileInfo.Length / 1024.0:F2} KB";
            }
        }

        private string SecurelyStoreFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            try
            {
                string uploadDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UploadedFiles");
                Directory.CreateDirectory(uploadDirectory);

                string fileName = System.IO.Path.GetFileName(filePath);
                string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                string destinationPath = System.IO.Path.Combine(uploadDirectory, uniqueFileName);

                // Copies the file to the secure location
                File.Copy(filePath, destinationPath);

                return destinationPath;
            }
            catch (Exception ex)
            {
                // Gets the error and show a message to the user
                Console.WriteLine($"Error storing file: {ex.Message}");
                MessageBox.Show("An error occurred while storing the file. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }

    public class Claim
    {
        public int ClaimId { get; set; }
        public string Lecturer { get; set; }
        public string Course { get; set; }
        public int Hours { get; set; }
        public decimal Rate { get; set; }
        public decimal Total => Hours * Rate;
        public ClaimStatus Status { get; set; }
    }


    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\nPlease contact support if the issue persists.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            Console.WriteLine($"Unhandled exception: {ex}");
        }
    }
}

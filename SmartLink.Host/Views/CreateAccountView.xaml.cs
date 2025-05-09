using System;
using System.Text.RegularExpressions;
using System.Windows;
using SmartLink.Host.Services;

namespace SmartLink.Host.Views
{
    public partial class CreateAccountView : Window
    {
        private readonly FirebaseService _firebaseService;
        
        public event EventHandler<string> EmailSet;
        
        public CreateAccountView()
        {
            InitializeComponent();
            
            // Get the Firebase service instance
            _firebaseService = FirebaseService.Instance;
        }
        
        private async void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            
            // Validate full name
            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Please enter your full name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Validate email
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Validate password
            if (!IsValidPassword(password))
            {
                MessageBox.Show("Password must be at least 8 characters and contain uppercase, lowercase, number, and special character.", 
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Check if passwords match
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            try
            {
                // Create user in Firebase Auth
                var userCredential = await _firebaseService.CreateUserWithEmailAndPasswordAsync(email, password);
                
                // Store additional user data in Firestore
                await _firebaseService.StoreUserDataAsync(userCredential.User.Uid, email, fullName);
                
                MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Return to login page with email filled
                EmailSet?.Invoke(this, email);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating account: {ex.Message}", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }
        
        private bool IsValidPassword(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        }
    }
}
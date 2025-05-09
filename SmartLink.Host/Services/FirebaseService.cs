using System;
using System.IO;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Google.Cloud.Firestore;

namespace SmartLink.Host.Services
{
    public class FirebaseService
    {
        private static FirebaseService? _instance;
        private readonly FirebaseAuthClient _authClient;
        private readonly FirestoreDb _firestoreDb;
        
        public static FirebaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FirebaseService();
                }
                return _instance;
            }
        }
        
        private FirebaseService()
        {
            // Initialize Firebase Auth
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyCOEQf2VYa9EXmfYJyTTGKuaRM6ZbgrBm8",
                AuthDomain = "smartlinkapp-1.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };
            
            _authClient = new FirebaseAuthClient(config);
            
            // Initialize Firestore
            string credentialsPath = GetCredentialsPath();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
            _firestoreDb = FirestoreDb.Create("smartlinkapp-1");
        }
        
        private string GetCredentialsPath()
        {
            // Try to find the credentials file in the project root directory
            string projectRootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
            string standardCredentialsPath = Path.Combine(projectRootPath, "firebase-credentials.json");
            
            if (File.Exists(standardCredentialsPath))
            {
                return standardCredentialsPath;
            }
            
            // Fall back to the specific path for the current developer
            string specificPath = @"C:\Users\nt667\source\repos\Smart-Link\smartlinkapp-1-firebase-adminsdk-fbsvc-41e49d3987.json";
            if (File.Exists(specificPath))
            {
                return specificPath;
            }
            
            throw new FileNotFoundException("Firebase credentials file not found. Please place it in the project root directory as 'firebase-credentials.json'.");
        }
        
        public async Task<UserCredential> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            return await _authClient.SignInWithEmailAndPasswordAsync(email, password);
        }
        
        public async Task<UserCredential> CreateUserWithEmailAndPasswordAsync(string email, string password)
        {
            return await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
        }
        
        public async Task StoreUserDataAsync(string userId, string email, string name)
        {
            var userDoc = _firestoreDb.Collection("users").Document(userId);
            await userDoc.SetAsync(new
            {
                email = email,
                name = name,
                createdAt = Timestamp.GetCurrentTimestamp(),
                isOnline = true
            });
        }
        
        public async Task UpdateUserOnlineStatusAsync(string userId, bool isOnline)
        {
            var userDoc = _firestoreDb.Collection("users").Document(userId);
            await userDoc.UpdateAsync("isOnline", isOnline);
        }
        
        public async Task SignOutAsync()
        {
            if (_authClient.User != null)
            {
                await UpdateUserOnlineStatusAsync(_authClient.User.Uid, false);
                _authClient.SignOut();
            }
        }
    }
}
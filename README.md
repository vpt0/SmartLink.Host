# Smart-Link Project
## Firebase Setup

For team members to work with this project, follow these steps:

1. Ask the project admin to grant you access to the Firebase project
2. Go to the Firebase Console (https://console.firebase.google.com/)
3. Select the "SmartLinkApp" project
4. Click on the gear icon (??) to open Project settings
5. Go to the "Service accounts" tab
6. Click "Generate new private key" to download your credentials
7. Rename the downloaded file to "firebase-credentials.json"
8. Place this file in the root directory of the project
9. Do NOT commit this file to the repository (it should be ignored by git)

The application will look for this file to authenticate with Firebase services.

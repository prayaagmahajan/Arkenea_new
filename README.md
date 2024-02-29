Prerequisites:
Install Visual Studio or Visual Studio Code.
Install .NET Core SDK.

1. Steps:
1.1. Clone the Repository:
1.2. Clone the repository to your local machine using the following command: git clone <repository_url>

2. Open the Project:
Open the project in Visual Studio or Visual Studio Code.

3. Restore Packages:
Restore the NuGet packages by running the following command in the terminal: dotnet restore

4. Set up Database:
update the connection string in the appsettings.json file. Then, apply migrations to create the database schema by running the following command in the terminal:
-> Add Migration <Migration_Name>
once migration is done run the below command.
-> update-database

Once Run it will open a coonection on Port: "localhost:5221"

Usage:

Register a new account by clicking on the "Register" link and providing the required information.
Log in with your registered account credentials.
Upload files to your profile, edit your profile information, and download uploaded files.
Reset your password if forgotten by clicking on the "Forgot Password" link on the login page.

<Note> once logged in there is a tab on navbar with your firstname click on that to navigate to profile section.
<Note> For profile picture Please provide Image Url.

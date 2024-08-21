# C971 Mobile Application Development Using C Sharp

## Overview

This project is a mobile application developed using .NET MAUI (Multi-platform App UI) as part of a college course on mobile application development. The application is designed to help students manage their academic terms, courses, and assessments.

## Features

- **Cross-platform Support**: The application runs on Android, iOS, and Windows platforms.
- **Academic Term Management**: 
  - Add, edit, delete, and store academic terms.
  - Each term includes a title, start date, and anticipated end date (using a date picker).
- **Course Management**: 
  - Add, edit, delete, and store up to six courses per term.
  - Each course includes the course name, start date, anticipated end date (using a date picker), and course status (using a picker).
  - Manage course instructor information, including name, phone, and email.
  - Add, share, and display optional notes.
  - Set notifications for the start and end dates of each course.
- **Assessment Management**: 
  - Add, edit, delete, and store two assessments per course (one objective and one performance assessment).
  - Include assessment names and set notifications for the start and end dates of each assessment.

## Technologies Used

- **.NET MAUI**: For building the cross-platform application.
- **C#**: The primary programming language used for application logic.
- **XAML**: For designing the user interface.
- **SQLite**: For local data storage.
- **Plugin.LocalNotification**: For setting notifications.
- **Plugin.Share**: For sharing notes via email or SMS.

## Installation

### Prerequisites

Before you begin, ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download/dotnet)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with the MAUI workload
- [Android SDK](https://developer.android.com/studio) (for Android development)
- [Xcode](https://developer.apple.com/xcode/) (for iOS development)

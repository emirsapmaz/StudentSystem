# Student Management System

A comprehensive web application built with **ASP.NET MVC** and **PostgreSQL** for managing users, courses, enrollments, grades, comments, and attendance in educational institutions.

## Features

### Role-Based Access Control
- **Admin:** Complete system control - manage all users, courses, enrollments, grades, comments, and attendance
- **Teacher:** Manage assigned courses, grade students, register attendance, and manage student users
- **Student:** View enrolled courses, grades, and teacher feedback

### Core Functionality
- User management with role-based permissions
- Course creation and management
- Student enrollment system
- Grade tracking and teacher comments
- Attendance registration with date-specific records
- Course status management (Started, Waiting, Finished)
- Responsive Bootstrap UI with custom styling

## Technology Stack

- **Backend:** ASP.NET MVC (.NET 8.0)
- **Database:** PostgreSQL with Entity Framework Core
- **Authentication:** ASP.NET Identity
- **Frontend:** Bootstrap 5, Font Awesome, jQuery
- **ORM:** Entity Framework Core with Code First migrations

## Prerequisites

Before running this application, ensure you have:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/)
- [PostgreSQL 13+](https://www.postgresql.org/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/your-username/student-management-system.git
cd student-management-system
```

### 2. Database Configuration
Create a PostgreSQL database named `StudentSystemDb` and update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=StudentSystemDb;Username=your_postgres_user;Password=your_postgres_password"
  }
}
```

### 3. Install Dependencies
```bash
dotnet restore
```

### 4. Apply Database Migrations
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The application will be available at `https://localhost:5001`

## Test Accounts

The application comes with pre-seeded test accounts:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@pusula.com | pusula |
| Teacher | teacher@pusula.com | pusula |
| Student | student@pusula.com | pusula |

## Project Structure

### Controllers
- **AdminController:** User management (Admin-only access)
- **CoursesController:** Course operations, enrollment, grading, attendance
- **TeachersController:** Student management (Teacher-only access)
- **HomeController:** Main Page Dashboard management
- **AccountController:** Authorization

### Models
- **User:** Extended Identity user with custom properties (FirstName, LastName, Gender, TeacherSubject)
- **Course:** Course information with teacher assignment and status tracking
- **Enrollment:** Student-course relationships with grades and comments
- **ClassAttendance:** Date-specific attendance records

### Key Views
- **Admin Dashboard:** `/Admin/Profile` - User dashboard for all roles
- **User Management:** `/Admin/Index`, `/Admin/Edit` - Admin user management
- **Student Management:** `/Teachers/Index`, `/Teachers/Edit` - Teacher student management
- **Course Management:** `/Courses/Index` - Role-based course listing
- **Course Details:** `/Courses/Details/{id}` - Comprehensive course management interface

## Usage Guide

### Admin Functions
1. User Management: Create, edit, and delete users at `/Admin/Index`
2. Course Management: Create and delete courses, manage enrollments
3. Full Access: Grade students, register attendance, update course status `/Admin/Index`
2. **Course Management:** Create and delete courses, manage enrollments
3. **Full Access:** Grade students, register attendance, update course status

### Teacher Functions
1. **Student Management:** Create, edit, and delete student accounts at `/Teachers/Index`
2. **Course Management:** View and manage assigned courses
3. **Academic Tasks:** Grade students, add comments, register attendance, update course status

### Student Functions
1. **Course Access:** View enrolled courses at `/Courses/Index`
2. **Academic Progress:** View grades and teacher comments at course details pages

## Bonus Features

### Attendance Management
- Date-specific attendance registration
- Modal interface with student checkboxes
- Duplicate attendance validation
- Accessible to Teachers and Admins

### Course Status Tracking
- Dynamic status updates (Started, Waiting, Finished)
- Color-coded status badges
- Auto-submit dropdown interface

### Teacher Student Management
- Complete CRUD operations for student accounts
- Teacher-role restricted access
- Mirrors admin functionality for student management

-Adding simple charts 
-UI/UX tweaks (dark mode, responsive design).
-Adding user search and filtering capabilities.
-Adding a GPA calculation and reporting screen for students.
-Adding a "student list for their own courses" filter for teachers.

## Development

### Database Migrations
When making model changes, create and apply new migrations:
```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

### Custom Styling
The application uses custom CSS variables in `wwwroot/css/site.css`:
- `--success-color`
- `--info-color`
- Custom responsive design with mobile-friendly interfaces

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Screenshots

<img width="2879" height="1509" alt="Ekran görüntüsü 2025-09-11 225322" src="https://github.com/user-attachments/assets/85ca95d9-2c66-438f-b019-2486225cf6cb" />
<img width="2837" height="1522" alt="Ekran görüntüsü 2025-09-11 225229" src="https://github.com/user-attachments/assets/88a5f386-a086-4dc9-a420-cb87b96486b4" />
<img width="2839" height="1519" alt="Ekran görüntüsü 2025-09-11 225254" src="https://github.com/user-attachments/assets/2bc3259e-2f65-402e-9037-69329ca0f35c" />
<img width="2877" height="1523" alt="Ekran görüntüsü 2025-09-11 225311" src="https://github.com/user-attachments/assets/5d92b6b0-c76c-44ff-96d6-647a3f4f111f" />








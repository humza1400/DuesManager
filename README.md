# DuesManager

A general-purpose ASP.NET Web Forms application for managing members and payments.  
This project was built as a learning exercise to explore core concepts of Web Forms development, including page lifecycle management, state management, data binding, authentication, and caching.
![image](https://github.com/user-attachments/assets/3f1c1889-4c9a-4a29-957d-9b8ac2cd696f)


## 🚀 Purpose
The project simulates a simple membership and dues management system.  
It’s designed to help learn and demonstrate:
- CRUD operations using Web Forms and C#
- ASP.NET Web Forms page lifecycle
- State management (ViewState, Session, Cookies)
- Authentication with Forms Authentication (Planned)
- Data caching and output caching
- Validation controls and best practices
- SQL Server database interactions (ADO.NET)

## 🛠️ Technologies Used
- **C#**
- **ASP.NET Web Forms**
- **.NET Framework 4.7.2 / 4.8**
- **SQL Server / LocalDB**
- **HTML/CSS/JavaScript** (Basic for UI interactions)
- **IIS Express** (For local hosting in development)

## 📂 Project Structure
```
/App_Data        → Application wide code
/App_Data        → LocalDB database files  
/Scripts         → JavaScript files 
/Styles          → CSS files  
Login.aspx       → Login page (Planned)
Members.aspx     → Manage members  
Payments.aspx    → Manage member payments  
Reports.aspx     → Reports page  
Web.config       → Project configuration and authentication setup
```

## ✅ Features
- **Login System** using Forms Authentication  (Planned)
- **Member Management**: Create, Read, Update, Delete members  
- **Payment Management**: Track payments made by members  
- **Page Lifecycle Demonstrations**: Logs key lifecycle events  
- **State Management Examples**: ViewState, Session, Application  
- **Caching**: Output caching on reports, data caching for performance  
- **Validation Controls**: Ensures data integrity  
- **Simple UI** with ASP.NET Web Forms server controls (GridView, DropDownList, etc.)

## 💡 How to Run Locally
1. Open the solution in **Visual Studio 2022 (or later)**  
2. Build the project  
3. Run the app with **IIS Express** (`F5` or `Ctrl+F5`)  
4. Access the app via browser (default URL: `http://localhost:xxxx`)

> ⚠️ Note: Make sure **SQL Server LocalDB** is installed for the default DB setup, or adjust the connection string in `Web.config` for another SQL Server instance.

## 📌 Future Improvements
- Role-based authorization  
- Responsive UI using Bootstrap  
- REST API for external integrations  
- Improved error handling and logging (Serilog / NLog)

## 📄 License
This project is for educational purposes.

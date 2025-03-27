using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using RestaurantManagement1.Models;



namespace RestaurantManagement.Controllers
{
    public class HomeController : Controller
    {

        restaurantDB1Entities restaurantDB = new restaurantDB1Entities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            ViewBag.error = null;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string employeeUsername, string employeePassword)
        {

            using (var context = new restaurantDB1Entities())
            {
                Employees loginEmployee = context.Employees.SqlQuery(
                    "SELECT * FROM Employees WHERE Username=@username AND Password=@password",
                    new SqlParameter("@username", employeeUsername),
                    new SqlParameter("@password", employeePassword)
                ).FirstOrDefault();

                Session["Username"] = loginEmployee.Username;

                if (loginEmployee != null)
                {
                    if (employeeUsername == "admin")
                    {
                        return RedirectToAction("Index2");
                    }
                    else
                    {
                        return RedirectToAction("Index3", new { employeeUsername = employeeUsername });
                    }
                }

                ViewBag.error = "Hatalı giriş";
                return View();
            }
        }






        private readonly restaurantDB1Entities db = new restaurantDB1Entities();


        public ActionResult Employees()
        {
            var employees = db.Database.SqlQuery<Employees>("SELECT * FROM Employees").ToList();
            return View(employees);
        }



        [HttpPost]
        public ActionResult AddEmployee(string employeeUsername, string employeePasword, string role, string employeeName, string employeeSurname, string employeeStatus)
        {
            db.Database.ExecuteSqlCommand("INSERT INTO Employees (Username, Password, Role, EmployeeName, EmployeeSurname, EmployeeStatus) VALUES (@Username, @Password, @Role, @Name, @Surname, @EmployeeStatus)",
                new SqlParameter("@Username", employeeUsername),
                new SqlParameter("@Password", employeePasword),
                new SqlParameter("@Role", role),
                new SqlParameter("@Name", employeeName),
                new SqlParameter("@Surname", employeeSurname),
                 new SqlParameter("@EmployeeStatus", employeeStatus));

            return RedirectToAction("Employees");
        }



        [HttpPost]
        public ActionResult DeleteEmployee(int employeeId)
        {
            db.Database.ExecuteSqlCommand("DELETE FROM Employees WHERE EmployeeID = @ID",
                new SqlParameter("@ID", employeeId));

            return RedirectToAction("Employees");
        }


        [HttpPost]
        public ActionResult UpdateEmployee(int employeeId, string newUsername, string newPassword, string newRole, string newName, string newSurname)
        {
            db.Database.ExecuteSqlCommand("UPDATE Employees " +
                "SET Username=@username, Password=@password, Role=@role, EmployeeName= @name, EmployeeSurname=@Surname " +
                "WHERE EmployeeID=@ID",
                new SqlParameter("@ID", employeeId),
                new SqlParameter("@username", newUsername),
                new SqlParameter("@password", newPassword),
                new SqlParameter("@role", newRole),
                new SqlParameter("@name", newName),
                new SqlParameter("@Surname", newSurname));

            return RedirectToAction("Employees");
        }

        public ActionResult Index2()
        {



            string username = Session["Username"] as string;


            if (username != "admin")
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index3");
            }

            using (var context = new restaurantDB1Entities())
            {
                var employees = db.Database.SqlQuery<Employees>("SELECT * FROM Employees").ToList();
                return View(employees);
            }
        }

        public ActionResult Index3()
        {

            using (var context = new restaurantDB1Entities())
            {
                var customers = context.Customers.ToList();
                return View(customers);
            }
        }


        public ActionResult Customers()
        {
            return View();

        }

        public ActionResult DepartedCustomers()
        {
            using (var context = new restaurantDB1Entities())
            {
                var departedCustomers = context.DepartedCustomers.ToList();
                return View(departedCustomers);
            }
        }








        public ActionResult Reservations()
        {


            using (var context = new restaurantDB1Entities())
            {
                var reservations = context.Reservations.ToList();
                return View(reservations);
            }

        }

        [HttpPost]

        public ActionResult AddReservation(string name, string surname, string phone, int numberOfPeople, DateTime reservationTime)

        {



            int capacity;

            if (numberOfPeople == 1)

                capacity = 1;

            else if (numberOfPeople == 2)

                capacity = 2;

            else if (numberOfPeople == 3 || numberOfPeople == 4)

                capacity = 4;

            else if (numberOfPeople <= 10)

                capacity = 10;

            else

            {

                TempData["ErrorMessage"] = "10 kişiden fazla rezervasyon kabul edilemiyor.";

                return RedirectToAction("Reservations");

            }



            using (var context = new restaurantDB1Entities())

            {



                string query = $"SELECT TOP 1 TableID FROM Tables " +

                               $"WHERE Capacity = {capacity} AND IsAvailable = 1";

                int? tableId = context.Database.SqlQuery<int?>(query).FirstOrDefault();



                if (!tableId.HasValue)

                {

                    TempData["ErrorMessage"] = "Üzgünüz, uygun masa bulunamadı.";

                    return RedirectToAction("Reservations");

                }





                query = $"SELECT TOP 1 EmployeeID FROM Employees " +

                        $"WHERE Role = 'Waiter' AND EmployeeStatus = 'available' " +

                        $"ORDER BY NEWID()";

                int? employeeId = context.Database.SqlQuery<int?>(query).FirstOrDefault();



                if (!employeeId.HasValue)

                {

                    TempData["ErrorMessage"] = "Üzgünüz, uygun garson bulunamadı.";

                    return RedirectToAction("Reservations");

                }





                query = "INSERT INTO Reservations (FirstName, LastName, Phone, NumberOfPeople, ReservationTime, ReservationStatus, TableID, EmployeeID) " +

                        "VALUES (@Name, @Surname, @Phone, @NumberOfPeople, @ReservationDateTime, 'Active', @TableID, @EmployeeID)";



                context.Database.ExecuteSqlCommand(query,

                    new SqlParameter("@Name", name),

                    new SqlParameter("@Surname", surname),

                    new SqlParameter("@Phone", phone),

                    new SqlParameter("@NumberOfPeople", numberOfPeople),

                    new SqlParameter("@ReservationDateTime", reservationTime),

                    new SqlParameter("@TableID", tableId.Value),

                    new SqlParameter("@EmployeeID", employeeId.Value));





                query = "UPDATE Tables SET IsAvailable = 0 WHERE TableID = @TableID";



                context.Database.ExecuteSqlCommand(query, new SqlParameter("@TableID", tableId.Value));





                query = "UPDATE Employees SET EmployeeStatus = 'not available' WHERE EmployeeID = @EmployeeID";



                context.Database.ExecuteSqlCommand(query, new SqlParameter("@EmployeeID", employeeId.Value));





                query = "INSERT INTO TableEmployees (EmployeeID, TableID) VALUES (@EmployeeID, @TableID)";

                context.Database.ExecuteSqlCommand(query,

                    new SqlParameter("@EmployeeID", employeeId.Value),

                    new SqlParameter("@TableID", tableId.Value));





                query = "INSERT INTO Customers (ReservationID, FirstName, LastName, Phone) " +

                      "VALUES ((SELECT MAX(ReservationID) FROM Reservations), @FirstName, @LastName, @Phone)";



                context.Database.ExecuteSqlCommand(query,

                    new SqlParameter("@FirstName", name),

                    new SqlParameter("@LastName", surname),

                    new SqlParameter("@Phone", phone));





                TempData["SuccessMessage"] = "Rezervasyon başarıyla eklendi.";

            }



            return RedirectToAction("Reservations");

        }








        [HttpPost]
        public ActionResult CompleteReservation(int reservationId, string name, string surname, string phone)
        {
            using (var context = new restaurantDB1Entities())
            {

                string query = "UPDATE Reservations SET ReservationStatus = 'Completed' WHERE ReservationID = @ReservationID";
                context.Database.ExecuteSqlCommand(query, new SqlParameter("@ReservationID", reservationId));


                var tableQuery = @"UPDATE Tables
                                   SET IsAvailable = 1
                                   WHERE TableID = (SELECT TableID FROM Reservations  WHERE ReservationID = @ReservationID);";


                var employeeQuery = @"UPDATE Employees
                                    SET EmployeeStatus = 'available'
                                    WHERE EmployeeID IN (SELECT E.EmployeeID
                                                        FROM Employees E
                                                        INNER JOIN TableEmployees TE ON E.EmployeeID= TE.EmployeeID
                                                        INNER JOIN Reservations R ON TE.TableID = R.TableID
                                                        WHERE R.ReservationID = @ReservationID);";

                context.Database.ExecuteSqlCommand(tableQuery, new SqlParameter("@ReservationID", reservationId));
                context.Database.ExecuteSqlCommand(employeeQuery, new SqlParameter("@ReservationID", reservationId));

                try
                {



                    string insertQuery = @" INSERT INTO DepartedCustomers (ReservationID, FirstName, LastName, Phone)
                                         SELECT ReservationID, FirstName, LastName, Phone FROM Customers
                                          WHERE ReservationID = @ReservationID";



                    context.Database.ExecuteSqlCommand(insertQuery, new SqlParameter("@ReservationID", reservationId));





                    string deleteQuery = @"DELETE FROM Customers WHERE ReservationID = @ReservationID";
                    context.Database.ExecuteSqlCommand(deleteQuery, new SqlParameter("@ReservationID", reservationId));



                    TempData["SuccessMessage"] = "Rezervasyon başaryla tamamlandı.";
                }
                catch (Exception ex)
                {
                    TempData["SuccessMessage"] = "Rezervasyon tamamlanırken hata oldu. : " + ex.Message;
                }
            }
            return RedirectToAction("Reservations");
        }

        public class TableEmployee
        {
            public int TableID { get; set; }
            public int EmployeeID { get; set; }
        }




        [HttpPost]
        public ActionResult UpdateReservation(int reservationId, string name, string surname, string phone, int numberOfPeople, DateTime reservationTime)
        {
            using (var context = new restaurantDB1Entities())
            {

                string query = "UPDATE Reservations SET FirstName = @Name, LastName = @Surname, Phone = @Phone, NumberOfPeople = @NumberOfPeople, ReservationTime = @ReservationTime WHERE ReservationID = @ReservationID";
                context.Database.ExecuteSqlCommand(query,
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Surname", surname),
                    new SqlParameter("@Phone", phone),
                    new SqlParameter("@NumberOfPeople", numberOfPeople),
                    new SqlParameter("@ReservationTime", reservationTime),
                    new SqlParameter("@ReservationID", reservationId));

                TempData["SuccessMessage"] = "Rezervasyon başarıyla güncellendi.";
            }

            return RedirectToAction("Reservations");
        }

        [HttpPost]
        public ActionResult CancelReservation(int reservationId)
        {
            using (var context = new restaurantDB1Entities())
            {

                string query = "UPDATE Reservations SET ReservationStatus = 'Cancelled' WHERE ReservationID = @ReservationID";
                context.Database.ExecuteSqlCommand(query, new SqlParameter("@ReservationID", reservationId));


                string deleteQuery = "DELETE FROM Customers WHERE ReservationID = @ReservationID";
                context.Database.ExecuteSqlCommand(deleteQuery, new SqlParameter("@ReservationID", reservationId));




                string updateTableQuery = "UPDATE Tables SET IsAvailable = 1 WHERE TableID = (SELECT TableID FROM Reservations WHERE ReservationID = @ReservationID)";
                context.Database.ExecuteSqlCommand(updateTableQuery, new SqlParameter("@ReservationID", reservationId));


                string updateEmployeeQuery = "UPDATE Employees SET EmployeeStatus = 'available' WHERE EmployeeID IN (SELECT EmployeeID FROM Reservations WHERE ReservationID = @ReservationID)";
                context.Database.ExecuteSqlCommand(updateEmployeeQuery, new SqlParameter("@ReservationID", reservationId));

                TempData["SuccessMessage"] = "Rezervasyon başarıyla iptal edildi.";
            }

            return RedirectToAction("Reservations");
        }







    }
}
using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ADONETDemo
{
  class Program
  {
    public static IConfiguration Configuration { get; set; }
    static void Main(string[] args)
    {
      Configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

      DBConfiguration dbConf = Configuration.GetSection("DB").Get<DBConfiguration>();

      SqlConnectionStringBuilder builder = new(dbConf.cs);
      builder.UserID = dbConf.user;
      builder.Password = dbConf.password;

      SqlConnection connection = new(builder.ConnectionString);

      SqlCommand query = new("SELECT c.CustomerID, c.FirstName, c.LastName, COUNT(o.CustomerID) Orders, SUM(o.TotalDue) Total from SalesLT.Customer c LEFT JOIN SalesLT.SalesOrderHeader o ON c.CustomerID = o.CustomerID GROUP BY c.CustomerID, c.FirstName, c.LastName HAVING COUNT(o.CustomerID)  > 0;", connection);


      connection.Open();

      SqlDataReader myReader = query.ExecuteReader();

      while (myReader.Read())
      {
        Console.WriteLine($"{myReader["FirstName"]} {myReader["LastName"]} has {myReader["Orders"]} orders with a total sale of {myReader["Total"]}");
      }

      connection.Close();

    }
  }
}

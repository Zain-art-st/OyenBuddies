using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

public static class DatabaseHelper
{
    private static readonly string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog = OyenGrooming; Integrated Security = True; Connect Timeout = 30; Encrypt=False;Trust Server Certificate=False;Application Intent = ReadWrite; Multi Subnet Failover=False";

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}

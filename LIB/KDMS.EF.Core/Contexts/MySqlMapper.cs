using MySqlConnector;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using KDMS.EF.Core.Infrastructure.Reverse;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace KDMS.EF.Core.Contexts;

public class MySqlMapper : KdmsContext
{
    public MySqlMapper(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string? connectionString = _configuration.GetConnectionString("Server");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }

    public bool IsConnection()
    {
        bool retValue = false;
        try
        {
            Database.GetDbConnection().Open();
            retValue = true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            Database.GetDbConnection().Close();
        }

        return retValue;
    }

    public static bool IsConnection(string connString)
    {
        bool retValue = false;
        MySqlConnection conn = new MySqlConnection(connString);
        try
        {
            conn.Open();
            retValue = true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            conn.Close();
        }

        return retValue;
    }

    public List<T> ExecuteQuery<T>(string query) where T : class, new()
    {
        using (var command = Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            Database.OpenConnection();
            using (var reader = command.ExecuteReader())
            {
                var lst = new List<T>();
                var lstColumns = new T().GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
                while (reader.Read())
                {
                    var newObject = new T();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        PropertyInfo prop = lstColumns.FirstOrDefault(a => a.Name.Replace("_", "").ToLower().Equals(name.Replace("_", "").ToLower()));
                        if (prop == null)
                            continue;

                        var val = reader.IsDBNull(i) ? null : reader[i];
                        prop.SetValue(newObject, val, null);
                    }
                    lst.Add(newObject);
                }
                return lst;
            }
        }
    }

    public bool RunQuery(string query)
    {
        bool retVal = false;
        try
        {
            using (var command = Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Database.OpenConnection();
                command.ExecuteNonQuery();
                retVal = true;
            }
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("{0} 쿼리 오류({1})", query, e.Message));
        }
        finally
        {
        }
        return retVal;
    }

    //public DataTable SelectQuery(string query)
    //{
    //    MySqlConnection conn = new MySqlConnection(connString);

    //    DataSet ds = new DataSet();
    //    MySqlDataAdapter oda = new MySqlDataAdapter(query, conn);
    //    DataTable dt = null;

    //    try
    //    {
    //        oda.Fill(ds, "ReturnTable");
    //        dt = ds.Tables["ReturnTable"];
    //    }
    //    catch (Exception e)
    //    {
    //        throw new Exception(string.Format("({0}) 쿼리 오류", query));
    //    }
    //    finally
    //    {
    //        conn.Close();
    //    }

    //    return dt;
    //}

    //public bool RunQuery(string query)
    //{
    //    MySqlConnection conn = new MySqlConnection(connString);

    //    //query = "begin " + query + " end;";

    //    bool retVal = true;
    //    MySqlCommand cmd = new MySqlCommand(query, conn);
    //    cmd.CommandType = CommandType.Text;
    //    try
    //    {
    //        if (conn.State == ConnectionState.Closed)
    //            conn.Open();

    //        cmd.ExecuteNonQuery();
    //    }
    //    catch (Exception e)
    //    {
    //        retVal = false;
    //        throw new Exception(string.Format("{0} 쿼리 오류({1})", query, e.Message));
    //    }
    //    finally
    //    {
    //        if (conn.State == ConnectionState.Open)
    //            conn.Close();
    //    }
    //    return retVal;
    //}

}

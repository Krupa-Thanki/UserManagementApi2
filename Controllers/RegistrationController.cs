using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using UserManagementApi.Models;

namespace UserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost]
        [Route("registration")]
        public string registration(Registration registration)
        {
            

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UserDetails").ToString());
            SqlCommand cmd = new SqlCommand("INSERT INTO registration(Username,Password,Email,Isactive,Isblock) VALUES('"+registration.Username+"','"+registration.Password + "','" +registration.Email +"',0,0)", con);
            con.Open();
            int i = cmd.ExecuteNonQuery();

            if (i > 0)
            {   
                //get userid for log
                cmd = new SqlCommand("SELECT Id FROM registration WHERE Username = '" + registration.Username + "' AND Password = '" + registration.Password + "'", con);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                int userid = Convert.ToInt16(dr["Id"].ToString());
                dr.Close();
                con.Close();

                //get date for log
                DateTime logdt = DateTime.Now;
                
                //get ip for log
                String hostname = Dns.GetHostName();
                String uip = Dns.GetHostByName(hostname).AddressList[3].ToString();

                UserLog u = new UserLog();
                u.Id = userid;
                u.ipaddress = uip;
                u.logdate = logdt.ToString();
                con.Open();
                cmd = new SqlCommand("INSERT INTO UserLog(logdate,ipaddress,Id,activity) VALUES('" + u.logdate + "','" + u.ipaddress + "'," + u.Id + ",'Register')", con);
                
                int j = cmd.ExecuteNonQuery();
                con.Close();


                return "Registration Completed. userid="+userid+" date="+logdt+" ip="+uip;
            }
            else
            {
                con.Close();
                return "Please Try Again.";
            }
            
        }


        [HttpPost]
        [Route("login")]
        public string login(Registration registration) 
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UserDetails").ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id, Isblock FROM registration WHERE Username = '" + registration.Username+ "' AND Password = '"+registration.Password+"'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();



            if (dr.HasRows)
            {
                int userid = Convert.ToInt16(dr["Id"].ToString());
                int b = Convert.ToInt16(dr["Isblock"].ToString());
                dr.Close();
                con.Close();

                if (b == 1)
                {
                    return "user blocked..";
                }
                else
                {
                    //get date for log
                    DateTime logdt = DateTime.Now;

                    //get ip for log
                    String hostname = Dns.GetHostName();
                    String uip = Dns.GetHostByName(hostname).AddressList[3].ToString();
                    UserLog u = new UserLog();
                    u.Id = userid;
                    u.ipaddress = uip;
                    u.logdate = logdt.ToString();

                    // add log detail
                    con.Open();
                    cmd = new SqlCommand("INSERT INTO UserLog(logdate,ipaddress,Id,activity) VALUES('" + u.logdate + "','" + u.ipaddress + "'," + u.Id + ",'Login')", con);
                    int j = cmd.ExecuteNonQuery();
                    con.Close();


                    //set isactive =1
                    con.Open();
                    cmd = new SqlCommand("UPDATE registration SET Isactive=1 WHERE Id=" + userid, con);
                    int k = cmd.ExecuteNonQuery();
                    con.Close();

                    return "Valid User";
                }
            }
            else
            {
                dr.Close();
                con.Close();
                return "Invalid User";
            }
            
        }


        [HttpPost]
        [Route("logout")]
        public string logout(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UserDetails").ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id FROM registration WHERE Username = '" + registration.Username + "' AND Password = '" + registration.Password + "'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();



            if (dr.HasRows)
            {
                int userid = Convert.ToInt16(dr["Id"].ToString());
                dr.Close();
                con.Close();

                //get date for log
                DateTime logdt = DateTime.Now;

                //get ip for log
                String hostname = Dns.GetHostName();
                String uip = Dns.GetHostByName(hostname).AddressList[3].ToString();
                UserLog u = new UserLog();
                u.Id = userid;
                u.ipaddress = uip;
                u.logdate = logdt.ToString();

                // add log detail
                con.Open();
                cmd = new SqlCommand("INSERT INTO UserLog(logdate,ipaddress,Id,activity) VALUES('" + u.logdate + "','" + u.ipaddress + "'," + u.Id + ",'Logout')", con);
                int j = cmd.ExecuteNonQuery();
                con.Close();


                //set isactive =0
                con.Open();
                cmd = new SqlCommand("UPDATE registration SET Isactive=0 WHERE Id=" + userid, con);
                int k = cmd.ExecuteNonQuery();
                con.Close();

                return "Logout success";
            }
            else
            {
                dr.Close();
                con.Close();
                return "Please login";
            }
        }


        [HttpPost]
        [Route("modify")]
        public string modifyUsername(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UserDetails").ToString());
            String s = " Only Username and Email allowed to Update Status :";
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id FROM registration WHERE Username = '" + registration.Username + "' AND Password = '" + registration.Password + "'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows) 
            {
                int userid = Convert.ToInt16(dr["Id"].ToString());
                dr.Close();
                con.Close();


                if (registration.newname !=null)
                { 
                
                    con.Open();
                    cmd = new SqlCommand("UPDATE registration SET Username='" + registration.newname + "' WHERE Id=" + userid, con);
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    s = s + "Update Username";
                }
                if (registration.Email != null)
                {
                    con.Open();
                    cmd = new SqlCommand("UPDATE registration SET Email='" + registration.Email + "' WHERE Id=" + userid, con);
                    int l = cmd.ExecuteNonQuery();
                    con.Close();
                    s = s + "Update Email";
                }
            }
            return s;

        }

        [HttpPost]
        [Route("changePWD")]
        public string changePWD(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UserDetails").ToString());
            String s = " Only Username and Email allowed to Update Status :";
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id FROM registration WHERE Username = '" + registration.Username + "' AND Password = '" + registration.Password + "'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                int userid = Convert.ToInt16(dr["Id"].ToString());
                dr.Close();
                con.Close();
                if (registration.newpwd != null)
                {

                    con.Open();
                    cmd = new SqlCommand("UPDATE registration SET Password='" + registration.newpwd + "' WHERE Id=" + userid, con);
                    int k = cmd.ExecuteNonQuery();
                    con.Close();
                    s = s + "Update pwd";

                }
            }
            return s;
        }

        [HttpPost]
        [Route("delete")]
        public string delete(Registration registration)
        {

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UserDetails").ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id FROM registration WHERE Username = '" + registration.Username + "' AND Password = '" + registration.Password + "'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                int userid = Convert.ToInt16(dr["Id"].ToString());
                dr.Close();


                cmd = new SqlCommand("DELETE FROM registration WHERE Id=" + userid, con);
                int k = cmd.ExecuteNonQuery();
                con.Close();

                return "User deleted successfully";
            }
            else 
            {
                dr.Close();
                con.Close();
                return "User not found"; 
            }
        }

        [HttpPost]
        [Route("block")]
        public string block(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UserDetails").ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id FROM registration WHERE Username = '" + registration.Username + "' AND Password = '" + registration.Password + "'", con);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                int userid = Convert.ToInt16(dr["Id"].ToString());
                dr.Close();
                con.Close();

                con.Open();
                cmd = new SqlCommand("UPDATE registration SET Isblock=1 WHERE Id=" + userid, con);
                int k = cmd.ExecuteNonQuery();
                con.Close();

                return "user blocked";
            }
            else
            {
                dr.Close();
                con.Close();
                return "invalid user";
            }
        }
    }
}

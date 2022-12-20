using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Data.SqlClient;


TcpListener serverSocket = new TcpListener(8888);
int requestCount = 0;
TcpClient clientSocket = default(TcpClient);
serverSocket.Start();
Console.WriteLine(" >> Server Started");
clientSocket = serverSocket.AcceptTcpClient();
Console.WriteLine(" >> Accept connection from client");
requestCount = 0;

while ((true))
{
    try
    {
        requestCount = requestCount + 1;
        NetworkStream networkStream = clientSocket.GetStream();
        byte[] bytesFrom = new byte[65536];
        networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
        string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("@"));
        Console.WriteLine(" >> Data from client - " + dataFromClient);
        int i = 0;
        string data = "";
        string serverResponse = "";
        if (dataFromClient.StartsWith("insert"))
        {
            try
            {
                SqlConnection venkatconn = new SqlConnection(@"Data Source= .\SQLEXPRESS ; Initial Catalog= socket ; Integrated Security = true");
                venkatconn.Open();
                SqlCommand cmd = new SqlCommand("sp_insert_Employee", venkatconn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter p1 = new SqlParameter("@query", SqlDbType.VarChar);
                cmd.Parameters.Add(p1).Value = dataFromClient;

                i = cmd.ExecuteNonQuery();
                venkatconn.Close();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());


            }
          
            if (i==0)
            {
                serverResponse = "Data not inserted available";
            }
            else
            {
                serverResponse = "Data inserted successfully"; ;
            }
        }
 
        if (dataFromClient.StartsWith("Fetch"))
        {
            int indexval = dataFromClient.IndexOf(" ");

            string empcodevalue = dataFromClient.Substring(indexval, dataFromClient.Length-indexval);

            try
            {
                SqlConnection venkatconn = new SqlConnection(@"Data Source= .\SQLEXPRESS ; Initial Catalog= socket ; Integrated Security = true");
                venkatconn.Open();
                SqlCommand cmd = new SqlCommand("sp_edit_tbl_Employee", venkatconn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter p1 = new SqlParameter("@empcode", SqlDbType.VarChar);
                cmd.Parameters.Add(p1).Value = empcodevalue.Trim();
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(ds);
                  data = ds.Tables[0].Rows[0][0].ToString();
                
                venkatconn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());


            }
            if (data == "")
            {
                serverResponse = "Data not  available";
            }
            else
            {
                serverResponse = data ;
            }
        }

        if(dataFromClient.StartsWith("update"))
        {
           
                try
                {
                    SqlConnection venkatconn = new SqlConnection(@"Data Source= .\SQLEXPRESS ; Initial Catalog= socket ; Integrated Security = true");
                    venkatconn.Open();
                    SqlCommand cmd = new SqlCommand("sp_update_tbl_Employee", venkatconn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter p1 = new SqlParameter("@query", SqlDbType.VarChar);
                    cmd.Parameters.Add(p1).Value = dataFromClient;

                    i = cmd.ExecuteNonQuery();
                    venkatconn.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());


                }
            if (i == 0)
            {
                serverResponse = "Data not updated";
            }
            else
            {
                serverResponse = "Data updated successfully"; ;
            }

        }

        if (dataFromClient.StartsWith("delete"))
        {

            try
            {
                SqlConnection venkatconn = new SqlConnection(@"Data Source= .\SQLEXPRESS ; Initial Catalog= socket ; Integrated Security = true");
                venkatconn.Open();
                SqlCommand cmd = new SqlCommand("sp_delete_tbl_Employee", venkatconn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter p1 = new SqlParameter("@query", SqlDbType.VarChar);
                cmd.Parameters.Add(p1).Value = dataFromClient;

                i = cmd.ExecuteNonQuery();
                venkatconn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());


            }
            if (i == 0)
            {
                serverResponse = "Data not Deleted";
            }
            else
            {
                serverResponse = "Data Deleted successfully"; ;
            }

        }

        Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
        networkStream.Write(sendBytes, 0, sendBytes.Length);
        networkStream.Flush();
        Console.WriteLine(" >> " + serverResponse);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        Console.ReadLine();
    }
}

clientSocket.Close();
serverSocket.Stop();
Console.WriteLine(" >> exit");
Console.ReadLine();
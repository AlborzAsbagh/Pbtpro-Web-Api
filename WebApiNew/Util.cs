using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Script.Serialization;

namespace WebApiNew
{
    /// <summary>
    /// Summary description for Fonksiyon
    /// </summary>
    public class Util
    {
        private string masterConStr, conStr, masterDBName, dbName;
        public bool MasterBaglantisi { get; set; }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal,
            int size, string filePath);

        private SqlConnection baglanti, masterBaglanti;

        public Util()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #region KodGrupları
        // Kod Grupları
        public string arizaTipleri = "32401";
        public string bakimTipleri = "32440";
        public string isEmriDurum = "32801";
        public string malzemeBirim = "13001";
        public string olcuBirim = "32001";
        public string malzemeTip = "13005";
        public string vardiyaTanimlari = "32759";

        #endregion
        #region Sabitler
        public static string isTalebi = "isTalebi";
        public static string isEmri = "isEmri";
        public static string malzemeTalep = "malzemeTalep";
        #endregion

        public static string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                255, HostingEnvironment.MapPath("~/Baglanti.ini"));
            return temp.ToString();

        }
        private void InitConnectionStrs()
        {
            string user, pass, server, catalog, catalogMaster;

            byte[] data = Convert.FromBase64String(IniReadValue("Server", "s"));
            user = Encoding.UTF8.GetString(data);
            data = Convert.FromBase64String(IniReadValue("Server", "a"));
            pass = Encoding.UTF8.GetString(data);
            data = Convert.FromBase64String(IniReadValue("Server", "e"));
            server = Encoding.UTF8.GetString(data);
            string b, t, i, k;
            data = Convert.FromBase64String(IniReadValue("Server", "b"));
            catalog = Encoding.UTF8.GetString(data);
            data = Convert.FromBase64String(IniReadValue("Server", "t"));
            t = Encoding.UTF8.GetString(data);
            data = Convert.FromBase64String(IniReadValue("Server", "i"));
            i = Encoding.UTF8.GetString(data);
            data = Convert.FromBase64String(IniReadValue("Server", "k"));
            catalogMaster = Encoding.UTF8.GetString(data);
            conStr = "Data Source=" + server.ToString() + ";Initial Catalog=" + catalog + ";Persist Security Info=True;User ID=" + user.ToString() + ";Password=" + pass.ToString();
            masterConStr = "Data Source=" + server.ToString() + ";Initial Catalog=" + catalogMaster + ";Persist Security Info=True;User ID=" + user.ToString() + ";Password=" + pass.ToString();
            if (t == "auWindows")
            {
                conStr = "Server= " + server + "; Database= " +  catalog + ";Trusted_Connection=true";
                masterConStr = "Server= " + server + "; Database= " +  catalogMaster + ";Trusted_Connection=true";
            }
            dbName = catalog;
            masterDBName = catalogMaster;

        }


        public SqlConnection baglan()
        {
            if (conStr == null || masterConStr == null)
                InitConnectionStrs();
            if (MasterBaglantisi)
            {
                if (masterBaglanti != null && masterBaglanti.State == ConnectionState.Open)
                    kapat();
                masterBaglanti = new SqlConnection(masterConStr);
                masterBaglanti.Open();
                return (masterBaglanti);
            }


            if (baglanti != null && baglanti.State == ConnectionState.Open)
                kapat();
            baglanti = new SqlConnection(conStr);
            baglanti.Open();
            return (baglanti);
        }

        public SqlConnection baglanCmd()
        {
            SqlConnection con = null;
            if (conStr == null || masterConStr == null)
                InitConnectionStrs();
            con = new SqlConnection(MasterBaglantisi ? masterConStr : conStr);
            con.Open();
            return (con);
        }
        public void kapat()
        {
            if (masterBaglanti != null)
            {
                masterBaglanti.Close();
                masterBaglanti.Dispose();
            }
            if (baglanti != null)
            {
                baglanti.Close();
                baglanti.Dispose();
            }
        }

        public static void SendNotificationToTopic(string _kod, string _baslik, string _body, string _refGrup, string[] _device)
        {
            try
            {
                string applicationID = "AAAAzHRMy90:APA91bHVz9OCtpSHBtAyzYqvmnU9HH03H8WYnZK_csN9SBnINpyCorM-XclZ7vD5MZcZnU7LEVPOkHHNuzkQQlfKsuX0_Mo3ePv9eDMtFh-7zkCdcYEDUwuBFiVFJDF14REUs4qc7tzG";
                string senderId = "878124518365";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    //  to = "/topics/" + _topic,
                    registration_ids = _device,
                    notification = new
                    {
                        body = _body,
                        title = _baslik,
                        sound = "default",
                        click_action = "net.orjin.omega.MAIN_ACTIVITY"
                    },
                    data = new
                    {
                        id = _kod,
                        grup = _refGrup
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;

            }
        }

        public int cmd(string sqlcumle, List<Prm> paramsList)
        {
            int sonuc = 0;

            using (SqlConnection baglan = this.baglanCmd())
            {
                using (SqlCommand sorgu = new SqlCommand(sqlcumle, baglan))
                {
                    foreach (Prm parameter in paramsList)
                    {
                        if (parameter.ParametreDeger == null)
                        {
                            sorgu.Parameters.Add(parameter.ParametreAdi, SqlDbType.NVarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            switch (parameter.ParametreTip)
                            {
                                case SqlDbType.Bit:
                                    sorgu.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToBoolean(parameter.ParametreDeger) ? 1 : 0;
                                    break;
                                case SqlDbType.NVarChar:
                                    sorgu.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToString(parameter.ParametreDeger);
                                    break;
                                default:
                                    sorgu.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = parameter.ParametreDeger;
                                    break;

                            }
                        }
                    }
                    try
                    {
                        sonuc = sorgu.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        sorgu.Dispose();
                        baglan.Close();
                        baglan.Dispose();
                        kapat();
                        throw;
                    }
                    finally
                    {
                        sorgu.Dispose();
                        baglan.Close();
                        baglan.Dispose();
                        kapat();

                    }
                }

            }
            return (sonuc);
        }  

        public DataTable GetDataTable(string sql, List<Prm> paramsList)
        {

            DataTable dt = new DataTable();
            using (SqlConnection baglanti = this.baglan())
            {
                using (SqlCommand kmt = new SqlCommand(sql, baglanti))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    foreach (Prm parameter in paramsList)
                    {
                        if (parameter.ParametreDeger == null)
                        {
                            kmt.Parameters.Add(parameter.ParametreAdi, SqlDbType.NVarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            switch (parameter.ParametreTip)
                            {
                                case SqlDbType.Bit:
                                    kmt.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToBoolean(parameter.ParametreDeger) ? 1 : 0;
                                    break;
                                case SqlDbType.NVarChar:
                                    kmt.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToString(parameter.ParametreDeger);
                                    break;
                                default:
                                    kmt.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = parameter.ParametreDeger;
                                    break;
                            }
                        }
                    }
                    try
                    {

                        adapter.SelectCommand = kmt;
                        adapter.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        adapter.Dispose();
                        baglanti.Close();
                        kapat();
                        kmt.Dispose();
                        throw;
                    }
                    finally
                    {
                        kmt.Dispose();
                        adapter.Dispose();
                        kapat();
                        baglanti.Close();
                    }
                }
            }
            return dt;
        }

        public DataSet GetDataSet(string sql, List<Prm> paramsList)
        {

            DataSet dt = new DataSet();
            using (SqlConnection baglanti = this.baglan())
            {
                using (SqlCommand kmt = new SqlCommand(sql, baglanti))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    foreach (Prm parameter in paramsList)
                    {
                        if (parameter.ParametreDeger == null)
                        {
                            kmt.Parameters.Add(parameter.ParametreAdi, SqlDbType.NVarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            switch (parameter.ParametreTip)
                            {
                                case SqlDbType.Bit:
                                    kmt.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToBoolean(parameter.ParametreDeger) ? 1 : 0;
                                    break;
                                case SqlDbType.NVarChar:
                                    kmt.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToString(parameter.ParametreDeger);
                                    break;
                                default:
                                    kmt.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = parameter.ParametreDeger;
                                    break;
                            }
                        }
                    }
                    try
                    {

                        adapter.SelectCommand = kmt;
                        adapter.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        adapter.Dispose();
                        baglanti.Close();
                        kapat();
                        kmt.Dispose();
                        throw;
                    }
                    finally
                    {
                        kmt.Dispose();
                        adapter.Dispose();
                        kapat();
                        baglanti.Close();
                    }
                }
            }
            return dt;
        }

        public DataTable GetDataTable(SqlCommand sqlCommand, List<Prm> paramsList)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();

            foreach (Prm parameter in paramsList)
            {
                if (parameter.ParametreDeger == null)
                {
                    sqlCommand.Parameters.Add(parameter.ParametreAdi, SqlDbType.NVarChar).Value = DBNull.Value;
                }
                else
                {
                    switch (parameter.ParametreTip)
                    {
                        case SqlDbType.Bit:
                            sqlCommand.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToBoolean(parameter.ParametreDeger) ? 1 : 0;
                            break;
                        case SqlDbType.NVarChar:
                            sqlCommand.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToString(parameter.ParametreDeger);
                            break;
                        default:
                            sqlCommand.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = parameter.ParametreDeger;
                            break;

                    }
                }
            }
            try
            {
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                adapter.Dispose();
                kapat();
                throw;
            }
            finally
            {
                adapter.Dispose();
                kapat();
            }
            return dt;
        }

        public DataTable GetDataTableDontClose(SqlCommand sqlCommand, List<Prm> paramsList)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();

            foreach (Prm parameter in paramsList)
            {
                if (parameter.ParametreDeger == null)
                {
                    sqlCommand.Parameters.Add(parameter.ParametreAdi, SqlDbType.NVarChar).Value = DBNull.Value;
                }
                else
                {
                    switch (parameter.ParametreTip)
                    {
                        case SqlDbType.Bit:
                            sqlCommand.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToBoolean(parameter.ParametreDeger) ? 1 : 0;
                            break;
                        case SqlDbType.NVarChar:
                            sqlCommand.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = Convert.ToString(parameter.ParametreDeger);
                            break;
                        default:
                            sqlCommand.Parameters.Add(parameter.ParametreAdi, parameter.ParametreTip).Value = parameter.ParametreDeger;
                            break;

                    }
                }
            }
            try
            {
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                adapter.Dispose();
                throw;
            }
            finally
            {
                adapter.Dispose();
            }
            return dt;
        }

        public DataRow GetDataRow(string sql, List<Prm> paramsList)
        {
            DataTable table = GetDataTable(sql, paramsList);
            if (table.Rows.Count == 0) return null;
            return table.Rows[0];
        }

        public DataRow GetDataRow(SqlCommand sqlCommand, List<Prm> paramsList)
        {
            DataTable table = GetDataTable(sqlCommand, paramsList);
            if (table.Rows.Count == 0) return null;
            return table.Rows[0];
        }

        public DataRow GetDataRowDontClose(SqlCommand sqlCommand, List<Prm> paramsList)
        {
            DataTable table = GetDataTableDontClose(sqlCommand, paramsList);

            if (table.Rows.Count == 0) return null;
            return table.Rows[0];
        }
        public string GetDataCell(string sql, List<Prm> paramsList)
        {
            DataTable table = GetDataTable(sql, paramsList);
            if (table.Rows.Count == 0) return null;
            try
            {
                return table.Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static int getFieldInt(DataRow dr, String fieldName)
        {
            try
            {
                return Convert.ToInt32(dr[fieldName]);
            }
            catch
            {
                return -1;
            }
        }
        public static double getFieldDouble(DataRow dr, String fieldName)
        {
            try
            {
                return Convert.ToDouble(dr[fieldName]);
            }
            catch
            {
                return 0;
            }
        }
        public static string getFieldString(DataRow dr, String fieldName)
        {
            try
            {
                return dr[fieldName].ToString();
            }
            catch
            {
                return "";
            }
        }
        public static bool getFieldBool(DataRow dr, String fieldName)
        {
            try
            {
                return Convert.ToBoolean(dr[fieldName]);
            }
            catch
            {
                return false;
            }
        }
        public static DateTime? getFieldDateTime(DataRow dr, String fieldName)
        {
            try
            {
                return Convert.ToDateTime(dr[fieldName]);
            }
            catch
            {
                return null;
            }
        }
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }

        private void InitDbNames()
        {
            byte[] data = Convert.FromBase64String(IniReadValue("Server", "b"));
            dbName = Encoding.UTF8.GetString(data);
            data = Convert.FromBase64String(IniReadValue("Server", "k"));
            masterDBName = Encoding.UTF8.GetString(data);

        }
        public string GetDbName()
        {
            if (String.IsNullOrEmpty(dbName) || String.IsNullOrEmpty(masterDBName))
                InitDbNames();
            return dbName;
        }

        public string GetMasterDbName()
        {

            if (String.IsNullOrEmpty(dbName) || String.IsNullOrEmpty(masterDBName))
                InitDbNames();
            return masterDBName;
        }
        public string GetConnectionString()
        {
            if (String.IsNullOrEmpty(conStr))
                InitConnectionStrs();
            return conStr;
        }


        public static string RemoveRtfFormatting(string rtfContent)
        {
            if (string.IsNullOrWhiteSpace(rtfContent)) 
                return "";

            rtfContent = rtfContent.Trim();


            Regex rtfRegEx = new Regex("({\\\\)(.+?)(})|(\\\\)(.+?)(\\b)",
                RegexOptions.IgnoreCase
                | RegexOptions.Multiline
                | RegexOptions.Singleline
                | RegexOptions.ExplicitCapture
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
            );
            string output = rtfRegEx.Replace(rtfContent, string.Empty);
            output = Regex.Replace(output, @"\}", string.Empty); //replacing the remaining braces


            return output; //to trim last char (line end)


        }

        public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(
                    Path.Combine(
                        dirPath,
                        Path.GetRandomFileName()
                    ),
                    1,
                    FileOptions.DeleteOnClose)
                )
                { }
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }

    }
}


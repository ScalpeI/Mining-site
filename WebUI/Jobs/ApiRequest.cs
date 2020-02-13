using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Domain.Model;
using System.Data.SqlClient;

namespace WebUI.Jobs
{
    public class ApiRequest : IJob
    {
        private bool up=false;

        public async Task Execute(IJobExecutionContext context)
        {
            if ((int.Parse(DateTime.Now.Minute.ToString().Substring(DateTime.Now.Minute.ToString().Length - 1, 1)) == 0 && up) || (int.Parse(DateTime.Now.Minute.ToString().Substring(DateTime.Now.Minute.ToString().Length - 1, 1)) == 5 && up))
            {
                up = false;
                RequestMrr req = new RequestMrr();
                RequestMinear reqm = new RequestMinear();
                RequestSp sp = new RequestSp();
                RequestRate rate = new RequestRate();
                req.Upload();
                sp.Upload();
                rate.Upload();
                reqm.Upload();
                DeleteDouble();
            } else if ((int.Parse(DateTime.Now.Minute.ToString().Substring(DateTime.Now.Minute.ToString().Length - 1, 1)) != 0 && !up) && (int.Parse(DateTime.Now.Minute.ToString().Substring(DateTime.Now.Minute.ToString().Length - 1, 1)) != 5))
            {
                up = true;
            }
        }
        public void DeleteDouble()
        {
            using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EFDbContext"].ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("DDoubleMrr", connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                // добавляем параметр
                command.ExecuteNonQuery();                
                // если нам не надо возвращать id
                //var result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
using Domain.Abstract;
using Domain.Entities;
using Newtonsoft.Json.Linq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class RatesController : Controller
    {
        private IRateRepository repository;
        private IMinearRepository minearRepository;
        private IConstRepository constRepository;
        private IMrrRepository mrrrepository;
        private IBtcRepository btcrepository;
        private ISpRepository sprepository;
        public RatesController(IRateRepository repo, IMinearRepository minr, IConstRepository @const, IMrrRepository mrr,IBtcRepository btc , ISpRepository sp)
        {
            repository = repo;
            minearRepository = minr;
            constRepository = @const;
            mrrrepository = mrr;
            btcrepository = btc;
            sprepository = sp;
        }
        public ViewResult Index()
        {
            using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EFDbContext"].ConnectionString))
            {
                connection.Open();
                SqlCommand command1 = new SqlCommand("sumhashrate", connection);
                // указываем, что команда представляет хранимую процедуру
                command1.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                // добавляем параметр
                ViewBag.hashrr = string.Format("{0:N}", command1.ExecuteScalar());
                connection.Close();
            }
            using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EFDbContext"].ConnectionString))
            {
                connection.Open();
                SqlCommand command1 = new SqlCommand("BSD", connection);
                // указываем, что команда представляет хранимую процедуру
                command1.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@username",
                    Value = User.Identity.Name
                };
                // добавляем параметр
                command1.Parameters.Add(nameParam);
                ViewBag.balanced = string.Format("{0:N}", command1.ExecuteScalar());
                connection.Close();
            }
            using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EFDbContext"].ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("BSB", connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@username",
                    Value = User.Identity.Name
                };
                // добавляем параметр
                command.Parameters.Add(nameParam);

                ViewBag.balance = string.Format("{0:F8}", command.ExecuteScalar());
                // если нам не надо возвращать id
                //var result = command.ExecuteNonQuery();
                connection.Close();
            }
            var mrr = mrrrepository.Mrrs.Where(w => w.owner == User.Identity.Name).OrderBy(w => w.date).GroupBy(w => w.date).Select(x => new { date = x.Min(w => w.date), sum = Math.Round(x.Sum(w => w.hash), 4) });
            var btc = btcrepository.Btcs.Where(w => w.owner == User.Identity.Name).OrderBy(w => w.date).GroupBy(w => w.date).Select(x => new { date = x.Min(w => w.date), sum = Math.Round(x.Sum(w => w.hash), 4) });
            var sp = sprepository.Sps.Where(w => w.owner == User.Identity.Name).OrderBy(w => w.date).GroupBy(w => w.date).Select(x => new { date = x.Min(w => w.date), sum = Math.Round(x.Sum(w => w.hash), 4) });
            var res = mrr.ToList().Concat(btc.ToList().Concat(sp.ToList()));
            ViewBag.hash = res.OrderByDescending(w => w.date).GroupBy(w => w.date).Select(x => Math.Round(x.Sum(w => w.sum), 4)).FirstOrDefault();
            ViewBag.Minear = string.Format("{0:F8}", constRepository.Consts.Select(x => x.coef).FirstOrDefault() * double.Parse(minearRepository.Minears.OrderBy(x => x.date).Select(x => x.fpps_mining_earnings).LastOrDefault(), CultureInfo.InvariantCulture));
            return View(repository.Rates.Select(x => x.last).FirstOrDefault());
        }
        public ViewResult HashList()
        {
            List<string> date = new List<string>();
            List<string> sum = new List<string>();
            List<DateTime> dts = new List<DateTime>();
            using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["EFDbContext"].ConnectionString))
            {
                connection.Open();
                SqlCommand command1 = new SqlCommand("sumhashratelist", connection);
                // указываем, что команда представляет хранимую процедуру
                command1.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                // добавляем параметр//
                using (SqlDataReader dr = command1.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        date.Add(dr.GetDateTime(dr.GetOrdinal("date")).ToString());
                        dts.Add(dr.GetDateTime(dr.GetOrdinal("date")));
                        sum.Add(dr.GetSqlDouble(dr.GetOrdinal("sum")).ToString());
                    }

                }
                connection.Close();
            }
            ViewBag.sum = sum.Take(18).ToList();
            ViewBag.date = date.Take(18).ToList();
            string dte = "";
            int i = 0;
            sum = sum.Take(30).Reverse().ToList();
            foreach (var dt in dts.Take(30).Reverse())
            {
                dte += "[Date.UTC("
                + dt.Year.ToString() + ","
                + dt.Month.ToString() + ","
                + dt.Day.ToString() + ","
                + dt.Hour.ToString() + ","
                + dt.Minute.ToString() + ","
                + dt.Second.ToString() + "),"
                + String.Format("{0:N}", sum.ElementAt(i).ToString()) + "],";
                i++;
            }
            dte = dte.Remove(dte.Length - 1, 1);
            ViewBag.count = 18;
            ViewBag.script = @"<script>Highcharts.chart('container', {chart: { type: 'spline'},title: {text: ''}, xAxis: {type: 'datetime',},yAxis: {title: {text: 'HashRate (TH/s)'},labels: {format: '{value}'}},tooltip: {headerFormat: '<b>{series.name}</b><br/>'},plotOptions: {series: { marker: {enabled: true }  } }, series: [{ name: 'HashRate', data: [" + dte + @"]}]});</script>";
            return View();
        }
        public ViewResult List()
        {
            int t = repository.Rates.Select(x => x.timestamp).FirstOrDefault();
            ViewBag.timestamp = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(t);
            return View(repository.Rates);
        }
        public ViewResult Minear()
        {
            return View(minearRepository.Minears.OrderByDescending(x => x.date).Take(30));
        }
    }
}

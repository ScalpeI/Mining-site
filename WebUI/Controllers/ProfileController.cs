using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.DataVisualization.Charting;
using Domain.Abstract;
using Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebUI.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        IUserRepository userrepository;
        IRateRepository raterepository;
        IMrrRepository mrrrepository;
        IBtcRepository btcrepository;
        ISpRepository sprepository;
        IMinearRepository minearRepository;
        IConstRepository constRepository;
        IPayoutRepository payoutRepository;
        public bool check()
        {
            if (payoutRepository.Payouts.Where(p => p.owner == User.Identity.Name).OrderBy(p => p.date).FirstOrDefault() != null) return true; else return false;
        }
        public ProfileController(IUserRepository repo,
            IRateRepository rate,
            IMrrRepository mrr,
            IBtcRepository btc,
            ISpRepository sp,
            IMinearRepository minear,
            IConstRepository @const,
            IPayoutRepository payout)
        {
            userrepository = repo;
            raterepository = rate;
            mrrrepository = mrr;
            btcrepository = btc;
            sprepository = sp;
            minearRepository = minear;
            constRepository = @const;
            payoutRepository = payout;
        }
        public ViewResult ProcessRequest()
        {
            return View();
        }
        public void ExportToExcel()
        {
            string Filename = String.Format("Payouts_{0}_{1}.xls",DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss"), User.Identity.Name);
            string FolderPath = HttpContext.Server.MapPath("/ExcelFiles/");
            string FilePath = Path.Combine(FolderPath, Filename);
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.File.Delete(FilePath);
            }

            string HtmlResult = RenderRazorViewToString("~/Views/Profile/GenerateExcel.cshtml", payoutRepository.Payouts.Where(p => p.owner == User.Identity.Name).OrderBy(p => p.date).ToList());

            byte[] ExcelBytes = Encoding.ASCII.GetBytes(HtmlResult);

            using (Stream file = System.IO.File.OpenWrite(FilePath))
            {
                file.Write(ExcelBytes, 0, ExcelBytes.Length);
            }

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(Filename));
            Response.WriteFile(FilePath);
            Response.End();
            Response.Flush();
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.File.Delete(FilePath);
            }
        }
        protected string RenderRazorViewToString(string viewName, object model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ViewResult Main()
        {
            ViewBag.check = check();
            User user = userrepository.Users.FirstOrDefault(r => r.Login == User.Identity.Name);
            ViewBag.Header = User.Identity.Name + " Profile";
            ViewBag.SubHeader = "Info";
            return View(user);
        }
        public ViewResult Payout()
        {
            ViewBag.Header = User.Identity.Name + " Profile";
            ViewBag.SubHeader = "Payouts";
            List<Payout> payout = payoutRepository.Payouts.Where(p => p.owner == User.Identity.Name).OrderBy(p => p.date).ToList();
            return View(payout);
        }

        public ViewResult Balance()
        {
            ViewBag.Header = User.Identity.Name + " Profile";
            ViewBag.SubHeader = "Balance";
            ViewBag.check = check();
            //присвоение рейта и стоимости
            double cconst = constRepository.Consts.FirstOrDefault().coef;
            double rate = raterepository.Rates.Select(q => q.open).FirstOrDefault();
            string minear = minearRepository.Minears.Select(m => m.fpps_mining_earnings).LastOrDefault();
            ViewBag.fminear = (double.Parse(minear) * cconst).ToString("F8");
            User user = userrepository.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            ViewBag.payout = payoutRepository.Payouts.Where(p => p.owner == User.Identity.Name).OrderBy(p => p.date).ToList();
            //начало дневной хеш
            //mrr
            int dcountmrr = 0;
            IEnumerable<double> dhashmrr = mrrrepository.Mrrs.Where(w => (w.owner == User.Identity.Name && w.date >= DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)))).GroupBy(w => w.date).Select(x => x.Sum(w => w.hash));
            double dhashmrrsum = 0;
            foreach (double hashmrronce in dhashmrr)
            {
                dhashmrrsum += hashmrronce;
                dcountmrr++;
            }
            //btc
            int dcountbtc = 0;
            IEnumerable<double> dhashbtc = btcrepository.Btcs.Where(w => (w.owner == User.Identity.Name && w.date >= DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)))).GroupBy(w => w.date).Select(x => x.Sum(w => w.hash));
            double dhashbtcsum = 0;
            foreach (double hashbtconce in dhashbtc)
            {
                dhashbtcsum += hashbtconce;
                dcountbtc++;
            }
            //sp
            int dcountsp = 0;
            IEnumerable<double> dhashsp = sprepository.Sps.Where(w => (w.owner == User.Identity.Name && w.date >= DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)))).GroupBy(w => w.date).Select(x => x.Sum(w => w.hash));
            double dhashspsum = 0;
            foreach (double hashsponce in dhashsp)
            {
                dhashspsum += hashsponce;
                dcountsp++;
            }
            if (dcountmrr == 0) dcountmrr = 1;
            if (dcountbtc == 0) dcountbtc = 1;
            if (dcountsp == 0) dcountsp = 1;
            double dayhash = (dhashmrrsum / 288) + (dhashbtcsum / 288) + (dhashspsum / 288);
            double balance1 = (dayhash * double.Parse(minear, CultureInfo.InvariantCulture)) * cconst;
            double balanced = ((dayhash * double.Parse(minear, CultureInfo.InvariantCulture)) * cconst) * rate;
            ViewBag.dayhash = dayhash.ToString("N");
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
                object balance = command.ExecuteScalar();
                ViewBag.balance = string.Format("{0:F8}", balance);
                ViewBag.futureBalance = string.Format("{0:F8}",balance1);
                // если нам не надо возвращать id
                //var result = command.ExecuteNonQuery();
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

            //ViewBag.balance = balance.ToString("F8");
            //ViewBag.balanced = balanced.ToString("N");
            //конец дневной хеш

            //начало часовой хеш
            //mrr
            IEnumerable<double> hhashmrr = mrrrepository.Mrrs.Where(w => (w.owner == User.Identity.Name && w.date >= DateTime.Now.ToUniversalTime().Subtract(new TimeSpan(0, 1, 0, 0)))).GroupBy(w => w.date).Select(x => x.Sum(w => w.hash));
            double hhashmrrsum = 0;
            foreach (double hashmrronce in hhashmrr)
            {
                hhashmrrsum += hashmrronce;
            }
            //btc
            IEnumerable<double> hhashbtc = btcrepository.Btcs.Where(w => (w.owner == User.Identity.Name && w.date >= DateTime.Now.ToUniversalTime().Subtract(new TimeSpan(0, 1, 0, 0)))).GroupBy(w => w.date).Select(x => x.Sum(w => w.hash));
            double hhashbtcsum = 0;
            foreach (double hashbtconce in hhashbtc)
            {
                hhashbtcsum += hashbtconce;
            }
            //sp
            IEnumerable<double> hhashsp = sprepository.Sps.Where(w => (w.owner == User.Identity.Name && w.date >= DateTime.Now.ToUniversalTime().Subtract(new TimeSpan(0, 1, 0, 0)))).GroupBy(w => w.date).Select(x => x.Sum(w => w.hash));
            double hhashspsum = 0;
            foreach (double hashsponce in hhashsp)
            {
                hhashspsum += hashsponce;
            }
            double hourhash = (hhashmrrsum / 12) + (hhashbtcsum / 12) + (hhashspsum / 12);
            ViewBag.hourhash = hourhash.ToString("N");
            //конец дневной хеш

            //начало за всё время
            //mrr
            int countmrr = 0;
            IEnumerable<double> hashmrr = mrrrepository.Mrrs.Where(w => w.owner == User.Identity.Name).OrderByDescending(w => w.date).GroupBy(w => w.date).Select(x => x.Sum(w => w.hash));
            double hashmrrsum = 0;
            foreach (double hashmrronce in hashmrr)
            {
                hashmrrsum += hashmrronce;
                countmrr++;
            }
            //btc
            int countbtc = 0;
            IEnumerable<double> hashbtc = btcrepository.Btcs.Where(w => w.owner == User.Identity.Name).OrderByDescending(w => w.date).GroupBy(w => w.date).Select(x => x.Sum(w => w.hash));
            double hashbtcsum = 0;
            foreach (double hashbtconce in hashbtc)
            {
                hashbtcsum += hashbtconce;
                countbtc++;
            }
            //sp
            int countsp = 0;
            IEnumerable<double> hashsp = sprepository.Sps.Where(w => w.owner == User.Identity.Name).OrderByDescending(w => w.date).GroupBy(w => w.date).Select(x => x.Sum(w => w.hash));
            double hashspsum = 0;
            foreach (double hashsponce in hashsp)
            {
                hashspsum += hashsponce;
                countsp++;
            }
            if (countmrr == 0) countmrr = 1;
            if (countbtc == 0) countbtc = 1;
            if (countsp == 0) countsp = 1;
            double hash = (hashmrrsum / countmrr) + (hashbtcsum / countbtc) + (hashspsum / countsp);
            ViewBag.hash = hash.ToString("N");
            //конец за всё время

            // начало график
            var mrr = mrrrepository.Mrrs.Where(w => w.owner == User.Identity.Name).OrderBy(w => w.date).GroupBy(w => w.date).Select(x => new { date = x.Min(w => (w.date).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds), sum = Math.Round(x.Sum(w => w.hash), 4) });
            var btc = btcrepository.Btcs.Where(w => w.owner == User.Identity.Name).OrderBy(w => w.date).GroupBy(w => w.date).Select(x => new { date = x.Min(w => (w.date).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds), sum = Math.Round(x.Sum(w => w.hash), 4) });
            var sp = sprepository.Sps.Where(w => w.owner == User.Identity.Name).OrderBy(w => w.date).GroupBy(w => w.date).Select(x => new { date = x.Min(w => (w.date).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds), sum = Math.Round(x.Sum(w => w.hash), 4) });
            var res = mrr.ToList().Concat(btc.ToList().Concat(sp.ToList()));
            var result = res.OrderBy(w => w.date).GroupBy(w => w.date).Select(x => new { date = x.Min(w => w.date), sum = Math.Round(x.Sum(w => w.sum), 4) });

            var mrrdate = mrrrepository.Mrrs.Where(w => w.owner == User.Identity.Name).OrderBy(w => (w.date).ToShortDateString()).GroupBy(w => (w.date).ToShortDateString()).Select(x => new { date = x.Min(w => DateTime.Parse((w.date).ToShortDateString()).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds), sum = Math.Round(x.Sum(w => w.hash) / 288, 4) });
            var btcdate = btcrepository.Btcs.Where(w => w.owner == User.Identity.Name).OrderBy(w => (w.date).ToShortDateString()).GroupBy(w => (w.date).ToShortDateString()).Select(x => new { date = x.Min(w => DateTime.Parse((w.date).ToShortDateString()).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds), sum = Math.Round(x.Sum(w => w.hash) / 288, 4) });
            var spdate = sprepository.Sps.Where(w => w.owner == User.Identity.Name).OrderBy(w => (w.date).ToShortDateString()).GroupBy(w => (w.date).ToShortDateString()).Select(x => new { date = x.Min(w => DateTime.Parse((w.date).ToShortDateString()).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds), sum = Math.Round(x.Sum(w => w.hash) / 288, 4) });
            var resdate = mrrdate.ToList().Concat(btcdate.ToList().Concat(spdate.ToList()));
            var resultdate = resdate.OrderBy(w => w.date).GroupBy(w => w.date).Select(x => new { date = x.Min(w => w.date), sum = Math.Round(x.Sum(w => w.sum), 4) });

            //var result = from m in mrr
            //             join b in btc on m.date equals b.date
            //             //join s in sp on m.date equals s.date
            //             select new
            //             {
            //                 date = m.date,
            //                 sum = m.sum + b.sum,
            //             };

            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Serialize(result);
            //serializedResult = serializedResult.Replace("\"date\":", "").Replace("\"sum\":", "").Replace("{", "[").Replace("}", "]");
            //using (StreamWriter file = new StreamWriter(Server.MapPath("~/content/json/") + User.Identity.Name + ".json", false, System.Text.Encoding.Default))
            //{
            //    file.Write(serializedResult);
            //}

            var serializerdate = new JavaScriptSerializer();
            var serializedResultdate = serializerdate.Serialize(resultdate);
            //serializedResultdate = serializedResultdate.Replace("\"date\":", "").Replace("\"sum\":", "").Replace("{", "[").Replace("}", "]");
            //using (StreamWriter file = new StreamWriter(Server.MapPath("~/content/json/") + User.Identity.Name + "date.json", false, System.Text.Encoding.Default))
            //{
            //    file.Write(serializedResultdate);
            //}
            ViewBag.scriptdate = "<script> var chart = am4core.useTheme(am4themes_animated);" +
                "var chart = am4core.create(\"chartdivdate\", am4charts.XYChart);" +
                "chart.data = " + serializedResultdate + ";" +
                "chart.dateFormatter.inputDateFormat = \"yyyy-MM-dd hh:mm:ss\";" +
                "var dateAxis = chart.xAxes.push(new am4charts.DateAxis());" +
                "var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());" +
                "var series = chart.series.push(new am4charts.LineSeries());" +
                "series.dataFields.valueY = \"sum\";" +
                "series.dataFields.dateX = \"date\";" +
                "series.tooltipText = \"{date.formatDate('dd MMMM yyyy HH:mm')} : {sum} TH/s\";" +
                "series.strokeWidth = 2;" +
                "series.minBulletDistance = 15;" +
                "series.tooltip.background.cornerRadius = 20;" +
                "series.tooltip.background.strokeOpacity = 0;" +
                "series.tooltip.pointerOrientation = \"vertical\";" +
                "series.tooltip.label.minWidth = 40;" +
                "series.tooltip.label.minHeight = 40;" +
                "series.tooltip.label.textAlign = \"middle\";" +
                "series.tooltip.label.textValign = \"middle\";" +
                "var bullet = series.bullets.push(new am4charts.CircleBullet());" +
                "bullet.circle.strokeWidth = 2;" +
                "bullet.circle.radius = 4;" +
                "bullet.circle.fill = am4core.color(\"#fff\");" +
                "var bullethover = bullet.states.create(\"hover\");" +
                "bullethover.properties.scale = 1.3;" +
                "chart.cursor = new am4charts.XYCursor();" +
                "chart.cursor.behavior = \"panXY\";" +
                "chart.cursor.xAxis = dateAxis;" +
                "chart.cursor.snapToSeries = series;" +
                "chart.scrollbarY = new am4core.Scrollbar();" +
                "chart.scrollbarY.parent = chart.leftAxesContainer;" +
                "chart.scrollbarY.toBack();" +
                "chart.scrollbarX = new am4charts.XYChartScrollbar();" +
                "chart.scrollbarX.series.push(series);" +
                "chart.scrollbarX.parent = chart.bottomAxesContainer;" +
                "dateAxis.start = 0.79;" +
                "dateAxis.keepSelection = true;" +
                "</script>";

            ViewBag.script = "<script> var chart = am4core.useTheme(am4themes_animated);" +
                "var chart = am4core.create(\"chartdiv\", am4charts.XYChart);" +
                "chart.data = " + serializedResult + ";" +
                "chart.dateFormatter.inputDateFormat = \"yyyy-MM-dd hh:mm:ss\";" +
                "var dateAxis = chart.xAxes.push(new am4charts.DateAxis());" +
                "var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());" +
                "var series = chart.series.push(new am4charts.LineSeries());" +
                "series.dataFields.valueY = \"sum\";" +
                "series.dataFields.dateX = \"date\";" +
                "series.tooltipText = \"{date.formatDate('dd MMMM yyyy HH:mm')} : {sum} TH/s\";" +
                "series.strokeWidth = 2;" +
                "series.minBulletDistance = 15;" +
                "series.tooltip.background.cornerRadius = 20;" +
                "series.tooltip.background.strokeOpacity = 0;" +
                "series.tooltip.pointerOrientation = \"vertical\";" +
                "series.tooltip.label.minWidth = 40;" +
                "series.tooltip.label.minHeight = 40;" +
                "series.tooltip.label.textAlign = \"middle\";" +
                "series.tooltip.label.textValign = \"middle\";" +
                "var bullet = series.bullets.push(new am4charts.CircleBullet());" +
                "bullet.circle.strokeWidth = 2;" +
                "bullet.circle.radius = 4;" +
                "bullet.circle.fill = am4core.color(\"#fff\");" +
                "var bullethover = bullet.states.create(\"hover\");" +
                "bullethover.properties.scale = 1.3;" +
                "chart.cursor = new am4charts.XYCursor();" +
                "chart.cursor.behavior = \"panXY\";" +
                "chart.cursor.xAxis = dateAxis;" +
                "chart.cursor.snapToSeries = series;" +
                "chart.scrollbarY = new am4core.Scrollbar();" +
                "chart.scrollbarY.parent = chart.leftAxesContainer;" +
                "chart.scrollbarY.toBack();" +
                "chart.scrollbarX = new am4charts.XYChartScrollbar();" +
                "chart.scrollbarX.series.push(series);" +
                "chart.scrollbarX.parent = chart.bottomAxesContainer;" +
                "dateAxis.start = 0.79;" +
                "dateAxis.keepSelection = true;" +
                "</script>";

            //ViewBag.script = @"<script>
            //                        $.getJSON(
            //                'http://mining-operator.org.uk/content/json/" + User.Identity.Name + ".json" + @"',
            //                function(data1) {

            //                            Highcharts.chart('container', {
            //                            chart:
            //                                {type: 'spline',
            //                                zoomType: 'x'
            //                            },
            //                        title:
            //                                {
            //                                text: 'Hashrate over time'
            //                        },
            //                        subtitle:
            //                                {
            //                                text: document.ontouchstart === undefined ?
            //                                    'Click and drag in the plot area to zoom in' : 'Pinch the chart to zoom in'
            //                        },
            //                        xAxis:
            //                                {
            //                                type: 'datetime'
            //                        },
            //                        yAxis:
            //                                {
            //                                title:
            //                                    {
            //                                    text: 'Hashrate (TH/s)'
            //                                }
            //                                },
            //                        legend:
            //                                {
            //                                enabled: false
            //                        },
            //                        plotOptions:
            //                                {
            //                                area:
            //                                    {
            //                                    fillColor:
            //                                        {
            //                                        linearGradient:
            //                                            {
            //                                            x1: 0,
            //                                        y1: 0,
            //                                        x2: 0,
            //                                        y2: 1
            //                                        },
            //                                    stops: [
            //                                        [0, Highcharts.getOptions().colors[0]],
            //                                        [1, Highcharts.Color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
            //                                    ]
            //                                },
            //                                marker: {
            //                                    radius: 2
            //                                },
            //                                lineWidth: 1,
            //                                states: {
            //                                    hover: {
            //                                        lineWidth: 1
            //                                    }
            //                                },
            //                                threshold: null
            //                            }
            //                        },

            //                        series: [{
            //                            type: 'area',
            //                            name: 'Hashrate',
            //                            data: data1
            //                        }]
            //                    });
            //                }
            //            );
            //</script>";

            //ViewBag.scriptdate = @"<script>
            //                        $.getJSON(
            //                'http://mining-operator.org.uk/content/json/" + User.Identity.Name + "date.json" + @"',
            //                function(data1) {

            //                            Highcharts.chart('containerdate', {
            //                            chart:
            //                                {type: 'spline',
            //                                zoomType: 'x'
            //                            },
            //                        title:
            //                                {
            //                                text: 'Hashrate over time'
            //                        },
            //                        subtitle:
            //                                {
            //                                text: document.ontouchstart === undefined ?
            //                                    'Click and drag in the plot area to zoom in' : 'Pinch the chart to zoom in'
            //                        },
            //                        xAxis:
            //                                {
            //                                type: 'datetime'
            //                        },
            //                        yAxis:
            //                                {
            //                                title:
            //                                    {
            //                                    text: 'Hashrate (TH/s)'
            //                                }
            //                                },
            //                        legend:
            //                                {
            //                                enabled: false
            //                        },
            //                        plotOptions:
            //                                {
            //                                area:
            //                                    {
            //                                    fillColor:
            //                                        {
            //                                        linearGradient:
            //                                            {
            //                                            x1: 0,
            //                                        y1: 0,
            //                                        x2: 0,
            //                                        y2: 1
            //                                        },
            //                                    stops: [
            //                                        [0, Highcharts.getOptions().colors[0]],
            //                                        [1, Highcharts.Color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
            //                                    ]
            //                                },
            //                                marker: {
            //                                    radius: 2
            //                                },
            //                                lineWidth: 1,
            //                                states: {
            //                                    hover: {
            //                                        lineWidth: 1
            //                                    }
            //                                },
            //                                threshold: null
            //                            }
            //                        },

            //                        series: [{
            //                            type: 'area',
            //                            name: 'Hashrate',
            //                            data: data1
            //                        }]
            //                    });
            //                }
            //            );
            //</script>";
            //конец график




            return View(user);
        }

    }
}
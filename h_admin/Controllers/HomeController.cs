using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using h_admin.Models;
using Newtonsoft.Json;

namespace h_admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        hdata context = new hdata();

        smsres.ReceiveSoapClient recm = new smsres.ReceiveSoapClient("ReceiveSoap");

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getcomm()
        {
            try
            {
                var res = context.tbl_comm.Select(x => new { x.pkID, x.Name, x.valuee, x.dis, x.typee }).ToList();
                return Json(new { state = true, m = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { state = false, m = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult commtblch(int id, string valuee)
        {
            try
            {
                var res = context.tbl_comm.Where(x => x.pkID == id).Single();
                res.valuee = valuee;
                context.SaveChanges();
                return Json(new { state = true, m = valuee }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { state = false, m = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult chgimg(int id)

        {

            try
            {




                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var file = System.Web.HttpContext.Current.Request.Files["image"];


                    if (file.ContentLength > 3000000) { return Json(new { state = false, m = "حجم عکس باید کوچکتر از 3 مگا بایت باشد" }, JsonRequestBehavior.AllowGet); }

                    FtpWebRequest ftpRequest =
                   (FtpWebRequest)WebRequest.Create("");



                    ftpRequest.Credentials = new NetworkCredential("", "");
                    ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                    ftpRequest.EnableSsl = true;
                    FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                    StreamReader streamReader = new StreamReader(response.GetResponseStream());



                    string imgname = "_" + id + "_";
                    string pic = "";
                    string line = streamReader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        if (line.Contains(imgname))
                        {
                            pic = line;
                            line = streamReader.ReadLine();


                            break;
                        }
                        else
                        {
                            line = streamReader.ReadLine();
                        }
                    }

                    streamReader.Close();

                    if (pic != "")
                    {
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://irwrs1.dnswebhost.com/httpdocs/hospital_img/" + pic);
                        request.Method = WebRequestMethods.Ftp.DeleteFile;
                        request.Credentials = new NetworkCredential("", "");

                        using (FtpWebResponse response2 = (FtpWebResponse)request.GetResponse())
                        {
                            pic = response2.StatusDescription;
                        }

                    }





                    string extention = Path.GetExtension(file.FileName).ToLower();

                    var uploadurl = "ftp://irwrs1.dnswebhost.com/httpdocs/hospital_img";
                    var uploadfilename = "_" + id + "_(" + DateTime.Now + ")" + extention;
                    uploadfilename = uploadfilename.Replace(@"/", "-");
                    uploadfilename = uploadfilename.Replace(@":", "-");
                    var username = "";
                    var password = "";
                    Stream streamObj = file.InputStream;
                    byte[] buffer = new byte[file.ContentLength];
                    streamObj.Read(buffer, 0, buffer.Length);
                    streamObj.Close();
                    streamObj = null;
                    string ftpurl = String.Format("{0}/{1}", uploadurl, uploadfilename);
                    var requestObj = FtpWebRequest.Create(ftpurl) as FtpWebRequest;

                    requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                    requestObj.Credentials = new NetworkCredential(username, password);

                    Stream requestStream = requestObj.GetRequestStream();

                    requestStream.Write(buffer, 0, buffer.Length);
                    requestStream.Flush();
                    requestStream.Close();
                    requestObj = null;





                    var res = context.tbl_comm.Where(x => x.pkID == id).Single();
                    res.valuee = "http://omg2.ir/hospital_img/" + uploadfilename;


                    context.SaveChanges();


                    return Json(new { state = true, nimg = uploadfilename, m = "عکس با موفقیت بارگزاری شد" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { state = false, m = "عکس یافت نشد" }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception e)
            {
                return Json(new { state = false, m = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult department()
        {
            return View();
        }

        public ActionResult getdep()
        {
            try
            {
                var res = context.tbl_Skills.Select(x => new { x.pkID, x.Skill }).ToList();
                return Json(new { state = true, m = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { state = false, m = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult sms()
        {
            return View();
        }

        [HttpPost]
        [ActionName("sendsms")]

        public async System.Threading.Tasks.Task<ActionResult> SubmitAsync(string txt, string phone)
        {
            try
            {
                string result = "";
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "username", Smsparam.username },
                        { "password", Smsparam.password },
                        { "to", phone },
                        { "from", Smsparam.number },
                        {"text",txt }



                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("https://rest.payamak-panel.com/api/SendSMS/SendSMS", content);
                    string responseString = await response.Content.ReadAsStringAsync();
                    result = responseString;
                }
                Retsendsms ret = JsonConvert.DeserializeObject<Retsendsms>(result);


                return Json(new { state = true, ret = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { state = false, ret = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("getstatus")]
        public async System.Threading.Tasks.Task<ActionResult> SubmitAsync(string recid)
        {

            string responseString = "";
            try
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "username", Smsparam.username },
                        { "password", Smsparam.password },
                        { "recID", recid }

                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("https://rest.payamak-panel.com/api/SendSMS/GetDeliveries2", content);
                    responseString = await response.Content.ReadAsStringAsync();
                    Retsendsms ret = JsonConvert.DeserializeObject<Retsendsms>(responseString);
                    return Json(new { state = true, ret = ret }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { state = false, ret = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ActionName("getsms")]
        public async System.Threading.Tasks.Task<ActionResult> SubmittAsync(string type, string count)
        {
            //var a = recm.GetMessages(Smsparam.username, Smsparam.password, 2, Smsparam.number, 0, 5);


            string responseString = "";
            try
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "username", Smsparam.username },
                        { "password", Smsparam.password },
                        { "location", type },
                        {"from",Smsparam.number },
                        {"index","0" },
                        {"count",count }

                    };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("https://rest.payamak-panel.com/api/SendSMS/GetMessages", content);
                    responseString = await response.Content.ReadAsStringAsync();


                    return Json(new { state = true, ret = responseString }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { state = false, ret = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult deletesms(string msgid)
        {
            try
            {
                int a = recm.RemoveMessages2(Smsparam.username, Smsparam.password, 2, msgid);
                return Json(new { state = true, ret = a }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { state = false, ret = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }



}
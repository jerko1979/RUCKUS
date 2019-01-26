
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ConsoleApp13.Model;
using System.Net.Mail;


public class Parsiranje
{

    public class LogConnection
    {
        //public string UserMac
        //{
        //    get { return this.User_MAC; }
        //    set { this.User_MAC = value; }
        //}
        public string User_MAC { get; set; }
        public string WLAN_NAME { get; set; }
        public string AP_MAC { get; set; }
        public string AP_NAME { get; set; }
        public int SNR { get; set; }
        public int RSSI { get; set; }
        public DateTime Datetime { get; set; }
        public string EventType { get; set; }

        public LogConnection()
        {

        }


    }


    /// <summary>
    ///  Metoda vraća posljednji kreirani file u direktoriju koji počinje sa "Syslog"
    /// </summary>
    /// <param name="Direktorij u kojem se nalazi .txt za obradu"></param>
    /// <returns>Posljednja kreirana datoteka koja u imenu sadrži "Syslog"</returns>
    static string NewestFileofDirectory(string directoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        if (directoryInfo == null || !directoryInfo.Exists)
            return null;

        FileInfo[] files = directoryInfo.GetFiles();
        DateTime recentWrite = DateTime.MaxValue;
        FileInfo recentFile = null;

        int iCounter = 0;
        foreach (FileInfo file in files)
        {


            if (file.Name.StartsWith("Syslog"))
            {
                if (file.LastWriteTime < recentWrite)
                {
                    recentWrite = file.LastWriteTime;
                    recentFile = file;
                    iCounter++;
                }

            }
            else
            {
                Console.WriteLine(file.Name);

            }

        }
        if (iCounter <= 0) { return null; }
        return recentFile.Name;

    }

    static void Main(string[] args)
    {


        


        
        string putanja = @"\\WS-VENTEX\files\";
        string path2 = "C:\\Users\\jerko.viskov\\Documents\\Logovi\\";

        //string connectionstring = "Server=ws-ventex;Database=Parsiranje;Uid=sa;Pwd=Test123!";
        //string connectionstring1 = "Server=VISKOV1\\SQLEXPRESS;Database=Parsiranje;Uid=sa;Pwd=Test123!";
        RuckusLogEntities db = new RuckusLogEntities();
        List<LogConnection> logovi = new List<LogConnection>();
        string filename = NewestFileofDirectory(putanja);
        int counter = 0;
        Ruckus_Log rc = new Ruckus_Log();
        User us = new User();
        Ssid s = new Ssid();
        Device d = new Device();
        EventType eve = new EventType();



        if (filename == null)
        {
            Environment.Exit(0);
        }

       

        StreamReader sr = new StreamReader(putanja + filename);       
        System.IO.StreamReader file = new System.IO.StreamReader(putanja + filename);
        try
        {
                       
            {
               



                string pattern3 = @"@@209";
                string pattern4 = @"@@202";
                string pattern5 = @"@@204";





                /// <summary>
                ///  Streamreader učita liniju texta
                /// </summary>
                String line1;
                while ((line1 = file.ReadLine()) != null)
                {
                    /// <summary>
                    ///  Nova instanca log za svaki event odnosno liniju texta.
                    /// </summary>
                    LogConnection log = new LogConnection();


                    string line2 = sr.ReadLine();
                    if (line2.Length>50)
                    {


                        string[] niz = line2.Split(new[] { "Jadranka-Ruckus-WLC Core: " }, StringSplitOptions.None);


                        string drugidio = niz[1];
                        string prvidio = niz[0];

                        log.Datetime = DateTime.Parse(prvidio.Substring(0, 20).Replace("-", " "));
                        Console.WriteLine(log.Datetime);
                       

                        /// <summary>
                        ///  Ispitujem koji o kojem je "tipu" loga riječ.3 tipa Nakon ispitavanja tretiram ga regex metodom.
                        /// </summary>

                        if (Regex.Match(drugidio, pattern3).Success)

                        {
                            string[] listapodataka = drugidio.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);


                            string[] podatak = listapodataka[2].ToString().Split(new[] { "=" }, StringSplitOptions.None);
                            log.AP_MAC = podatak[1].Replace(@"""", "");




                            string[] podatak1 = listapodataka[3].Split(new[] { "=" }, StringSplitOptions.None);
                            log.User_MAC = podatak1[1].Replace(@"""", "");

                            string[] podatak2 = listapodataka[4].Split(new[] { "=" }, StringSplitOptions.None);
                            log.WLAN_NAME = podatak2[1].Replace(@"""", "");

                            string[] podatak3 = listapodataka[10].Split(new[] { "=" }, StringSplitOptions.None);
                            log.AP_NAME = podatak3[1].Replace(@"""", "");


                            log.EventType = "Roaming";

                            if (listapodataka.Length == 36)
                            {
                                string[] podatak4 = listapodataka[17].Split(new[] { "=" }, StringSplitOptions.None);

                                log.RSSI = Int32.Parse(podatak4[1].Replace(@"""", ""));
                            }

                            if (listapodataka.Length == 39)
                            {
                                string[] podatak4 = listapodataka[19].Split(new[] { "=" }, StringSplitOptions.None);

                                log.RSSI = Int32.Parse(podatak4[1].Replace(@"""", ""));
                            }
                        }


                        if (Regex.Match(drugidio, pattern4).Success)
                        {

                            string[] listapodataka = drugidio.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            string[] podatak = listapodataka[2].ToString().Split(new[] { "=" }, StringSplitOptions.None);
                            log.AP_MAC = podatak[1].Replace(@"""", "");

                            string[] podatak1 = listapodataka[3].Split(new[] { "=" }, StringSplitOptions.None);
                            log.User_MAC = podatak1[1].Replace(@"""", "");

                            string[] podatak2 = listapodataka[4].Split(new[] { "=" }, StringSplitOptions.None);
                            log.WLAN_NAME = podatak2[1].Replace(@"""", "");

                            log.EventType = "clientJoin";

                            string[] podatak3 = listapodataka[10].Split(new[] { "=" }, StringSplitOptions.None);
                            log.AP_NAME = podatak3[1].Replace(@"""", "");


                        }
                        if (Regex.Match(drugidio, pattern5).Success)
                        {
                            string[] listapodataka = drugidio.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            Console.WriteLine(listapodataka.Length);
                            string[] podatak = listapodataka[2].ToString().Split(new[] { "=" }, StringSplitOptions.None);
                            log.AP_MAC = podatak[1].Replace(@"""", "");

                            string[] podatak1 = listapodataka[3].Split(new[] { "=" }, StringSplitOptions.None);
                            log.User_MAC = podatak1[1].Replace(@"""", "");

                            string[] podatak2 = listapodataka[4].Split(new[] { "=" }, StringSplitOptions.None);
                            log.WLAN_NAME = podatak2[1].Replace(@"""", "");

                            string[] podatak3 = listapodataka[10].Split(new[] { "=" }, StringSplitOptions.None);
                            log.AP_NAME = podatak3[1].Replace(@"""", "");

                            log.EventType = "clientDisconnect";

                            if (listapodataka.Length == 48)
                            {
                                string[] podatak4 = listapodataka[29].Split(new[] { "=" }, StringSplitOptions.None);

                                log.RSSI = Int32.Parse(podatak4[1].Replace(@"""", ""));
                            }

                            if (listapodataka.Length == 51)
                            {
                                string[] podatak4 = listapodataka[31].Split(new[] { "=" }, StringSplitOptions.None);

                                log.RSSI = Int32.Parse(podatak4[1].Replace(@"""", ""));
                            }

                        }
                        logovi.Add(log);
                        counter++;
                    }
                                            
                   

                }
                sr.Close();
                sr.Dispose();
                file.Close();
                file.Dispose();
            }



        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
           

            LogHistory history2 = new LogHistory(filename + ".......Zapis neuspješan!" + e);
            string sourceFile = System.IO.Path.Combine(putanja, filename);
            string destFile = System.IO.Path.Combine(@"\\WS-VENTEX\files\izbrisano", filename);
            MailMessage mail = new MailMessage("ruckus-log@jadranka.hr", "jerko.viskov@jadranka.hr,ivan.koharovic@jadranka.hr");
            {
                mail.Subject = "RUCKUS ALARMS";
                mail.Body = e.ToString();
            }

            SmtpClient client = new SmtpClient();
            {
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = "webmail2.jadranka.hr";
                client.Send(mail);
            }


            sr.Close();
            sr.Dispose();
            file.Close();
            file.Dispose();
            System.IO.File.Move((putanja + filename), destFile);
            Environment.Exit(0);
            return;
        }
        

        


        int u = 0;

        /// <summary>
        ///  Spremanje u bazu.Entity framework klase.
        /// </summary>
        foreach (LogConnection m in logovi)
        {

            EventType tmpEvent = db.EventType.Where(x => x.EventName == m.EventType).FirstOrDefault<EventType>();


            if (tmpEvent == null)
            {


                eve.EventName = m.EventType;
                db.EventType.Add(eve);
                rc.EventTypeId = eve.EventId;


            }
            else
            {
                rc.EventTypeId = tmpEvent.EventId;


            }

            User tmpMac = db.User.Where(x => x.UserMac == m.User_MAC).FirstOrDefault<User>();


            if (tmpMac == null)
            {


                us.UserMac = m.User_MAC;
                db.User.Add(us);
                rc.UserId = us.UserId;

            }
            else
            {
                rc.UserId = tmpMac.UserId;


            }

            Device tmpDev = db.Device.Where(x => x.DeviceMac == m.AP_MAC&&x.DeviceName==m.AP_NAME).FirstOrDefault<Device>();

            if (tmpDev == null)
            {

                d.DeviceName = m.AP_NAME;
                d.DeviceMac = m.AP_MAC;
                db.Device.Add(d);
                rc.DeviceId = d.DeviceId;

            }
            else
            {
                rc.DeviceId = tmpDev.DeviceId;
            }

            Ssid tmpSsid = db.Ssid.Where(x => x.SsidName == m.WLAN_NAME).FirstOrDefault<Ssid>();

            if (tmpSsid == null)
            {

                s.SsidName = m.WLAN_NAME;
                rc.SsidId = s.SsidId;
                db.Ssid.Add(s);


            }
            else
            {
                rc.SsidId = tmpSsid.SsidId;
            }



            rc.TimeStamp = m.Datetime;
            rc.Snr = m.SNR;
            rc.Rssi = m.RSSI;


            db.Ruckus_Log.Add(rc);
            //db.SaveChanges();

            Console.WriteLine(u);

            u++;


            try

            {
                db.SaveChanges();




            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
                MailMessage mail = new MailMessage("ruckus-log@jadranka.hr", "jerko.viskov@jadranka.hr,ivan.koharovic@jadranka.hr");
                {
                    mail.Subject = "RUCKUS ALARMS";
                    mail.Body = e.ToString();
                    
                }

                SmtpClient client = new SmtpClient();
                {
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = "webmail2.jadranka.hr";
                    client.Send(mail);
                  
                }


                LogHistory history1 = new LogHistory(filename + ".......Zapis neuspješan!" + e);
                string sourceFile = System.IO.Path.Combine(putanja, filename);
                string destFile = System.IO.Path.Combine(@"\\WS-VENTEX\files\izbrisano", filename);
                System.IO.File.Move((putanja + filename), destFile);

                db.Database.CurrentTransaction.Rollback();
                Environment.Exit(0);
                return;

            }
        }

        //Nakon što se log parsira premješta se u folder "izbrisano".

        string path1 = @"\\WS-VENTEX\files\";
        string path10 = @"C:\Users\jerko.viskov\Documents\logovi\";
        if (System.IO.File.Exists(path1 + filename))
        {

            Console.WriteLine("Brisanje filea");
            Console.WriteLine(path1 + filename);
            string sourceFile = System.IO.Path.Combine(path1, filename);
            string destFile = System.IO.Path.Combine(@"\\WS-VENTEX\files\izbrisano", filename);
            System.IO.File.Move((path1 + filename), destFile);

        }

        LogHistory history = new LogHistory(filename + "........Zapis uspješan!");

    }
  }

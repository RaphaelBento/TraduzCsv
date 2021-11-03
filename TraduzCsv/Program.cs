using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TraduzCsv
{
    public class Evento
    {
        public string NomeFranqueado { get; set; }
        public string DTSTART { get; set; }
        public string DTEND { get; set; }
        public string DTSTAMP { get; set; }
        public string UID { get; set; }
        public string CREATED { get; set; }
        public string DESCRIPTION { get; set; }
        public string LAST_MODIFIED { get; set; }
        public string LOCATION { get; set; }
        public string SEQUENCE { get; set; }
        public string STATUS { get; set; }
        public string SUMMARY { get; set; }
        public string TRANSP { get; set; }
    }
    class Program
    {
        
        public static string CleanString(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty).Replace("\\n", " ").Replace("\\r", " ").Replace("\\ n", " ").Replace("\\ r", " ").Replace("::~", "").
                Replace("-:~", "").Replace("<", "").Replace("&", "").Replace("nbsp\\", "").Replace("amp\\", "").Replace("\\", "").Replace(":~::-", "").Replace(":~", "").
                Replace(";", "").Replace(" ;", "").Replace("00S", "STATUS").Replace("n\\n", "");
        }
        static void Main(string[] args)
        {
            string path = @"C:\Users\Public\Takeout\Agenda";
            string[] files = Directory.GetFiles(path, "*.ics", SearchOption.TopDirectoryOnly);

            List<string> valores = new List<string>();
            var ID_DO_EVENTO = 1;
            var dicionarioDeEventos = new Dictionary<int, Evento>();
            var deveProcessarProximaLinha = false;

            foreach (string file in files)
            {
                var Name= Path.GetFileNameWithoutExtension(file);
                Console.WriteLine($"Processando arquivo {file}");
                string[] linhas = File.ReadAllLines(file);
                var linhaDeObservacao = false;
                string ConverteData;
                DateTime dateTime = new DateTime();

                foreach (string linha in linhas)
                {
                    if (linha.Contains("BEGIN:VCALENDAR") )
                    {
                        ID_DO_EVENTO++;
                        deveProcessarProximaLinha = true;
                        dicionarioDeEventos.Add(ID_DO_EVENTO, new Evento());
                        dicionarioDeEventos[ID_DO_EVENTO].NomeFranqueado = Name;
                        continue;
                    }
                    if (linha.Contains("BEGIN:VEVENT"))
                    {
                        ID_DO_EVENTO++;
                        deveProcessarProximaLinha = true;
                        dicionarioDeEventos.Add(ID_DO_EVENTO, new Evento());
                        continue;
                    }

                    if (deveProcessarProximaLinha)
                    {
                        if (linha.StartsWith("DTSTART:"))
                        {
                            ConverteData = linha.Replace("DTSTART:", "").Substring(0, 8).Insert(4, "/").Insert(7, "/");
                            dateTime = DateTime.Parse(ConverteData);
                            dicionarioDeEventos[ID_DO_EVENTO].DTSTART = dateTime.ToString("dd/MM/yyyy");
                        }
                        if (linha.StartsWith("DTEND:"))
                        {
                            ConverteData = linha.Replace("DTEND:", "").Substring(0, 8).Insert(4,"/").Insert(7,"/");
                            dateTime = DateTime.Parse(ConverteData);
                            dicionarioDeEventos[ID_DO_EVENTO].DTEND += dateTime.ToString("dd/MM/yyyy");
                            linhaDeObservacao = false;
                        }

                        if (linha.StartsWith("DTSTAMP:") )
                        {
                            ConverteData = linha.Replace("DTSTAMP:", "").Substring(0, 8).Insert(4, "/").Insert(7, "/");
                            dateTime = DateTime.Parse(ConverteData);
                            dicionarioDeEventos[ID_DO_EVENTO].DTSTAMP += dateTime.ToString("dd/MM/yyyy");
                        }
                     
                        if (linha.StartsWith("UID:"))
                        {
                            dicionarioDeEventos[ID_DO_EVENTO].UID += CleanString(linha.Replace("UID:", ""));
                        }
                      
                        if (linha.StartsWith("CREATED:"))
                        {
                            dicionarioDeEventos[ID_DO_EVENTO].CREATED += CleanString(linha.Replace("CREATED:", ""));
                        }

                        if (linha.StartsWith("DESCRIPTION:") || linhaDeObservacao)
                        {
                            dicionarioDeEventos[ID_DO_EVENTO].DESCRIPTION += CleanString(linha.Replace("DESCRIPTION:", ""));
                            linhaDeObservacao = true;
                        }
                      
                        if (linha.StartsWith("LAST-MODIFIED:"))
                        {
                            linhaDeObservacao = false;
                            dicionarioDeEventos[ID_DO_EVENTO].LAST_MODIFIED += CleanString(linha.Replace("LAST-MODIFIED:", ""));
                        }

                        if (linha.StartsWith("LOCATION:"))
                        {
                            dicionarioDeEventos[ID_DO_EVENTO].LOCATION += CleanString(linha.Replace("LOCATION:", ""));
                        }

                        if (linha.StartsWith("SEQUENCE:"))
                        {
                            dicionarioDeEventos[ID_DO_EVENTO].SEQUENCE += CleanString(linha.Replace("SEQUENCE:", ""));
                        }

                        if (linha.StartsWith("STATUS:"))
                        {
                            dicionarioDeEventos[ID_DO_EVENTO].STATUS += CleanString(linha.Replace("STATUS:", ""));
                        }

                        if (linha.StartsWith("SUMMARY:"))
                        {
                            dicionarioDeEventos[ID_DO_EVENTO].SUMMARY += CleanString(linha.Replace("SUMMARY:", ""));
                        }

                        if (linha.StartsWith("TRANSP:"))
                        {
                            dicionarioDeEventos[ID_DO_EVENTO].TRANSP += CleanString(linha.Replace("TRANSP:", ""));
                        }
                    }

                    if (linha.Contains("END:VEVENT"))
                    {
                        ID_DO_EVENTO++;
                        deveProcessarProximaLinha = false;
                        continue;
                    }
                }            
            }

            using (StreamWriter writer = new StreamWriter(@"C:\Users\Public\Takeout\Arquivo convertido\Arquivo.csv"))
            {
                foreach (var item in dicionarioDeEventos.Values)
                {
                    if (item.NomeFranqueado != null)
                    {
                        writer.Write("\n");
                        writer.Write("NOME FRANQUEADO: " + item.NomeFranqueado + ";");//Pode ser nomeado como titular da agenda
                    }
                    else
                    {
                        writer.Write(item.DTSTART+" " + item.DTEND+" "+ item.DTSTAMP+" "+ item.UID+ item.CREATED+ item.DESCRIPTION+ item.LAST_MODIFIED + item.SEQUENCE + item.STATUS + item.SUMMARY + item.TRANSP + ";");                 
                    }
                }
            }
        }
    }
}
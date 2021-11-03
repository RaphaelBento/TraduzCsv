

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ExpressoesRegulares
{
    public class Posicional
    {
        public string ID { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Cpf { get; set; }

        public void ParseSubstring(string stringPosicional)
        {
            ID = stringPosicional.Substring(0, 5);
            Nome = stringPosicional.Substring(5, 30);
            DataNascimento = DateTime.ParseExact(stringPosicional.Substring(35, 8), "ddMMyyyy", new CultureInfo("pt-BR"));
            Cpf = stringPosicional.Substring(43, 11);
        }

        public void ParseExpressaoRegular(string stringPosicional)
        {
            Regex regex = new Regex(@"(?<ID>[A-Z\d]{5})(?<Nome>.{0,50})(?<DataNascimento>\d{8})(?<Cpf>\d{11})");

            Match match = regex.Match(stringPosicional);

            if (match.Success)
            {
                ID = match.Groups["ID"].Value;
                Nome = match.Groups["Nome"].Value;
                DataNascimento = DateTime.ParseExact(match.Groups["DataNascimento"].Value, "ddMMyyyy", new CultureInfo("pt-BR"));
                Cpf = match.Groups["Cpf"].Value;
            }
            else
            {
                throw new FormatException("A string posicional não está corretamente formatada.");
            }
        }
    }
}
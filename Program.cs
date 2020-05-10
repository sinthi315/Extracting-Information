using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace extract_information
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] stopWords = System.IO.File.ReadAllLines(@"C:\Filter.txt");
            string line;
            WebClient web = new WebClient();
            Console.SetBufferSize(Console.BufferWidth, 32766);
            string html = web.DownloadString("http://pastebin.com/raw.php?i=ZVB1jnAc");
            var stream = web.OpenRead("http://pastebin.com/raw.php?i=ZVB1jnAc");
            var reader = new StreamReader(stream);
            List<string> url = new List<string>();
            List<string> email = new List<string>();
            List<string> pass = new List<string>();
            List<string> Passwords = new List<string>();
            List<string> lines = new List<string>();
            List<string> lines1 = new List<string>();
            List<string> lines2 = new List<string>();

            while ((line = reader.ReadLine()) != null)
            {
                lines2.Add(line);
                string[] i = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string l in i)
                {
                    lines.Add(l);
                }
            }

            url = extractAssociatedPage(html);
            lines1 = lines2.Except(url).ToList();
            email = emailAddress(html);
            pass = Password(lines2, url, email);
            Passwords = P(lines2);
            var diff = lines.Except(url);
            var diff2 = diff.Except(email);
            var diff3 = diff2.Except(Passwords);
            var diff4 = diff3.Except(stopWords);
            var EMAIL = email.Union(diff4);
            var PASS = pass.Except(stopWords);
            var pas = Passwords.Except(stopWords);
            
            foreach(string smt in Passwords)
            {
                Console.WriteLine(smt);
            }

        }

        static List<string> extractAssociatedPage(string html)
        {
            List<string> url = new List<string>();
            string regex = "http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\/\\?\\.\\:\\;\'\\,]*)?";
            MatchCollection matches = new Regex(regex, RegexOptions.Singleline | RegexOptions.Compiled).Matches(html);

            foreach(Match m in matches)
            {
                string match = m.Groups[0].Value;
                url.Add(match);
            }

            return url;
        }

        static List<string> emailAddress(string html)
        {
            List<string> email = new List<string>();
            string regex = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            MatchCollection matches = new Regex(regex, RegexOptions.Singleline | RegexOptions.Compiled).Matches(html);

            foreach (Match m in matches)
            {
                string match = m.Groups[0].Value;
                email.Add(match);
            }
            return email;
        }

        static List<string> Password(List<string> line2, List<string> url, List<string> email)
        {
            List<string> pass = new List<string>();
            foreach(string line in line2)
            {
                if (url.Contains(line))
                {
                    break;
                }
                else if (email.Contains(line))
                {
                    break;
                }
                else
                {
                    string[] items = line.Split(new char[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    Regex r = new Regex(@"^(?=.*\d)(?=.*[A-Za-z])[A-Za-z0-9]{1,10}");
                    Regex str = new Regex(@"^[a-zA-Z0-9]*");

                    foreach (string item in items)
                    {
                        if (str.IsMatch(item))
                        {
                            if (r.IsMatch(item))
                            {
                                pass.Add(item);
                            }
                        }
                    }
                }
            }

            return pass;
        }

        static List<string> P(List<string> lines2)
        {
            List<string> Passwords = new List<string>();
            foreach (string line in lines2)
            {
                string[] i = line.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                int index = Array.IndexOf(i, i.LastOrDefault());
                var intersect = i.Skip(index);
                foreach (var v in intersect)
                {
                    Passwords.Add(v);
                }
            }
            return Passwords;
        }
    }
}

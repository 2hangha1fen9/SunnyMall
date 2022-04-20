using BLL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Entities db = new Entities())
            {
                var news = db.News.ToList();
                foreach(var item in news)
                {
                    Console.WriteLine(item.PushTime.ToString().Contains("5.20"));
                }
            }
        }
    }
}

using System.Text;
using OnlineCourses.Core.Legacy;

Console.OutputEncoding = Encoding.UTF8;
var z = new zapys("Z-1001", "Іваненко");
z.Add("CS-101", 3, 250m);
z.Add("DB-201", 12, 90m);
Console.WriteLine(z.ProcessData(false));
Console.WriteLine(z.ProcessData(true));
Console.WriteLine(z.Ck());
Console.WriteLine(z.Chg(1));
Console.Write(z.Rep());

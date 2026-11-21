using System.Text;
using OnlineCourses.Core.Legacy;

Console.OutputEncoding = Encoding.UTF8;
var subscription = new Subscription("Z-1001", "Іваненко");
subscription.AddLine("CS-101", 3, 250m);
subscription.AddLine("DB-201", 12, 90m);
Console.WriteLine(subscription.CalculateTotal(false));
Console.WriteLine(subscription.CalculateTotal(true));
Console.WriteLine(subscription.IsValid());
Console.WriteLine(subscription.TryChangeStatus(1));
Console.Write(subscription.BuildReport());

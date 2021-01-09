using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PortChat
{
    public static void Main()
    {
        TimeReader.Handler handler = new TimeReader.Handler();
        Task<bool> result = handler.Run();
        result.Wait();
    }
}

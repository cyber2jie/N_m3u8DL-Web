namespace N_m3u8DL_Web.Exit;

public delegate void ExitHandler();

public class Exit
{

    private ExitHandler exitHandler;
    public Exit(ExitHandler exitHandler)
    {
        this.exitHandler = exitHandler;
    }

    public void OnExit(Object eventSender, EventArgs e)
    {
        exitHandler();
    }
    public void RegisterExitHandler()
    {
        //AppDomain.CurrentDomain.ProcessExit += OnExit;
        Console.CancelKeyPress += OnExit;

    }
}

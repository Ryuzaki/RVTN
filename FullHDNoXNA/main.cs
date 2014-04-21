using System;

using QuartzTypeLib;

class PlayFile
{
    public static void Main(string[] args)
    {
        FilgraphManagerClass graphClass = null;
        try
        {
            graphClass = new FilgraphManagerClass();
            graphClass.RenderFile(@"C:\Users\Ryuzaki\Desktop\AviTokaiNoHitorigurashi.avi");            
            graphClass.Run();
            int evCode;
            graphClass.WaitForCompletion(-1, out evCode);
        }
        catch (Exception) { }
        finally
        {
            graphClass = null;
        }
    }
}

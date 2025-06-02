using LoggingWithStreamWriter;

using (var logger = new LogWriter("log.txt"))
{
    logger.WriteLog("INFO", "Application started.");
    logger.WriteLog("ERROR", "An unexpecter error ocurred.");
    logger.WriteLog("INFO", "Application finished.");

}
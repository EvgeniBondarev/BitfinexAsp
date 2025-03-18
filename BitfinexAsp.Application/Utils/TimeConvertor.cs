namespace BitfinexAsp.Utils;

public static class TimeConvertor
{
    public static int ConvertSecondsToMinutes(int periodInSec)
    {
        int minutes = periodInSec / 60;
        return minutes;
    }
}
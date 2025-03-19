namespace BitfinexAsp.Utils;

/// <summary>
/// Класс для конвертации временных значений.
/// </summary>
public static class TimeConvertor
{
    public static int ConvertSecondsToMinutes(int periodInSec)
    {
        int minutes = periodInSec / 60;
        return minutes;
    }
}
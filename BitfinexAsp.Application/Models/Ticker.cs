using System.ComponentModel.DataAnnotations;

namespace BitfinexAsp.Models.JsonToModelConverter.Converters;

public class Ticker
{
    /// <summary>
    /// Цена последнего наивысшего бида (предложение на покупку)
    /// </summary>
    [Display(Name = "Цена бида")]
    public float Bid { get; set; }

    /// <summary>
    /// Сумма размеров 25 наивысших бидов
    /// </summary>
    [Display(Name = "Объем бида")]
    public float BidSize { get; set; }

    /// <summary>
    /// Цена последнего наименьшего аска (предложение на продажу)
    /// </summary>
    [Display(Name = "Цена аска")]
    public float Ask { get; set; }

    /// <summary>
    /// Сумма размеров 25 наименьших асков
    /// </summary>
    [Display(Name = "Объем аска")]
    public float AskSize { get; set; }

    /// <summary>
    /// Изменение цены за последние 24 часа
    /// </summary>
    [Display(Name = "Изменение за 24 часа")]
    public float DailyChange { get; set; }

    /// <summary>
    /// Относительное изменение цены за последние 24 часа (в процентах, умноженное на 100)
    /// </summary>
    [Display(Name = "Относительное изменение (%)")]
    public float DailyChangeRelative { get; set; }

    /// <summary>
    /// Цена последней сделки
    /// </summary>
    [Display(Name = "Цена последней сделки")]
    public float LastPrice { get; set; }

    /// <summary>
    /// Объем торгов за последние 24 часа
    /// </summary>
    [Display(Name = "Объем за 24 часа")]
    public float Volume { get; set; }

    /// <summary>
    /// Максимальная цена за последние 24 часа
    /// </summary>
    [Display(Name = "Максимальная цена")]
    public float High { get; set; }

    /// <summary>
    /// Минимальная цена за последние 24 часа
    /// </summary>
    [Display(Name = "Минимальная цена")]
    public float Low { get; set; }
}
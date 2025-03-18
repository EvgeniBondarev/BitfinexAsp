using System.ComponentModel.DataAnnotations;

namespace BitfinexAsp.Models;


public class Candle
{
    /// <summary>
    /// Валютная пара
    /// </summary>
    [Display(Name = "Валютная пара")]
    public string Pair { get; set; }

    /// <summary>
    /// Цена открытия
    /// </summary>
    [Display(Name = "Цена открытия")]
    public decimal OpenPrice { get; set; }

    /// <summary>
    /// Максимальная цена
    /// </summary>
    [Display(Name = "Максимальная цена")]
    public decimal HighPrice { get; set; }

    /// <summary>
    /// Минимальная цена
    /// </summary>
    [Display(Name = "Минимальная цена")]
    public decimal LowPrice { get; set; }

    /// <summary>
    /// Цена закрытия
    /// </summary>
    [Display(Name = "Цена закрытия")]
    public decimal ClosePrice { get; set; }

    /// <summary>
    /// Общая сумма сделок
    /// </summary>
    [Display(Name = "Общая сумма сделок")]
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Общий объем
    /// </summary>
    [Display(Name = "Общий объем")]
    public decimal TotalVolume { get; set; }

    /// <summary>
    /// Время
    /// </summary>
    [Display(Name = "Время")]
    public DateTimeOffset OpenTime { get; set; }
}
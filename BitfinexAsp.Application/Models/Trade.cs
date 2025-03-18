using System;
using System.ComponentModel.DataAnnotations;

namespace BitfinexAsp.Models;

public class Trade
{
    /// <summary>
    /// Валютная пара
    /// </summary>
    [Display(Name = "Валютная пара")]
    public string Pair { get; set; }

    /// <summary>
    /// Цена трейда
    /// </summary>
    [Display(Name = "Цена")]
    public decimal Price { get; set; }

    /// <summary>
    /// Объем трейда
    /// </summary>
    [Display(Name = "Объем")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Направление (buy/sell)
    /// </summary>
    [Display(Name = "Направление")]
    public string Side { get; set; }

    /// <summary>
    /// Время трейда
    /// </summary>
    [Display(Name = "Время")]
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// Id трейда
    /// </summary>
    [Display(Name = "ID")]
    public string Id { get; set; }
}
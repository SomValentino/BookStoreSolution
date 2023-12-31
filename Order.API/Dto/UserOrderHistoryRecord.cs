using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using Order.API.Models;

namespace Order.API.Dto;

public record UserOrderHistoryRecord {
    [JsonConverter (typeof (JsonStringEnumConverter))]
    public OrderStatus? OrderStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
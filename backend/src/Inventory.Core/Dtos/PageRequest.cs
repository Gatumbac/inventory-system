using System.ComponentModel.DataAnnotations;
using Inventory.Core.Constants;

namespace Inventory.Core.Dtos;

public class PageRequest
{
    public const int MaxPage = 100000;
    public const int MaxPageSize = 50;

    [Range(1, MaxPage, ErrorMessage = ErrorMessages.PageInvalid)]
    public int Page { get; set; } = 1;

    [Range(1, MaxPageSize, ErrorMessage = ErrorMessages.PageSizeInvalid)]
    public int PageSize { get; set; } = 10;
}

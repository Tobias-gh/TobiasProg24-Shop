using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Dtos;

public record CartDto(
    Guid Id,
    string SessionId,
    IEnumerable<CartItemDto> Items,
    int TotalItems,
    decimal TotalPrice,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? ProductDescription,
    decimal ProductPrice,
    int Quantity,
    decimal Subtotal,
    int AvailableStock,
    DateTime AddedAt
);

public record AddToCartRequest(
    Guid ProductId,
    int Quantity
);
public record UpdateCartItemRequest(
    int Quantity    
);

public record CartSummaryDto(
    int TotalItems,
    decimal TotalPrice
);


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Dtos;

    public  record CategoryDto(
        Guid Id,
        string Name,
        string? Description,
        int ProductCount
        );


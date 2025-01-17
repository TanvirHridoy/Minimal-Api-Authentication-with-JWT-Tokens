﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinimalApi.DTO;

public class ApplicationUser:IdentityUser<string>
{
    [Required(ErrorMessage = "FullName is required.")]
    [MaxLength(150)]
    public required string FullName { get; set; }

    [Required]
    [Column(TypeName = "datetime")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }
}

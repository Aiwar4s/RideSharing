﻿using System.ComponentModel.DataAnnotations;
using webapi.Auth.Entities;

namespace webapi.Data.Entities
{
    public class Driver
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        [Required]
        public required string UserId {  get; set; }
        public required User User { get; set; }
    }
}

public record DriverDto(int Id, string Name, string Email, string PhoneNumber, string UserId);
public record CreateDriverDto(string Name, string Email, string PhoneNumber, string UserId);
public record UpdateDriverDto(string Email, string PhoneNumber);

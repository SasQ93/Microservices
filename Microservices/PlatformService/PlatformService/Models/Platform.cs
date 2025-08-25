using System.ComponentModel.DataAnnotations;

namespace PlatformServices.Models;

public class Platform
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Publisher { get; set; }

    [Required]
    public string Cost { get; set; }
}

// sql
/*
create database PlatformService;
use PlatformService;
create table platform (
Id int not null auto_increment primary key,
Name varchar(100) not null,
Publisher varchar(100) not null,
Cost varchar(100) not null
);

insert into platform (Name, Publisher, Cost) values
('DotNet', 'Microsoft', 'Free'),
('Kubernetes', 'Cloud Native Computing Foundation', 'Free'),
('SQL Server Express', 'Microsoft', 'Free')
;

select * from platform;

*/
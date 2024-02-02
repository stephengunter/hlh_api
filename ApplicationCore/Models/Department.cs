using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models;

public class Department : BaseCategory<Department>
{
   public string? Key { get; set; }

   public virtual ICollection<Job>? Jobs { get; set; }

}


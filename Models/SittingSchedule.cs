using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeanScene.Web.Models;

public partial class SittingSchedule
{   
    public int SittingScheduleId { get; set; }

    [Display(Name = "Sitting Type")]
    public string Stype { get; set; } = null!;
    [Display(Name = "Start Time")]
    public TimeOnly StartTime { get; set; }
    [Display(Name = "End Time")]
    public TimeOnly EndTime { get; set; }

    [Display(Name = "Capacity")]
    public int Scapacity { get; set; }

    public string Status { get; set; } = null!;

    //public bool IsClosed { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

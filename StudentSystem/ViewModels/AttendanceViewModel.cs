using Microsoft.AspNetCore.Mvc;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Runtime.Intrinsics.Arm;

namespace StudentSystem.ViewModels
{

    public class AttendanceViewModel
    {
        public int CourseId { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, bool> StudentPresence { get; set; } 
    }

}

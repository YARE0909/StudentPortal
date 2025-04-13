namespace StudentPortal.Models
{
    public class TimetableConflictReport
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string PreferredCourse { get; set; }
        public string PreferredDay { get; set; }
        public string PreferredTimeSlot { get; set; }
        public string Remarks { get; set; }

        // Instead of response, we have a "Noted" flag
        public bool IsNoted { get; set; } = false;
        public DateTime DateSubmitted { get; set; } = DateTime.Now;

     
        public Student Student { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Uploading_File.Models
{
    public class StudentModel
    {
        
        public int Studentid { get; set; }
        public string StudentName { get; set; }
        public string Section { get; set; }
        public int Subject1 { get; set; }
        public int Subject2 { get; set; }
        public int Total { get; set; }
    }
}

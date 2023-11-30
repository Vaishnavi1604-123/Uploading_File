using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Uploading_File.Models
{
    public class UploadFileModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [Display(Name = "Select File")]
        public IFormFile File { get; set; }
        //    [Required(ErrorMessage = "Please provide a file path.")]
        //    [Display(Name = "File Path")]
        //    public string FilePath { get; set; }
        //}
    }
}

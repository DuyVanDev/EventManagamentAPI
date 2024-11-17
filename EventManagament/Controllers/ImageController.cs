using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace EventManagament.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;

        public ImageController(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Chưa chọn tệp hình ảnh");

            // Kiểm tra định dạng file
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Tệp không phải là hình ảnh");

            // Upload trực tiếp lên Cloudinary
            using var inputStream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, inputStream),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                return BadRequest(uploadResult.Error.Message);

            // Trả về URL của ảnh sau khi upload thành công
            return Ok(new { Url = uploadResult.SecureUrl.AbsoluteUri });
        }
    }
}

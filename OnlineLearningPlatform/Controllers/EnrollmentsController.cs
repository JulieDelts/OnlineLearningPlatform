using Microsoft.AspNetCore.Mvc;
using OnlineLearningPlatform.Models.Requests;

namespace OnlineLearningPlatform.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController: ControllerBase
    {
        [HttpPost()]
        public ActionResult<Guid> Enroll([FromBody] EnrollmentRequest request)
        {
            var newEnrollmentId = Guid.NewGuid();
            return Ok(newEnrollmentId);
        }

        [HttpDelete("{id}")]
        public ActionResult Disenroll(int id)
        {
            return NoContent();
        }
    }
}

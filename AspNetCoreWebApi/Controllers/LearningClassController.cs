using AspNetCoreWebApi.Models;
using AspNetCoreWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebApi.Controllers
{
    [Route("api/v1/learning-class")]
    [ApiController]
    public class LearningClassController : ControllerBase
    {
        private readonly LearningClassCrudService _learningClassCrudService;

        public LearningClassController(LearningClassCrudService learningClassCrudService)
        {
            _learningClassCrudService = learningClassCrudService;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string? lecturerName, [FromQuery] int page = 1)
        {
            var learningClassData = await _learningClassCrudService.GetLearningClasses(lecturerName, page);

            return Ok(learningClassData);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateLearningClassForm newLearningClass)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var (IsSuccess, Field, Message) = await _learningClassCrudService.CreateLearningClass(newLearningClass);

            if (!IsSuccess)
            {
                ModelState.AddModelError(Field, Message);

                return ValidationProblem(ModelState);
            }

            return Ok(Message);
        }
    }
}

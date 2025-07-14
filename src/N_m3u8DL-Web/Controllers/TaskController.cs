using Microsoft.AspNetCore.Mvc;
using N_m3u8DL_Web.Model.Task;
using N_m3u8DL_Web.Service;

namespace N_m3u8DL_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {

        private readonly ITaskService taskService;

        public TaskController(ITaskService taskService)
        {
            this.taskService = taskService;
        }


        [HttpPost("add")]
        public IResult Add([FromBody] TaskAddForm taskAdd)
        { 
            TaskAddResult taskAddResult = taskService.Add(taskAdd);

            return Results.Json(taskAddResult);
        }

        [HttpPost("delete/{taskId}")]
        public IResult Delete(int taskId)
        {
            return Results.Json(taskService.Delete(taskId));
        }

        [HttpPost("list")]
        public IResult List([FromBody]TaskListForm taskList) 
        {
           return Results.Json(taskService.List(taskList));
        }


        [HttpPost("status")]
        public IResult Status([FromBody] TaskStatusForm taskStatusForm)
        {
            return Results.Json(taskService.Status(taskStatusForm));
        }

    }
}

using GeoCidadao.UserPostsAPI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GeoCidadao.UserPostsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserPostsController(IUserPostsService service) : ControllerBase
{
    private readonly IUserPostsService _service = service;


}
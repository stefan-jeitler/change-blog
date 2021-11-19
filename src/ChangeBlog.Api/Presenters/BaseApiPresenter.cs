using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters;

public abstract class BaseApiPresenter
{
    public ActionResult Response { get; protected set; } = new StatusCodeResult(500);
}
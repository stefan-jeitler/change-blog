using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Shared.Presenters;

public abstract class BaseApiPresenter
{
    public ActionResult Response { get; protected set; } = new StatusCodeResult(500);
}
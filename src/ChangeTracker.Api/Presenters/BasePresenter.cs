using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters
{
    public abstract class BasePresenter
    {
        public ActionResult Response { get; protected set; } = new StatusCodeResult(500);
    }
}
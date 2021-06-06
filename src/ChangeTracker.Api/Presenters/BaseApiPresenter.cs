using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Presenters
{
    public abstract class BaseApiPresenter
    {
        public ActionResult Response { get; protected set; } = new StatusCodeResult(500);
    }
}